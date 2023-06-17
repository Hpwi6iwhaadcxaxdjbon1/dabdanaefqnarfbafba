using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000228 RID: 552
public class SkinnedMultiMesh : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x04000DAB RID: 3499
	public bool shadowOnly;

	// Token: 0x04000DAC RID: 3500
	public List<SkinnedMultiMesh.Part> parts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x04000DAD RID: 3501
	public Dictionary<string, Transform> boneDict = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x04000DAE RID: 3502
	[NonSerialized]
	public List<SkinnedMultiMesh.Part> createdParts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x04000DAF RID: 3503
	[NonSerialized]
	public long lastBuildHash;

	// Token: 0x04000DB0 RID: 3504
	[NonSerialized]
	public MaterialPropertyBlock sharedPropertyBlock;

	// Token: 0x04000DB1 RID: 3505
	[NonSerialized]
	public MaterialPropertyBlock hairPropertyBlock;

	// Token: 0x04000DB2 RID: 3506
	public float skinNumber;

	// Token: 0x04000DB3 RID: 3507
	public float meshNumber;

	// Token: 0x04000DB4 RID: 3508
	public float hairNumber;

	// Token: 0x04000DB5 RID: 3509
	public int skinType;

	// Token: 0x04000DB6 RID: 3510
	public SkinSetCollection SkinCollection;

	// Token: 0x04000DB7 RID: 3511
	private ArticulatedOccludee articulatedOccludee;

	// Token: 0x04000DB8 RID: 3512
	private LODGroup lodGroup;

	// Token: 0x04000DB9 RID: 3513
	private List<Renderer> renderers = new List<Renderer>(32);

	// Token: 0x04000DBA RID: 3514
	private List<Animator> animators = new List<Animator>(8);

	// Token: 0x04000DBB RID: 3515
	internal bool IsVisible = true;

	// Token: 0x04000DBC RID: 3516
	private LOD[] lods = new LOD[4];

	// Token: 0x04000DBD RID: 3517
	private static LOD[] emptyLOD = new LOD[1];

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060010BD RID: 4285 RVA: 0x0000EB37 File Offset: 0x0000CD37
	public List<Renderer> Renderers
	{
		get
		{
			return this.renderers;
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060010BE RID: 4286 RVA: 0x0000EB3F File Offset: 0x0000CD3F
	public List<Animator> Animators
	{
		get
		{
			return this.animators;
		}
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x0000EB47 File Offset: 0x0000CD47
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside && !base.gameObject.GetComponent<LODGroup>())
		{
			base.gameObject.AddComponent<LODGroup>();
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x0000EB6B File Offset: 0x0000CD6B
	private void Awake()
	{
		this.articulatedOccludee = base.GetComponent<ArticulatedOccludee>();
		this.lodGroup = base.GetComponent<LODGroup>();
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x0000EB85 File Offset: 0x0000CD85
	public void BuildBoneDictionary()
	{
		if (this.boneDict.Count > 0)
		{
			return;
		}
		this.BuildBoneDictionary(base.transform);
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x00070D8C File Offset: 0x0006EF8C
	private void BuildBoneDictionary(Transform t)
	{
		if (t != base.transform && !this.boneDict.ContainsKey(t.name))
		{
			this.boneDict.Add(t.name, t);
		}
		for (int i = 0; i < t.childCount; i++)
		{
			this.BuildBoneDictionary(t.GetChild(i));
		}
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x00070DEC File Offset: 0x0006EFEC
	public void Clear()
	{
		this.ClearLODGroup();
		foreach (SkinnedMultiMesh.Part part in this.createdParts)
		{
			GameManager.client.Retire(part.gameObject);
		}
		this.renderers.Clear();
		this.animators.Clear();
		if (this.articulatedOccludee != null)
		{
			this.articulatedOccludee.ClearVisibility();
		}
		this.createdParts.Clear();
		this.lastBuildHash = 0L;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00070E90 File Offset: 0x0006F090
	public SkinnedMultiMesh.Part[] FindParts(string name)
	{
		return Enumerable.ToArray<SkinnedMultiMesh.Part>(Enumerable.Where<SkinnedMultiMesh.Part>(this.createdParts, (SkinnedMultiMesh.Part x) => x.name == name));
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x00070EC8 File Offset: 0x0006F0C8
	public long WorkoutPartsHash()
	{
		if (this.parts == null)
		{
			return 0L;
		}
		return Enumerable.Sum<SkinnedMultiMesh.Part>(this.parts, delegate(SkinnedMultiMesh.Part x)
		{
			if (!(x.gameObject == null))
			{
				return (long)x.gameObject.GetHashCode() + (long)((x.item == null) ? 0UL : ((ulong)x.item.uid + (ulong)((long)((int)x.item.skin))));
			}
			return 0L;
		}) + (this.shadowOnly ? 1L : 0L) + (long)(this.skinNumber * 100000f) + (long)(this.meshNumber * 100000f) + (long)(this.hairNumber * 100000f);
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x00070F44 File Offset: 0x0006F144
	private void GatherRenderersAndAnimators()
	{
		this.renderers.Clear();
		if (this.lodGroup != null && this.lodGroup.lodCount > 0)
		{
			LOD[] array = this.lodGroup.GetLODs();
			for (int i = 0; i < array.Length; i++)
			{
				this.renderers.AddRange(array[i].renderers);
			}
		}
		this.animators.Clear();
		base.GetComponentsInChildren<Animator>(true, this.animators);
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x00070FC4 File Offset: 0x0006F1C4
	public void DeformCreatedPart(string strName, int blendShapeIndex)
	{
		for (int i = 0; i < this.createdParts.Count; i++)
		{
			if (this.createdParts[i].gameObject.name == strName)
			{
				List<MorphCache> list = Pool.GetList<MorphCache>();
				this.createdParts[i].gameObject.GetComponentsInChildren<MorphCache>(true, list);
				foreach (MorphCache morphCache in list)
				{
					morphCache.SetBlendShape(blendShapeIndex);
				}
				Pool.FreeList<MorphCache>(ref list);
			}
		}
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x0007106C File Offset: 0x0006F26C
	public void RebuildModel(PlayerModel model, bool reset)
	{
		long num = this.WorkoutPartsHash();
		if (this.lastBuildHash == num)
		{
			return;
		}
		if (this.boneDict.Count == 0)
		{
			this.BuildBoneDictionary(base.transform);
		}
		this.Clear();
		List<Renderer> list = Pool.GetList<Renderer>();
		List<BoneRetarget> list2 = Pool.GetList<BoneRetarget>();
		List<SkinnedMeshRenderer> list3 = Pool.GetList<SkinnedMeshRenderer>();
		List<SkinnedMeshRendererInfo> list4 = Pool.GetList<SkinnedMeshRendererInfo>();
		List<ComponentInfo> list5 = Pool.GetList<ComponentInfo>();
		List<IItemSetup> list6 = Pool.GetList<IItemSetup>();
		this.sharedPropertyBlock = ((this.sharedPropertyBlock != null) ? this.sharedPropertyBlock : new MaterialPropertyBlock());
		this.hairPropertyBlock = ((this.hairPropertyBlock != null) ? this.hairPropertyBlock : new MaterialPropertyBlock());
		foreach (SkinnedMultiMesh.Part part in this.parts)
		{
			if (part.gameObject == null)
			{
				Debug.LogWarning("Skinned multi mesh part not found: " + part.name);
			}
			else
			{
				GameObject gameObject = null;
				if (!string.IsNullOrEmpty(part.name))
				{
					gameObject = GameManager.client.CreatePrefab(part.name, base.transform, false);
				}
				else
				{
					gameObject = Object.Instantiate<GameObject>(part.gameObject);
					gameObject.transform.SetParent(base.transform, false);
					gameObject.SetActive(false);
				}
				if (gameObject == null)
				{
					Debug.LogWarning("Skinned multi mesh part not found: " + part.name);
				}
				else
				{
					gameObject.SetLayerRecursive(base.gameObject.layer);
					list.Clear();
					list2.Clear();
					list3.Clear();
					list4.Clear();
					list5.Clear();
					list6.Clear();
					gameObject.GetComponentsInChildren<Renderer>(true, list);
					gameObject.GetComponentsInChildren<BoneRetarget>(true, list2);
					gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list3);
					gameObject.GetComponentsInChildren<SkinnedMeshRendererInfo>(true, list4);
					gameObject.GetComponentsInChildren<ComponentInfo>(true, list5);
					gameObject.GetComponentsInChildren<IItemSetup>(true, list6);
					if (reset)
					{
						foreach (ComponentInfo componentInfo in list5)
						{
							componentInfo.Reset();
						}
						MaterialReplacement.Reset(part.gameObject);
					}
					this.sharedPropertyBlock.Clear();
					this.hairPropertyBlock.Clear();
					PlayerModelSkin component = gameObject.GetComponent<PlayerModelSkin>();
					if (component && this.SkinCollection)
					{
						model.SetSkinProperties(this.sharedPropertyBlock);
						component.Setup(this.SkinCollection, this.skinNumber, this.meshNumber);
					}
					PlayerModelHairCap component2 = gameObject.GetComponent<PlayerModelHairCap>();
					if (component2 && this.SkinCollection)
					{
						component2.SetupHairCap(this.SkinCollection, this.hairNumber, this.meshNumber, this.sharedPropertyBlock);
					}
					PlayerModelHair component3 = gameObject.GetComponent<PlayerModelHair>();
					if (component3 && this.SkinCollection)
					{
						component3.Setup(this.SkinCollection, this.hairNumber, this.meshNumber, this.hairPropertyBlock);
					}
					Wearable component4 = gameObject.GetComponent<Wearable>();
					if (component4 && !string.IsNullOrEmpty(component4.followBone))
					{
						Transform transform = this.FindBone(component4.followBone);
						if (transform != null)
						{
							gameObject.transform.SetParent(transform, false);
							gameObject.transform.localPosition = Vector3.zero;
							gameObject.transform.localRotation = Quaternion.identity;
						}
						else
						{
							Debug.LogWarning("Couldn't retarget bone: " + component4.followBone);
						}
					}
					MeshReplacement.Process(gameObject, this.skinType == 1);
					foreach (BoneRetarget boneRetarget in list2)
					{
						string name = boneRetarget.transform.parent.name;
						Transform transform2 = this.FindBone(name);
						if (transform2 != null)
						{
							boneRetarget.transform.SetParent(transform2, false);
							boneRetarget.gameObject.name = part.name;
							this.createdParts.Add(new SkinnedMultiMesh.Part
							{
								gameObject = boneRetarget.gameObject,
								name = part.name,
								item = part.item
							});
						}
						else
						{
							Debug.LogWarning("Couldn't retarget bone: " + name);
						}
					}
					foreach (SkinnedMeshRendererInfo skinnedMeshRendererInfo in list4)
					{
						skinnedMeshRendererInfo.RemapBones(this);
					}
					MaterialPropertyBlock materialPropertyBlock = (component3 != null) ? this.hairPropertyBlock : this.sharedPropertyBlock;
					foreach (Renderer renderer in list)
					{
						this.DoShadowOverride(renderer, materialPropertyBlock);
						if (materialPropertyBlock != null)
						{
							renderer.SetPropertyBlock(materialPropertyBlock);
						}
					}
					if (part.item != null)
					{
						foreach (IItemSetup itemSetup in list6)
						{
							itemSetup.OnItemSetup(part.item);
						}
					}
					gameObject.AwakeFromInstantiate();
					gameObject.SetActive(this.IsVisible);
					this.createdParts.Add(new SkinnedMultiMesh.Part
					{
						gameObject = gameObject,
						name = part.name,
						item = part.item
					});
				}
			}
		}
		foreach (SkinnedMultiMesh.Part part2 in this.createdParts)
		{
			Wearable component5 = part2.gameObject.GetComponent<Wearable>();
			if (component5 != null && component5.deformHair != Wearable.DeformHair.None)
			{
				this.DeformCreatedPart("assets/prefabs/clothes/hair/hair_head.prefab", component5.deformHair - Wearable.DeformHair.BaseballCap);
			}
		}
		Pool.FreeList<Renderer>(ref list);
		Pool.FreeList<BoneRetarget>(ref list2);
		Pool.FreeList<SkinnedMeshRenderer>(ref list3);
		Pool.FreeList<SkinnedMeshRendererInfo>(ref list4);
		Pool.FreeList<ComponentInfo>(ref list5);
		Pool.FreeList<IItemSetup>(ref list6);
		ParticleSystemRenderer[] componentsInChildren = base.GetComponentsInChildren<ParticleSystemRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = !this.shadowOnly;
		}
		this.UpdateLODGroup();
		Animator component6 = base.GetComponent<Animator>();
		if (component6 != null && component6.cullingMode == 1)
		{
			component6.cullingMode = 0;
			component6.cullingMode = 1;
		}
		if (this.articulatedOccludee != null && this.articulatedOccludee.enabled)
		{
			this.articulatedOccludee.ProcessVisibility(this.lodGroup);
		}
		this.GatherRenderersAndAnimators();
		this.lastBuildHash = num;
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x000717B8 File Offset: 0x0006F9B8
	public Transform FindBone(string strName)
	{
		if (this.boneDict.Count == 0)
		{
			this.BuildBoneDictionary(base.transform);
		}
		Transform result = null;
		if (this.boneDict.TryGetValue(strName, ref result))
		{
			return result;
		}
		return base.transform;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x0000EBA2 File Offset: 0x0000CDA2
	internal void DoShadowOverride(Renderer renderer, MaterialPropertyBlock block)
	{
		this.SetupLightShadowBiasScale(renderer, block, this.shadowOnly);
		if (!this.shadowOnly)
		{
			return;
		}
		renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x000717F8 File Offset: 0x0006F9F8
	private void SetupLightShadowBiasScale(Renderer renderer, MaterialPropertyBlock block, bool shadowOnly)
	{
		float value = 1f;
		if (shadowOnly)
		{
			value = 10f;
		}
		block.SetFloat("_ShadowBiasScale", value);
		renderer.SetPropertyBlock(block);
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x00071828 File Offset: 0x0006FA28
	public void SetVisible(bool bVisible)
	{
		if (bVisible == this.IsVisible)
		{
			return;
		}
		foreach (SkinnedMultiMesh.Part part in this.createdParts)
		{
			part.gameObject.SetActive(this.IsVisible);
		}
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x0000EBC2 File Offset: 0x0000CDC2
	private void ClearLODGroup()
	{
		if (this.lodGroup == null)
		{
			return;
		}
		this.lodGroup.SetLODs(SkinnedMultiMesh.emptyLOD);
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x00071890 File Offset: 0x0006FA90
	private void UpdateLODGroup()
	{
		if (this.lodGroup == null)
		{
			return;
		}
		List<Renderer> list = Pool.GetList<Renderer>();
		List<Renderer> list2 = Pool.GetList<Renderer>();
		List<Renderer> list3 = Pool.GetList<Renderer>();
		List<Renderer> list4 = Pool.GetList<Renderer>();
		List<Renderer> list5 = Pool.GetList<Renderer>();
		base.GetComponentsInChildren<Renderer>(true, list);
		foreach (Renderer renderer in list)
		{
			if (!(renderer.gameObject.ToBaseEntity() != base.gameObject.ToBaseEntity()) && !(renderer is ParticleSystemRenderer))
			{
				string name = renderer.name;
				if (name.EndsWith("0"))
				{
					list2.Add(renderer);
				}
				else if (name.EndsWith("1"))
				{
					list3.Add(renderer);
				}
				else if (name.EndsWith("2"))
				{
					list4.Add(renderer);
				}
				else if (name.EndsWith("3"))
				{
					list5.Add(renderer);
				}
			}
		}
		this.lods[0].renderers = list2.ToArray();
		this.lods[1].renderers = list3.ToArray();
		this.lods[2].renderers = list4.ToArray();
		this.lods[3].renderers = list5.ToArray();
		this.lods[0].screenRelativeTransitionHeight = 0.5f;
		this.lods[1].screenRelativeTransitionHeight = 0.2f;
		this.lods[2].screenRelativeTransitionHeight = 0.1f;
		this.lods[3].screenRelativeTransitionHeight = 0.001f;
		this.lodGroup.SetLODs(this.lods);
		Pool.FreeList<Renderer>(ref list);
		Pool.FreeList<Renderer>(ref list2);
		Pool.FreeList<Renderer>(ref list3);
		Pool.FreeList<Renderer>(ref list4);
		Pool.FreeList<Renderer>(ref list5);
	}

	// Token: 0x02000229 RID: 553
	public struct Part
	{
		// Token: 0x04000DBE RID: 3518
		public GameObject gameObject;

		// Token: 0x04000DBF RID: 3519
		public string name;

		// Token: 0x04000DC0 RID: 3520
		public Item item;
	}
}
