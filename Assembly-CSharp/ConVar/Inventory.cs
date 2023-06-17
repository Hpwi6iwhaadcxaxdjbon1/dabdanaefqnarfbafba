using System;

namespace ConVar
{
	// Token: 0x0200086B RID: 2155
	[ConsoleSystem.Factory("inventory")]
	public class Inventory : ConsoleSystem
	{
		// Token: 0x06002F00 RID: 12032 RVA: 0x0002428E File Offset: 0x0002248E
		[ClientVar]
		public static void toggle()
		{
			if (PlayerInput.hasNoKeyInput)
			{
				return;
			}
			UIInventory.ToggleInventory();
		}

		// Token: 0x06002F01 RID: 12033 RVA: 0x000E90A4 File Offset: 0x000E72A4
		[ClientVar]
		public static void examineheld()
		{
			if (PlayerInput.hasNoKeyInput)
			{
				return;
			}
			BasePlayer entity = LocalPlayer.Entity;
			if (entity == null)
			{
				return;
			}
			HeldEntity heldEntity = entity.GetHeldEntity();
			if (heldEntity == null)
			{
				return;
			}
			heldEntity.Examine();
		}

		// Token: 0x06002F02 RID: 12034 RVA: 0x0002429D File Offset: 0x0002249D
		[ClientVar]
		public static void togglecrafting()
		{
			if (PlayerInput.hasNoKeyInput)
			{
				return;
			}
			UICrafting.Toggle();
		}

		// Token: 0x06002F03 RID: 12035 RVA: 0x000242AC File Offset: 0x000224AC
		[ClientVar]
		public static void ResetCraftCounts()
		{
			if (LocalPlayer.isAdmin)
			{
				LocalPlayer.ResetCraftCounts();
			}
		}

		// Token: 0x06002F04 RID: 12036 RVA: 0x000242BA File Offset: 0x000224BA
		[ClientVar]
		public static void ListCraftCounts()
		{
			if (LocalPlayer.isAdmin)
			{
				LocalPlayer.ListCraftCounts();
			}
		}
	}
}
