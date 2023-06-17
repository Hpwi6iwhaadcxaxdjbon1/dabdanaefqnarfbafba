using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C4 RID: 196
public class ShopFrontLootPanel : LootPanel
{
	// Token: 0x040006B8 RID: 1720
	public Text playerLabelA;

	// Token: 0x040006B9 RID: 1721
	public Text playerLabelB;

	// Token: 0x040006BA RID: 1722
	public GameObject confirmButton;

	// Token: 0x040006BB RID: 1723
	public GameObject confirmHelp;

	// Token: 0x040006BC RID: 1724
	public GameObject denyButton;

	// Token: 0x040006BD RID: 1725
	public GameObject denyHelp;

	// Token: 0x040006BE RID: 1726
	public GameObject waitingText;

	// Token: 0x040006BF RID: 1727
	public GameObject exchangeInProgressImage;

	// Token: 0x040006C0 RID: 1728
	public Translate.Phrase acceptedPhrase;

	// Token: 0x040006C1 RID: 1729
	public Translate.Phrase noOnePhrase;

	// Token: 0x06000A58 RID: 2648 RVA: 0x0000A3D8 File Offset: 0x000085D8
	public ShopFront GetShopfront()
	{
		return base.GetContainerEntity() as ShopFront;
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00055188 File Offset: 0x00053388
	public override void Update()
	{
		base.Update();
		ShopFront shopfront = this.GetShopfront();
		if (shopfront == null)
		{
			return;
		}
		this.playerLabelA.text = ((shopfront.vendorPlayer == null) ? this.noOnePhrase.translated : shopfront.vendorPlayer.displayName);
		this.playerLabelB.text = ((shopfront.customerPlayer == null) ? this.noOnePhrase.translated : shopfront.customerPlayer.displayName);
		bool flag = shopfront.HasFlag(BaseEntity.Flags.Reserved1);
		bool flag2 = shopfront.HasFlag(BaseEntity.Flags.Reserved2);
		if (flag)
		{
			Text text = this.playerLabelA;
			text.text = text.text + " " + this.acceptedPhrase.translated;
		}
		if (flag2)
		{
			Text text2 = this.playerLabelB;
			text2.text = text2.text + " " + this.acceptedPhrase.translated;
		}
		bool flag3 = shopfront.HasFlag(BaseEntity.Flags.Reserved3);
		bool flag4 = shopfront.vendorPlayer != null && shopfront.customerPlayer != null;
		bool flag5 = (flag && LocalPlayer.Entity == shopfront.vendorPlayer) || (flag2 && LocalPlayer.Entity == shopfront.customerPlayer);
		this.confirmButton.SetActive(!flag5 && flag4 && !flag3);
		this.confirmHelp.SetActive(!flag5 && flag4 && !flag3);
		this.denyButton.SetActive(flag5 && flag4 && !flag3);
		this.denyHelp.SetActive(flag5 && flag4 && !flag3);
		this.waitingText.SetActive(!flag4 && !flag3);
		this.exchangeInProgressImage.SetActive(flag3);
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0000A3E5 File Offset: 0x000085E5
	public void AcceptClicked()
	{
		this.GetShopfront().ServerRPC("AcceptClicked");
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0000A3F7 File Offset: 0x000085F7
	public void CancelClicked()
	{
		this.GetShopfront().ServerRPC("CancelClicked");
	}
}
