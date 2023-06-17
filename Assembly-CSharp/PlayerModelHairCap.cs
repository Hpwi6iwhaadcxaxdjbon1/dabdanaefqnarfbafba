using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
[RequireComponent(typeof(PlayerModelSkin))]
public class PlayerModelHairCap : MonoBehaviour
{
	// Token: 0x04000D39 RID: 3385
	[InspectorFlags]
	public HairCapMask hairCapMask;

	// Token: 0x06001064 RID: 4196 RVA: 0x0006E75C File Offset: 0x0006C95C
	public void SetupHairCap(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet != null)
		{
			for (int i = 0; i < 5; i++)
			{
				if ((this.hairCapMask & (HairCapMask)(1 << i)) != (HairCapMask)0)
				{
					float typeNum;
					float seed;
					PlayerModelHair.GetRandomVariation(hairNum, i, index, out typeNum, out seed);
					HairType hairType = (HairType)i;
					HairSetCollection.HairSetEntry hairSetEntry = skinSet.HairCollection.Get(hairType, typeNum);
					if (hairSetEntry.HairSet != null)
					{
						HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
						if (hairDyeCollection != null)
						{
							HairDye hairDye = hairDyeCollection.Get(seed);
							if (hairDye != null)
							{
								hairDye.ApplyCap(hairDyeCollection, hairType, block);
							}
						}
					}
				}
			}
		}
	}
}
