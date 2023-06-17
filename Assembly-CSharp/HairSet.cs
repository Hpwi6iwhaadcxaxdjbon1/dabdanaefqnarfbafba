using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020005D4 RID: 1492
[CreateAssetMenu(menuName = "Rust/Hair Set")]
public class HairSet : ScriptableObject
{
	// Token: 0x04001DFE RID: 7678
	public HairSet.MeshReplace[] MeshReplacements;

	// Token: 0x04001DFF RID: 7679
	public HairSet.MaterialReplace[] MaterialReplacements;

	// Token: 0x060021F6 RID: 8694 RVA: 0x000B75A4 File Offset: 0x000B57A4
	private void OnEnable()
	{
		HairSet.MeshReplace[] meshReplacements = this.MeshReplacements;
		for (int i = 0; i < meshReplacements.Length; i++)
		{
			meshReplacements[i].Cache();
		}
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000B75D0 File Offset: 0x000B57D0
	public void Process(PlayerModelHair playerModelHair, HairDyeCollection dyeCollection, HairDye dye, MaterialPropertyBlock block)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		playerModelHair.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			if (!(skinnedMeshRenderer.sharedMesh == null) && !(skinnedMeshRenderer.sharedMaterial == null))
			{
				string name = skinnedMeshRenderer.sharedMesh.name;
				string name2 = skinnedMeshRenderer.sharedMaterial.name;
				if (!skinnedMeshRenderer.gameObject.activeSelf)
				{
					skinnedMeshRenderer.gameObject.SetActive(true);
				}
				for (int i = 0; i < this.MeshReplacements.Length; i++)
				{
					if (this.MeshReplacements[i].Test(name))
					{
						SkinnedMeshRenderer replace = this.MeshReplacements[i].Replace;
						if (replace == null)
						{
							skinnedMeshRenderer.gameObject.SetActive(false);
						}
						else
						{
							skinnedMeshRenderer.sharedMesh = replace.sharedMesh;
							skinnedMeshRenderer.rootBone = replace.rootBone;
							skinnedMeshRenderer.bones = this.MeshReplacements[i].GetBones();
						}
					}
				}
				PlayerModelHair.RendererMaterials rendererMaterials;
				if (playerModelHair.Materials.TryGetValue(skinnedMeshRenderer, ref rendererMaterials))
				{
					Array.Copy(rendererMaterials.original, rendererMaterials.replacement, rendererMaterials.original.Length);
					for (int j = 0; j < rendererMaterials.original.Length; j++)
					{
						for (int k = 0; k < this.MaterialReplacements.Length; k++)
						{
							if (this.MaterialReplacements[k].Test(rendererMaterials.names[j]))
							{
								rendererMaterials.replacement[j] = this.MaterialReplacements[k].Replace;
							}
						}
					}
					skinnedMeshRenderer.sharedMaterials = rendererMaterials.replacement;
				}
				else
				{
					Debug.LogWarning("[HairSet.Process] Missing cached renderer materials in " + playerModelHair.name);
				}
				if (dye != null && skinnedMeshRenderer.gameObject.activeSelf)
				{
					dye.Apply(dyeCollection, block);
				}
			}
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000B77E4 File Offset: 0x000B59E4
	public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
	{
		List<MorphCache> list = Pool.GetList<MorphCache>();
		obj.GetComponentsInChildren<MorphCache>(true, list);
		foreach (MorphCache morphCache in list)
		{
			morphCache.Setup();
			if (blendShapeIndex >= 0)
			{
				morphCache.SetBlendShape(blendShapeIndex);
			}
		}
		Pool.FreeList<MorphCache>(ref list);
	}

	// Token: 0x020005D5 RID: 1493
	[Serializable]
	public class MeshReplace
	{
		// Token: 0x04001E00 RID: 7680
		[HideInInspector]
		public string FindName;

		// Token: 0x04001E01 RID: 7681
		public Mesh Find;

		// Token: 0x04001E02 RID: 7682
		public SkinnedMeshRenderer Replace;

		// Token: 0x04001E03 RID: 7683
		private Transform[] bones;

		// Token: 0x04001E04 RID: 7684
		private MorphCache.CompressedMeshData cachedMeshData;

		// Token: 0x04001E05 RID: 7685
		private List<MorphCache.CompressedBlendShape> cachedBlendShapes = new List<MorphCache.CompressedBlendShape>(32);

		// Token: 0x060021FA RID: 8698 RVA: 0x000B7854 File Offset: 0x000B5A54
		public void Cache()
		{
			if (this.Replace != null && this.Replace.sharedMesh != null)
			{
				this.GetBones();
				this.cachedMeshData = MorphCache.CacheMeshData(this.Replace.sharedMesh);
				MorphCache.CacheBlendShapes(this.cachedMeshData, ref this.cachedBlendShapes);
			}
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x0001AF8B File Offset: 0x0001918B
		public void Release()
		{
			MorphCache.ReleaseMeshData(ref this.cachedMeshData);
			MorphCache.ReleaseBlendShapes(ref this.cachedBlendShapes);
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x0001AFA3 File Offset: 0x000191A3
		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}

		// Token: 0x060021FD RID: 8701 RVA: 0x0001AFB1 File Offset: 0x000191B1
		public Transform[] GetBones()
		{
			if (this.bones == null)
			{
				this.bones = this.Replace.bones;
			}
			return this.bones;
		}
	}

	// Token: 0x020005D6 RID: 1494
	[Serializable]
	public class MaterialReplace
	{
		// Token: 0x04001E06 RID: 7686
		[HideInInspector]
		public string FindName;

		// Token: 0x04001E07 RID: 7687
		public Material Find;

		// Token: 0x04001E08 RID: 7688
		public Material Replace;

		// Token: 0x060021FF RID: 8703 RVA: 0x0001AFE7 File Offset: 0x000191E7
		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}
	}
}
