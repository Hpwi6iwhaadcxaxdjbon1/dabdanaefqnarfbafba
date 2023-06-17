using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003B9 RID: 953
public class RendererGroup : Pool.IPooled
{
	// Token: 0x0400149A RID: 5274
	public bool Invalidated;

	// Token: 0x0400149B RID: 5275
	public bool NeedsRefresh;

	// Token: 0x0400149C RID: 5276
	public bool Processing;

	// Token: 0x0400149D RID: 5277
	public bool Preserving;

	// Token: 0x0400149E RID: 5278
	public ListHashSet<RendererBatch> Renderers = new ListHashSet<RendererBatch>(8);

	// Token: 0x0400149F RID: 5279
	public List<RendererBatch> TempRenderers = new List<RendererBatch>();

	// Token: 0x040014A0 RID: 5280
	public List<MeshRendererBatch> Batches = new List<MeshRendererBatch>();

	// Token: 0x040014A1 RID: 5281
	public List<MeshRendererBatch> TempBatches = new List<MeshRendererBatch>();

	// Token: 0x040014A2 RID: 5282
	public List<MeshRendererInstance> TempInstances = new List<MeshRendererInstance>();

	// Token: 0x040014A3 RID: 5283
	private RendererGrid grid;

	// Token: 0x040014A4 RID: 5284
	private RendererCell cell;

	// Token: 0x040014A5 RID: 5285
	private RendererKey key;

	// Token: 0x040014A6 RID: 5286
	private Action updateData;

