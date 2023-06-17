using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class SocketMod_SphereCheck : SocketMod
{
	// Token: 0x04000BD4 RID: 3028
	public float sphereRadius = 1f;

	// Token: 0x04000BD5 RID: 3029
	public LayerMask layerMask;

	// Token: 0x04000BD6 RID: 3030
	public bool wantsCollide;

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0006666C File Offset: 0x0006486C
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x000666DC File Offset: 0x000648DC
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		if (this.wantsCollide == GamePhysics.CheckSphere(vector, this.sphereRadius, this.layerMask.value, 0))
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
		if (Input.GetKeyDown(KeyCode.KeypadDivide))
		{
			Debug.Log("Sphere Radius: " + this.sphereRadius);
			DDraw.Sphere(vector, this.sphereRadius, Color.red, 20f, true);
			DDraw.Text(this.hierachyName, vector, Color.white, 20f);
			List<Collider> list = Pool.GetList<Collider>();
			Vis.Colliders<Collider>(vector, this.sphereRadius, list, this.layerMask.value, 2);
			foreach (Collider collider in list)
			{
				DDraw.Line(vector, collider.transform.position, Color.green, 20f, true, true);
				DDraw.Text(collider.name, collider.transform.position, Color.white, 20f);
			}
			Pool.FreeList<Collider>(ref list);
		}
		return false;
	}
}
