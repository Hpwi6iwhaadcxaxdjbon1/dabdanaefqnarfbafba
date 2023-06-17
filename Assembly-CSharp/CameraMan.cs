using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class CameraMan : SingletonComponent<CameraMan>
{
	// Token: 0x04000C31 RID: 3121
	public bool OnlyControlWhenCursorHidden = true;

	// Token: 0x04000C32 RID: 3122
	public bool NeedBothMouseButtonsToZoom;

	// Token: 0x04000C33 RID: 3123
	public float LookSensitivity = 1f;

	// Token: 0x04000C34 RID: 3124
	public float MoveSpeed = 1f;

	// Token: 0x04000C35 RID: 3125
	private bool startCulling;

	// Token: 0x04000C36 RID: 3126
	private Vector3 view;

	// Token: 0x04000C37 RID: 3127
	private Vector3 velocity;

	// Token: 0x04000C38 RID: 3128
	private float zoom = 0.5f;

	// Token: 0x06000F17 RID: 3863 RVA: 0x0000D832 File Offset: 0x0000BA32
	private void Start()
	{
		base.transform.position = MainCamera.mainCamera.transform.position;
		base.transform.rotation = MainCamera.mainCamera.transform.rotation;
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x00067974 File Offset: 0x00065B74
	private void OnEnable()
	{
		this.startCulling = PlayerCull.enabled;
		PlayerCull.enabled = false;
		if (LocalPlayer.Entity)
		{
			LocalPlayer.Entity.Frozen = true;
		}
		base.transform.position = MainCamera.mainCamera.transform.position;
		base.transform.rotation = MainCamera.mainCamera.transform.rotation;
		this.view = base.transform.rotation.eulerAngles;
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x0000D868 File Offset: 0x0000BA68
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		PlayerCull.enabled = this.startCulling;
		if (LocalPlayer.Entity)
		{
			LocalPlayer.Entity.Frozen = false;
		}
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x0000D894 File Offset: 0x0000BA94
	public void TogglePlayerFreeze()
	{
		if (!LocalPlayer.Entity)
		{
			return;
		}
		LocalPlayer.Entity.Frozen = !LocalPlayer.Entity.Frozen;
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000F1B RID: 3867 RVA: 0x0000D8BA File Offset: 0x0000BABA
	public static bool Active
	{
		get
		{
			return SingletonComponent<CameraMan>.Instance && SingletonComponent<CameraMan>.Instance.enabled;
		}
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x000679F8 File Offset: 0x00065BF8
	private void Update()
	{
		if (MainCamera.mainCamera == null)
		{
			return;
		}
		if (this.OnlyControlWhenCursorHidden && Cursor.visible)
		{
			return;
		}
		if (ConVar.Client.camlerp >= 1f)
		{
			base.transform.position = MainCamera.mainCamera.transform.position;
			base.transform.rotation = MainCamera.mainCamera.transform.rotation;
		}
		this.zoom = Mathf.InverseLerp(1f, 120f, MainCamera.mainCamera.fieldOfView);
		this.DoControls();
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x00067A88 File Offset: 0x00065C88
	private void DoControls()
	{
		if (LocalPlayer.Entity == null || LocalPlayer.Entity.Frozen)
		{
			if (Buttons.Attack2.IsDown && (!this.NeedBothMouseButtonsToZoom || Buttons.Attack.IsDown))
			{
				this.zoom = Mathf.Clamp(this.zoom + UnityEngine.Input.GetAxisRaw("Mouse Y") * -0.033f, 0.01f, 1f);
			}
			else if (Buttons.Duck.IsDown)
			{
				this.view.z = this.view.z + UnityEngine.Input.GetAxisRaw("Mouse X") * 1.5f;
			}
			else
			{
				this.view.y = this.view.y + UnityEngine.Input.GetAxisRaw("Mouse X") * 3f * this.zoom * this.LookSensitivity;
				this.view.x = this.view.x - UnityEngine.Input.GetAxisRaw("Mouse Y") * 3f * this.zoom * this.LookSensitivity;
			}
			base.transform.rotation = Quaternion.Euler(this.view);
			Vector3 vector = Vector3.zero;
			if (Buttons.Forward.IsDown)
			{
				vector += Vector3.forward;
			}
			if (Buttons.Backward.IsDown)
			{
				vector += Vector3.back;
			}
			if (Buttons.Left.IsDown)
			{
				vector += Vector3.left;
			}
			if (Buttons.Right.IsDown)
			{
				vector += Vector3.right;
			}
			vector.Normalize();
			if (Buttons.Sprint.IsDown)
			{
				vector *= 5f;
			}
			if (Buttons.Jump.IsDown)
			{
				vector *= 0.2f;
			}
			this.velocity += base.transform.rotation * vector * 0.05f * this.MoveSpeed * ConVar.Client.camspeed;
			this.velocity = Vector3.Lerp(this.velocity, Vector3.zero, 0.2f);
			base.transform.position += this.velocity;
		}
		else
		{
			Vector3 forward = LocalPlayer.Entity.transform.position - base.transform.position + Vector3.up;
			base.transform.rotation = Quaternion.LookRotation(forward);
		}
		MainCamera.mainCamera.transform.position = Vector3.Lerp(MainCamera.mainCamera.transform.position, base.transform.position, ConVar.Client.camlerp);
		MainCamera.mainCamera.transform.rotation = Quaternion.Slerp(MainCamera.mainCamera.transform.rotation, base.transform.rotation, ConVar.Client.camlerp);
		MainCamera.mainCamera.fieldOfView = Mathf.Lerp(1f, 120f, this.zoom);
	}
}
