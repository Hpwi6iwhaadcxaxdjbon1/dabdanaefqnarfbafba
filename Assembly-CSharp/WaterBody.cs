using System;
using UnityEngine;

// Token: 0x0200054F RID: 1359
[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
	// Token: 0x04001AE5 RID: 6885
	public WaterBodyType Type = WaterBodyType.Lake;

	// Token: 0x04001AE6 RID: 6886
	public Renderer Renderer;

	// Token: 0x04001AE7 RID: 6887
	public Collider[] Triggers;

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06001E70 RID: 7792 RVA: 0x00018124 File Offset: 0x00016324
	// (set) Token: 0x06001E6F RID: 7791 RVA: 0x0001811B File Offset: 0x0001631B
	public Transform Transform { get; private set; }

	// Token: 0x06001E71 RID: 7793 RVA: 0x0001812C File Offset: 0x0001632C
	private void Awake()
	{
		this.Transform = base.transform;
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001E73 RID: 7795 RVA: 0x00018143 File Offset: 0x00016343
	// (set) Token: 0x06001E72 RID: 7794 RVA: 0x0001813A File Offset: 0x0001633A
	public MeshFilter MeshFilter { get; private set; }

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06001E75 RID: 7797 RVA: 0x00018154 File Offset: 0x00016354
	// (set) Token: 0x06001E74 RID: 7796 RVA: 0x0001814B File Offset: 0x0001634B
	public Mesh SharedMesh { get; private set; }

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06001E77 RID: 7799 RVA: 0x00018165 File Offset: 0x00016365
	// (set) Token: 0x06001E76 RID: 7798 RVA: 0x0001815C File Offset: 0x0001635C
	public Material Material { get; private set; }

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06001E79 RID: 7801 RVA: 0x00018176 File Offset: 0x00016376
	// (set) Token: 0x06001E78 RID: 7800 RVA: 0x0001816D File Offset: 0x0001636D
	public int DepthPass { get; private set; }

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06001E7B RID: 7803 RVA: 0x00018187 File Offset: 0x00016387
	// (set) Token: 0x06001E7A RID: 7802 RVA: 0x0001817E File Offset: 0x0001637E
	public int DepthDisplacementPass { get; private set; }

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x06001E7D RID: 7805 RVA: 0x00018198 File Offset: 0x00016398
	// (set) Token: 0x06001E7C RID: 7804 RVA: 0x0001818F File Offset: 0x0001638F
	public int OcclusionPass { get; private set; }

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06001E7F RID: 7807 RVA: 0x000181A9 File Offset: 0x000163A9
	// (set) Token: 0x06001E7E RID: 7806 RVA: 0x000181A0 File Offset: 0x000163A0
	public int CausticsPass { get; private set; }

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001E81 RID: 7809 RVA: 0x000181BA File Offset: 0x000163BA
	// (set) Token: 0x06001E80 RID: 7808 RVA: 0x000181B1 File Offset: 0x000163B1
	public int OcclusionCausticsPass { get; private set; }

	// Token: 0x06001E82 RID: 7810 RVA: 0x000A7A10 File Offset: 0x000A5C10
	private void OnEnable()
	{
		if (this.Renderer == null)
		{
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			Collider[] componentsInChildren2 = base.GetComponentsInChildren<Collider>();
			if (componentsInChildren.Length != 0)
			{
				this.Renderer = componentsInChildren[0];
			}
			if (componentsInChildren2.Length != 0)
			{
				this.Triggers = componentsInChildren2;
			}
		}
		if (this.Renderer != null)
		{
			this.Transform = this.Renderer.transform;
			this.MeshFilter = this.Renderer.GetComponent<MeshFilter>();
			this.SharedMesh = this.MeshFilter.sharedMesh;
			this.Material = this.Renderer.sharedMaterials[0];
			this.DepthPass = this.Material.FindPass("WATER_DEPTH");
			this.DepthDisplacementPass = this.Material.FindPass("WATER_DEPTH_DISPLACEMENT");
			this.OcclusionPass = this.Material.FindPass("WATER_OCCLUSION");
			this.CausticsPass = this.Material.FindPass("WATER_CAUSTICS");
			this.OcclusionCausticsPass = this.Material.FindPass("WATER_OCCLUSION_CAUSTICS");
		}
		WaterSystem.RegisterBody(this);
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x000181C2 File Offset: 0x000163C2
	private void OnDisable()
	{
		WaterSystem.UnregisterBody(this);
	}
}
