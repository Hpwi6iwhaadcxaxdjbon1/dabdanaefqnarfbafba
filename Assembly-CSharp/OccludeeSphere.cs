using System;
using UnityEngine;

// Token: 0x020007AC RID: 1964
public struct OccludeeSphere
{
	// Token: 0x04002641 RID: 9793
	public int id;

	// Token: 0x04002642 RID: 9794
	public OccludeeState state;

	// Token: 0x04002643 RID: 9795
	public OcclusionCulling.Sphere sphere;

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06002A9C RID: 10908 RVA: 0x0002127C File Offset: 0x0001F47C
	public bool IsRegistered
	{
		get
		{
			return this.id >= 0;
		}
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x0002128A File Offset: 0x0001F48A
	public void Invalidate()
	{
		this.id = -1;
		this.state = null;
		this.sphere = default(OcclusionCulling.Sphere);
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x000212A6 File Offset: 0x0001F4A6
	public OccludeeSphere(int id)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = new OcclusionCulling.Sphere(Vector3.zero, 0f);
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x000212D7 File Offset: 0x0001F4D7
	public OccludeeSphere(int id, OcclusionCulling.Sphere sphere)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = sphere;
	}
}
