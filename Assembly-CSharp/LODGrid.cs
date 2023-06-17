using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020003C3 RID: 963
public class LODGrid : SingletonComponent<LODGrid>, IClientComponent
{
	// Token: 0x040014D3 RID: 5331
	public static bool Paused = false;

	// Token: 0x040014D4 RID: 5332
	public float CellSize = 50f;

	// Token: 0x040014D5 RID: 5333
	public float MaxMilliseconds = 0.1f;

	// Token: 0x040014D6 RID: 5334
	public const float MaxRefreshDistance = 500f;

	// Token: 0x040014D7 RID: 5335
	public static float TreeMeshDistance = 500f;

	// Token: 0x040014D8 RID: 5336
	private bool treeRefreshRequest;

	// Token: 0x040014D9 RID: 5337
	private Vector3 treeRefreshPosition;

	// Token: 0x040014DA RID: 5338
	private List<TreeLOD> treeQueue = new List<TreeLOD>();

	// Token: 0x040014DB RID: 5339
	private ListHashSet<TreeLOD> treeMeshes = new ListHashSet<TreeLOD>(8);

	// Token: 0x040014DC RID: 5340
	private WorldSpaceGrid<LODCell> grid;

	// Token: 0x040014DD RID: 5341
	private Vector2i curCell;

	// Token: 0x040014DE RID: 5342
	private Stopwatch watch = Stopwatch.StartNew();

