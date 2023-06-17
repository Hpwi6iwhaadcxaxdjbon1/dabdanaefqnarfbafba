using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public static class Vis
{
	// Token: 0x040015C8 RID: 5576
	private static Collider[] colBuffer = new Collider[8192];

	// Token: 0x06001953 RID: 6483 RVA: 0x0008F688 File Offset: 0x0008D888
	public static void Colliders<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Collider
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		int num = Physics.OverlapSphereNonAlloc(position, radius, Vis.colBuffer, layerMask, triggerInteraction);
		if (num >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			T t = Vis.colBuffer[i] as T;
			Vis.colBuffer[i] = null;
			if (!(t == null) && t.enabled)
			{
				if (t.transform.CompareTag("MeshColliderBatch"))
				{
					t.transform.GetComponent<MeshColliderBatch>().LookupColliders<T>(position, radius, list);
				}
				else
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x0008F73C File Offset: 0x0008D93C
	public static void Components<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Component
	{
		List<Collider> list2 = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(position, radius, list2, layerMask, triggerInteraction);
		for (int i = 0; i < list2.Count; i++)
		{
			T component = list2[i].gameObject.GetComponent<T>();
			if (!(component == null))
			{
				list.Add(component);
			}
		}
		Pool.FreeList<Collider>(ref list2);
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x0008F79C File Offset: 0x0008D99C
	public static void Entities<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : BaseEntity
	{
		List<Collider> list2 = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(position, radius, list2, layerMask, triggerInteraction);
		for (int i = 0; i < list2.Count; i++)
		{
			T t = list2[i].gameObject.ToBaseEntity() as T;
			if (!(t == null))
			{
				list.Add(t);
			}
		}
		Pool.FreeList<Collider>(ref list2);
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x0008F804 File Offset: 0x0008DA04
	public static void EntityComponents<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : EntityComponentBase
	{
		List<Collider> list2 = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(position, radius, list2, layerMask, triggerInteraction);
		for (int i = 0; i < list2.Count; i++)
		{
			BaseEntity baseEntity = list2[i].gameObject.ToBaseEntity();
			if (!(baseEntity == null))
			{
				T component = baseEntity.gameObject.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
		Pool.FreeList<Collider>(ref list2);
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x0008F878 File Offset: 0x0008DA78
	public static void Colliders<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Collider
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.position, layerMask);
		int num = Physics.OverlapBoxNonAlloc(bounds.position, bounds.extents, Vis.colBuffer, bounds.rotation, layerMask, triggerInteraction);
		if (num >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			T t = Vis.colBuffer[i] as T;
			Vis.colBuffer[i] = null;
			if (!(t == null) && t.enabled)
			{
				if (t.transform.CompareTag("MeshColliderBatch"))
				{
					t.transform.GetComponent<MeshColliderBatch>().LookupColliders<T>(bounds, list);
				}
				else
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x0008F940 File Offset: 0x0008DB40
	public static void Components<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Component
	{
		List<Collider> list2 = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(bounds, list2, layerMask, triggerInteraction);
		for (int i = 0; i < list2.Count; i++)
		{
			T component = list2[i].gameObject.GetComponent<T>();
			if (!(component == null))
			{
				list.Add(component);
			}
		}
		Pool.FreeList<Collider>(ref list2);
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x0008F99C File Offset: 0x0008DB9C
	public static void Entities<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : BaseEntity
	{
		List<Collider> list2 = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(bounds, list2, layerMask, triggerInteraction);
		for (int i = 0; i < list2.Count; i++)
		{
			T t = list2[i].gameObject.ToBaseEntity() as T;
			if (!(t == null))
			{
				list.Add(t);
			}
		}
		Pool.FreeList<Collider>(ref list2);
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x0008FA04 File Offset: 0x0008DC04
	public static void EntityComponents<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : EntityComponentBase
	{
		List<Collider> list2 = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(bounds, list2, layerMask, triggerInteraction);
		for (int i = 0; i < list2.Count; i++)
		{
			BaseEntity baseEntity = list2[i].gameObject.ToBaseEntity();
			if (!(baseEntity == null))
			{
				T component = baseEntity.gameObject.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
		Pool.FreeList<Collider>(ref list2);
	}
}
