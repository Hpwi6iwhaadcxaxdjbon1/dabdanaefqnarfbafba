using System;
using Rust;
using UnityEngine;

// Token: 0x0200074B RID: 1867
public abstract class MeshBatch : MonoBehaviour
{
	// Token: 0x1700028D RID: 653
	// (get) Token: 0x0600287F RID: 10367 RVA: 0x0001F75E File Offset: 0x0001D95E
	// (set) Token: 0x06002880 RID: 10368 RVA: 0x0001F766 File Offset: 0x0001D966
	public bool NeedsRefresh { get; private set; }

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06002881 RID: 10369 RVA: 0x0001F76F File Offset: 0x0001D96F
	// (set) Token: 0x06002882 RID: 10370 RVA: 0x0001F777 File Offset: 0x0001D977
	public int Count { get; private set; }

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06002883 RID: 10371 RVA: 0x0001F780 File Offset: 0x0001D980
	// (set) Token: 0x06002884 RID: 10372 RVA: 0x0001F788 File Offset: 0x0001D988
	public int BatchedCount { get; private set; }

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06002885 RID: 10373 RVA: 0x0001F791 File Offset: 0x0001D991
	// (set) Token: 0x06002886 RID: 10374 RVA: 0x0001F799 File Offset: 0x0001D999
	public int VertexCount { get; private set; }

	// Token: 0x06002887 RID: 10375
	protected abstract void AllocMemory();

	// Token: 0x06002888 RID: 10376
	protected abstract void FreeMemory();

	// Token: 0x06002889 RID: 10377
	protected abstract void RefreshMesh();

	// Token: 0x0600288A RID: 10378
	protected abstract void ApplyMesh();

	// Token: 0x0600288B RID: 10379
	protected abstract void ToggleMesh(bool state);

	// Token: 0x0600288C RID: 10380
	protected abstract void OnPooled();

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x0600288D RID: 10381
	public abstract int VertexCapacity { get; }

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x0600288E RID: 10382
	public abstract int VertexCutoff { get; }

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x0600288F RID: 10383 RVA: 0x0001F7A2 File Offset: 0x0001D9A2
	public int AvailableVertices
	{
		get
		{
			return Mathf.Clamp(this.VertexCapacity, this.VertexCutoff, 65534) - this.VertexCount;
		}
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x0001F7C1 File Offset: 0x0001D9C1
	public void Alloc()
	{
		this.AllocMemory();
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x0001F7C9 File Offset: 0x0001D9C9
	public void Free()
	{
		this.FreeMemory();
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x0001F7D1 File Offset: 0x0001D9D1
	public void Refresh()
	{
		this.RefreshMesh();
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x0001F7D9 File Offset: 0x0001D9D9
	public void Apply()
	{
		this.NeedsRefresh = false;
		this.ApplyMesh();
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x0001F7E8 File Offset: 0x0001D9E8
	public void Display()
	{
		this.ToggleMesh(true);
		this.BatchedCount = this.Count;
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x0001F7FD File Offset: 0x0001D9FD
	public void Invalidate()
	{
		this.ToggleMesh(false);
		this.BatchedCount = 0;
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000D068C File Offset: 0x000CE88C
	protected void AddVertices(int vertices)
	{
		this.NeedsRefresh = true;
		int count = this.Count;
		this.Count = count + 1;
		this.VertexCount += vertices;
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x0001F80D File Offset: 0x0001DA0D
	protected void OnEnable()
	{
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x0001F82B File Offset: 0x0001DA2B
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
		this.OnPooled();
	}
}
