using System;
using System.Collections;
using System.Diagnostics;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Token: 0x0200038C RID: 908
public class FoliageGrid : SingletonComponent<FoliageGrid>, IClientComponent
{
	// Token: 0x040013D5 RID: 5077
	public static bool Paused;

	// Token: 0x040013D6 RID: 5078
	public GameObjectRef BatchPrefab;

	// Token: 0x040013D7 RID: 5079
	public float CellSize = 50f;

	// Token: 0x040013D8 RID: 5080
	public float MaxMilliseconds = 0.1f;

	// Token: 0x040013D9 RID: 5081
	public LayerSelect FoliageLayer = 0;

	// Token: 0x040013DA RID: 5082
	public ShadowCastingMode FoliageShadows;

	// Token: 0x040013DB RID: 5083
	public const float MaxRefreshDistance = 100f;

	// Token: 0x040013DC RID: 5084
	internal ListHashSet<FoliagePlacement> Placements = new ListHashSet<FoliagePlacement>(8);

	// Token: 0x040013DD RID: 5085
	private WorldSpaceGrid<FoliageCell> grid;

	// Token: 0x040013DE RID: 5086
	private Vector2i curCell;

	// Token: 0x040013DF RID: 5087
	private Stopwatch watch = Stopwatch.StartNew();

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x0600170B RID: 5899 RVA: 0x00013611 File Offset: 0x00011811
	public bool Initialized
	{
		get
		{
			return this.grid != null;
		}
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x0001361C File Offset: 0x0001181C
	protected void OnEnable()
	{
		base.StartCoroutine(this.UpdateCoroutine());
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x0001362B File Offset: 0x0001182B
	public static void RefreshAll(bool force = false)
	{
		if (SingletonComponent<FoliageGrid>.Instance)
		{
			SingletonComponent<FoliageGrid>.Instance.Refresh(force);
		}
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x00089254 File Offset: 0x00087454
	public void Refresh(bool force = false)
	{
		if (this.grid == null)
		{
			this.Init();
		}
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				this.grid[i, j].Refresh(force);
			}
		}
		this.curCell = this.GetCurrentCellCoordinates();
	}

	// Token: 0x1700012A RID: 298
	public FoliageCell this[Vector3 worldPos]
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

	// Token: 0x06001710 RID: 5904 RVA: 0x000892BC File Offset: 0x000874BC
	private void Init()
	{
		this.grid = new WorldSpaceGrid<FoliageCell>(TerrainMeta.Size.x * 2f, this.CellSize);
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				this.grid[i, j] = new FoliageCell(this, this.grid.GridToWorldCoords(new Vector2i(i, j)));
			}
		}
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x00013660 File Offset: 0x00011860
	public MeshDataBatch CreateInstance()
	{
		GameObject gameObject = GameManager.client.CreatePrefab(this.BatchPrefab.resourcePath, true);
		SceneManager.MoveGameObjectToScene(gameObject, Generic.BatchingScene);
		return gameObject.GetComponent<MeshDataBatch>();
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x00013688 File Offset: 0x00011888
	public void RecycleInstance(MeshDataBatch instance)
	{
		GameManager.client.Retire(instance.gameObject);
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x0001369A File Offset: 0x0001189A
	public void AddPlacement(FoliagePlacement placement)
	{
		if (this.grid == null)
		{
			this.Init();
		}
		placement.Init();
		this.Placements.Add(placement);
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06001714 RID: 5908 RVA: 0x0008933C File Offset: 0x0008753C
	public bool NeedsTimeout
	{
		get
		{
			return this.watch.Elapsed.TotalMilliseconds > (double)this.MaxMilliseconds;
		}
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x000136BC File Offset: 0x000118BC
	public void ResetTimeout()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x000136D4 File Offset: 0x000118D4
	private IEnumerator UpdateCoroutine()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (!Application.isReceiving && !Application.isLoading && !FoliageGrid.Paused && this.grid != null)
			{
				this.ResetTimeout();
				int num = Mathf.CeilToInt(100f * this.grid.CellSizeInverse);
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
						FoliageCell foliageCell = this.grid[x, z];
						if (foliageCell.NeedsRefresh())
						{
							IEnumerator enumerator = foliageCell.RefreshAsync();
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								yield return obj;
							}
							enumerator = null;
						}
						if (this.NeedsTimeout)
						{
							yield return CoroutineEx.waitForEndOfFrame;
							this.ResetTimeout();
						}
						num3 = z;
					}
					num3 = x;
				}
				this.curCell = newCell;
				newCell = default(Vector2i);
			}
		}
		yield break;
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x000136E3 File Offset: 0x000118E3
	private Vector2i GetCurrentCellCoordinates()
	{
		if (!MainCamera.isValid)
		{
			return this.curCell;
		}
		return this.grid.WorldToGridCoords(MainCamera.position);
	}
}
