using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
[CreateAssetMenu(menuName = "Rust/Skin Set")]
public class SkinSet : ScriptableObject
{
	// Token: 0x04001E41 RID: 7745
	public string Label;

	// Token: 0x04001E42 RID: 7746
	public SkinSet.MeshReplace[] MeshReplacements;

	// Token: 0x04001E43 RID: 7747
	public SkinSet.MaterialReplace[] MaterialReplacements;

	// Token: 0x04001E44 RID: 7748
	public Gradient SkinColour;

	// Token: 0x04001E45 RID: 7749
	public HairSetCollection HairCollection;

	// Token: 0x06002223 RID: 8739 RVA: 0x000B7C48 File Offset: 0x000B5E48
	public void Process(GameObject obj, float Seed)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		obj.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			if (!(skinnedMeshRenderer.sharedMesh == null) && !(skinnedMeshRenderer.sharedMaterial == null))
			{
				string name = skinnedMeshRenderer.sharedMesh.name;
				string name2 = skinnedMeshRenderer.sharedMaterial.name;
				for (int i = 0; i < this.MeshReplacements.Length; i++)
				{
					if (this.MeshReplacements[i].Test(name))
					{
						SkinnedMeshRenderer skinnedMeshRenderer2 = this.MeshReplacements[i].Get(Seed);
						skinnedMeshRenderer.sharedMesh = skinnedMeshRenderer2.sharedMesh;
						skinnedMeshRenderer.rootBone = skinnedMeshRenderer2.rootBone;
						skinnedMeshRenderer.bones = skinnedMeshRenderer2.bones;
					}
				}
				for (int j = 0; j < this.MaterialReplacements.Length; j++)
				{
					if (this.MaterialReplacements[j].Test(name2))
					{
						skinnedMeshRenderer.sharedMaterial = this.MaterialReplacements[j].Get(Seed);
					}
				}
			}
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x0001B129 File Offset: 0x00019329
	internal Color GetSkinColor(float skinNumber)
	{
		return this.SkinColour.Evaluate(skinNumber);
	}

	// Token: 0x020005E9 RID: 1513
	[Serializable]
	public class MeshReplace
	{
		// Token: 0x04001E46 RID: 7750
		[HideInInspector]
		public string FindName;

		// Token: 0x04001E47 RID: 7751
		public Mesh Find;

		// Token: 0x04001E48 RID: 7752
		public SkinnedMeshRenderer[] Replace;

		// Token: 0x06002226 RID: 8742 RVA: 0x0001B137 File Offset: 0x00019337
		public SkinnedMeshRenderer Get(float MeshNumber)
		{
			return this.Replace[Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)this.Replace.Length), 0, this.Replace.Length - 1)];
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x0001B160 File Offset: 0x00019360
		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}
	}

	// Token: 0x020005EA RID: 1514
	[Serializable]
	public class MaterialReplace
	{
		// Token: 0x04001E49 RID: 7753
		[HideInInspector]
		public string FindName;

		// Token: 0x04001E4A RID: 7754
		public Material Find;

		// Token: 0x04001E4B RID: 7755
		public Material[] Replace;

		// Token: 0x06002229 RID: 8745 RVA: 0x0001B16E File Offset: 0x0001936E
		public Material Get(float MeshNumber)
		{
			return this.Replace[Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)this.Replace.Length), 0, this.Replace.Length - 1)];
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x0001B197 File Offset: 0x00019397
		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}
	}
}
