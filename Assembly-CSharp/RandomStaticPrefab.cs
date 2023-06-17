using System;
using UnityEngine;

// Token: 0x020004B3 RID: 1203
public class RandomStaticPrefab : MonoBehaviour
{
	// Token: 0x040018C3 RID: 6339
	public uint Seed;

	// Token: 0x040018C4 RID: 6340
	public float Probability = 0.5f;

	// Token: 0x040018C5 RID: 6341
	public string ResourceFolder = string.Empty;

	// Token: 0x06001BE0 RID: 7136 RVA: 0x00099DF4 File Offset: 0x00097FF4
	protected void Start()
	{
		uint num = SeedEx.Seed(base.transform.position, World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		Prefab.LoadRandom("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, ref num, null, null, true).Spawn(base.transform);
		GameManager.Destroy(this, 0f);
	}
}
