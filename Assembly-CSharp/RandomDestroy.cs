using System;
using UnityEngine;

// Token: 0x020004AF RID: 1199
public class RandomDestroy : MonoBehaviour
{
	// Token: 0x040018B1 RID: 6321
	public uint Seed;

	// Token: 0x040018B2 RID: 6322
	public float Probability = 0.5f;

	// Token: 0x06001BD2 RID: 7122 RVA: 0x00099ABC File Offset: 0x00097CBC
	protected void Start()
	{
		uint num = SeedEx.Seed(base.transform.position, World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		GameManager.Destroy(base.gameObject, 0f);
	}
}
