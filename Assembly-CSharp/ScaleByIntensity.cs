using System;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class ScaleByIntensity : MonoBehaviour
{
	// Token: 0x04000D9A RID: 3482
	public Vector3 initialScale = Vector3.zero;

	// Token: 0x04000D9B RID: 3483
	public Light intensitySource;

	// Token: 0x04000D9C RID: 3484
	public float maxIntensity = 1f;

	// Token: 0x060010B3 RID: 4275 RVA: 0x0000EAE4 File Offset: 0x0000CCE4
	private void Start()
	{
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x00070A94 File Offset: 0x0006EC94
	private void Update()
	{
		base.transform.localScale = (this.intensitySource.enabled ? (this.initialScale * this.intensitySource.intensity / this.maxIntensity) : Vector3.zero);
	}
}
