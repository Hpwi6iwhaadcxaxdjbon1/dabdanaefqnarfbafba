using System;
using UnityEngine;

// Token: 0x020004B2 RID: 1202
public class RandomStaticObject : MonoBehaviour
{
	// Token: 0x040018C0 RID: 6336
	public uint Seed;

	// Token: 0x040018C1 RID: 6337
	public float Probability = 0.5f;

	// Token: 0x040018C2 RID: 6338
	public GameObject[] Candidates;

	// Token: 0x06001BDE RID: 7134 RVA: 0x00099D34 File Offset: 0x00097F34
	protected void Start()
	{
		uint num = SeedEx.Seed(base.transform.position, World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			for (int i = 0; i < this.Candidates.Length; i++)
			{
				GameManager.Destroy(this.Candidates[i], 0f);
			}
			GameManager.Destroy(this, 0f);
			return;
		}
		int num2 = SeedRandom.Range(num, 0, base.transform.childCount);
		for (int j = 0; j < this.Candidates.Length; j++)
		{
			GameObject gameObject = this.Candidates[j];
			if (j == num2)
			{
				gameObject.SetActive(true);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
		}
		GameManager.Destroy(this, 0f);
	}
}
