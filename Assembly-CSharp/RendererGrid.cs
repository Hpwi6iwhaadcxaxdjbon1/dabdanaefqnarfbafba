using System;
using System.Collections;
using System.Diagnostics;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020003B7 RID: 951
public class RendererGrid : SingletonComponent<RendererGrid>, IClientComponent
{
	// Token: 0x0400148E RID: 5262
	public static bool Paused;

	// Token: 0x0400148F RID: 5263
	public GameObjectRef BatchPrefab;

	// Token: 0x04001490 RID: 5264
	public float CellSize = 50f;

	// Token: 0x04001491 RID: 5265
	public float MaxMilliseconds = 0.1f;

	// Token: 0x04001492 RID: 5266
	private WorldSpaceGrid<RendererCell> grid;

	// Token: 0x04001493 RID: 5267
	private Stopwatch watch = Stopwatch.StartNew();

	// Token: 0x060017C9 RID: 6089 RVA: 0x00013D84 File Offset: 0x00011F84
	protected void OnEnable()
	{
		base.StartCoroutine(this.UpdateCoroutine());
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x00013D93 File Offset: 0x00011F93
	public static void RefreshAll()
	{
		if (SingletonComponent<RendererGrid>.Instance)
		{
			SingletonComponent<RendererGrid>.Instance.Refresh();
		}
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x0008BA14 File Offset: 0x00089C14
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
				this.grid[i, j].Refresh();
			}
		}
	}

	// Token: 0x17000142 RID: 322
	public RendererCell this[Vector3 worldPos]
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

	// Token: 0x060017CD RID: 6093 RVA: 0x0008BA70 File Offset: 0x00089C70
	private void Init()
	{
		this.grid = new WorldSpaceGrid<RendererCell>(TerrainMeta.Size.x * 2f, this.CellSize);
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				this.grid[i, j] = new RendererCell(this, this.grid.GridToWorldCoords(new Vector2i(i, j)));
			}
		}
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x00013DC7 File Offset: 0x00011FC7
	public MeshRendererBatch CreateInstance()
	{
		GameObject gameObject = GameManager.client.CreatePrefab(this.BatchPrefab.resourcePath, true);
		SceneManager.MoveGameObjectToScene(gameObject, Generic.BatchingScene);
		return gameObject.GetComponent<MeshRendererBatch>();
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x00013688 File Offset: 0x00011888
	public void RecycleInstance(MeshRendererBatch instance)
	{
		GameManager.client.Retire(instance.gameObject);
	}

	// Token: 0x060017D0 RID: 6096 RVA: 0x0008BAF0 File Offset: 0x00089CF0
	public int MeshCount()
	{
		if (this.grid == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				num += this.grid[i, j].MeshCount();
			}
		}
		return num;
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x0008BB4C File Offset: 0x00089D4C
	public int BatchedMeshCount()
	{
		if (this.grid == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				num += this.grid[i, j].BatchedMeshCount();
			}
		}
		return num;
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x060017D2 RID: 6098 RVA: 0x0008BBA8 File Offset: 0x00089DA8
	public bool NeedsTimeout
	{
		get
		{
			return this.watch.Elapsed.TotalMilliseconds > (double)this.MaxMilliseconds;
		}
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x00013DEF File Offset: 0x00011FEF
	public void ResetTimeout()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	// Token: 0x060017D4 RID: 6100 RVA: 0x00013E07 File Offset: 0x00012007
	private IEnumerator UpdateCoroutine()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (!Application.isReceiving && !Application.isLoading && !RendererGrid.Paused && this.grid != null)
			{
				this.ResetTimeout();
				int num;
				for (int x = 0; x < this.grid.CellCount; x = num + 1)
				{
					for (int z = 0; z < this.grid.CellCount; z = num + 1)
					{
						RendererCell rendererCell = this.grid[x, z];
						if (rendererCell.NeedsRefresh())
						{
							IEnumerator enumerator = rendererCell.RefreshAsync();
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
						num = z;
					}
					num = x;
				}
			}
		}
		yield break;
	}
}
