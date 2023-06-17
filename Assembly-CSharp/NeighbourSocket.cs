using System;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class NeighbourSocket : Socket_Base
{
	// Token: 0x06000E7B RID: 3707 RVA: 0x0000D30E File Offset: 0x0000B50E
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x000654B8 File Offset: 0x000636B8
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
