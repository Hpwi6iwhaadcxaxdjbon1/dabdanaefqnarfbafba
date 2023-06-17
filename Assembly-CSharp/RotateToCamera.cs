using System;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class RotateToCamera : MonoBehaviour, IClientComponent
{
	// Token: 0x04000D99 RID: 3481
	public float maxDistance = 30f;

	// Token: 0x060010B1 RID: 4273 RVA: 0x0000EA92 File Offset: 0x0000CC92
	private void LateUpdate()
	{
		if (!MainCamera.isValid)
		{
			return;
		}
		if (MainCamera.SqrDistance(base.transform.position) > this.maxDistance * this.maxDistance)
		{
			return;
		}
		base.transform.LookAt(MainCamera.mainCamera.transform);
	}
}
