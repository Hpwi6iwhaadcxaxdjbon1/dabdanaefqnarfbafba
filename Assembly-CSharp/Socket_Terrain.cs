using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class Socket_Terrain : Socket_Base
{
	// Token: 0x04000BB2 RID: 2994
	public float placementHeight;

	// Token: 0x04000BB3 RID: 2995
	public bool alignToNormal;

	// Token: 0x06000E95 RID: 3733 RVA: 0x00065A90 File Offset: 0x00063C90
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
		Gizmos.DrawCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0000D397 File Offset: 0x0000B597
	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x00065BA8 File Offset: 0x00063DA8
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Vector3 eulerAngles = this.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		Vector3 direction = target.ray.direction;
		direction.y = 0f;
		direction.Normalize();
		Vector3 upwards = Vector3.up;
		if (this.alignToNormal)
		{
			upwards = target.normal;
		}
		Quaternion rotation = Quaternion.LookRotation(direction, upwards) * Quaternion.Euler(0f, eulerAngles.y, 0f) * Quaternion.Euler(target.rotation);
		Vector3 vector = target.position;
		vector -= rotation * this.position;
		return new Construction.Placement
		{
			rotation = rotation,
			position = vector
		};
	}
}
