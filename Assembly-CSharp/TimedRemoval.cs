using System;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class TimedRemoval : MonoBehaviour
{
	// Token: 0x04000DCF RID: 3535
	public Object objectToDestroy;

	// Token: 0x04000DD0 RID: 3536
	public float removeDelay = 1f;

	// Token: 0x060010E5 RID: 4325 RVA: 0x0000EC6A File Offset: 0x0000CE6A
	private void OnEnable()
	{
		Object.Destroy(this.objectToDestroy, this.removeDelay);
	}
}
