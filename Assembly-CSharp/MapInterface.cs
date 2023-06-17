using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000633 RID: 1587
public class MapInterface : SingletonComponent<MapInterface>
{
	// Token: 0x04001F80 RID: 8064
	public static bool IsOpen;

	// Token: 0x04001F81 RID: 8065
	public RawImage mapImage;

	// Token: 0x04001F82 RID: 8066
	public Image cameraPositon;

	// Token: 0x04001F83 RID: 8067
	public ScrollRectEx scrollRect;

	// Token: 0x04001F84 RID: 8068
	public PaintableImageGrid paintGrid;

	// Token: 0x04001F85 RID: 8069
	public UIPaintBox paintBox;

	// Token: 0x04001F86 RID: 8070
	public Toggle showGridToggle;

	// Token: 0x04001F87 RID: 8071
	public Button FocusButton;

	// Token: 0x04001F88 RID: 8072
	public GameObject monumentMarkerContainer;

	// Token: 0x04001F89 RID: 8073
	public GameObject monumentMarkerPrefab;

	// Token: 0x04001F8A RID: 8074
	public CanvasGroup CanvasGroup;

	// Token: 0x04001F8B RID: 8075
	public bool followingPlayer = true;

	// Token: 0x04001F8C RID: 8076
	public static float LastOpened;

	// Token: 0x04001F8D RID: 8077
	private MapGrid MapGrid;

	// Token: 0x04001F8E RID: 8078
	private NeedsCursor NeedsCursor;

	// Token: 0x04001F8F RID: 8079
	private NeedsMouseButtons NeedsMouseButtons;

	// Token: 0x04001F90 RID: 8080
	private NeedsMouseWheel NeedsMouseWheel;

	// Token: 0x04001F91 RID: 8081
	private float nextSave;

	// Token: 0x04001F92 RID: 8082
	internal static MapEntity lastActiveMap;

	// Token: 0x04001F93 RID: 8083
	public TeamMemberMapMarker[] teamPositions;

