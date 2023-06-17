using System;
using ConVar;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200063D RID: 1597
public class UIHUD : SingletonComponent<UIHUD>, IUIScreen
{
	// Token: 0x04001FAE RID: 8110
	public UIChat chatPanel;

	// Token: 0x04001FAF RID: 8111
	public HudElement Hunger;

	// Token: 0x04001FB0 RID: 8112
	public HudElement Thirst;

	// Token: 0x04001FB1 RID: 8113
	public HudElement Health;

	// Token: 0x04001FB2 RID: 8114
	public HudElement PendingHealth;

	// Token: 0x04001FB3 RID: 8115
	public HudElement VehicleHealth;

	// Token: 0x04001FB4 RID: 8116
	public RawImage compassStrip;

	// Token: 0x04001FB5 RID: 8117
	public CanvasGroup compassGroup;

	// Token: 0x04001FB6 RID: 8118
	public RectTransform vitalsRect;

	// Token: 0x04001FB7 RID: 8119
	protected CanvasGroup canvasGroup;

	// Token: 0x04001FB8 RID: 8120
	private bool visible;

	// Token: 0x04001FB9 RID: 8121
	private Material compassStripMaterial;

	// Token: 0x04001FBA RID: 8122
	private Material compassStripMaterialInst;

	// Token: 0x04001FBB RID: 8123
	private bool compassToggle;

	// Token: 0x0600239E RID: 9118 RVA: 0x000BCD3C File Offset: 0x000BAF3C
	private void Update()
	{
		bool flag = this.shouldShowHud && this.visible;
		using (TimeWarning.New("Update Visibility", 0.1f))
		{
			this.canvasGroup.alpha = (flag ? 1f : 0f);
			this.canvasGroup.interactable = flag;
			this.canvasGroup.blocksRaycasts = flag;
		}
		if (this.chatPanel && ConVar.Graphics.chat != this.chatPanel.gameObject.activeSelf)
		{
			this.chatPanel.gameObject.SetActive(ConVar.Graphics.chat);
		}
		if (flag)
		{
			using (TimeWarning.New("UpdateVitals", 0.1f))
			{
				this.UpdateVitals();
			}
		}
		if (LocalPlayer.Entity != null)
		{
			bool flag2 = false;
			if (ConVar.Graphics.compass == 1)
			{
				flag2 = true;
			}
			else if (ConVar.Graphics.compass == 2 && Buttons.Compass.IsDown)
			{
				flag2 = true;
			}
			else if (ConVar.Graphics.compass == 3)
			{
				this.compassToggle = (Buttons.Compass.JustPressed ? (!this.compassToggle) : this.compassToggle);
				flag2 = this.compassToggle;
			}
			if (UIInventory.isOpen || UICrafting.isOpen)
			{
				flag2 = false;
				this.compassGroup.alpha = 0f;
			}
			float target = flag2 ? 1f : 0f;
			this.compassGroup.alpha = Mathf.MoveTowards(this.compassGroup.alpha, target, UnityEngine.Time.deltaTime * 6f);
			Vector3 vector = LocalPlayer.Entity.eyes.HeadForward();
			vector.y = 0f;
			vector.Normalize();
			float z = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector.x, -vector.z) * 57.29578f + 90f).eulerAngles.z;
			this.compassStripMaterialInst.CopyPropertiesFromMaterial(this.compassStripMaterial);
			this.compassStripMaterialInst.SetFloat("_CompassScroll", z / 360f);
		}
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x000BCF70 File Offset: 0x000BB170
	private void UpdateVitals()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		this.Hunger.SetValue(LocalPlayer.Entity.metabolism.calories.value, LocalPlayer.Entity.metabolism.calories.max);
		this.Thirst.SetValue(LocalPlayer.Entity.metabolism.hydration.value, LocalPlayer.Entity.metabolism.hydration.max);
		this.Health.SetValue(LocalPlayer.Entity.health, LocalPlayer.Entity.MaxHealth());
		bool flag = false;
		if (LocalPlayer.Entity.isMounted)
		{
			BaseVehicle mountedVehicle = LocalPlayer.Entity.GetMountedVehicle();
			if (mountedVehicle != null && mountedVehicle.shouldShowHudHealth)
			{
				this.VehicleHealth.SetValue(mountedVehicle.health, mountedVehicle.MaxHealth());
				flag = true;
			}
		}
		this.VehicleHealth.gameObject.SetActive(flag);
		this.vitalsRect.offsetMin = new Vector2(this.vitalsRect.offsetMin.x, flag ? 108f : 80f);
		this.PendingHealth.SetValue(LocalPlayer.Entity.health + LocalPlayer.Entity.metabolism.pending_health.value, LocalPlayer.Entity.MaxHealth());
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x060023A0 RID: 9120 RVA: 0x000BD0C8 File Offset: 0x000BB2C8
	private bool shouldShowHud
	{
		get
		{
			bool result;
			using (TimeWarning.New("shouldShowHud", 0.1f))
			{
				if (!ConVar.Graphics.hud)
				{
					result = false;
				}
				else if (SingletonComponent<CameraMan>.Instance && SingletonComponent<CameraMan>.Instance.isActiveAndEnabled)
				{
					result = false;
				}
				else if (!LevelManager.isLoaded)
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}
	}

	// Token: 0x060023A1 RID: 9121 RVA: 0x0001C321 File Offset: 0x0001A521
	protected override void Awake()
	{
		base.Awake();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		MapInterface.SetOpen(false);
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000BD138 File Offset: 0x000BB338
	private void OnEnable()
	{
		this.compassStripMaterialInst = new Material(this.compassStrip.material)
		{
			hideFlags = HideFlags.DontSave
		};
		this.compassStripMaterial = this.compassStrip.material;
		this.compassStrip.material = this.compassStripMaterialInst;
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x0001C33B File Offset: 0x0001A53B
	private void OnDisable()
	{
		Object.DestroyImmediate(this.compassStripMaterialInst);
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x0001C348 File Offset: 0x0001A548
	public void SetVisible(bool b)
	{
		this.visible = b;
	}
}
