using System;
using UnityEngine;

// Token: 0x0200029D RID: 669
public class Binocular : AttackEntity
{
	// Token: 0x04000F7D RID: 3965
	public float[] fovs;

	// Token: 0x04000F7E RID: 3966
	public GameObjectRef fovChangeEffect;

	// Token: 0x04000F7F RID: 3967
	private bool vmVisible = true;

	// Token: 0x04000F80 RID: 3968
	private bool isAiming;

	// Token: 0x04000F81 RID: 3969
	private float timeAiming;

	// Token: 0x04000F82 RID: 3970
	private int currentFovIndex;

	// Token: 0x04000F83 RID: 3971
	public float smoothSpeed = 0.05f;

	// Token: 0x060012D7 RID: 4823 RVA: 0x00010249 File Offset: 0x0000E449
	public override void OnDeploy()
	{
		this.ClearOverlays();
		this.isAiming = false;
		base.OnDeploy();
		if (this.viewModel)
		{
			this.viewModel.ironSights = false;
		}
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x00010277 File Offset: 0x0000E477
	public override void OnHolster()
	{
		this.ClearOverlays();
		base.OnHolster();
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x00079FAC File Offset: 0x000781AC
	public void ClearOverlays()
	{
		UIBlackoutOverlay uiblackoutOverlay = UIBlackoutOverlay.Get(UIBlackoutOverlay.blackoutType.FULLBLACK);
		UIBlackoutOverlay uiblackoutOverlay2 = UIBlackoutOverlay.Get(UIBlackoutOverlay.blackoutType.BINOCULAR);
		uiblackoutOverlay.SetAlpha(0f);
		uiblackoutOverlay2.SetAlpha(0f);
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x00079FDC File Offset: 0x000781DC
	public override void OnFrame()
	{
		base.OnFrame();
		if (this.isAiming)
		{
			this.timeAiming += Time.deltaTime;
		}
		else
		{
			this.timeAiming = 0f;
		}
		float b = (this.isAiming && this.timeAiming < 0.5f) ? 1f : 0f;
		float b2 = (this.isAiming && this.timeAiming > 0.25f) ? 1f : 0f;
		bool viewmodelVisiblity = !this.isAiming || this.timeAiming < 0.5f;
		this.SetViewmodelVisiblity(viewmodelVisiblity);
		UIBlackoutOverlay uiblackoutOverlay = UIBlackoutOverlay.Get(UIBlackoutOverlay.blackoutType.FULLBLACK);
		UIBlackoutOverlay uiblackoutOverlay2 = UIBlackoutOverlay.Get(UIBlackoutOverlay.blackoutType.BINOCULAR);
		uiblackoutOverlay.SetAlpha(Mathf.Lerp(uiblackoutOverlay.GetAlpha(), b, Time.deltaTime * 20f));
		uiblackoutOverlay2.SetAlpha(Mathf.Lerp(uiblackoutOverlay2.GetAlpha(), b2, Time.deltaTime * 20f));
		if (this.viewModel && this.viewModel.instance)
		{
			float num = this.fovs[this.currentFovIndex];
			if (this.viewModel.instance.ironSights.fieldOfViewOffset != num)
			{
				this.viewModel.instance.ironSights.fieldOfViewOffset = Mathf.MoveTowards(this.viewModel.instance.ironSights.fieldOfViewOffset, num, Time.deltaTime * 100f);
			}
		}
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x0007A144 File Offset: 0x00078344
	public Vector3 GetScreenPos(Vector3 oldPos)
	{
		if (this.viewModel.instance == null)
		{
			return Vector3.zero;
		}
		Transform targetPoint = this.viewModel.instance.GetComponent<IronSights>().aimPoint.targetPoint;
		Vector3 vector = MainCamera.mainCamera.WorldToScreenPoint(targetPoint.transform.position);
		float num = 1f;
		Vector3 zero = Vector3.zero;
		return Vector3.SmoothDamp(oldPos, new Vector3(vector.x * num, vector.y * num, 0f), ref zero, this.smoothSpeed);
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x0007A1D0 File Offset: 0x000783D0
	public void SetViewmodelVisiblity(bool visible)
	{
		if (visible == this.vmVisible || this.viewModel.instance == null)
		{
			return;
		}
		Renderer[] componentsInChildren = this.viewModel.instance.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = visible;
		}
		this.vmVisible = visible;
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x0007A22C File Offset: 0x0007842C
	public override void OnInput()
	{
		base.OnInput();
		if (!this.IsFullyDeployed())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		bool flag = ownerPlayer.CanAttack();
		bool flag2 = ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY) && flag;
		if (this.viewModel)
		{
			this.viewModel.ironSights = flag2;
			ownerPlayer.modelState.aiming = flag2;
			if (flag2 && ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
			{
				this.currentFovIndex++;
				if (this.currentFovIndex >= this.fovs.Length)
				{
					this.currentFovIndex = 0;
				}
				Effect.client.Run(this.fovChangeEffect.resourcePath, LocalPlayer.Entity.eyes.gameObject);
			}
		}
		if (flag2)
		{
			ownerPlayer.BlockSprint(0.2f);
		}
		this.isAiming = flag2;
	}
}
