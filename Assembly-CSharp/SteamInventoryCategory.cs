using System;
using UnityEngine;

// Token: 0x020005EC RID: 1516
[CreateAssetMenu(menuName = "Rust/Generic Steam Inventory Category")]
public class SteamInventoryCategory : ScriptableObject
{
	// Token: 0x04001E4D RID: 7757
	[Header("Steam Inventory")]
	public bool canBeSoldToOtherUsers;

	// Token: 0x04001E4E RID: 7758
	public bool canBeTradedWithOtherUsers;

	// Token: 0x04001E4F RID: 7759
	public bool isCommodity;

	// Token: 0x04001E50 RID: 7760
	public SteamInventoryCategory.Price price;

	// Token: 0x04001E51 RID: 7761
	public SteamInventoryCategory.DropChance dropChance;

	// Token: 0x04001E52 RID: 7762
	public bool CanBeInCrates = true;

	// Token: 0x020005ED RID: 1517
	public enum Price
	{
		// Token: 0x04001E54 RID: 7764
		CannotBuy,
		// Token: 0x04001E55 RID: 7765
		VLV25,
		// Token: 0x04001E56 RID: 7766
		VLV50,
		// Token: 0x04001E57 RID: 7767
		VLV75,
		// Token: 0x04001E58 RID: 7768
		VLV100,
		// Token: 0x04001E59 RID: 7769
		VLV150,
		// Token: 0x04001E5A RID: 7770
		VLV200,
		// Token: 0x04001E5B RID: 7771
		VLV250,
		// Token: 0x04001E5C RID: 7772
		VLV300,
		// Token: 0x04001E5D RID: 7773
		VLV350,
		// Token: 0x04001E5E RID: 7774
		VLV400,
		// Token: 0x04001E5F RID: 7775
		VLV450,
		// Token: 0x04001E60 RID: 7776
		VLV500,
		// Token: 0x04001E61 RID: 7777
		VLV550,
		// Token: 0x04001E62 RID: 7778
		VLV600,
		// Token: 0x04001E63 RID: 7779
		VLV650,
		// Token: 0x04001E64 RID: 7780
		VLV700,
		// Token: 0x04001E65 RID: 7781
		VLV750,
		// Token: 0x04001E66 RID: 7782
		VLV800,
		// Token: 0x04001E67 RID: 7783
		VLV850,
		// Token: 0x04001E68 RID: 7784
		VLV900,
		// Token: 0x04001E69 RID: 7785
		VLV950,
		// Token: 0x04001E6A RID: 7786
		VLV1000,
		// Token: 0x04001E6B RID: 7787
		VLV1100,
		// Token: 0x04001E6C RID: 7788
		VLV1200,
		// Token: 0x04001E6D RID: 7789
		VLV1300,
		// Token: 0x04001E6E RID: 7790
		VLV1400,
		// Token: 0x04001E6F RID: 7791
		VLV1500,
		// Token: 0x04001E70 RID: 7792
		VLV1600,
		// Token: 0x04001E71 RID: 7793
		VLV1700,
		// Token: 0x04001E72 RID: 7794
		VLV1800,
		// Token: 0x04001E73 RID: 7795
		VLV1900,
		// Token: 0x04001E74 RID: 7796
		VLV2000,
		// Token: 0x04001E75 RID: 7797
		VLV2500,
		// Token: 0x04001E76 RID: 7798
		VLV3000,
		// Token: 0x04001E77 RID: 7799
		VLV3500,
		// Token: 0x04001E78 RID: 7800
		VLV4000,
		// Token: 0x04001E79 RID: 7801
		VLV4500,
		// Token: 0x04001E7A RID: 7802
		VLV5000,
		// Token: 0x04001E7B RID: 7803
		VLV6000,
		// Token: 0x04001E7C RID: 7804
		VLV7000,
		// Token: 0x04001E7D RID: 7805
		VLV8000,
		// Token: 0x04001E7E RID: 7806
		VLV9000,
		// Token: 0x04001E7F RID: 7807
		VLV10000
	}

	// Token: 0x020005EE RID: 1518
	public enum DropChance
	{
		// Token: 0x04001E81 RID: 7809
		NeverDrop,
		// Token: 0x04001E82 RID: 7810
		VeryRare,
		// Token: 0x04001E83 RID: 7811
		Rare,
		// Token: 0x04001E84 RID: 7812
		Common,
		// Token: 0x04001E85 RID: 7813
		VeryCommon,
		// Token: 0x04001E86 RID: 7814
		ExtremelyRare
	}
}
