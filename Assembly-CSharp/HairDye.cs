using System;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
[Serializable]
public class HairDye
{
	// Token: 0x04001DE0 RID: 7648
	[ColorUsage(false, true)]
	public Color capBaseColor;

	// Token: 0x04001DE1 RID: 7649
	public Material sourceMaterial;

	// Token: 0x04001DE2 RID: 7650
	[InspectorFlags]
	public HairDye.CopyPropertyMask copyProperties;

	// Token: 0x04001DE3 RID: 7651
	private static MaterialPropertyDesc[] transferableProps = new MaterialPropertyDesc[]
	{
		new MaterialPropertyDesc("_DyeColor", typeof(Color)),
		new MaterialPropertyDesc("_RootColor", typeof(Color)),
		new MaterialPropertyDesc("_TipColor", typeof(Color)),
		new MaterialPropertyDesc("_Brightness", typeof(float)),
		new MaterialPropertyDesc("_DyeRoughness", typeof(float)),
		new MaterialPropertyDesc("_DyeScatter", typeof(float)),
		new MaterialPropertyDesc("_HairSpecular", typeof(float)),
		new MaterialPropertyDesc("_HairRoughness", typeof(float))
	};

	// Token: 0x04001DE4 RID: 7652
	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	// Token: 0x04001DE5 RID: 7653
	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	// Token: 0x04001DE6 RID: 7654
	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	// Token: 0x04001DE7 RID: 7655
	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");

	// Token: 0x060021F0 RID: 8688 RVA: 0x000B730C File Offset: 0x000B550C
	public void Apply(HairDyeCollection collection, MaterialPropertyBlock block)
	{
		if (this.sourceMaterial != null)
		{
			for (int i = 0; i < 8; i++)
			{
				if ((this.copyProperties & (HairDye.CopyPropertyMask)(1 << i)) != (HairDye.CopyPropertyMask)0)
				{
					MaterialPropertyDesc materialPropertyDesc = HairDye.transferableProps[i];
					if (this.sourceMaterial.HasProperty(materialPropertyDesc.nameID))
					{
						if (materialPropertyDesc.type == typeof(Color))
						{
							block.SetColor(materialPropertyDesc.nameID, this.sourceMaterial.GetColor(materialPropertyDesc.nameID));
						}
						else if (materialPropertyDesc.type == typeof(float))
						{
							block.SetFloat(materialPropertyDesc.nameID, this.sourceMaterial.GetFloat(materialPropertyDesc.nameID));
						}
					}
				}
			}
		}
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x000B73D8 File Offset: 0x000B55D8
	public void ApplyCap(HairDyeCollection collection, HairType type, MaterialPropertyBlock block)
	{
		if (collection.applyCap)
		{
			if (type == HairType.Head || type == HairType.Armpit || type == HairType.Pubic)
			{
				block.SetColor(HairDye._HairBaseColorUV1, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV1, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
				return;
			}
			if (type == HairType.Facial)
			{
				block.SetColor(HairDye._HairBaseColorUV2, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV2, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
			}
		}
	}

	// Token: 0x020005D1 RID: 1489
	public enum CopyProperty
	{
		// Token: 0x04001DE9 RID: 7657
		DyeColor,
		// Token: 0x04001DEA RID: 7658
		RootColor,
		// Token: 0x04001DEB RID: 7659
		TipColor,
		// Token: 0x04001DEC RID: 7660
		Brightness,
		// Token: 0x04001DED RID: 7661
		DyeRoughness,
		// Token: 0x04001DEE RID: 7662
		DyeScatter,
		// Token: 0x04001DEF RID: 7663
		Specular,
		// Token: 0x04001DF0 RID: 7664
		Roughness,
		// Token: 0x04001DF1 RID: 7665
		Count
	}

	// Token: 0x020005D2 RID: 1490
	[Flags]
	public enum CopyPropertyMask
	{
		// Token: 0x04001DF3 RID: 7667
		DyeColor = 1,
		// Token: 0x04001DF4 RID: 7668
		RootColor = 2,
		// Token: 0x04001DF5 RID: 7669
		TipColor = 4,
		// Token: 0x04001DF6 RID: 7670
		Brightness = 8,
		// Token: 0x04001DF7 RID: 7671
		DyeRoughness = 16,
		// Token: 0x04001DF8 RID: 7672
		DyeScatter = 32,
		// Token: 0x04001DF9 RID: 7673
		Specular = 64,
		// Token: 0x04001DFA RID: 7674
		Roughness = 128
	}
}
