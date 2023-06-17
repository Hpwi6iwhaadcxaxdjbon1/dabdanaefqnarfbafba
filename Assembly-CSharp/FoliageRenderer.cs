using System;
using Rust;
using UnityEngine;

// Token: 0x02000391 RID: 913
public class FoliageRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x0400140A RID: 5130
	public Material material;

	// Token: 0x0400140B RID: 5131
	public Mesh LOD0;

	// Token: 0x0400140C RID: 5132
	public Mesh LOD1;

	// Token: 0x0400140D RID: 5133
	private FoliageGroup batchGroup;

	// Token: 0x0400140E RID: 5134
	private MeshInstance batchInstance;

	// Token: 0x06001743 RID: 5955 RVA: 0x00013872 File Offset: 0x00011A72
	protected void OnEnable()
	{
		this.Add();
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x0001387A File Offset: 0x00011A7A
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x00089EC8 File Offset: 0x000880C8
	public void Add()
	{
		if (this.batchGroup != null)
		{
			this.Remove();
		}
		if (base.transform == null)
		{
			return;
		}
		if (SingletonComponent<FoliageGrid>.Instance == null)
		{
			return;
		}
		FoliageCell foliageCell = SingletonComponent<FoliageGrid>.Instance[base.transform.position];
		this.batchGroup = foliageCell.FindBatchGroup(this.material);
		this.batchGroup.Add(this);
		this.batchInstance.position = base.transform.position;
		this.batchInstance.rotation = base.transform.rotation;
		this.batchInstance.scale = base.transform.lossyScale;
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x0001388A File Offset: 0x00011A8A
	public void Remove()
	{
		if (this.batchGroup == null)
		{
			return;
		}
		this.batchGroup.Remove(this);
		this.batchGroup = null;
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x000138A8 File Offset: 0x00011AA8
	public void Refresh()
	{
		this.Remove();
		this.Add();
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x000138B6 File Offset: 0x00011AB6
	public void AddBatch(FoliageGroup batchGroup, float lod, uint seed)
	{
		this.batchInstance.mesh = this.LOD0;
		batchGroup.Add(this.batchInstance);
	}
}
