using System;
using UnityEngine;

// Token: 0x0200033B RID: 827
public class BuoyancyPoint : MonoBehaviour
{
	// Token: 0x040012C4 RID: 4804
	public float buoyancyForce = 10f;

	// Token: 0x040012C5 RID: 4805
	public float size = 0.1f;

	// Token: 0x040012C6 RID: 4806
	public float randomOffset;

	// Token: 0x040012C7 RID: 4807
	public float waveScale = 0.2f;

	// Token: 0x040012C8 RID: 4808
	public float waveFrequency = 1f;

	// Token: 0x040012C9 RID: 4809
	public bool wasSubmergedLastFrame;

	// Token: 0x040012CA RID: 4810
	public float nexSplashTime;

	// Token: 0x040012CB RID: 4811
	public bool doSplashEffects = true;

	// Token: 0x060015D1 RID: 5585 RVA: 0x0001269C File Offset: 0x0001089C
	public void Start()
	{
		this.randomOffset = Random.Range(0f, 20f);
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x000126B3 File Offset: 0x000108B3
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, this.size * 0.5f);
	}
}
