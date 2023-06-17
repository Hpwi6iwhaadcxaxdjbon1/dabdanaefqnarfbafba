using System;
using UnityEngine;

// Token: 0x02000729 RID: 1833
public static class RaycastHitEx
{
	// Token: 0x0600280F RID: 10255 RVA: 0x000CEBF8 File Offset: 0x000CCDF8
	public static Transform GetTransform(this RaycastHit hit)
	{
		if (hit.triangleIndex < 0)
		{
			return hit.transform;
		}
		if (!hit.transform.CompareTag("MeshColliderBatch"))
		{
			return hit.transform;
		}
		MeshColliderBatch component = hit.transform.GetComponent<MeshColliderBatch>();
		if (!component)
		{
			return hit.transform;
		}
		Transform transform = component.LookupTransform(hit.triangleIndex);
		if (!transform)
		{
			return hit.transform;
		}
		return transform;
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000CEC70 File Offset: 0x000CCE70
	public static Rigidbody GetRigidbody(this RaycastHit hit)
	{
		if (hit.triangleIndex < 0)
		{
			return hit.rigidbody;
		}
		if (!hit.transform.CompareTag("MeshColliderBatch"))
		{
			return hit.rigidbody;
		}
		MeshColliderBatch component = hit.transform.GetComponent<MeshColliderBatch>();
		if (!component)
		{
			return hit.rigidbody;
		}
		Rigidbody rigidbody = component.LookupRigidbody(hit.triangleIndex);
		if (!rigidbody)
		{
			return hit.rigidbody;
		}
		return rigidbody;
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000CECE8 File Offset: 0x000CCEE8
	public static Collider GetCollider(this RaycastHit hit)
	{
		if (hit.triangleIndex < 0)
		{
			return hit.collider;
		}
		if (!hit.transform.CompareTag("MeshColliderBatch"))
		{
			return hit.collider;
		}
		MeshColliderBatch component = hit.transform.GetComponent<MeshColliderBatch>();
		if (!component)
		{
			return hit.collider;
		}
		Collider collider = component.LookupCollider(hit.triangleIndex);
		if (!collider)
		{
			return hit.collider;
		}
		return collider;
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x0001F341 File Offset: 0x0001D541
	public static BaseEntity GetEntity(this RaycastHit hit)
	{
		return hit.GetTransform().gameObject.ToBaseEntity();
	}
}
