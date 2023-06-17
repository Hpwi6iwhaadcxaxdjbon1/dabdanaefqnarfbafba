using System;
using Network;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class LiquidVessel : HeldEntity
{
	// Token: 0x04000614 RID: 1556
	private float busyTime;

	// Token: 0x0600095A RID: 2394 RVA: 0x00050220 File Offset: 0x0004E420
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidVessel.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00050264 File Offset: 0x0004E464
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
		Item item = this.GetItem();
		return item != null && item.contents != null && item.contents.itemList != null && item.contents.itemList.Count != 0;
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x000502CC File Offset: 0x0004E4CC
	public bool CanFillHere(Vector3 pos)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && (double)ownerPlayer.WaterFactor() > 0.05;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00046050 File Offset: 0x00044250
	public int AmountHeld()
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x000097A7 File Offset: 0x000079A7
	public float HeldFraction()
	{
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x000097B8 File Offset: 0x000079B8
	public bool IsFull()
	{
		return this.HeldFraction() >= 1f;
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00007FB5 File Offset: 0x000061B5
	public int MaxHoldable()
	{
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00050300 File Offset: 0x0004E500
	public bool AnyPressed()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY) || ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY));
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x0005034C File Offset: 0x0004E54C
	public override void OnInput()
	{
		base.OnInput();
		if (this.viewModel == null)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (this.busyTime < Time.time)
		{
			if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY) && this.CanDrink())
			{
				this.viewModel.Trigger("drink");
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Gesture, "drink");
				this.busyTime += 3f;
			}
			if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY))
			{
				if (this.CanFillHere(ownerPlayer.transform.position) && !this.IsFull())
				{
					this.viewModel.Trigger("collect");
				}
				else if (this.HeldFraction() > 0.2f)
				{
					this.viewModel.Trigger("empty");
					ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Gesture, "drop_item");
				}
				this.busyTime += 2f;
			}
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0005045C File Offset: 0x0004E65C
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (name == "drink")
		{
			base.ServerRPC("DoDrink");
		}
		if (name == "empty")
		{
			base.ServerRPC("DoEmpty");
		}
		if (name == "fill")
		{
			base.ServerRPC("DoFill");
		}
	}
}
