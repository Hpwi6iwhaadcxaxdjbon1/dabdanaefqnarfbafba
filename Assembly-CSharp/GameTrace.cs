using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200023E RID: 574
public static class GameTrace
{
	// Token: 0x04000DF2 RID: 3570
	private const int tracePadding = 5;

	// Token: 0x06001136 RID: 4406 RVA: 0x0007274C File Offset: 0x0007094C
	public static void TraceAll(HitTest test, List<TraceInfo> traces, int layerMask = -5)
	{
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		Vector3 origin = test.AttackRay.origin;
		Vector3 direction = test.AttackRay.direction;
		float maxDistance = test.MaxDistance;
		float radius = test.Radius;
		if ((layerMask & 16384) != 0)
		{
			layerMask &= -16385;
			GamePhysics.TraceAllUnordered(new Ray(origin - direction * 5f, direction), radius, list, maxDistance + 5f, 16384, 0);
			for (int i = 0; i < list.Count; i++)
			{
				RaycastHit raycastHit = list[i];
				raycastHit.distance -= 5f;
				list[i] = raycastHit;
			}
		}
		GamePhysics.TraceAllUnordered(new Ray(origin, direction), radius, list, maxDistance, layerMask, 0);
		for (int j = 0; j < list.Count; j++)
		{
			RaycastHit hit = list[j];
			Collider collider = hit.GetCollider();
			if (!collider.isTrigger)
			{
				ColliderInfo component = collider.GetComponent<ColliderInfo>();
				if (!component || component.Filter(test))
				{
					BaseCollision component2 = collider.GetComponent<BaseCollision>();
					if (component2 != null)
					{
						component2.TraceTest(test, traces);
					}
					else if (hit.distance > 0f)
					{
						TraceInfo traceInfo = default(TraceInfo);
						traceInfo.valid = true;
						traceInfo.distance = hit.distance;
						traceInfo.partID = 0U;
						traceInfo.point = hit.point;
						traceInfo.normal = hit.normal;
						traceInfo.collider = collider;
						traceInfo.material = collider.GetMaterialAt(hit.point);
						traceInfo.entity = collider.gameObject.ToBaseEntity();
						traceInfo.bone = Model.GetTransform(collider.transform, hit.point, traceInfo.entity);
						if (!traceInfo.entity || (traceInfo.entity.isClient && traceInfo.entity != test.ignoreEntity))
						{
							traces.Add(traceInfo);
						}
					}
				}
			}
		}
		traces.Sort((TraceInfo a, TraceInfo b) => a.distance.CompareTo(b.distance));
		Pool.FreeList<RaycastHit>(ref list);
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x00072998 File Offset: 0x00070B98
	public static bool Trace(HitTest test, int layerMask = -5)
	{
		List<TraceInfo> list = Pool.GetList<TraceInfo>();
		GameTrace.TraceAll(test, list, layerMask);
		bool flag = list.Count > 0;
		if (flag)
		{
			if (test.BestHit)
			{
				for (int i = 0; i < list.Count; i++)
				{
					TraceInfo traceInfo = list[i];
					if (traceInfo.valid)
					{
						traceInfo.UpdateHitTest(test);
						break;
					}
				}
			}
			else
			{
				list[0].UpdateHitTest(test);
			}
		}
		Pool.FreeList<TraceInfo>(ref list);
		return flag;
	}
}
