using System;
using ProtoBuf;

// Token: 0x0200029E RID: 670
public class CrossbowWeapon : BaseProjectile
{
	// Token: 0x04000F84 RID: 3972
	private SwapArrows swapArrows;

	// Token: 0x060012DF RID: 4831 RVA: 0x0001029F File Offset: 0x0000E49F
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.swapArrows = base.GetComponent<SwapArrows>();
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x0007A310 File Offset: 0x00078510
	public override void OnInput()
	{
		if (this.viewModel)
		{
			this.viewModel.SetBool("loaded", this.primaryMagazine.contents > 0);
		}
		if (this.worldModelAnimator != null)
		{
			if (this.primaryMagazine.contents > 0)
			{
				this.worldModelAnimator.SetFloat("loaded", 1f);
			}
			else
			{
				this.worldModelAnimator.SetFloat("loaded", 0f);
			}
		}
		base.OnInput();
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x0007A398 File Offset: 0x00078598
	public override void OnFrame()
	{
		base.OnFrame();
		if (this.viewModel && this.viewModel.instance)
		{
			SwapArrows component = this.viewModel.instance.GetComponent<SwapArrows>();
			if (component)
			{
				component.UpdateAmmoType(this.primaryMagazine.ammoType);
			}
		}
		if (this.swapArrows)
		{
			this.swapArrows.UpdateAmmoType(this.primaryMagazine.ammoType);
		}
	}
}
