using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class DrawArrow : MonoBehaviour
{
	// Token: 0x04000E45 RID: 3653
	public Color color = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04000E46 RID: 3654
	public float length = 0.2f;

	// Token: 0x04000E47 RID: 3655
	public float arrowLength = 0.02f;

	// Token: 0x06001188 RID: 4488 RVA: 0x00074A00 File Offset: 0x00072C00
	private void OnDrawGizmos()
	{
		Vector3 forward = base.transform.forward;
		Vector3 up = Camera.current.transform.up;
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.position + forward * this.length;
		Gizmos.color = this.color;
		Gizmos.DrawLine(position, vector);
		Gizmos.DrawLine(vector, vector + up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector, vector - up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector + up * this.arrowLength - forward * this.arrowLength, vector - up * this.arrowLength - forward * this.arrowLength);
	}
}
