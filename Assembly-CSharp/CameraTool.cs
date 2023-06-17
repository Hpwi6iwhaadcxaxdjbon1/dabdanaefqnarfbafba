using System;
using Network;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class CameraTool : HeldEntity
{
	// Token: 0x0400051A RID: 1306
	public GameObjectRef screenshotEffect;

	// Token: 0x0400051B RID: 1307
	[NonSerialized]
	public float cameraFOV;

	// Token: 0x0400051C RID: 1308
	[NonSerialized]
	public float focalDistance;

	// Token: 0x0400051D RID: 1309
	internal float focalDistanceSmooth;

	// Token: 0x06000831 RID: 2097 RVA: 0x0004AEF0 File Offset: 0x000490F0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CameraTool.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0004AF34 File Offset: 0x00049134
	public override void EditViewAngles()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY))
		{
			this.cameraFOV += ownerPlayer.input.viewDelta.y * -0.3f;
			this.cameraFOV = Mathf.Clamp(this.cameraFOV, -30f, 10f);
			ownerPlayer.input.viewDelta = Vector2.zero;
		}
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0004AFB8 File Offset: 0x000491B8
	public override void OnFrame()
	{
		base.OnFrame();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.input.state.IsDown(BUTTON.FIRE_THIRD))
		{
			this.focalDistance = Mathf.SmoothDamp(this.focalDistance, MainCamera.Distance(LocalPlayer.Entity.lookingAtPoint), ref this.focalDistanceSmooth, 0.2f);
		}
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0004B020 File Offset: 0x00049220
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
		{
			Client.Steam.Screenshots.Trigger();
		}
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0004B06C File Offset: 0x0004926C
	public override void ModifyCamera()
	{
		MainCamera.mainCamera.fieldOfView += this.cameraFOV;
		MainCamera.depthOfField.wants = true;
		MainCamera.depthOfField.focalDistance = this.focalDistance;
		MainCamera.depthOfField.focalSize = 0.05f;
		MainCamera.depthOfField.blurSize = 3f;
	}
}
