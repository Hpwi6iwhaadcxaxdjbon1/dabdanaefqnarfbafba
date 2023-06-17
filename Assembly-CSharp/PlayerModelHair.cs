using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class PlayerModelHair : MonoBehaviour
{
	// Token: 0x04000D2E RID: 3374
	public HairType type;

	// Token: 0x04000D2F RID: 3375
	private Dictionary<Renderer, PlayerModelHair.RendererMaterials> materials;

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x0600105D RID: 4189 RVA: 0x0000E758 File Offset: 0x0000C958
	public Dictionary<Renderer, PlayerModelHair.RendererMaterials> Materials
	{
		get
		{
			return this.materials;
		}
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x0006E57C File Offset: 0x0006C77C
	private void CacheOriginalMaterials()
	{
		if (this.materials == null)
		{
			this.materials = new Dictionary<Renderer, PlayerModelHair.RendererMaterials>();
			List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
			base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
			this.materials.Clear();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
			{
				this.materials.Add(skinnedMeshRenderer, new PlayerModelHair.RendererMaterials(skinnedMeshRenderer));
			}
			Pool.FreeList<SkinnedMeshRenderer>(ref list);
		}
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x0006E610 File Offset: 0x0006C810
	private void Setup(HairType type, HairSetCollection hair, int meshIndex, float typeNum, float dyeNum, MaterialPropertyBlock block)
	{
		this.CacheOriginalMaterials();
		HairSetCollection.HairSetEntry hairSetEntry = hair.Get(type, typeNum);
		if (hairSetEntry.HairSet == null)
		{
			Debug.LogWarning("Hair.Get returned a NULL hair");
			return;
		}
		int blendShapeIndex = -1;
		if (type == HairType.Facial || type == HairType.Eyebrow)
		{
			blendShapeIndex = meshIndex;
		}
		HairDye dye = null;
		HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
		if (hairDyeCollection != null)
		{
			dye = hairDyeCollection.Get(dyeNum);
		}
		hairSetEntry.HairSet.Process(this, hairDyeCollection, dye, block);
		hairSetEntry.HairSet.ProcessMorphs(base.gameObject, blendShapeIndex);
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x0006E690 File Offset: 0x0006C890
	public void Setup(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet == null)
		{
			Debug.LogError("Skin.Get returned a NULL skin");
			return;
		}
		int typeIndex = (int)this.type;
		float typeNum;
		float dyeNum;
		PlayerModelHair.GetRandomVariation(hairNum, typeIndex, index, out typeNum, out dyeNum);
		this.Setup(this.type, skinSet.HairCollection, index, typeNum, dyeNum, block);
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x0000E760 File Offset: 0x0000C960
	public static void GetRandomVariation(float hairNum, int typeIndex, int meshIndex, out float typeNum, out float dyeNum)
	{
		int num = Mathf.FloorToInt(hairNum * 100000f);
		Random.InitState(num + typeIndex);
		typeNum = Random.Range(0f, 1f);
		Random.InitState(num + meshIndex);
		dyeNum = Random.Range(0f, 1f);
	}

	// Token: 0x02000215 RID: 533
	public struct RendererMaterials
	{
		// Token: 0x04000D30 RID: 3376
		public string[] names;

		// Token: 0x04000D31 RID: 3377
		public Material[] original;

		// Token: 0x04000D32 RID: 3378
		public Material[] replacement;

		// Token: 0x06001063 RID: 4195 RVA: 0x0006E6F0 File Offset: 0x0006C8F0
		public RendererMaterials(Renderer r)
		{
			this.original = r.sharedMaterials;
			this.replacement = (this.original.Clone() as Material[]);
			this.names = new string[this.original.Length];
			for (int i = 0; i < this.original.Length; i++)
			{
				this.names[i] = this.original[i].name;
			}
		}
	}
}