	// Token: 0x06001842 RID: 6210 RVA: 0x000143C1 File Offset: 0x000125C1
	protected void OnEnable()
	{
		base.StartCoroutine(this.UpdateCoroutine());
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x000143D0 File Offset: 0x000125D0
	public static void RefreshAll()
	{
		if (SingletonComponent<LODGrid>.Instance)
		{
			SingletonComponent<LODGrid>.Instance.Refresh();
		}
	}

	// Token: 0x06001844 RID: 6212 RVA: 0x0008C980 File Offset: 0x0008AB80
	public void Refresh()
	{
		if (this.grid == null)
		{
			this.Init();
		}
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				this.grid[i, j].ChangeLOD();
			}
		}
		this.curCell = this.GetCurrentCellCoordinates();
		this.treeRefreshRequest = true;
		this.UpdateTreeMeshes();
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x000143E8 File Offset: 0x000125E8
	public static void Refresh(ILOD component, Transform transform, ref LODCell cell)
	{
		if (!SingletonComponent<LODGrid>.Instance || cell)
		{
			return;
		}
		LODGrid.Remove(component, transform, ref cell);
		LODGrid.Add(component, transform, ref cell);
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x00014410 File Offset: 0x00012610
	public static void Add(ILOD component, Transform transform, ref LODCell cell)
	{
		if (!SingletonComponent<LODGrid>.Instance || cell)
		{
			return;
		}
		cell = SingletonComponent<LODGrid>.Instance[transform.position];
		cell.Add(component, transform);
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x00014443 File Offset: 0x00012643
	public static void Remove(ILOD component, Transform transform, ref LODCell cell)
	{
		if (!SingletonComponent<LODGrid>.Instance || !cell)
		{
			return;
		}
		cell.Remove(component, transform);
		cell = null;
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x00014467 File Offset: 0x00012667
	public static void AddTreeMesh(TreeLOD component)
	{
		if (!SingletonComponent<LODGrid>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LODGrid>.Instance.treeMeshes.Contains(component))
		{
			SingletonComponent<LODGrid>.Instance.treeMeshes.Add(component);
			SingletonComponent<LODGrid>.Instance.treeRefreshRequest = true;
		}
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x000144A3 File Offset: 0x000126A3
	public static void RemoveTreeMesh(TreeLOD component)
	{
		if (!SingletonComponent<LODGrid>.Instance)
		{
			return;
		}
		SingletonComponent<LODGrid>.Instance.treeMeshes.Remove(component);
		SingletonComponent<LODGrid>.Instance.treeRefreshRequest = true;
	}

	// Token: 0x1700014B RID: 331
	public LODCell this[Vector3 worldPos]
	{
		get
		{
			if (this.grid == null)
			{
				this.Init();
			}
			return this.grid[worldPos];
		}
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x0008C9F4 File Offset: 0x0008ABF4
	private void Init()
	{
		this.grid = new WorldSpaceGrid<LODCell>(TerrainMeta.Size.x * 2f, this.CellSize);
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				this.grid[i, j] = new LODCell(this.grid.GridToWorldCoords(new Vector2i(i, j)), this.CellSize);
			}
		}
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x0008CA78 File Offset: 0x0008AC78
	private void UpdateTreeMeshes()
	{
		if (Tree.meshes > 0 && Tree.meshes < 100 && this.treeMeshes.Count > Tree.meshes && MainCamera.isValid)
		{
			if (this.treeRefreshRequest || Vector3.Distance(MainCamera.position, this.treeRefreshPosition) > 1f)
			{
				this.treeRefreshRequest = false;
				this.treeRefreshPosition = MainCamera.position;
				TreeLOD[] buffer = this.treeMeshes.Values.Buffer;
				int count = this.treeMeshes.Values.Count;
				this.treeQueue.Clear();
				for (int i = 0; i < count; i++)
				{
					this.treeQueue.Add(buffer[i]);
				}
				this.treeQueue.Sort(TreeLOD.Comparison);
				LODGrid.TreeMeshDistance = this.treeQueue[Tree.meshes - 1].CurrentDistance;
				for (int j = 0; j < Tree.meshes; j++)
				{
					this.treeQueue[j].ChangeLOD();
				}
				return;
			}
		}
		else
		{
			LODGrid.TreeMeshDistance = 500f;
		}
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x0600184D RID: 6221 RVA: 0x0008CB90 File Offset: 0x0008AD90
	public bool NeedsTimeout
	{
		get
		{
			return this.watch.Elapsed.TotalMilliseconds > (double)this.MaxMilliseconds;
		}
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x000144EA File Offset: 0x000126EA
	public void ResetTimeout()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x00014502 File Offset: 0x00012702
	private IEnumerator UpdateCoroutine()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (!Application.isReceiving && !Application.isLoading && !LODGrid.Paused && this.grid != null)
			{
				this.ResetTimeout();
				int num = Mathf.CeilToInt(500f * this.grid.CellSizeInverse);
				Vector2i newCell = this.GetCurrentCellCoordinates();
				int num2 = Mathx.Clamp(Mathf.Min(this.curCell.x, newCell.x) - num, 0, this.grid.CellCount - 1);
				int maxLeaveX = Mathx.Clamp(Mathf.Max(this.curCell.x, newCell.x) + num, 0, this.grid.CellCount - 1);
				int minLeaveZ = Mathx.Clamp(Mathf.Min(this.curCell.y, newCell.y) - num, 0, this.grid.CellCount - 1);
				int maxLeaveZ = Mathx.Clamp(Mathf.Max(this.curCell.y, newCell.y) + num, 0, this.grid.CellCount - 1);
				int num3;
				for (int x = num2; x <= maxLeaveX; x = num3 + 1)
				{
					for (int z = minLeaveZ; z <= maxLeaveZ; z = num3 + 1)
					{
						this.grid[x, z].ChangeLOD();
						if (this.NeedsTimeout)
						{
							this.UpdateTreeMeshes();
							yield return CoroutineEx.waitForEndOfFrame;
							this.ResetTimeout();
						}
						num3 = z;
					}
					num3 = x;
				}
				this.curCell = newCell;
				this.UpdateTreeMeshes();
				newCell = default(Vector2i);
			}
		}
		yield break;
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x00014511 File Offset: 0x00012711
	private Vector2i GetCurrentCellCoordinates()
	{
		if (!MainCamera.isValid)
		{
			return this.curCell;
		}
		return this.grid.WorldToGridCoords(MainCamera.position);
	}
}
