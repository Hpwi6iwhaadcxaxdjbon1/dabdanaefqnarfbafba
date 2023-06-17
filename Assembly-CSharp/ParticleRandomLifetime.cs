using System;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class ParticleRandomLifetime : MonoBehaviour
{
	// Token: 0x04000D2A RID: 3370
	public ParticleSystem mySystem;

	// Token: 0x04000D2B RID: 3371
	public float minScale = 0.5f;

	// Token: 0x04000D2C RID: 3372
	public float maxScale = 1f;

	// Token: 0x06001058 RID: 4184 RVA: 0x0006E518 File Offset: 0x0006C718
	public void Awake()
	{
		if (!this.mySystem)
		{
			return;
		}
		float startLifetime = Random.Range(this.minScale, this.maxScale);
		this.mySystem.startLifetime = startLifetime;
	}
}
