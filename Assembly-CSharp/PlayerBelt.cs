using System;

// Token: 0x0200030D RID: 781
public class PlayerBelt
{
	// Token: 0x04001158 RID: 4440
	public static int SelectedSlot = -1;

	// Token: 0x04001159 RID: 4441
	protected BasePlayer player;

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x0600145F RID: 5215 RVA: 0x000115D6 File Offset: 0x0000F7D6
	public static int MaxBeltSlots
	{
		get
		{
			return 6;
		}
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x000115D9 File Offset: 0x0000F7D9
	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x0007E324 File Offset: 0x0007C524
	public Item GetItemInSlot(int slot)
	{
		if (this.player == null)
		{
			return null;
		}
		if (this.player.inventory == null)
		{
			return null;
		}
		if (this.player.inventory.containerBelt == null)
		{
			return null;
		}
		return this.player.inventory.containerBelt.GetSlot(slot);
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x000115E8 File Offset: 0x0000F7E8
	public Item GetActiveItem()
	{
		if (PlayerBelt.SelectedSlot == -1)
		{
			return null;
		}
		return this.GetItemInSlot(PlayerBelt.SelectedSlot);
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x0007E380 File Offset: 0x0007C580
	public bool CanHoldItem()
	{
		return !(this.player == null) && !this.player.IsWounded() && !this.player.IsSleeping() && (!this.player.isMounted || this.player.GetMounted().CanHoldItems());
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x0007E3E0 File Offset: 0x0007C5E0
	public void FrameUpdate()
	{
		Item activeItem = this.GetActiveItem();
		if (activeItem == null)
		{
			return;
		}
		if (!activeItem.CanBeHeld() || !this.CanHoldItem() || this.player.equippingBlocked)
		{
			this.player.HeldEntityEnd();
			PlayerBelt.SelectedSlot = -1;
		}
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x0007E428 File Offset: 0x0007C628
	private void ChangeSelect(int iSlot, bool allowRunAction = false)
	{
		Item activeItem = this.GetActiveItem();
		Item item = null;
		Item itemInSlot = this.GetItemInSlot(iSlot);
		if (itemInSlot != null && itemInSlot.CanBeHeld())
		{
			item = itemInSlot;
			if (itemInSlot.isBroken)
			{
				this.player.ChatMessage("Cannot equip - Item broken");
			}
		}
		if (itemInSlot != null && allowRunAction && itemInSlot.BeltBarSelect(this.player))
		{
			return;
		}
		if (item == activeItem)
		{
			item = null;
		}
		if (activeItem != null)
		{
			this.player.HeldEntityEnd();
			PlayerBelt.SelectedSlot = -1;
		}
		if (item != null)
		{
			PlayerBelt.SelectedSlot = iSlot;
			this.player.HeldEntityStart();
		}
		if (this.GetActiveItem() == null)
		{
			PlayerBelt.SelectedSlot = -1;
		}
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x0007E4C0 File Offset: 0x0007C6C0
	public void DoNextItemDirection(InputState state, int direction)
	{
		int num = (this.GetActiveItem() == null) ? 0 : (PlayerBelt.SelectedSlot + direction);
		for (int i = 0; i < PlayerBelt.MaxBeltSlots; i++)
		{
			num = (PlayerBelt.MaxBeltSlots + num) % PlayerBelt.MaxBeltSlots;
			Item itemInSlot = this.GetItemInSlot(num);
			if (itemInSlot != null && itemInSlot.CanBeHeld())
			{
				this.ChangeSelect(num, false);
				return;
			}
			num += direction;
		}
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x0007E520 File Offset: 0x0007C720
	public void ClientInput(InputState state)
	{
		if (!this.CanHoldItem())
		{
			PlayerBelt.SelectedSlot = -1;
			return;
		}
		if (PlayerInput.hasNoKeyInput)
		{
			return;
		}
		if (Buttons.Slot1.JustPressed)
		{
			this.ChangeSelect(0, true);
		}
		else if (Buttons.Slot2.JustPressed)
		{
			this.ChangeSelect(1, true);
		}
		else if (Buttons.Slot3.JustPressed)
		{
			this.ChangeSelect(2, true);
		}
		else if (Buttons.Slot4.JustPressed)
		{
			this.ChangeSelect(3, true);
		}
		else if (Buttons.Slot5.JustPressed)
		{
			this.ChangeSelect(4, true);
		}
		else if (Buttons.Slot6.JustPressed)
		{
			this.ChangeSelect(5, true);
		}
		else if (Buttons.Slot7.JustPressed)
		{
			this.ChangeSelect(6, true);
		}
		else if (Buttons.Slot8.JustPressed)
		{
			this.ChangeSelect(7, true);
		}
		else if (Buttons.InvPrev.JustPressed)
		{
			this.DoNextItemDirection(state, -1);
		}
		else if (Buttons.InvNext.JustPressed)
		{
			this.DoNextItemDirection(state, 1);
		}
		if (this.GetActiveItem() == null)
		{
			PlayerBelt.SelectedSlot = -1;
		}
	}
}
