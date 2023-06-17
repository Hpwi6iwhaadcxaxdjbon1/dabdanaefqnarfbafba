using System;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class BirthdayBalloonHandler : MonoBehaviour
{
	// Token: 0x04001051 RID: 4177
	public float checkOffset = 1f;

	// Token: 0x04001052 RID: 4178
	public float checkRadius = 0.5f;

	// Token: 0x060013D7 RID: 5079 RVA: 0x00010E27 File Offset: 0x0000F027
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(this.GetCheckOrigin(), this.checkRadius);
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x00010E44 File Offset: 0x0000F044
	public Vector3 GetCheckOrigin()
	{
		return base.transform.position + Vector3.up * this.checkOffset;
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x00010E66 File Offset: 0x0000F066
	public void SetOn()
	{
		if (!Physics.CheckSphere(this.GetCheckOrigin(), this.checkRadius, 1218519041, 1))
		{
			base.gameObject.SetActive(true);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x00010E9A File Offset: 0x0000F09A
	public void SetOff()
	{
		base.gameObject.SetActive(false);
	}
}
