using System;

namespace UnityEngine
{
	// Token: 0x0200082D RID: 2093
	public static class CollisionEx
	{
		// Token: 0x06002D75 RID: 11637 RVA: 0x000E4898 File Offset: 0x000E2A98
		public static BaseEntity GetEntity(this Collision col)
		{
			if (col.transform.CompareTag("MeshColliderBatch") && col.gameObject.GetComponent<MeshColliderBatch>())
			{
				for (int i = 0; i < col.contacts.Length; i++)
				{
					ContactPoint contactPoint = col.contacts[i];
					Ray ray = new Ray(contactPoint.point + contactPoint.normal * 0.01f, -contactPoint.normal);
					RaycastHit hit;
					if (col.collider.Raycast(ray, ref hit, 1f))
					{
						return hit.GetEntity();
					}
				}
			}
			return col.gameObject.ToBaseEntity();
		}
	}
}
