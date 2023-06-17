using System;
using UnityEngine;

// Token: 0x02000414 RID: 1044
public class Wearable : MonoBehaviour, IItemSetup, IPrefabPreProcess
{
	// Token: 0x040015CD RID: 5581
	[InspectorFlags]
	public Wearable.RemoveSkin removeSkin;

	// Token: 0x040015CE RID: 5582
	[InspectorFlags]
	public Wearable.RemoveHair removeHair;

	// Token: 0x040015CF RID: 5583
	public Wearable.DeformHair deformHair;

	// Token: 0x040015D0 RID: 5584
	public bool showCensorshipCube;

	// Token: 0x040015D1 RID: 5585
	public bool showCensorshipCubeBreasts;

	// Token: 0x040015D2 RID: 5586
	public bool forceHideCensorshipBreasts;

	// Token: 0x040015D3 RID: 5587
	[InspectorFlags]
	public Wearable.OccupationSlots occupationUnder;

	// Token: 0x040015D4 RID: 5588
	[InspectorFlags]
	public Wearable.OccupationSlots occupationOver;

	// Token: 0x040015D5 RID: 5589
	public string followBone;

	// Token: 0x040015D6 RID: 5590
	private static LOD[] emptyLOD = new LOD[1];

	// Token: 0x06001967 RID: 6503 RVA: 0x0008FFC8 File Offset: 0x0008E1C8
	public void OnItemSetup(Item item)
	{
		ItemMod[] itemMods = item.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnObjectSetup(item, base.gameObject);
		}
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x00090000 File Offset: 0x0008E200
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		foreach (LODGroup lodgroup in base.GetComponentsInChildren<LODGroup>(true))
		{
			lodgroup.SetLODs(Wearable.emptyLOD);
			preProcess.RemoveComponent(lodgroup);
		}
		if (clientside)
		{
			this.SetupRendererCache(preProcess);
		}
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x00090044 File Offset: 0x0008E244
	public void SetupRendererCache(IPrefabProcessor preProcess)
	{
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>(true))
		{
			GameObject gameObject = renderer.gameObject;
			if (renderer is SkinnedMeshRenderer)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
				if (gameObject.GetComponent<SkinnedMeshRendererInfo>() == null)
				{
					gameObject.AddComponent<SkinnedMeshRendererInfo>().Initialize(skinnedMeshRenderer);
				}
				Transform transform = skinnedMeshRenderer.FindRig();
				if (transform != null)
				{
					if (preProcess != null)
					{
						preProcess.NominateForDeletion(transform.gameObject);
					}
					else
					{
						Object.DestroyImmediate(transform.gameObject);
					}
				}
			}
			else if (renderer is MeshRenderer)
			{
				MeshRenderer source = renderer as MeshRenderer;
				if (gameObject.GetComponent<MeshRendererInfo>() == null)
				{
					gameObject.AddComponent<MeshRendererInfo>().Initialize(source);
				}
			}
		}
	}

	// Token: 0x02000415 RID: 1045
	[Flags]
	public enum RemoveSkin
	{
		// Token: 0x040015D8 RID: 5592
		Torso = 1,
		// Token: 0x040015D9 RID: 5593
		Feet = 2,
		// Token: 0x040015DA RID: 5594
		Hands = 4,
		// Token: 0x040015DB RID: 5595
		Legs = 8,
		// Token: 0x040015DC RID: 5596
		Head = 16
	}

	// Token: 0x02000416 RID: 1046
	[Flags]
	public enum RemoveHair
	{
		// Token: 0x040015DE RID: 5598
		Head = 1,
		// Token: 0x040015DF RID: 5599
		Eyebrow = 2,
		// Token: 0x040015E0 RID: 5600
		Facial = 4,
		// Token: 0x040015E1 RID: 5601
		Armpit = 8,
		// Token: 0x040015E2 RID: 5602
		Pubic = 16
	}

	// Token: 0x02000417 RID: 1047
	[Flags]
	public enum DeformHair
	{
		// Token: 0x040015E4 RID: 5604
		None = 0,
		// Token: 0x040015E5 RID: 5605
		BaseballCap = 1,
		// Token: 0x040015E6 RID: 5606
		BoonieHat = 2,
		// Token: 0x040015E7 RID: 5607
		CandleHat = 3,
		// Token: 0x040015E8 RID: 5608
		MinersHat = 4,
		// Token: 0x040015E9 RID: 5609
		WoodHelmet = 5
	}

	// Token: 0x02000418 RID: 1048
	[Flags]
	public enum OccupationSlots
	{
		// Token: 0x040015EB RID: 5611
		HeadTop = 1,
		// Token: 0x040015EC RID: 5612
		Face = 2,
		// Token: 0x040015ED RID: 5613
		HeadBack = 4,
		// Token: 0x040015EE RID: 5614
		TorsoFront = 8,
		// Token: 0x040015EF RID: 5615
		TorsoBack = 16,
		// Token: 0x040015F0 RID: 5616
		LeftShoulder = 32,
		// Token: 0x040015F1 RID: 5617
		RightShoulder = 64,
		// Token: 0x040015F2 RID: 5618
		LeftArm = 128,
		// Token: 0x040015F3 RID: 5619
		RightArm = 256,
		// Token: 0x040015F4 RID: 5620
		LeftHand = 512,
		// Token: 0x040015F5 RID: 5621
		RightHand = 1024,
		// Token: 0x040015F6 RID: 5622
		Groin = 2048,
		// Token: 0x040015F7 RID: 5623
		Bum = 4096,
		// Token: 0x040015F8 RID: 5624
		LeftKnee = 8192,
		// Token: 0x040015F9 RID: 5625
		RightKnee = 16384,
		// Token: 0x040015FA RID: 5626
		LeftLeg = 32768,
		// Token: 0x040015FB RID: 5627
		RightLeg = 65536,
		// Token: 0x040015FC RID: 5628
		LeftFoot = 131072,
		// Token: 0x040015FD RID: 5629
		RightFoot = 262144
	}
}
