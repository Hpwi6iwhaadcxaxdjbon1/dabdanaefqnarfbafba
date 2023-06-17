using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch.Extend;
using JSON;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000089 RID: 137
public class CommunityEntity : PointEntity
{
	// Token: 0x0400054B RID: 1355
	public static CommunityEntity ServerInstance = null;

	// Token: 0x0400054C RID: 1356
	public static CommunityEntity ClientInstance = null;

	// Token: 0x0400054D RID: 1357
	private static List<GameObject> AllUi = new List<GameObject>();

	// Token: 0x0400054E RID: 1358
	private static Dictionary<string, GameObject> UiDict = new Dictionary<string, GameObject>();

	// Token: 0x0400054F RID: 1359
	private static Dictionary<uint, List<MaskableGraphic>> requestingTextureImages = new Dictionary<uint, List<MaskableGraphic>>();

	// Token: 0x06000863 RID: 2147 RVA: 0x0004BCFC File Offset: 0x00049EFC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CommunityEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 804751572U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: AddUI ");
				}
				using (TimeWarning.New("AddUI", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AddUI(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in AddUI", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 2050890860U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CL_ReceiveFilePng ");
				}
				using (TimeWarning.New("CL_ReceiveFilePng", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CL_ReceiveFilePng(msg3);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in CL_ReceiveFilePng", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
			if (rpc == 3571246553U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: DestroyUI ");
				}
				using (TimeWarning.New("DestroyUI", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DestroyUI(msg4);
						}
					}
					catch (Exception exception3)
					{
						Net.cl.Disconnect("RPC Error in DestroyUI", true);
						Debug.LogException(exception3);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x00008D15 File Offset: 0x00006F15
	public override void InitShared()
	{
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = this;
		}
		else
		{
			CommunityEntity.ClientInstance = this;
		}
		base.InitShared();
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x00008D33 File Offset: 0x00006F33
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = null;
			return;
		}
		CommunityEntity.ClientInstance = null;
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0004C044 File Offset: 0x0004A244
	public static void DestroyServerCreatedUI()
	{
		foreach (GameObject gameObject in CommunityEntity.AllUi)
		{
			if (gameObject)
			{
				Object.Destroy(gameObject);
			}
		}
		CommunityEntity.AllUi.Clear();
		CommunityEntity.UiDict.Clear();
		CommunityEntity.requestingTextureImages.Clear();
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0004C0BC File Offset: 0x0004A2BC
	public void SetVisible(bool b)
	{
		Canvas[] componentsInChildren = base.GetComponentsInChildren<Canvas>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(b);
		}
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00008D50 File Offset: 0x00006F50
	private static void RegisterUi(GameObject go)
	{
		CommunityEntity.AllUi.Add(go);
		CommunityEntity.UiDict[go.name] = go;
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x0004C0F0 File Offset: 0x0004A2F0
	[BaseEntity.RPC_Client]
	public void AddUI(BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		Array array = Array.Parse(text);
		if (array == null)
		{
			return;
		}
		foreach (Value value in array)
		{
			Object obj = value.Obj;
			GameObject gameObject = this.FindPanel(obj.GetString("parent", "Overlay"));
			if (gameObject == null)
			{
				Debug.LogWarning("AddUI: Unknown Parent for \"" + obj.GetString("name", "AddUI CreatedPanel") + "\": " + obj.GetString("parent", "Overlay"));
				break;
			}
			GameObject gameObject2 = new GameObject(obj.GetString("name", "AddUI CreatedPanel"));
			gameObject2.transform.SetParent(gameObject.transform, false);
			CommunityEntity.RegisterUi(gameObject2);
			RectTransform component = gameObject2.GetComponent<RectTransform>();
			if (component)
			{
				component.anchorMin = new Vector2(0f, 0f);
				component.anchorMax = new Vector2(1f, 1f);
				component.offsetMin = new Vector2(0f, 0f);
				component.offsetMax = new Vector2(1f, 1f);
			}
			foreach (Value value2 in obj.GetArray("components"))
			{
				this.CreateComponents(gameObject2, value2.Obj);
			}
			if (obj.ContainsKey("fadeOut"))
			{
				gameObject2.AddComponent<CommunityEntity.FadeOut>().duration = obj.GetFloat("fadeOut", 0f);
			}
		}
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0004C2E4 File Offset: 0x0004A4E4
	private GameObject FindPanel(string name)
	{
		GameObject result;
		if (CommunityEntity.UiDict.TryGetValue(name, ref result))
		{
			return result;
		}
		Transform transform = TransformEx.FindChildRecursive(base.transform, name);
		if (transform)
		{
			return transform.gameObject;
		}
		return null;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0004C320 File Offset: 0x0004A520
	private void CreateComponents(GameObject go, Object obj)
	{
		string @string = obj.GetString("type", "UnityEngine.UI.Text");
		uint num = <PrivateImplementationDetails>.ComputeStringHash(@string);
		if (num <= 1466421966U)
		{
			if (num <= 976416075U)
			{
				if (num != 938738728U)
				{
					if (num != 976416075U)
					{
						return;
					}
					if (!(@string == "UnityEngine.UI.Image"))
					{
						return;
					}
					Image image = go.AddComponent<Image>();
					image.sprite = FileSystem.Load<Sprite>(obj.GetString("sprite", "Assets/Content/UI/UI.Background.Tile.psd"), true);
					image.material = FileSystem.Load<Material>(obj.GetString("material", "Assets/Icons/IconMaterial.mat"), true);
					image.color = ColorEx.Parse(obj.GetString("color", "1.0 1.0 1.0 1.0"));
					image.type = (Image.Type)Enum.Parse(typeof(Image.Type), obj.GetString("imagetype", "Simple"));
					if (obj.ContainsKey("png"))
					{
						this.SetImageFromServer(image, uint.Parse(obj.GetString("png", "")));
					}
					this.GraphicComponentCreated(image, obj);
					return;
				}
				else
				{
					if (!(@string == "UnityEngine.UI.Outline"))
					{
						return;
					}
					Outline outline = go.AddComponent<Outline>();
					outline.effectColor = ColorEx.Parse(obj.GetString("color", "1.0 1.0 1.0 1.0"));
					outline.effectDistance = Vector2Ex.Parse(obj.GetString("distance", "1.0 -1.0"));
					outline.useGraphicAlpha = obj.ContainsKey("useGraphicAlpha");
					return;
				}
			}
			else if (num != 1120441549U)
			{
				if (num != 1466421966U)
				{
					return;
				}
				if (!(@string == "UnityEngine.UI.InputField"))
				{
					return;
				}
				Text text = go.AddComponent<Text>();
				text.text = obj.GetString("text", "Text");
				text.fontSize = obj.GetInt("fontSize", 14);
				text.font = FileSystem.Load<Font>("Assets/Content/UI/Fonts/" + obj.GetString("font", "RobotoCondensed-Bold.ttf"), true);
				text.alignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), obj.GetString("align", "UpperLeft"));
				text.color = ColorEx.Parse(obj.GetString("color", "1.0 1.0 1.0 1.0"));
				InputField inputField = go.AddComponent<InputField>();
				inputField.textComponent = text;
				inputField.characterLimit = obj.GetInt("characterLimit", 0);
				if (obj.ContainsKey("command"))
				{
					string cmd = obj.GetString("command", "");
					inputField.onEndEdit.AddListener(delegate(string value)
					{
						ConsoleNetwork.ClientRunOnServer(cmd + " " + value);
					});
				}
				if (obj.ContainsKey("password"))
				{
					inputField.inputType = 2;
				}
				this.GraphicComponentCreated(text, obj);
				return;
			}
			else
			{
				if (!(@string == "UnityEngine.UI.RawImage"))
				{
					return;
				}
				RawImage rawImage = go.AddComponent<RawImage>();
				rawImage.texture = FileSystem.Load<Texture>(obj.GetString("sprite", "Assets/Icons/rust.png"), true);
				rawImage.color = ColorEx.Parse(obj.GetString("color", "1.0 1.0 1.0 1.0"));
				if (obj.ContainsKey("material"))
				{
					rawImage.material = FileSystem.Load<Material>(obj.GetString("material", ""), true);
				}
				if (obj.ContainsKey("url"))
				{
					Global.Runner.StartCoroutine(this.LoadTextureFromWWW(rawImage, obj.GetString("url", "")));
				}
				if (obj.ContainsKey("png"))
				{
					this.SetImageFromServer(rawImage, uint.Parse(obj.GetString("png", "")));
				}
				this.GraphicComponentCreated(rawImage, obj);
				return;
			}
		}
		else
		{
			if (num <= 2471485801U)
			{
				if (num != 1665405120U)
				{
					if (num != 2471485801U)
					{
						return;
					}
					if (!(@string == "RectTransform"))
					{
						return;
					}
					RectTransform component = go.GetComponent<RectTransform>();
					if (component)
					{
						component.anchorMin = Vector2Ex.Parse(obj.GetString("anchormin", "0.0 0.0"));
						component.anchorMax = Vector2Ex.Parse(obj.GetString("anchormax", "1.0 1.0"));
						component.offsetMin = Vector2Ex.Parse(obj.GetString("offsetmin", "0.0 0.0"));
						component.offsetMax = Vector2Ex.Parse(obj.GetString("offsetmax", "1.0 1.0"));
						return;
					}
				}
				else
				{
					if (!(@string == "Countdown"))
					{
						return;
					}
					CommunityEntity.Countdown countdown = go.AddComponent<CommunityEntity.Countdown>();
					countdown.endTime = obj.GetInt("endTime", 0);
					countdown.startTime = obj.GetInt("startTime", 0);
					countdown.step = obj.GetInt("step", 1);
					if (obj.ContainsKey("command"))
					{
						countdown.command = obj.GetString("command", "");
					}
				}
				return;
			}
			if (num != 3307054824U)
			{
				if (num != 4090570613U)
				{
					if (num != 4278175142U)
					{
						return;
					}
					if (!(@string == "UnityEngine.UI.Button"))
					{
						return;
					}
					Button button = go.AddComponent<Button>();
					if (obj.ContainsKey("command"))
					{
						string cmd = obj.GetString("command", "");
						button.onClick.AddListener(delegate()
						{
							ConsoleNetwork.ClientRunOnServer(cmd);
						});
					}
					if (obj.ContainsKey("close"))
					{
						string pnlName = obj.GetString("close", "");
						button.onClick.AddListener(delegate()
						{
							this.DestroyPanel(pnlName);
						});
					}
					Image image2 = go.AddComponent<Image>();
					image2.sprite = FileSystem.Load<Sprite>(obj.GetString("sprite", "Assets/Content/UI/UI.Background.Tile.psd"), true);
					image2.material = FileSystem.Load<Material>(obj.GetString("material", "Assets/Icons/IconMaterial.mat"), true);
					image2.color = ColorEx.Parse(obj.GetString("color", "1.0 1.0 1.0 1.0"));
					image2.type = (Image.Type)Enum.Parse(typeof(Image.Type), obj.GetString("imagetype", "Simple"));
					button.image = image2;
					this.GraphicComponentCreated(image2, obj);
					return;
				}
				else
				{
					if (!(@string == "UnityEngine.UI.Text"))
					{
						return;
					}
					Text text2 = go.AddComponent<Text>();
					text2.text = obj.GetString("text", "Text");
					text2.fontSize = obj.GetInt("fontSize", 14);
					text2.font = FileSystem.Load<Font>("Assets/Content/UI/Fonts/" + obj.GetString("font", "RobotoCondensed-Bold.ttf"), true);
					text2.alignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), obj.GetString("align", "UpperLeft"));
					text2.color = ColorEx.Parse(obj.GetString("color", "1.0 1.0 1.0 1.0"));
					this.GraphicComponentCreated(text2, obj);
					return;
				}
			}
			else
			{
				if (!(@string == "NeedsCursor"))
				{
					return;
				}
				go.AddComponent<NeedsCursor>();
				return;
			}
		}
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00008D6E File Offset: 0x00006F6E
	private void GraphicComponentCreated(Graphic c, Object obj)
	{
		if (obj.ContainsKey("fadeIn"))
		{
			c.canvasRenderer.SetAlpha(0f);
			c.CrossFadeAlpha(1f, obj.GetFloat("fadeIn", 0f), true);
		}
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00008DA9 File Offset: 0x00006FA9
	private IEnumerator LoadTextureFromWWW(RawImage c, string p)
	{
		WWW www = new WWW(p.Trim());
		while (!www.isDone)
		{
			yield return null;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log(string.Concat(new string[]
			{
				"Error downloading image: ",
				p,
				" (",
				www.error,
				")"
			}));
			www.Dispose();
			yield break;
		}
		Texture2D texture = www.texture;
		if (texture == null || c == null)
		{
			Debug.Log("Error downloading image: " + p + " (not an image)");
			www.Dispose();
			yield break;
		}
		c.texture = texture;
		www.Dispose();
		yield break;
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00008DBF File Offset: 0x00006FBF
	[BaseEntity.RPC_Client]
	public void DestroyUI(BaseEntity.RPCMessage msg)
	{
		this.DestroyPanel(msg.read.String());
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0004CA14 File Offset: 0x0004AC14
	private void DestroyPanel(string pnlName)
	{
		GameObject gameObject;
		if (!CommunityEntity.UiDict.TryGetValue(pnlName, ref gameObject))
		{
			return;
		}
		CommunityEntity.UiDict.Remove(pnlName);
		if (!gameObject)
		{
			return;
		}
		CommunityEntity.FadeOut component = gameObject.GetComponent<CommunityEntity.FadeOut>();
		if (component)
		{
			component.FadeOutAndDestroy();
			return;
		}
		Object.Destroy(gameObject);
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x0004CA64 File Offset: 0x0004AC64
	[BaseEntity.RPC_Client]
	public void CL_ReceiveFilePng(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		byte[] array = msg.read.BytesWithSize();
		if (array == null)
		{
			return;
		}
		if (FileStorage.client.Store(array, FileStorage.Type.png, this.net.ID, 0U) != num)
		{
			base.Log("Client/Server FileStorage CRC differs");
		}
		List<MaskableGraphic> list;
		if (!CommunityEntity.requestingTextureImages.TryGetValue(num, ref list))
		{
			return;
		}
		CommunityEntity.requestingTextureImages.Remove(num);
		foreach (MaskableGraphic component in list)
		{
			this.LoadPngIntoGraphic(component, array);
		}
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0004CB14 File Offset: 0x0004AD14
	private void SetImageFromServer(MaskableGraphic component, uint textureID)
	{
		byte[] array = FileStorage.client.Get(textureID, FileStorage.Type.png, this.net.ID);
		if (array == null)
		{
			List<MaskableGraphic> list;
			if (!CommunityEntity.requestingTextureImages.TryGetValue(textureID, ref list))
			{
				list = new List<MaskableGraphic>();
				CommunityEntity.requestingTextureImages[textureID] = list;
				base.RequestFileFromServer(textureID, FileStorage.Type.png, "CL_ReceiveFilePng");
			}
			list.Add(component);
			return;
		}
		this.LoadPngIntoGraphic(component, array);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0004CB7C File Offset: 0x0004AD7C
	private void LoadPngIntoGraphic(MaskableGraphic component, byte[] bytes)
	{
		Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		ImageConversion.LoadImage(texture2D, bytes);
		Image image = component as Image;
		if (image)
		{
			image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
			return;
		}
		RawImage rawImage = component as RawImage;
		if (rawImage)
		{
			rawImage.texture = texture2D;
		}
	}

	// Token: 0x0200008A RID: 138
	private class Countdown : MonoBehaviour
	{
		// Token: 0x04000550 RID: 1360
		public string command = "";

		// Token: 0x04000551 RID: 1361
		public int endTime;

		// Token: 0x04000552 RID: 1362
		public int startTime;

		// Token: 0x04000553 RID: 1363
		public int step = 1;

		// Token: 0x04000554 RID: 1364
		private int sign = 1;

		// Token: 0x04000555 RID: 1365
		private Text textComponent;

		// Token: 0x06000875 RID: 2165 RVA: 0x0004CBFC File Offset: 0x0004ADFC
		private void Start()
		{
			this.textComponent = base.GetComponent<Text>();
			if (this.textComponent)
			{
				this.textComponent.text = this.textComponent.text.Replace("%TIME_LEFT%", this.startTime.ToString());
			}
			if (this.startTime == this.endTime)
			{
				this.End();
			}
			if (this.step == 0)
			{
				this.step = 1;
			}
			if (this.startTime > this.endTime && this.step > 0)
			{
				this.sign = -1;
			}
			base.InvokeRepeating("UpdateCountdown", (float)this.step, (float)this.step);
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0004CCA8 File Offset: 0x0004AEA8
		private void UpdateCountdown()
		{
			this.startTime += this.step * this.sign;
			if (this.textComponent)
			{
				this.textComponent.text = this.textComponent.text.Replace("%TIME_LEFT%", this.startTime.ToString());
			}
			if (this.startTime == this.endTime)
			{
				if (!string.IsNullOrEmpty(this.command))
				{
					ConsoleNetwork.ClientRunOnServer(this.command);
				}
				this.End();
			}
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0004CD34 File Offset: 0x0004AF34
		private void End()
		{
			base.CancelInvoke("UpdateCountdown");
			CommunityEntity.FadeOut component = base.GetComponent<CommunityEntity.FadeOut>();
			if (component)
			{
				component.FadeOutAndDestroy();
				return;
			}
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0200008B RID: 139
	private class FadeOut : MonoBehaviour
	{
		// Token: 0x04000556 RID: 1366
		public float duration;

		// Token: 0x06000879 RID: 2169 RVA: 0x0004CD70 File Offset: 0x0004AF70
		public void FadeOutAndDestroy()
		{
			base.Invoke("Kill", this.duration + 0.1f);
			Graphic[] components = base.gameObject.GetComponents<Graphic>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].CrossFadeAlpha(0f, this.duration, false);
			}
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x00008E27 File Offset: 0x00007027
		public void Kill()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
