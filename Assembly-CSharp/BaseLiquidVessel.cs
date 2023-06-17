using System;
using Network;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class BaseLiquidVessel : AttackEntity
{
	// Token: 0x040003E1 RID: 993
	[Header("Liquid Vessel")]
	public GameObjectRef thrownWaterObject;

	// Token: 0x040003E2 RID: 994
	public GameObjectRef ThrowEffect3P;

	// Token: 0x040003E3 RID: 995
	public SoundDefinition throwSound3P;

	// Token: 0x040003E4 RID: 996
	public GameObjectRef fillFromContainer;

	// Token: 0x040003E5 RID: 997
	public GameObjectRef fillFromWorld;

	// Token: 0x040003E6 RID: 998
	public bool hasLid;

	// Token: 0x040003E7 RID: 999
	public float throwScale = 10f;

	// Token: 0x040003E8 RID: 1000
	public bool canDrinkFrom;

	// Token: 0x040003E9 RID: 1001
	public bool updateVMWater;

	// Token: 0x040003EA RID: 1002
	public float minThrowFrac;

	// Token: 0x040003EB RID: 1003
	public bool useThrowAnim;

	// Token: 0x040003EC RID: 1004
	public float fillMlPerSec = 500f;

	// Token: 0x040003ED RID: 1005
	private float timeSinceLastAttack = float.PositiveInfinity;

	// Token: 0x040003EE RID: 1006
	private bool wasFilling;

	// Token: 0x040003EF RID: 1007
	private float nextFreeTime;

	// Token: 0x0600071D RID: 1821 RVA: 0x0004600C File Offset: 0x0004420C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLiquidVessel.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00046050 File Offset: 0x00044250
	public int AmountHeld()
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00007FA4 File Offset: 0x000061A4
	public float HeldFraction()
	{
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00007FB5 File Offset: 0x000061B5
	public int MaxHoldable()
	{
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x0004607C File Offset: 0x0004427C
	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		if (!this.canDrinkFrom)
		{
			return false;
		}
		Item item = this.GetItem();
		return item != null && item.contents != null && item.contents.itemList != null && item.contents.itemList.Count != 0;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00007FCC File Offset: 0x000061CC
	private bool IsWeaponBusy()
	{
		return Time.realtimeSinceStartup < this.nextFreeTime;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00007FDB File Offset: 0x000061DB
	private void SetBusyFor(float dur)
	{
		this.nextFreeTime = Time.realtimeSinceStartup + dur;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00007FEA File Offset: 0x000061EA
	private void ClearBusy()
	{
		this.nextFreeTime = Time.realtimeSinceStartup - 1f;
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x000460F0 File Offset: 0x000442F0
	public virtual void CLThrow()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (this.useThrowAnim)
		{
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Gesture, "drop_item");
		}
		else
		{
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Attack, "");
		}
		if (this.viewModel)
		{
			this.viewModel.Play("empty");
		}
		this.SetBusyFor(2f);
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00046158 File Offset: 0x00044358
	public override void OnInput()
	{
		base.OnInput();
		this.timeSinceLastAttack += Time.deltaTime;
		if (!this.IsFullyDeployed())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		Item slot = this.GetItem().contents.GetSlot(0);
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY) && slot != null && !this.IsWeaponBusy())
		{
			this.timeSinceLastAttack = 0f;
			if (this.CanDrink())
			{
				if (this.viewModel)
				{
					this.viewModel.Trigger("drink");
				}
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Gesture, "drink");
				this.SetBusyFor(3f);
			}
			else if ((float)slot.amount >= 0.2f * (float)this.MaxHoldable())
			{
				this.CLThrow();
			}
		}
		bool flag = false;
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY) && !this.IsWeaponBusy())
		{
			bool flag2 = this.CanFillFromWorld();
			if (!this.GetFacingLiquidContainer() && !flag2)
			{
				if (this.CanThrow())
				{
					this.CLThrow();
				}
			}
			else
			{
				flag = true;
			}
		}
		if (this.updateVMWater && this.viewModel && this.viewModel.instance != null)
		{
			float num = (slot == null) ? 0f : this.HeldFraction();
			this.viewModel.instance.BroadcastMessage("UpdateWaterLevel", num, SendMessageOptions.DontRequireReceiver);
		}
		if (this.wasFilling != flag)
		{
			this.viewModel.SetBool("filling", flag);
			base.ServerRPC<bool>("SendFilling", flag);
		}
		this.wasFilling = flag;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00046300 File Offset: 0x00044500
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (name == "empty")
		{
			base.ServerRPC("ThrowContents");
		}
		if (name == "drink" && this.canDrinkFrom)
		{
			base.ServerRPC("DoDrink");
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x0004634C File Offset: 0x0004454C
	public bool CanFillFromWorld()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && ownerPlayer.WaterFactor() >= 0.05f;
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00007FFD File Offset: 0x000061FD
	public bool CanThrow()
	{
		return this.HeldFraction() > this.minThrowFrac;
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0004637C File Offset: 0x0004457C
	public LiquidContainer GetFacingLiquidContainer()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		RaycastHit hit;
		if (Physics.Raycast(ownerPlayer.eyes.HeadRay(), ref hit, 2f, 1236478737))
		{
			BaseEntity entity = hit.GetEntity();
			if (entity && !hit.collider.gameObject.CompareTag("Not Player Usable") && !hit.collider.gameObject.CompareTag("Usable Primary"))
			{
				return entity.GetComponent<LiquidContainer>();
			}
		}
		return null;
	}
}
