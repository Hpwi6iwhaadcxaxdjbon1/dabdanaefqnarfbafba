using System;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class rottest : MonoBehaviour
{
	// Token: 0x040007BB RID: 1979
	public Transform turretBase;

	// Token: 0x040007BC RID: 1980
	public Vector3 aimDir;

	// Token: 0x06000B4D RID: 2893 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0000AE2A File Offset: 0x0000902A
	private void Update()
	{
		this.aimDir = new Vector3(0f, 45f * Mathf.Sin(Time.time * 6f), 0f);
		this.UpdateAiming();
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00058558 File Offset: 0x00056758
	public void UpdateAiming()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(0f, this.aimDir.y, 0f);
		if (base.transform.localRotation != quaternion)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, Time.deltaTime * 8f);
		}
	}
}
