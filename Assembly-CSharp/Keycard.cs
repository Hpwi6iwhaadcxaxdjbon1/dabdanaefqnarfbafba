using System;
using UnityEngine;

// Token: 0x020002A3 RID: 675
public class Keycard : AttackEntity
{
	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x060012F4 RID: 4852 RVA: 0x0007AA08 File Offset: 0x00078C08
	public int accessLevel
	{
		get
		{
			Item item = this.GetItem();
			if (item == null)
			{
				return 0;
			}
			ItemModKeycard component = item.info.GetComponent<ItemModKeycard>();
			if (component == null)
			{
				return 0;
			}
			return component.accessLevel;
		}
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x0007AA40 File Offset: 0x00078C40
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
		if (base.HasAttackCooldown())
		{
			return;
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY))
		{
			this.Swipe();
		}
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x0007AA94 File Offset: 0x00078C94
	public override void OnFrame()
	{
		base.OnFrame();
		if (this.viewModel && this.viewModel.instance)
		{
			SwapKeycard component = this.viewModel.instance.GetComponent<SwapKeycard>();
			if (component)
			{
				component.UpdateAccessLevel(this.accessLevel);
			}
		}
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x0007AAEC File Offset: 0x00078CEC
	public void Swipe()
	{
		base.StartAttackCooldown(this.repeatDelay);
		if (this.viewModel == null || this.viewModel.instance == null)
		{
			return;
		}
		this.viewModel.instance.Trigger("attack");
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x0007AB3C File Offset: 0x00078D3C
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (name == "Swipe")
		{
			BaseEntity lookingAtEntity = LocalPlayer.Entity.lookingAtEntity;
			if (lookingAtEntity != null)
			{
				CardReader component = lookingAtEntity.GetComponent<CardReader>();
				if (component && Vector3Ex.Distance2D(base.transform.position, LocalPlayer.Entity.transform.position) < 1f)
				{
					component.ClientCardSwiped(this);
				}
			}
		}
	}
}
