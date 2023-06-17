using System;
using System.Collections;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020003B5 RID: 949
public class RendererCell
{
	// Token: 0x04001482 RID: 5250
	public Vector3 position;

	// Token: 0x04001483 RID: 5251
	public RendererGrid grid;

	// Token: 0x04001484 RID: 5252
	public bool interrupt;

	// Token: 0x04001485 RID: 5253
	private ListDictionary<RendererKey, RendererGroup> batches = new ListDictionary<RendererKey, RendererGroup>(8);

	// Token: 0x060017BA RID: 6074 RVA: 0x00013D24 File Offset: 0x00011F24
	public RendererCell(RendererGrid grid, Vector3 position)
	{
		this.grid = grid;
		this.position = position;
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x0008B3D4 File Offset: 0x000895D4
	public bool NeedsRefresh()
	{
		BufferList<RendererGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i].NeedsRefresh)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x0008B410 File Offset: 0x00089610
	public int MeshCount()
	{
		int num = 0;
		BufferList<RendererGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			num += values[i].MeshCount();
		}
		return num;
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x0008B44C File Offset: 0x0008964C
	public int BatchedMeshCount()
	{
		int num = 0;
		BufferList<RendererGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			num += values[i].BatchedMeshCount();
		}
		return num;
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x0008B488 File Offset: 0x00089688
	public void Refresh()
	{
		this.interrupt = false;
		BufferList<RendererGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			RendererGroup rendererGroup = values[i];
			if (!rendererGroup.Processing)
			{
				if (rendererGroup.Count > 0)
				{
					rendererGroup.Start();
					if (rendererGroup.Processing)
					{
						rendererGroup.UpdateData();
						rendererGroup.CreateBatches();
						rendererGroup.RefreshBatches();
						rendererGroup.ApplyBatches();
						rendererGroup.DisplayBatches();
					}
					rendererGroup.End();
				}
				else
				{
					rendererGroup.Clear();
					this.DestroyRendererGroup(ref rendererGroup);
					this.batches.RemoveAt(i--);
				}
			}
		}
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x00013D46 File Offset: 0x00011F46
	public IEnumerator RefreshAsync()
	{
		this.interrupt = false;
		BufferList<RendererGroup> batchGroups = this.batches.Values;
		for (int k = 0; k < batchGroups.Count; k++)
		{
			RendererGroup rendererGroup = batchGroups[k];
			if (rendererGroup.Count > 0)
			{
				rendererGroup.Start();
			}
		}
		int i = 0;
		while (i < batchGroups.Count && !this.interrupt)
		{
			RendererGroup rendererGroup2 = batchGroups[i];
			if (rendererGroup2.Processing)
			{
				if (Batching.renderer_threading)
				{
					IEnumerator enumerator = rendererGroup2.UpdateDataAsync();
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						yield return obj;
					}
					this.grid.ResetTimeout();
					enumerator = null;
				}
				else
				{
					rendererGroup2.UpdateData();
					if (this.grid.NeedsTimeout)
					{
						yield return CoroutineEx.waitForEndOfFrame;
						this.grid.ResetTimeout();
					}
				}
			}
			int num = i;
			i = num + 1;
		}
		i = 0;
		while (i < batchGroups.Count && !this.interrupt)
		{
			RendererGroup rendererGroup3 = batchGroups[i];
			if (rendererGroup3.Processing)
			{
				rendererGroup3.CreateBatches();
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
			RendererGroup rendererGroup4 = batchGroups[i];
			if (rendererGroup4.Processing)
			{
				if (Batching.renderer_threading)
				{
					IEnumerator enumerator = rendererGroup4.RefreshBatchesAsync();
					while (enumerator.MoveNext())
					{
						object obj2 = enumerator.Current;
						yield return obj2;
					}
					this.grid.ResetTimeout();
					enumerator = null;
				}
				else
				{
					rendererGroup4.RefreshBatches();
					if (this.grid.NeedsTimeout)
					{
						yield return CoroutineEx.waitForEndOfFrame;
						this.grid.ResetTimeout();
					}
				}
			}
			int num = i;
			i = num + 1;
		}
		i = 0;
		while (i < batchGroups.Count && !this.interrupt)
		{
			RendererGroup batchGroup = batchGroups[i];
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
			RendererGroup rendererGroup5 = batchGroups[num2];
			if (rendererGroup5.Processing)
			{
				rendererGroup5.DisplayBatches();
			}
			num2++;
		}
		for (int l = 0; l < batchGroups.Count; l++)
		{
			RendererGroup rendererGroup6 = batchGroups[l];
			if (rendererGroup6.Processing || rendererGroup6.Preserving)
			{
				rendererGroup6.End();
			}
			else if (rendererGroup6.Count == 0 && !this.interrupt)
			{
				rendererGroup6.Clear();
				this.DestroyRendererGroup(ref rendererGroup6);
				this.batches.RemoveAt(l--);
			}
		}
		yield break;
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x0008B528 File Offset: 0x00089728
	public RendererGroup FindBatchGroup(RendererBatch renderer)
	{
		RendererKey rendererKey = new RendererKey(renderer);
		RendererGroup rendererGroup;
		if (!this.batches.TryGetValue(rendererKey, ref rendererGroup))
		{
			rendererGroup = this.CreateRendererGroup(this.grid, this, rendererKey);
			this.batches.Add(rendererKey, rendererGroup);
		}
		return rendererGroup;
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x00013D55 File Offset: 0x00011F55
	private RendererGroup CreateRendererGroup(RendererGrid grid, RendererCell cell, RendererKey key)
	{
		RendererGroup rendererGroup = Pool.Get<RendererGroup>();
		rendererGroup.Initialize(grid, cell, key);
		return rendererGroup;
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x00013D65 File Offset: 0x00011F65
	private void DestroyRendererGroup(ref RendererGroup grp)
	{
		Pool.Free<RendererGroup>(ref grp);
	}
}
