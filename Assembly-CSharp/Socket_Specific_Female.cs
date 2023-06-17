using System;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class Socket_Specific_Female : Socket_Base
{
	// Token: 0x04000BAF RID: 2991
	public int rotationDegrees;

	// Token: 0x04000BB0 RID: 2992
	public int rotationOffset;

	// Token: 0x04000BB1 RID: 2993
	public string[] allowedMaleSockets;

	// Token: 0x06000E91 RID: 3729 RVA: 0x00065848 File Offset: 0x00063A48
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x0000D30E File Offset: 0x0000B50E
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x00065A58 File Offset: 0x00063C58
	public bool CanAccept(Socket_Specific socket)
	{
		foreach (string text in this.allowedMaleSockets)
		{
			if (socket.targetSocketName == text)
			{
				return true;
			}
		}
		return false;
	}
}
