using System;
using System.Collections;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x0200038A RID: 906
public class FoliageCell
{
	// Token: 0x040013C6 RID: 5062
	public Vector3 position;

	// Token: 0x040013C7 RID: 5063
	public FoliageGrid grid;

	// Token: 0x040013C8 RID: 5064
	public bool interrupt;

	// Token: 0x040013C9 RID: 5065
	public float lod;

	// Token: 0x040013CA RID: 5066
	public uint seed;

	// Token: 0x040013CB RID: 5067
	private ListHashSet<FoliagePlacement> placements;

	// Token: 0x040013CC RID: 5068
	private ListDictionary<FoliageKey, FoliageGroup> batches = new ListDictionary<FoliageKey, FoliageGroup>(8);

	// Token: 0x060016FC RID: 5884 RVA: 0x000135B0 File Offset: 0x000117B0
	public FoliageCell(FoliageGrid grid, Vector3 position)
	{
		this.grid = grid;
		this.position = position;
		this.seed = SeedEx.Seed(position, global::World.Seed);
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x00088A6C File Offset: 0x00086C6C
	private void Init()
	{
		this.placements = new ListHashSet<FoliagePlacement>(8);
		Vector3 pivot = this.position;
		float cellSize = this.grid.CellSize;
		for (int i = 0; i < this.grid.Placements.Count; i++)
		{
			FoliagePlacement foliagePlacement = this.grid.Placements[i];
			if (foliagePlacement.CheckBatch(pivot, cellSize))
			{
				this.placements.Add(foliagePlacement);
			}
		}
	}

	// Token: 0x060016FE RID: 5886 RVA: 0x00088ADC File Offset: 0x00086CDC
	public bool NeedsRefresh()
	{
		float num = this.CalculateLOD();
		if (this.lod != num)
		{
			return true;
		}
		BufferList<FoliageGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			FoliageGroup foliageGroup = values[i];
			if (foliageGroup.NeedsRefresh || foliageGroup.LOD != num)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060016FF RID: 5887 RVA: 0x00088B34 File Offset: 0x00086D34
	public void Refresh(bool force = false)
	{
		this.interrupt = false;
		this.lod = this.CalculateLOD();
		BufferList<FoliageGroup> values = this.batches.Values;
		if (this.lod != 1f)
		{
			if (this.placements == null)
			{
				this.Init();
			}
			for (int i = 0; i < this.placements.Count; i++)
			{
				this.FindBatchGroup(this.placements[i].material);
			}
		}
		if (force)
		{
			for (int j = 0; j < values.Count; j++)
			{
				FoliageGroup foliageGroup = values[j];
				if (!foliageGroup.Processing && (foliageGroup.Count > 0 || this.lod != 1f))
				{
					foliageGroup.NeedsRefresh = true;
				}
			}
		}
		for (int k = 0; k < values.Count; k++)
		{
			FoliageGroup foliageGroup2 = values[k];
			if (!foliageGroup2.Processing)
			{
				if (foliageGroup2.Count > 0 || this.lod != 1f)
				{
					foliageGroup2.Start();
					if (foliageGroup2.Processing)
					{
						foliageGroup2.UpdateData();
						foliageGroup2.CreateBatches();
						foliageGroup2.RefreshBatches();
						foliageGroup2.ApplyBatches();
						foliageGroup2.DisplayBatches();
					}
					foliageGroup2.End();
				}
				else
				{
					foliageGroup2.Clear();
					this.DestroyFoliageGroup(ref foliageGroup2);
					this.batches.RemoveAt(k--);
				}
			}
		}
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x000135E3 File Offset: 0x000117E3
	public IEnumerator RefreshAsync()
	{
		this.interrupt = false;
		this.lod = this.CalculateLOD();
		BufferList<FoliageGroup> batchGroups = this.batches.Values;
		if (this.lod != 1f)
		{
			if (this.placements == null)
			{
				this.Init();
			}
			for (int k = 0; k < this.placements.Count; k++)
			{
				this.FindBatchGroup(this.placements[k].material);
			}
		}
		for (int l = 0; l < batchGroups.Count; l++)
		{
			FoliageGroup foliageGroup = batchGroups[l];
			if (foliageGroup.Count > 0 || this.lod != 1f)
			{
				foliageGroup.Start();
			}
		}
		int i = 0;
		while (i < batchGroups.Count && !this.interrupt)
		{
			FoliageGroup foliageGroup2 = batchGroups[i];
			if (foliageGroup2.Processing)
			{
				IEnumerator enumerator = foliageGroup2.UpdateDataAsync();
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					yield return obj;
				}
				this.grid.ResetTimeout();
				enumerator = null;
			}
			int num = i;
			i = num + 1;
		}
		i = 0;
		while (i < batchGroups.Count && !this.interrupt)
		{
			FoliageGroup foliageGroup3 = batchGroups[i];
			if (foliageGroup3.Processing)
			{
				foliageGroup3.CreateBatches();
				if (this.grid.NeedsTimeout)
				{
					yield return CoroutineEx.waitForEndOfFrame;
					this.grid.ResetTimeout();
				}
			}
			int num = i;
			i = num + 1;
		}
		i = 0;
		while (i < batchGroups.Count && !this.interrupt)
		{
			FoliageGroup foliageGroup4 = batchGroups[i];
			if (foliageGroup4.Processing)
			{
				IEnumerator enumerator = foliageGroup4.RefreshBatchesAsync();
				while (enumerator.MoveNext())
				{
					object obj2 = enumerator.Current;
					yield return obj2;
				}
				this.grid.ResetTimeout();
				enumerator = null;
			}
			int num = i;
			i = num + 1;
		}
		i = 0;
		while (i < batchGroups.Count && !this.interrupt)
		{
			FoliageGroup batchGroup = batchGroups[i];
			int num;
			if (batchGroup.Processing)
			{
				int j = 0;
				while (j < batchGroup.TempBatches.Count && !this.interrupt)
				{
					batchGroup.TempBatches[j].Apply();
					if (this.grid.NeedsTimeout)
					{
						yield return CoroutineEx.waitForEndOfFrame;
						this.grid.ResetTimeout();
					}
					num = j;
					j = num + 1;
				}
			}
			batchGroup = null;
			num = i;
			i = num + 1;
		}
		int num2 = 0;
		while (num2 < batchGroups.Count && !this.interrupt)
		{
			FoliageGroup foliageGroup5 = batchGroups[num2];
			if (foliageGroup5.Processing)
			{
				foliageGroup5.DisplayBatches();
			}
			num2++;
		}
		for (int m = 0; m < batchGroups.Count; m++)
		{
			FoliageGroup foliageGroup6 = batchGroups[m];
			if (foliageGroup6.Processing || foliageGroup6.Preserving)
			{
				foliageGroup6.End();
			}
			else if (foliageGroup6.Count == 0 && !this.interrupt)
			{
				foliageGroup6.Clear();
				this.DestroyFoliageGroup(ref foliageGroup6);
				this.batches.RemoveAt(m--);
			}
		}
		yield break;
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x00088C90 File Offset: 0x00086E90
	private float CalculateLOD()
	{
		if (TerrainMeta.HeightMap == null)
		{
			return 1f;
		}
		Vector3 pos = new Vector3(this.position.x, TerrainMeta.HeightMap.GetHeight(this.position), this.position.z);
		float num = Mathx.Discretize01(Mathf.InverseLerp(this.grid.CellSize, 100f - this.grid.CellSize, MainCamera.Distance(pos)), 5);
		if (num == 0f || num == 1f)
		{
			return num;
		}
		return Mathf.Max(num, Mathf.Lerp(0.75f, 0f, Grass.quality01));
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x00088D38 File Offset: 0x00086F38
	public FoliageGroup FindBatchGroup(Material material)
	{
		FoliageKey foliageKey = new FoliageKey(material);
		FoliageGroup foliageGroup;
		if (!this.batches.TryGetValue(foliageKey, ref foliageGroup))
		{
			foliageGroup = this.CreateFoliageGroup(this.grid, this, foliageKey);
			this.batches.Add(foliageKey, foliageGroup);
		}
		return foliageGroup;
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x00088D7C File Offset: 0x00086F7C
	private FoliageGroup CreateFoliageGroup(FoliageGrid grid, FoliageCell cell, FoliageKey key)
	{
		FoliageGroup foliageGroup = Pool.Get<FoliageGroup>();
		foliageGroup.Initialize(grid, cell, key);
		if (this.placements == null)
		{
			this.Init();
		}
		for (int i = 0; i < this.placements.Count; i++)
		{
			FoliagePlacement foliagePlacement = this.placements[i];
			if (!(foliagePlacement.material != key.material))
			{
				foliageGroup.Add(foliagePlacement);
			}
		}
		return foliageGroup;
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x000135F2 File Offset: 0x000117F2
	private void DestroyFoliageGroup(ref FoliageGroup grp)
	{
		Pool.Free<FoliageGroup>(ref grp);
	}
}
