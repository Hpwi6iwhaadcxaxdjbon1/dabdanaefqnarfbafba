using System;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class PlayerInput : EntityComponent<BasePlayer>
{
	// Token: 0x04001165 RID: 4453
	public InputState state = new InputState();

	// Token: 0x04001166 RID: 4454
	[NonSerialized]
	public bool hadInputBuffer = true;

	// Token: 0x04001167 RID: 4455
	[NonSerialized]
	private Quaternion bodyRotation;

	// Token: 0x04001168 RID: 4456
	[NonSerialized]
	private Vector3 bodyAngles;

	// Token: 0x04001169 RID: 4457
	[NonSerialized]
	private Quaternion headRotation;

	// Token: 0x0400116A RID: 4458
	[NonSerialized]
	private Vector3 headAngles;

	// Token: 0x0400116B RID: 4459
	[NonSerialized]
	public Vector3 recoilAngles;

	// Token: 0x0400116C RID: 4460
	[NonSerialized]
	public Vector2 viewDelta;

	// Token: 0x0400116D RID: 4461
	[NonSerialized]
	public Vector3 headDelta;

	// Token: 0x0400116E RID: 4462
	private int mouseWheelUp;

	// Token: 0x0400116F RID: 4463
	private int mouseWheelDn;

	// Token: 0x04001170 RID: 4464
	public float mouseX;

	// Token: 0x04001171 RID: 4465
	public float mouseY;

	// Token: 0x04001172 RID: 4466
	internal TrackIR trackir;

	// Token: 0x04001173 RID: 4467
	public Vector3 offsetAngles = Vector3.zero;

	// Token: 0x04001174 RID: 4468
	private int ignoredButtons;

	// Token: 0x04001175 RID: 4469
	private bool hasKeyFocus;

	// Token: 0x06001490 RID: 5264 RVA: 0x0001185A File Offset: 0x0000FA5A
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.state.Clear();
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x0001186F File Offset: 0x0000FA6F
	protected void OnDestroy()
	{
		if (this.trackir != null)
		{
			this.trackir.Shutdown();
		}
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x0007EFA8 File Offset: 0x0007D1A8
	public void FrameUpdate()
	{
		Facepunch.Input.Frame();
		this.Flip();
		this.viewDelta = Vector2.zero;
		if (Cursor.visible)
		{
			return;
		}
		if (Cursor.lockState == CursorLockMode.None)
		{
			return;
		}
		float num = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
		if (NeedsMouseWheel.AnyActive())
		{
			num = 0f;
		}
		if (this.mouseWheelUp == 0 && num > 0f)
		{
			this.mouseWheelUp = 1;
		}
		if (this.mouseWheelDn == 0 && num < 0f)
		{
			this.mouseWheelDn = 1;
		}
		this.UpdateViewAngles();
		if (!UnityEngine.Input.anyKeyDown)
		{
			this.hadInputBuffer = true;
		}
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x0007F038 File Offset: 0x0007D238
	private void Flip()
	{
		this.UpdateKeyFocus(!this.hasOnlyPartialKeyInput && !PlayerInput.hasNoKeyInput);
		using (InputMessage inputMessage = this.BuildInputState(true))
		{
			this.state.Flip(inputMessage);
		}
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x0007F090 File Offset: 0x0007D290
	private void AdjustMouseMovement(ref Vector2 mm)
	{
		if (ConVar.Input.flipy)
		{
			mm.y = -mm.y;
		}
		if (base.baseEntity.IsWounded())
		{
			mm *= 0.01f;
			return;
		}
		float d = ConVar.Input.sensitivity * 3f * (MainCamera.mainCamera.fieldOfView / 100f);
		mm *= d;
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x0007F104 File Offset: 0x0007D304
	private void UpdateViewAngles()
	{
		if (MainCamera.mainCamera == null)
		{
			return;
		}
		Vector2 vector;
		vector.x = UnityEngine.Input.GetAxis("Mouse X");
		vector.y = UnityEngine.Input.GetAxis("Mouse Y");
		this.AdjustMouseMovement(ref vector);
		this.viewDelta.x = vector.x;
		this.viewDelta.y = vector.y;
		if (this.trackir == null)
		{
			this.trackir = new TrackIR(20430, (long)((ulong)-137431073), (long)((ulong)-1456208425), 119, false);
			try
			{
				this.trackir.Init();
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Couldn't connect to TrackIR: " + ex);
			}
		}
		TrackIR.LPTRACKIRDATA data = this.trackir.GetData();
		Vector3 vector2;
		vector2.x = -data.fNPX;
		vector2.y = data.fNPY;
		vector2.z = -data.fNPZ;
		Vector3 vector3;
		vector3.y = -data.fNPYaw;
		vector3.x = data.fNPPitch;
		vector3.z = data.fNPRoll;
		this.headDelta.x = vector3.x * 0.01f;
		this.headDelta.y = vector3.y * 0.01f;
		this.headDelta.z = vector3.z * 0.01f;
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x0007F268 File Offset: 0x0007D468
	public void ApplyViewAngles()
	{
		if (Buttons.AltLook.IsDown && !Buttons.Attack2.IsDown)
		{
			this.headAngles.x = this.headAngles.x - this.viewDelta.y;
			this.headAngles.y = this.headAngles.y + this.viewDelta.x;
			this.headAngles.z = 0f;
			this.viewDelta = Vector2.zero;
		}
		else
		{
			this.headAngles = Vector3.Slerp(this.headAngles, Vector3.zero, UnityEngine.Time.smoothDeltaTime * 20f);
		}
		if (base.baseEntity.isMounted)
		{
			this.offsetAngles.x = this.offsetAngles.x - this.viewDelta.y;
			this.offsetAngles.y = this.offsetAngles.y + this.viewDelta.x;
			BaseMountable mounted = base.baseEntity.GetMounted();
			this.offsetAngles.x = Mathf.Clamp(this.offsetAngles.x, mounted.pitchClamp.x, mounted.pitchClamp.y);
			this.offsetAngles.y = Mathf.Clamp(this.offsetAngles.y, mounted.yawClamp.x, mounted.yawClamp.y);
			Quaternion rhs = Quaternion.Euler(this.recoilAngles);
			Quaternion rhs2 = Quaternion.Euler(this.offsetAngles.x, this.offsetAngles.y, 0f);
			this.bodyAngles = BaseMountable.ConvertVector((Quaternion.Euler(base.baseEntity.GetMounted().transform.eulerAngles) * rhs2 * rhs).eulerAngles);
		}
		else
		{
			this.bodyAngles.x = this.bodyAngles.x - this.viewDelta.y;
			this.bodyAngles.y = this.bodyAngles.y + this.viewDelta.x;
			this.bodyAngles.z = 0f;
		}
		if (this.headDelta != Vector3.zero)
		{
			this.headAngles.x = this.headDelta.x;
			this.headAngles.y = this.headDelta.y;
			this.headAngles.z = this.headDelta.z;
		}
		this.headAngles.x = Mathf.Clamp(this.headAngles.x, -80f, 80f);
		this.headAngles.y = Mathf.Clamp(this.headAngles.y, -140f, 140f);
		this.headAngles.z = Mathf.Clamp(this.headAngles.z, -80f, 80f);
		this.bodyAngles.x = Mathf.Clamp(this.bodyAngles.x, -89f, 89f);
		this.bodyAngles.y = this.bodyAngles.y % 360f;
		Vector3 vector = this.bodyAngles + (base.baseEntity.isMounted ? Vector3.zero : this.recoilAngles);
		vector.x = Mathf.Clamp(vector.x, -89f, 89f);
		vector.y %= 360f;
		this.bodyRotation = Quaternion.Euler(vector);
		this.headRotation = Quaternion.Euler(0f, this.headAngles.y, 0f) * Quaternion.Euler(0f, 0f, this.headAngles.z) * Quaternion.Euler(this.headAngles.x, 0f, 0f);
		this.viewDelta = Vector2.zero;
		this.headDelta = Vector2.zero;
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x00011884 File Offset: 0x0000FA84
	public void SetViewVars(Vector3 newAngles)
	{
		this.bodyAngles = newAngles;
		this.bodyRotation = Quaternion.Euler(this.bodyAngles);
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x0007F65C File Offset: 0x0007D85C
	public void FinalizeRecoil()
	{
		this.SetViewVars(this.ClientLookVars() + this.recoilAngles);
		Quaternion lhs = Quaternion.Euler(this.offsetAngles);
		Quaternion rhs = Quaternion.Euler(this.recoilAngles);
		this.offsetAngles = BaseMountable.ConvertVector((lhs * rhs).eulerAngles);
		this.recoilAngles = Vector3.zero;
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x0001189E File Offset: 0x0000FA9E
	public Quaternion ClientAngles()
	{
		return this.bodyRotation;
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x000118A6 File Offset: 0x0000FAA6
	public Vector3 ClientLookVars()
	{
		return this.bodyAngles;
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x000118AE File Offset: 0x0000FAAE
	public Quaternion HeadAngles()
	{
		return this.headRotation;
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x000118B6 File Offset: 0x0000FAB6
	public Vector3 HeadLookVars()
	{
		return this.headAngles;
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x0007F6C0 File Offset: 0x0007D8C0
	private void ModifyInputState(InputMessage state)
	{
		if (this.hasOnlyPartialKeyInput)
		{
			state.buttons &= -1025;
			state.buttons &= -2049;
			state.buttons &= -257;
		}
		if (PlayerInput.hasNoKeyInput)
		{
			state.buttons = 0;
		}
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x0007F71C File Offset: 0x0007D91C
	internal bool IsButtonDownNative(BUTTON btn)
	{
		if (NeedsKeyboard.AnyActive())
		{
			return false;
		}
		if (btn <= BUTTON.DUCK)
		{
			if (btn <= BUTTON.LEFT)
			{
				if (btn == BUTTON.FORWARD)
				{
					return Buttons.Forward.IsDown;
				}
				if (btn == BUTTON.BACKWARD)
				{
					return Buttons.Backward.IsDown;
				}
				if (btn == BUTTON.LEFT)
				{
					return Buttons.Left.IsDown;
				}
			}
			else
			{
				if (btn == BUTTON.RIGHT)
				{
					return Buttons.Right.IsDown;
				}
				if (btn == BUTTON.JUMP)
				{
					return Buttons.Jump.IsDown;
				}
				if (btn == BUTTON.DUCK)
				{
					return Buttons.Duck.IsDown;
				}
			}
		}
		else if (btn <= BUTTON.FIRE_PRIMARY)
		{
			if (btn == BUTTON.SPRINT)
			{
				return Buttons.Sprint.IsDown;
			}
			if (btn == BUTTON.USE)
			{
				return Buttons.Use.IsDown;
			}
			if (btn == BUTTON.FIRE_PRIMARY)
			{
				return Buttons.Attack.IsDown;
			}
		}
		else
		{
			if (btn == BUTTON.FIRE_SECONDARY)
			{
				return Buttons.Attack2.IsDown;
			}
			if (btn == BUTTON.RELOAD)
			{
				return Buttons.Reload.IsDown;
			}
			if (btn == BUTTON.FIRE_THIRD)
			{
				return Buttons.Attack3.IsDown;
			}
		}
		return false;
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0007F834 File Offset: 0x0007DA34
	private InputMessage BuildInputState(bool allowModify = true)
	{
		InputMessage inputMessage = Pool.Get<InputMessage>();
		inputMessage.aimAngles = this.bodyRotation.eulerAngles;
		inputMessage.buttons = 0;
		float num = base.baseEntity.isMounted ? ConVar.Input.vehicle_sensitivity : ConVar.Input.sensitivity;
		float num2 = (base.baseEntity.isMounted ? ConVar.Input.vehicle_flipy : ConVar.Input.flipy) ? -1f : 1f;
		inputMessage.mouseDelta.x = ((!Buttons.AltLook.IsDown && !Cursor.visible) ? (UnityEngine.Input.GetAxis("Mouse X") * num) : 0f);
		inputMessage.mouseDelta.y = ((!Buttons.AltLook.IsDown && !Cursor.visible) ? (UnityEngine.Input.GetAxis("Mouse Y") * num * num2) : 0f);
		Facepunch.Input.Update();
		this.UpdateButton(inputMessage, BUTTON.FORWARD);
		this.UpdateButton(inputMessage, BUTTON.BACKWARD);
		this.UpdateButton(inputMessage, BUTTON.LEFT);
		this.UpdateButton(inputMessage, BUTTON.RIGHT);
		this.UpdateButton(inputMessage, BUTTON.JUMP);
		this.UpdateButton(inputMessage, BUTTON.DUCK);
		this.UpdateButton(inputMessage, BUTTON.SPRINT);
		this.UpdateButton(inputMessage, BUTTON.USE);
		this.UpdateButton(inputMessage, BUTTON.FIRE_PRIMARY);
		this.UpdateButton(inputMessage, BUTTON.FIRE_SECONDARY);
		this.UpdateButton(inputMessage, BUTTON.RELOAD);
		this.UpdateButton(inputMessage, BUTTON.FIRE_THIRD);
		if (this.mouseWheelUp == 2)
		{
			this.mouseWheelUp = 0;
		}
		if (this.mouseWheelDn == 2)
		{
			this.mouseWheelDn = 0;
		}
		if (this.mouseWheelUp == 1)
		{
			this.mouseWheelUp++;
		}
		if (this.mouseWheelDn == 1)
		{
			this.mouseWheelDn++;
		}
		if (allowModify)
		{
			this.ModifyInputState(inputMessage);
		}
		return inputMessage;
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x000118BE File Offset: 0x0000FABE
	private void UpdateButton(InputMessage state, BUTTON btn)
	{
		if (this.IsButtonDownNative(btn))
		{
			if ((this.ignoredButtons & (int)btn) == 0)
			{
				state.buttons |= (int)btn;
				return;
			}
		}
		else if ((this.ignoredButtons & (int)btn) != 0)
		{
			this.ignoredButtons &= (int)(~(int)btn);
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x0007F9E0 File Offset: 0x0007DBE0
	public void IgnoreCurrentButtons()
	{
		int num = (this.state.current != null) ? this.state.current.buttons : 0;
		using (InputMessage inputMessage = this.BuildInputState(false))
		{
			this.ignoredButtons |= (inputMessage.buttons & ~num);
		}
		this.Flip();
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x060014A2 RID: 5282 RVA: 0x000118FB File Offset: 0x0000FAFB
	private bool hasOnlyPartialKeyInput
	{
		get
		{
			return !LocalPlayer.isSleeping && Cursor.visible;
		}
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x060014A3 RID: 5283 RVA: 0x00011910 File Offset: 0x0000FB10
	public static bool hasNoKeyInput
	{
		get
		{
			return !LocalPlayer.isSleeping && (NeedsKeyboard.AnyActive() || HudMenuInput.AnyActive() || UIChat.isOpen || DeveloperTools.isOpen || MainMenuSystem.isOpen);
		}
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x00011949 File Offset: 0x0000FB49
	private void UpdateKeyFocus(bool hasFocus)
	{
		if (this.hasKeyFocus == hasFocus)
		{
			return;
		}
		this.hasKeyFocus = hasFocus;
		if (this.hasKeyFocus)
		{
			this.IgnoreCurrentButtons();
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0007FA50 File Offset: 0x0007DC50
	public static void IgnoreCurrentKeys()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (LocalPlayer.Entity.input == null)
		{
			return;
		}
		LocalPlayer.Entity.input.IgnoreCurrentButtons();
		LocalPlayer.Entity.input.hadInputBuffer = false;
	}
}
