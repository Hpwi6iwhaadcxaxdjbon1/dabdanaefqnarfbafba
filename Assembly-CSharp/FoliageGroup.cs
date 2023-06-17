using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200038E RID: 910
public class FoliageGroup : Pool.IPooled
{
	// Token: 0x040013EA RID: 5098
	public float LOD;

	// Token: 0x040013EB RID: 5099
	public bool NeedsRefresh;

	// Token: 0x040013EC RID: 5100
	public bool Processing;

	// Token: 0x040013ED RID: 5101
	public bool Preserving;

	// Token: 0x040013EE RID: 5102
	public ListHashSet<FoliagePlacement> Placements = new ListHashSet<FoliagePlacement>(8);

	// Token: 0x040013EF RID: 5103
	public ListHashSet<FoliageRenderer> Renderers = new ListHashSet<FoliageRenderer>(8);

	// Token: 0x040013F0 RID: 5104
	public List<FoliageRenderer> TempRenderers = new List<FoliageRenderer>();

	// Token: 0x040013F1 RID: 5105
	public List<MeshDataBatch> Batches = new List<MeshDataBatch>();

	// Token: 0x040013F2 RID: 5106
	public List<MeshDataBatch> TempBatches = new List<MeshDataBatch>();

	// Token: 0x040013F3 RID: 5107
	public List<MeshInstance> TempInstances = new List<MeshInstance>();

	// Token: 0x040013F4 RID: 5108
	private FoliageGrid grid;

	// Token: 0x040013F5 RID: 5109
	private FoliageCell cell;

	// Token: 0x040013F6 RID: 5110
	private FoliageKey key;

	// Token: 0x040013F7 RID: 5111
	private Action updateData;

