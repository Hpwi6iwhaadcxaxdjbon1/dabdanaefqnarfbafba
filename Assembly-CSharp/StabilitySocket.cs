using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class StabilitySocket : Socket_Base
{
	// Token: 0x04000BD8 RID: 3032
	[Range(0f, 1f)]
	public float support = 1f;

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0000D30E File Offset: 0x0000B50E
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x000654B8 File Offset: 0x000636B8
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		OBB selectBounds = base.GetSelectBounds(position, rotation);
		OBB selectBounds2 = socket.GetSelectBounds(socketPosition, socketRotation);
		return selectBounds.Intersects(selectBounds2);
	}
}