	// Token: 0x040014A7 RID: 5287
	private Action refreshBatches;

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x060017DD RID: 6109 RVA: 0x00013E56 File Offset: 0x00012056
	public float Size
	{
		get
		{
			return this.grid.CellSize;
		}
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x060017DE RID: 6110 RVA: 0x00013E63 File Offset: 0x00012063
	public Vector3 Position
	{
		get
		{
			return this.cell.position;
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x060017DF RID: 6111 RVA: 0x00013E70 File Offset: 0x00012070
	public int Count
	{
		get
		{
			return this.Renderers.Count;
		}
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x00013E7D File Offset: 0x0001207D
	public void Initialize(RendererGrid grid, RendererCell cell, RendererKey key)
	{
		this.grid = grid;
		this.cell = cell;
		this.key = key;
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x0008BD40 File Offset: 0x00089F40
	public void EnterPool()
	{
		this.Invalidated = false;
		this.NeedsRefresh = false;
		this.Processing = false;
		this.Preserving = false;
		this.Renderers.Clear();
		this.TempRenderers.Clear();
		this.Batches.Clear();
		this.TempBatches.Clear();
		this.TempInstances.Clear();
		this.grid = null;
		this.cell = null;
		this.key = default(RendererKey);
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x00002ECE File Offset: 0x000010CE
	public void LeavePool()
	{
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x00013E94 File Offset: 0x00012094
	public void Add(RendererBatch renderer)
	{
		this.Renderers.Add(renderer);
		this.NeedsRefresh = true;
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x00013EA9 File Offset: 0x000120A9
	public void Remove(RendererBatch renderer)
	{
		this.Renderers.Remove(renderer);
		this.NeedsRefresh = true;
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x0008BDBC File Offset: 0x00089FBC
	public void Invalidate()
	{
		if (!this.Invalidated)
		{
			for (int i = 0; i < this.Batches.Count; i++)
			{
				this.Batches[i].Invalidate();
			}
			this.Invalidated = true;
		}
		this.cell.interrupt = true;
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x00013EBF File Offset: 0x000120BF
	public void Add(MeshRendererInstance instance)
	{
		this.TempInstances.Add(instance);
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x0008BE0C File Offset: 0x0008A00C
	public void UpdateData()
	{
		this.TempInstances.Clear();
		int num = 0;
		while (num < this.TempRenderers.Count && !this.cell.interrupt)
		{
			this.TempRenderers[num].AddBatch(this);
			num++;
		}
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x0008BE5C File Offset: 0x0008A05C
	public void CreateBatches()
	{
		if (this.TempInstances.Count == 0)
		{
			return;
		}
		MeshRendererBatch meshRendererBatch = this.CreateBatch();
		for (int i = 0; i < this.TempInstances.Count; i++)
		{
			MeshRendererInstance instance = this.TempInstances[i];
			if (meshRendererBatch.AvailableVertices < instance.mesh.vertexCount)
			{
				meshRendererBatch = this.CreateBatch();
			}
			meshRendererBatch.Add(instance);
		}
		this.TempInstances.Clear();
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x0008BED0 File Offset: 0x0008A0D0
	public void RefreshBatches()
	{
		int num = 0;
		while (num < this.TempBatches.Count && !this.cell.interrupt)
		{
			this.TempBatches[num].Refresh();
			num++;
		}
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x0008BF14 File Offset: 0x0008A114
	public void ApplyBatches()
	{
		int num = 0;
		while (num < this.TempBatches.Count && !this.cell.interrupt)
		{
			this.TempBatches[num].Apply();
			num++;
		}
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x0008BF58 File Offset: 0x0008A158
	public void DisplayBatches()
	{
		int num = 0;
		while (num < this.TempBatches.Count && !this.cell.interrupt)
		{
			this.TempBatches[num].Display();
			num++;
		}
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x00013ECD File Offset: 0x000120CD
	public IEnumerator UpdateDataAsync()
	{
		if (this.updateData == null)
		{
			this.updateData = new Action(this.UpdateData);
		}
		return Parallel.Coroutine(this.updateData);
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x00013EF4 File Offset: 0x000120F4
	public IEnumerator RefreshBatchesAsync()
	{
		if (this.refreshBatches == null)
		{
			this.refreshBatches = new Action(this.RefreshBatches);
		}
		return Parallel.Coroutine(this.refreshBatches);
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x0008BF9C File Offset: 0x0008A19C
	public void Start()
	{
		if (this.NeedsRefresh)
		{
			this.Processing = true;
			this.TempRenderers.Clear();
			this.TempRenderers.AddRange(this.Renderers.Values);
			this.NeedsRefresh = false;
			return;
		}
		this.Preserving = true;
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x0008BFE8 File Offset: 0x0008A1E8
	public void End()
	{
		if (this.Processing)
		{
			if (!this.cell.interrupt)
			{
				this.Clear();
				for (int i = 0; i < this.TempBatches.Count; i++)
				{
					this.TempBatches[i].Free();
				}
				List<MeshRendererBatch> batches = this.Batches;
				this.Batches = this.TempBatches;
				this.TempBatches = batches;
				this.Invalidated = false;
			}
			else
			{
				this.Cancel();
			}
			this.TempRenderers.Clear();
			this.Processing = false;
			return;
		}
		this.Preserving = false;
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x0008C07C File Offset: 0x0008A27C
	public void Clear()
	{
		for (int i = 0; i < this.Batches.Count; i++)
		{
			this.grid.RecycleInstance(this.Batches[i]);
		}
		this.Batches.Clear();
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x0008C0C4 File Offset: 0x0008A2C4
	public void Cancel()
	{
		for (int i = 0; i < this.TempBatches.Count; i++)
		{
			this.grid.RecycleInstance(this.TempBatches[i]);
		}
		this.TempBatches.Clear();
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x0008C10C File Offset: 0x0008A30C
	public int MeshCount()
	{
		int num = 0;
		for (int i = 0; i < this.Batches.Count; i++)
		{
			num += this.Batches[i].Count;
		}
		return num;
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x0008C148 File Offset: 0x0008A348
	public int BatchedMeshCount()
	{
		int num = 0;
		for (int i = 0; i < this.Batches.Count; i++)
		{
			num += this.Batches[i].BatchedCount;
		}
		return num;
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x0008C184 File Offset: 0x0008A384
	public MeshRendererBatch CreateBatch()
	{
		MeshRendererBatch meshRendererBatch = this.grid.CreateInstance();
		meshRendererBatch.Setup(this.cell.position, this.key.material, this.key.shadows, this.key.layer);
		meshRendererBatch.Alloc();
		this.TempBatches.Add(meshRendererBatch);
		return meshRendererBatch;
	}
}
