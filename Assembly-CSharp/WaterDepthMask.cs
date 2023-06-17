using System;
using UnityEngine;

// Token: 0x02000553 RID: 1363
public class WaterDepthMask : MonoBehaviour
{
	// Token: 0x04001AFC RID: 6908
	private Mesh mesh;

	// Token: 0x04001AFD RID: 6909
	private Material material;

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x06001EA9 RID: 7849 RVA: 0x00018363 File Offset: 0x00016563
	public Mesh Mesh
	{
		get
		{
			return this.mesh;
		}
	}

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x06001EAA RID: 7850 RVA: 0x0001836B File Offset: 0x0001656B
	public Material Material
	{
		get
		{
			return this.material;
		}
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x00018373 File Offset: 0x00016573
	private void OnEnable()
	{
		this.mesh = base.GetComponent<MeshFilter>().sharedMesh;
		this.material = base.GetComponent<Renderer>().sharedMaterial;
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x00018397 File Offset: 0x00016597
	private void OnBecameVisible()
	{
		WaterSystem.RegisterDepthMask(this);
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x0001839F File Offset: 0x0001659F
	private void OnBecameInvisible()
	{
		WaterSystem.UnregisterDepthMask(this);
	}
}
