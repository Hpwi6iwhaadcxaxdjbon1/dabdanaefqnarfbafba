using System;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class AnimatedBuildingBlock : StabilityEntity
{
	// Token: 0x04000FA3 RID: 4003
	private bool animatorNeedsInitializing = true;

	// Token: 0x04000FA4 RID: 4004
	private bool animatorIsOpen = true;

	// Token: 0x04000FA5 RID: 4005
	private bool isAnimating;

	// Token: 0x0600133D RID: 4925 RVA: 0x000105B5 File Offset: 0x0000E7B5
	public override void ClientOnEnable()
	{
		base.ClientOnEnable();
		this.UpdateAnimationParameters(true);
	}

	// Token: 0x0600133E RID: 4926 RVA: 0x000105C4 File Offset: 0x0000E7C4
	private void SetBatching(bool state)
	{
		this.model.animator.gameObject.BroadcastBatchingToggle(state);
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x000105DC File Offset: 0x0000E7DC
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		this.UpdateAnimationParameters(false);
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x0007B2B8 File Offset: 0x000794B8
	protected void UpdateAnimationParameters(bool init)
	{
		if (!this.model)
		{
			return;
		}
		if (!this.model.animator)
		{
			return;
		}
		if (!this.model.animator.isInitialized)
		{
			return;
		}
		bool flag = this.animatorNeedsInitializing || this.animatorIsOpen != base.IsOpen() || (init && this.isAnimating);
		bool flag2 = this.animatorNeedsInitializing || init;
		if (flag)
		{
			this.isAnimating = true;
			this.model.animator.enabled = true;
			this.model.animator.SetBool("open", this.animatorIsOpen = base.IsOpen());
			if (flag2)
			{
				this.model.animator.fireEvents = false;
				int num = 0;
				while ((float)num < 20f)
				{
					this.model.animator.Update(1f);
					num++;
				}
				this.PutAnimatorToSleep();
			}
			else
			{
				this.model.animator.fireEvents = base.isClient;
				if (base.isClient)
				{
					this.SetBatching(false);
				}
			}
		}
		else if (flag2)
		{
			this.PutAnimatorToSleep();
		}
		this.animatorNeedsInitializing = false;
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x000105ED File Offset: 0x0000E7ED
	protected void OnAnimatorFinished()
	{
		if (!this.isAnimating)
		{
			this.PutAnimatorToSleep();
		}
		this.isAnimating = false;
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x0007B3E4 File Offset: 0x000795E4
	private void PutAnimatorToSleep()
	{
		if (!this.model || !this.model.animator)
		{
			Debug.LogWarning(base.transform.GetRecursiveName("") + " has missing model/animator", base.gameObject);
			return;
		}
		this.model.animator.enabled = false;
		if (base.isClient)
		{
			this.SetBatching(true);
		}
		this.OnAnimatorDisabled();
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x00002ECE File Offset: 0x000010CE
	protected virtual void OnAnimatorDisabled()
	{
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool SupportsChildDeployables()
	{
		return false;
	}
}
