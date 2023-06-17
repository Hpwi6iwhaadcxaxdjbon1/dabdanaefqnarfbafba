using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class PathInterestNode : MonoBehaviour
{
	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000B63 RID: 2915 RVA: 0x0000AEDA File Offset: 0x000090DA
	// (set) Token: 0x06000B64 RID: 2916 RVA: 0x0000AEE2 File Offset: 0x000090E2
	public float NextVisitTime { get; set; }

	// Token: 0x06000B65 RID: 2917 RVA: 0x0000AEEB File Offset: 0x000090EB
	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
