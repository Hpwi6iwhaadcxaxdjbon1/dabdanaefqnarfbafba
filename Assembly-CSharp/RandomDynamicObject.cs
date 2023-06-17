using System;
using Rust;
using UnityEngine;

// Token: 0x020004B0 RID: 1200
public class RandomDynamicObject : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040018B3 RID: 6323
	public uint Seed;

	// Token: 0x040018B4 RID: 6324
	public float Distance = 100f;

	// Token: 0x040018B5 RID: 6325
	public float Probability = 0.5f;

	// Token: 0x040018B6 RID: 6326
	public GameObject[] Candidates;

	// Token: 0x040018B7 RID: 6327
	private GameObject instance;

	// Token: 0x040018B8 RID: 6328
	private LODCell cell;

	// Token: 0x06001BD4 RID: 7124 RVA: 0x00099B14 File Offset: 0x00097D14
	protected void OnEnable()
	{
		uint num = SeedEx.Seed(base.transform.position, World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability || SingletonComponent<LODGrid>.Instance == null)
		{
			for (int i = 0; i < this.Candidates.Length; i++)
			{
				GameManager.Destroy(this.Candidates[i], 0f);
			}
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		int num2 = SeedRandom.Range(num, 0, base.transform.childCount);
		for (int j = 0; j < this.Candidates.Length; j++)
		{
			GameObject gameObject = this.Candidates[j];
			if (j == num2)
			{
				this.instance = gameObject;
				this.instance.SetActive(false);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
		}
		if (this.instance != null)
		{
			LODGrid.Add(this, base.transform, ref this.cell);
			return;
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x00016C6C File Offset: 0x00014E6C
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		LODGrid.Remove(this, base.transform, ref this.cell);
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x00016C88 File Offset: 0x00014E88
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x00016C9C File Offset: 0x00014E9C
	public void ChangeLOD()
	{
		this.instance.SetActive(LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(this.Distance));
	}
}
