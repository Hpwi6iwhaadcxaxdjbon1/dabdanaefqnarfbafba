using System;
using UnityEngine;

// Token: 0x02000774 RID: 1908
public class IronsightAimPoint : MonoBehaviour
{
	// Token: 0x0400249C RID: 9372
	public Transform targetPoint;

	// Token: 0x06002998 RID: 10648 RVA: 0x000D3840 File Offset: 0x000D1A40
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Vector3 normalized = (this.targetPoint.position - base.transform.position).normalized;
		Gizmos.color = Color.red;
		this.DrawArrow(base.transform.position, base.transform.position + normalized * 0.1f, 0.1f);
		Gizmos.color = Color.cyan;
		this.DrawArrow(base.transform.position, this.targetPoint.position, 0.02f);
		Gizmos.color = Color.yellow;
		this.DrawArrow(this.targetPoint.position, this.targetPoint.position + normalized * 3f, 0.02f);
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000D391C File Offset: 0x000D1B1C
	private void DrawArrow(Vector3 start, Vector3 end, float arrowLength)
	{
		Vector3 normalized = (end - start).normalized;
		Vector3 up = Camera.current.transform.up;
		Gizmos.DrawLine(start, end);
		Gizmos.DrawLine(end, end + up * arrowLength - normalized * arrowLength);
		Gizmos.DrawLine(end, end - up * arrowLength - normalized * arrowLength);
		Gizmos.DrawLine(end + up * arrowLength - normalized * arrowLength, end - up * arrowLength - normalized * arrowLength);
	}
}
