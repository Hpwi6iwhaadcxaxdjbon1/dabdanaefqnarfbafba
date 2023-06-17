using System;
using Rust;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class RandomDynamicPrefab : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040018B9 RID: 6329
	public uint Seed;

	// Token: 0x040018BA RID: 6330
	public float Distance = 100f;

	// Token: 0x040018BB RID: 6331
	public float Probability = 0.5f;

	// Token: 0x040018BC RID: 6332
	public string ResourceFolder = string.Empty;

	// Token: 0x040018BD RID: 6333
	private Prefab prefab;

	// Token: 0x040018BE RID: 6334
	private GameObject instance;

	// Token: 0x040018BF RID: 6335
	private LODCell cell;

	// Token: 0x06001BD9 RID: 7129 RVA: 0x00099C18 File Offset: 0x00097E18
	protected void OnEnable()
	{
		uint num = SeedEx.Seed(base.transform.position, World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability || SingletonComponent<LODGrid>.Instance == null)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		this.prefab = Prefab.LoadRandom("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, ref num, GameManager.client, null, true);
		if (this.prefab != null)
		{
			LODGrid.Add(this, base.transform, ref this.cell);
			return;
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x00016CE0 File Offset: 0x00014EE0
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		LODGrid.Remove(this, base.transform, ref this.cell);
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x00016CFC File Offset: 0x00014EFC
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x00099CC0 File Offset: 0x00097EC0
	public void ChangeLOD()
	{
		if (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(this.Distance))
		{
			if (!this.instance)
			{
				this.instance = GameManager.client.CreatePrefab(this.prefab.Name, base.transform, true);
				return;
			}
		}
		else if (this.instance)
		{
			GameManager.Destroy(this.instance, 0f);
		}
	}
}