	// Token: 0x0600236A RID: 9066 RVA: 0x0001C06E File Offset: 0x0001A26E
	public static void SetOpen(bool open)
	{
		if (SingletonComponent<MapInterface>.Instance)
		{
			SingletonComponent<MapInterface>.Instance.ForceOpen(open);
		}
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x0001C087 File Offset: 0x0001A287
	protected override void Awake()
	{
		base.Awake();
		this.MapGrid = base.GetComponentInChildren<MapGrid>();
		this.NeedsCursor = base.GetComponentInChildren<NeedsCursor>();
		this.NeedsMouseButtons = base.GetComponentInChildren<NeedsMouseButtons>();
		this.NeedsMouseWheel = base.GetComponentInChildren<NeedsMouseWheel>();
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000BBE3C File Offset: 0x000BA03C
	private void OnEnable()
	{
		MapInterface.IsOpen = false;
		this.CanvasGroup.alpha = 0f;
		this.CanvasGroup.interactable = false;
		this.CanvasGroup.blocksRaycasts = false;
		this.followingPlayer = true;
		this.NeedsCursor.enabled = false;
		this.NeedsMouseButtons.enabled = false;
		this.NeedsMouseWheel.enabled = false;
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x0001C0BF File Offset: 0x0001A2BF
	public void FollowPlayer()
	{
		this.followingPlayer = true;
		this.FocusButton.interactable = false;
	}

	// Token: 0x0600236E RID: 9070 RVA: 0x0001C0D4 File Offset: 0x0001A2D4
	public void MarkersDirty()
	{
		if (base.IsInvoking(new Action(this.UpdateMarkers)))
		{
			return;
		}
		base.Invoke(new Action(this.UpdateMarkers), 1f);
	}

	// Token: 0x0600236F RID: 9071 RVA: 0x000BBEA4 File Offset: 0x000BA0A4
	private void SetupMonuments()
	{
		if (TerrainMeta.Path == null || TerrainMeta.Path.Monuments == null || TerrainMeta.Path.Monuments.Count == 0)
		{
			return;
		}
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo.shouldDisplayOnMap)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.monumentMarkerPrefab);
				gameObject.transform.SetParent(this.monumentMarkerContainer.transform, false);
				gameObject.GetComponent<MonumentMarker>().Setup(monumentInfo);
				Vector2 v = MapInterface.WorldPosToImagePos(monumentInfo.transform.position);
				gameObject.GetComponent<Text>().rectTransform.localPosition = v;
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06002370 RID: 9072 RVA: 0x000BBF84 File Offset: 0x000BA184
	private void SetupMarkers()
	{
		foreach (MapMarker mapMarker in MapMarker.mapMarkers)
		{
			if (!(mapMarker == null) && !(mapMarker.transform == null))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(mapMarker.markerObj);
				gameObject.transform.SetParent(this.monumentMarkerContainer.transform, false);
				mapMarker.SetupUIMarker(gameObject);
				Vector2 v = MapInterface.WorldPosToImagePos(mapMarker.transform.position);
				gameObject.GetComponent<RectTransform>().localPosition = v;
			}
		}
	}

	// Token: 0x06002371 RID: 9073 RVA: 0x0001C102 File Offset: 0x0001A302
	public void UpdateMarkers()
	{
		this.monumentMarkerContainer.transform.DestroyChildren();
		this.SetupMonuments();
		this.SetupMarkers();
	}

	// Token: 0x06002372 RID: 9074 RVA: 0x000BC034 File Offset: 0x000BA234
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		MapInterface.IsOpen = false;
		this.NeedsCursor.enabled = false;
		this.NeedsMouseButtons.enabled = false;
		this.NeedsMouseWheel.enabled = false;
		this.paintBox.gameObject.SetActive(false);
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x000BC084 File Offset: 0x000BA284
	private void CenterOn(Vector3 vec)
	{
		Vector2 pos = MapInterface.WorldPosToImagePos(vec);
		this.scrollRect.CenterOnPosition(pos);
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x0001C120 File Offset: 0x0001A320
	public static void ResetMap()
	{
		if (SingletonComponent<MapInterface>.Instance == null)
		{
			return;
		}
		SingletonComponent<MapInterface>.Instance.paintGrid.Clear();
		SingletonComponent<MapInterface>.Instance.MapGrid.ClearGrid();
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x0001C14E File Offset: 0x0001A34E
	private void ClearImages()
	{
		this.paintGrid.Clear();
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000BC0A4 File Offset: 0x000BA2A4
	private void Update()
	{
		this.CanvasGroup.alpha = Mathf.Lerp(this.CanvasGroup.alpha, (float)(MapInterface.IsOpen ? 1 : 0), UnityEngine.Time.deltaTime * 20f);
		if (!MapInterface.IsOpen)
		{
			return;
		}
		if (UnityEngine.Input.GetKey(KeyCode.Mouse1) || UnityEngine.Input.GetKey(KeyCode.Mouse2))
		{
			this.followingPlayer = false;
			this.FocusButton.interactable = true;
		}
	}

	// Token: 0x06002377 RID: 9079 RVA: 0x000BC118 File Offset: 0x000BA318
	public static Vector2 WorldPosToImagePos(Vector3 worldPos)
	{
		if (SingletonComponent<MapInterface>.Instance == null)
		{
			return Vector2.zero;
		}
		Vector3 vector = TerrainMeta.Normalize(worldPos);
		Vector3 v = default(Vector3);
		v.x = vector.x * SingletonComponent<MapInterface>.Instance.scrollRect.content.rect.width;
		v.y = vector.z * SingletonComponent<MapInterface>.Instance.scrollRect.content.rect.height;
		v.z = 0f;
		v.x -= SingletonComponent<MapInterface>.Instance.scrollRect.content.rect.width * 0.5f;
		v.y -= SingletonComponent<MapInterface>.Instance.scrollRect.content.rect.height * 0.5f;
		return v;
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x000BC208 File Offset: 0x000BA408
	public void UpdatePlayerPosition(Vector3 position, Vector3 forward)
	{
		Vector3 vector = forward;
		vector.y = 0f;
		vector.Normalize();
		Vector2 v = MapInterface.WorldPosToImagePos(position);
		this.cameraPositon.rectTransform.localPosition = v;
		this.cameraPositon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector.x, -vector.z) * 57.29578f + 90f);
		if (this.followingPlayer)
		{
			this.CenterOn(position);
		}
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000BC294 File Offset: 0x000BA494
	private void SaveUpdate(MapEntity ent)
	{
		if (this.nextSave > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		this.nextSave = UnityEngine.Time.realtimeSinceStartup + 4f;
		UIPaintableImage[,] images = this.paintGrid.images;
		int upperBound = images.GetUpperBound(0);
		int upperBound2 = images.GetUpperBound(1);
		for (int i = images.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = images.GetLowerBound(1); j <= upperBound2; j++)
			{
				UIPaintableImage uipaintableImage = images[i, j];
				if (uipaintableImage.imageHash == 0U && !uipaintableImage.isLocked && !uipaintableImage.isBlank)
				{
					byte[] data = uipaintableImage.ToPng(ent);
					if (ent)
					{
						ent.PaintImageUpdate(uipaintableImage.imageNumber, uipaintableImage.imageHash, data);
					}
				}
			}
		}
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000BC354 File Offset: 0x000BA554
	private void LoadGrid(MapEntity ent, PaintableImageGrid grid, uint[] imageArray)
	{
		UIPaintableImage[,] images = grid.images;
		int upperBound = images.GetUpperBound(0);
		int upperBound2 = images.GetUpperBound(1);
		for (int i = images.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = images.GetLowerBound(1); j <= upperBound2; j++)
			{
				UIPaintableImage uipaintableImage = images[i, j];
				if (uipaintableImage.isLocked)
				{
					uint num = 0U;
					if (imageArray != null)
					{
						num = imageArray[uipaintableImage.imageNumber];
					}
					if (num == 0U)
					{
						uipaintableImage.ClearTexture();
						uipaintableImage.isLocked = false;
						uipaintableImage.isBlank = true;
					}
					else if (uipaintableImage.imageHash == num)
					{
						uipaintableImage.isLocked = false;
						uipaintableImage.isLoading = false;
					}
					else
					{
						byte[] array = FileStorage.client.Get(num, FileStorage.Type.png, ent.net.ID);
						if (array != null)
						{
							uipaintableImage.FromData(array);
							uipaintableImage.isLocked = false;
							uipaintableImage.isLoading = false;
							uipaintableImage.imageHash = num;
							return;
						}
						if (!uipaintableImage.isLoading)
						{
							ent.RequestFileFromServer(num, FileStorage.Type.png, "Cl_ReceiveFilePng");
							uipaintableImage.isLoading = true;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x0001C15B File Offset: 0x0001A35B
	private void LoadUpdate(MapEntity ent)
	{
		this.LoadGrid(ent, this.paintGrid, ent ? ent.paintImages : null);
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x0600237C RID: 9084 RVA: 0x000BC470 File Offset: 0x000BA670
	public static MapEntity localPlayerMap
	{
		get
		{
			if (LocalPlayer.Entity == null)
			{
				return null;
			}
			if (LocalPlayer.Entity.inventory == null)
			{
				return null;
			}
			if (LocalPlayer.Entity.inventory.containerBelt == null)
			{
				return null;
			}
			Item item = LocalPlayer.Entity.inventory.containerBelt.FindItemsByItemName("map");
			if (item == null)
			{
				return null;
			}
			MapEntity mapEntity = item.GetHeldEntity() as MapEntity;
			if (mapEntity == null)
			{
				return null;
			}
			return mapEntity;
		}
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x0001C17B File Offset: 0x0001A37B
	public void ShowGridToggle()
	{
		this.MapGrid.SetGridVisible(this.showGridToggle.isOn);
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000BC4EC File Offset: 0x000BA6EC
	private void ForceOpen(bool open)
	{
		if (MapInterface.IsOpen == open)
		{
			return;
		}
		if (SingletonComponent<MapInterface>.Instance == null)
		{
			return;
		}
		this.NeedsMouseWheel.enabled = open;
		this.NeedsCursor.enabled = open;
		this.NeedsMouseButtons.enabled = open;
		this.CanvasGroup.blocksRaycasts = open;
		this.CanvasGroup.interactable = open;
		MapInterface.IsOpen = open;
		if (open)
		{
			MapInterface.LastOpened = UnityEngine.Time.realtimeSinceStartup;
			Analytics.MapOpened++;
			this.MapGrid.InitializeGrid();
			this.showGridToggle.isOn = this.MapGrid.IsGridVisible();
			this.MarkersDirty();
		}
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000BC594 File Offset: 0x000BA794
	public void ClientTeamUpdated()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (!RelationshipManager.TeamsEnabled())
		{
			return;
		}
		PlayerTeam clientTeam = LocalPlayer.Entity.clientTeam;
		for (int i = 0; i < this.teamPositions.Length; i++)
		{
			TeamMemberMapMarker teamMemberMapMarker = this.teamPositions[i];
			if (clientTeam != null && i < clientTeam.members.Count && clientTeam.members[i].userID != LocalPlayer.Entity.userID)
			{
				if (!clientTeam.members[i].online || clientTeam.members[i].healthFraction == 0f)
				{
					teamMemberMapMarker.gameObject.SetActive(false);
				}
				else
				{
					Vector2 v = MapInterface.WorldPosToImagePos(clientTeam.members[i].position);
					teamMemberMapMarker.rectTransform.localPosition = v;
					string text = Global.streamermode ? RandomUsernames.Get(clientTeam.members[i].userID) : clientTeam.members[i].displayName;
					teamMemberMapMarker.toolTip.Text = text;
					teamMemberMapMarker.nameTagText.text = text;
					teamMemberMapMarker.gameObject.SetActive(true);
				}
			}
			else
			{
				teamMemberMapMarker.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000BC6E8 File Offset: 0x000BA8E8
	public static void DoPlayerUpdate()
	{
		MapEntity localPlayerMap = MapInterface.localPlayerMap;
		if (!MapInterface.IsOpen && localPlayerMap == null)
		{
			return;
		}
		BasePlayer entity = LocalPlayer.Entity;
		if (entity == null)
		{
			return;
		}
		if (!entity.IsSpectating() && !entity.CanInteract())
		{
			return;
		}
		if (MapInterface.lastActiveMap != localPlayerMap)
		{
			MapInterface.lastActiveMap = localPlayerMap;
			SingletonComponent<MapInterface>.Instance.ClearImages();
		}
		if (localPlayerMap)
		{
			SingletonComponent<MapInterface>.Instance.LoadUpdate(localPlayerMap);
		}
		if (MapInterface.IsOpen)
		{
			SingletonComponent<MapInterface>.Instance.UpdatePlayerPosition(entity.transform.position, entity.eyes.BodyForward());
		}
		if (localPlayerMap)
		{
			SingletonComponent<MapInterface>.Instance.SaveUpdate(localPlayerMap);
		}
	}
}
