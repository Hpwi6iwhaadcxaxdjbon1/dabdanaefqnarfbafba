using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class Buttons
{
	// Token: 0x04000DFB RID: 3579
	[ClientVar]
	public static Buttons.ConButton Console = new Buttons.ConButton();

	// Token: 0x04000DFC RID: 3580
	[ClientVar]
	public static Buttons.ConButton Forward = new Buttons.ConButton();

	// Token: 0x04000DFD RID: 3581
	[ClientVar]
	public static Buttons.ConButton Backward = new Buttons.ConButton();

	// Token: 0x04000DFE RID: 3582
	[ClientVar]
	public static Buttons.ConButton Left = new Buttons.ConButton();

	// Token: 0x04000DFF RID: 3583
	[ClientVar]
	public static Buttons.ConButton Right = new Buttons.ConButton();

	// Token: 0x04000E00 RID: 3584
	[ClientVar]
	public static Buttons.ConButton Jump = new Buttons.ConButton();

	// Token: 0x04000E01 RID: 3585
	[ClientVar]
	public static Buttons.ConButton Duck = new Buttons.ConButton();

	// Token: 0x04000E02 RID: 3586
	[ClientVar]
	public static Buttons.ConButton Sprint = new Buttons.ConButton();

	// Token: 0x04000E03 RID: 3587
	[ClientVar]
	public static Buttons.ConButton Use = new Buttons.ConButton();

	// Token: 0x04000E04 RID: 3588
	[ClientVar]
	public static Buttons.ConButton Inventory = new Buttons.ConButton();

	// Token: 0x04000E05 RID: 3589
	[ClientVar]
	public static Buttons.ConButton Chat = new Buttons.ConButton();

	// Token: 0x04000E06 RID: 3590
	[ClientVar]
	public static Buttons.ConButton Reload = new Buttons.ConButton();

	// Token: 0x04000E07 RID: 3591
	[ClientVar]
	public static Buttons.ConButton Voice = new Buttons.ConButton();

	// Token: 0x04000E08 RID: 3592
	[ClientVar]
	public static Buttons.ConButton InvNext = new Buttons.ConButton();

	// Token: 0x04000E09 RID: 3593
	[ClientVar]
	public static Buttons.ConButton InvPrev = new Buttons.ConButton();

	// Token: 0x04000E0A RID: 3594
	[ClientVar]
	public static Buttons.ConButton Slot1 = new Buttons.ConButton();

	// Token: 0x04000E0B RID: 3595
	[ClientVar]
	public static Buttons.ConButton Slot2 = new Buttons.ConButton();

	// Token: 0x04000E0C RID: 3596
	[ClientVar]
	public static Buttons.ConButton Slot3 = new Buttons.ConButton();

	// Token: 0x04000E0D RID: 3597
	[ClientVar]
	public static Buttons.ConButton Slot4 = new Buttons.ConButton();

	// Token: 0x04000E0E RID: 3598
	[ClientVar]
	public static Buttons.ConButton Slot5 = new Buttons.ConButton();

	// Token: 0x04000E0F RID: 3599
	[ClientVar]
	public static Buttons.ConButton Slot6 = new Buttons.ConButton();

	// Token: 0x04000E10 RID: 3600
	[ClientVar]
	public static Buttons.ConButton Slot7 = new Buttons.ConButton();

	// Token: 0x04000E11 RID: 3601
	[ClientVar]
	public static Buttons.ConButton Slot8 = new Buttons.ConButton();

	// Token: 0x04000E12 RID: 3602
	[ClientVar]
	public static Buttons.ConButton Attack = new Buttons.ConButton();

	// Token: 0x04000E13 RID: 3603
	[ClientVar]
	public static Buttons.ConButton Attack2 = new Buttons.ConButton();

	// Token: 0x04000E14 RID: 3604
	[ClientVar]
	public static Buttons.ConButton Attack3 = new Buttons.ConButton();

	// Token: 0x04000E15 RID: 3605
	[ClientVar]
	public static Buttons.ConButton AltLook = new Buttons.ConButton();

	// Token: 0x04000E16 RID: 3606
	[ClientVar]
	public static Buttons.ConButton Map = new Buttons.ConButton();

	// Token: 0x04000E17 RID: 3607
	[ClientVar]
	public static Buttons.ConButton Compass = new Buttons.ConButton();

	// Token: 0x02000243 RID: 579
	public class ConButton : ConsoleSystem.IConsoleCommand
	{
		// Token: 0x04000E19 RID: 3609
		private int frame;

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06001143 RID: 4419 RVA: 0x0000F37F File Offset: 0x0000D57F
		// (set) Token: 0x06001144 RID: 4420 RVA: 0x0000F387 File Offset: 0x0000D587
		public bool IsDown { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06001145 RID: 4421 RVA: 0x0000F390 File Offset: 0x0000D590
		public bool JustPressed
		{
			get
			{
				return this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06001146 RID: 4422 RVA: 0x0000F3A9 File Offset: 0x0000D5A9
		public bool JustReleased
		{
			get
			{
				return !this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x0000F3C2 File Offset: 0x0000D5C2
		public void Call(ConsoleSystem.Arg arg)
		{
			this.IsDown = arg.GetBool(0, false);
			this.frame = Time.frameCount;
		}
	}
}