	// Token: 0x040013F8 RID: 5112
	private Action refreshBatches;

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06001720 RID: 5920 RVA: 0x0001371A File Offset: 0x0001191A
	public float Size
	{
		get
		{
			return this.grid.CellSize;
		}
	}

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06001721 RID: 5921 RVA: 0x00013727 File Offset: 0x00011927
	public Vector3 Position
	{
		get
		{
			return this.cell.position;
		}
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06001722 RID: 5922 RVA: 0x00013734 File Offset: 0x00011934
	public int Count
	{
		get
		{
			return this.Renderers.Count;
		}
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x00013741 File Offset: 0x00011941
	public void Initialize(FoliageGrid grid, FoliageCell cell, FoliageKey key)
	{
		this.LOD = -1f;
		this.grid = grid;
		this.cell = cell;
		this.key = key;
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x00089630 File Offset: 0x00087830
	public void EnterPool()
	{
		this.LOD = 0f;
		this.NeedsRefresh = false;
		this.Processing = false;
		this.Preserving = false;
		this.Placements.Clear();
		this.Renderers.Clear();
		this.TempRenderers.Clear();
		this.Batches.Clear();
		this.TempBatches.Clear();
		this.TempInstances.Clear();
		this.grid = null;
		this.cell = null;
		this.key = default(FoliageKey);
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x00002ECE File Offset: 0x000010CE
	public void LeavePool()
	{
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x00013763 File Offset: 0x00011963
	public void Add(FoliageRenderer renderer)
	{
		this.Renderers.Add(renderer);
		this.NeedsRefresh = true;
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x00013778 File Offset: 0x00011978
	public void Remove(FoliageRenderer renderer)
	{
		this.Renderers.Remove(renderer);
		this.NeedsRefresh = true;
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x0001378E File Offset: 0x0001198E
	public void Add(FoliagePlacement placement)
	{
		this.Placements.Add(placement);
		this.NeedsRefresh = true;
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x000137A3 File Offset: 0x000119A3
	public void Remove(FoliagePlacement placement)
	{
		this.Placements.Remove(placement);
		this.NeedsRefresh = true;
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x000137B9 File Offset: 0x000119B9
	public void Add(MeshInstance instance)
	{
		this.TempInstances.Add(instance);
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x000896BC File Offset: 0x000878BC
	public void UpdateData()
	{
		this.TempInstances.Clear();
		if (this.LOD != 1f)
		{
			uint seed = this.cell.seed;
			BufferList<FoliagePlacement> values = this.Placements.Values;
			for (int i = 0; i < values.Count; i++)
			{
				values[i].AddBatch(this, this.LOD, seed++);
			}
			for (int j = 0; j < this.TempRenderers.Count; j++)
			{
				this.TempRenderers[j].AddBatch(this, this.LOD, seed++);
			}
		}
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x00089758 File Offset: 0x00087958
	public void CreateBatches()
	{
		if (this.TempInstances.Count == 0)
		{
			return;
		}
		MeshDataBatch meshDataBatch = this.CreateBatch();
		for (int i = 0; i < this.TempInstances.Count; i++)
		{
			MeshInstance instance = this.TempInstances[i];
			if (meshDataBatch.AvailableVertices < instance.mesh.vertexCount)
			{
				meshDataBatch = this.CreateBatch();
			}
			meshDataBatch.Add(instance);
		}
		this.TempInstances.Clear();
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x000897CC File Offset: 0x000879CC
	public void RefreshBatches()
	{
		for (int i = 0; i < this.TempBatches.Count; i++)
		{
			this.TempBatches[i].Refresh();
		}
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x00089800 File Offset: 0x00087A00
	public void ApplyBatches()
	{
		for (int i = 0; i < this.TempBatches.Count; i++)
		{
			this.TempBatches[i].Apply();
		}
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x00089834 File Offset: 0x00087A34
	public void DisplayBatches()
	{
		for (int i = 0; i < this.TempBatches.Count; i++)
		{
			this.TempBatches[i].Display();
		}
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x000137C7 File Offset: 0x000119C7
	public IEnumerator UpdateDataAsync()
	{
		if (this.updateData == null)
		{
			this.updateData = new Action(this.UpdateData);
		}
		return Parallel.Coroutine(this.updateData);
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x000137EE File Offset: 0x000119EE
	public IEnumerator RefreshBatchesAsync()
	{
		if (this.refreshBatches == null)
		{
			this.refreshBatches = new Action(this.RefreshBatches);
		}
		return Parallel.Coroutine(this.refreshBatches);
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x00089868 File Offset: 0x00087A68
	public void Start()
	{
		if (this.NeedsRefresh || this.LOD != this.cell.lod)
		{
			this.Processing = true;
			this.TempRenderers.AddRange(this.Renderers.Values);
			this.NeedsRefresh = false;
			this.LOD = this.cell.lod;
			return;
		}
		this.Preserving = true;
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x000898D0 File Offset: 0x00087AD0
	public void End()
	{
		if (this.Processing)
		{
			this.Clear();
			for (int i = 0; i < this.TempBatches.Count; i++)
			{
				this.TempBatches[i].Free();
			}
			List<MeshDataBatch> batches = this.Batches;
			this.Batches = this.TempBatches;
			this.TempBatches = batches;
			this.TempRenderers.Clear();
			this.Processing = false;
			return;
		}
		this.Preserving = false;
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x00089948 File Offset: 0x00087B48
	public void Clear()
	{
		for (int i = 0; i < this.Batches.Count; i++)
		{
			this.grid.RecycleInstance(this.Batches[i]);
		}
		this.Batches.Clear();
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x00089990 File Offset: 0x00087B90
	public void Cancel()
	{
		for (int i = 0; i < this.TempBatches.Count; i++)
		{
			this.grid.RecycleInstance(this.TempBatches[i]);
		}
		this.TempBatches.Clear();
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x000899D8 File Offset: 0x00087BD8
	public int MeshCount()
	{
		int num = 0;
		for (int i = 0; i < this.Batches.Count; i++)
		{
			num += this.Batches[i].Count;
		}
		return num;
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x00089A14 File Offset: 0x00087C14
	public int BatchedMeshCount()
	{
		int num = 0;
		for (int i = 0; i < this.Batches.Count; i++)
		{
			num += this.Batches[i].BatchedCount;
		}
		return num;
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x00089A50 File Offset: 0x00087C50
	public MeshDataBatch CreateBatch()
	{
		MeshDataBatch meshDataBatch = this.grid.CreateInstance();
		meshDataBatch.Setup(this.cell.position, this.key.material, this.grid.FoliageShadows, this.grid.FoliageLayer);
		meshDataBatch.Alloc();
		this.TempBatches.Add(meshDataBatch);
		return meshDataBatch;
	}
}
