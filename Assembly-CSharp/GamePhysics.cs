using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x0200023B RID: 571
public static class GamePhysics
{
	// Token: 0x04000DE3 RID: 3555
	public const int BufferLength = 8192;

	// Token: 0x04000DE4 RID: 3556
	private static RaycastHit[] hitBuffer = new RaycastHit[8192];

	// Token: 0x04000DE5 RID: 3557
	private static Collider[] colBuffer = new Collider[8192];

	// Token: 0x0600110F RID: 4367 RVA: 0x0000EF8D File Offset: 0x0000D18D
	public static bool CheckSphere(Vector3 position, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		return Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x0000EFA1 File Offset: 0x0000D1A1
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision((start + end) * 0.5f, layerMask);
		return Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x0000EFC7 File Offset: 0x0000D1C7
	public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		return Physics.CheckBox(obb.position, obb.extents, obb.rotation, layerMask, triggerInteraction);
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x0000EFF0 File Offset: 0x0000D1F0
	public static bool CheckBounds(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		return Physics.CheckBox(bounds.center, bounds.extents, Quaternion.identity, layerMask, triggerInteraction);
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x0000F01B File Offset: 0x0000D21B
	public static void OverlapSphere(Vector3 position, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList(Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0000F03B File Offset: 0x0000D23B
	public static void OverlapCapsule(Vector3 point0, Vector3 point1, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList(Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x0000F068 File Offset: 0x0000D268
	public static void OverlapOBB(OBB obb, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList(Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x0000F09C File Offset: 0x0000D29C
	public static void OverlapBounds(Bounds bounds, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList(Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x000722EC File Offset: 0x000704EC
	private static void BufferToList(int count, List<Collider> list)
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.colBuffer[i]);
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x00072330 File Offset: 0x00070530
	public static bool CheckSphere<T>(Vector3 pos, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x0007235C File Offset: 0x0007055C
	public static bool CheckCapsule<T>(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x00072388 File Offset: 0x00070588
	public static bool CheckOBB<T>(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x000723B4 File Offset: 0x000705B4
	public static bool CheckBounds<T>(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x000723E0 File Offset: 0x000705E0
	private static bool CheckComponent<T>(List<Collider> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gameObject.GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x0000F0D2 File Offset: 0x0000D2D2
	public static void OverlapSphere<T>(Vector3 position, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList<T>(Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x0000F0F2 File Offset: 0x0000D2F2
	public static void OverlapCapsule<T>(Vector3 point0, Vector3 point1, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList<T>(Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x0000F11F File Offset: 0x0000D31F
	public static void OverlapOBB<T>(OBB obb, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList<T>(Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x0000F153 File Offset: 0x0000D353
	public static void OverlapBounds<T>(Bounds bounds, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList<T>(Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0007241C File Offset: 0x0007061C
	private static void BufferToList<T>(int count, List<T> list) where T : Component
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			T component = GamePhysics.colBuffer[i].gameObject.GetComponent<T>();
			if (component)
			{
				list.Add(component);
			}
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x00072478 File Offset: 0x00070678
	public static bool Trace(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction);
		if (list.Count == 0)
		{
			hitInfo = default(RaycastHit);
			Pool.FreeList<RaycastHit>(ref list);
			return false;
		}
		GamePhysics.Sort(list);
		hitInfo = list[0];
		Pool.FreeList<RaycastHit>(ref list);
		return true;
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x0000F189 File Offset: 0x0000D389
	public static void TraceAll(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		GamePhysics.TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction);
		GamePhysics.Sort(hits);
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x000724CC File Offset: 0x000706CC
	public static void TraceAllUnordered(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		int num;
		if (radius == 0f)
		{
			num = Physics.RaycastNonAlloc(ray, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		else
		{
			num = Physics.SphereCastNonAlloc(ray, radius, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		if (num == 0)
		{
			return;
		}
		if (num >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding hit buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = GamePhysics.hitBuffer[i];
			if (GamePhysics.Verify(raycastHit))
			{
				hits.Add(raycastHit);
			}
		}
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x00072548 File Offset: 0x00070748
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding = 0f)
	{
		Vector3 a = p1 - p0;
		float magnitude = a.magnitude;
		if (magnitude <= padding + padding)
		{
			return true;
		}
		Vector3 b = a / magnitude * padding;
		RaycastHit raycastHit;
		if (!Physics.Linecast(p0 + b, p1 - b, ref raycastHit, layerMask, 1))
		{
			if (ConVar.Vis.lineofsight)
			{
				UnityEngine.DDraw.Line(p0, p1, Color.green, 60f, true, true);
			}
			return true;
		}
		if (ConVar.Vis.lineofsight)
		{
			UnityEngine.DDraw.Line(p0, p1, Color.red, 60f, true, true);
			UnityEngine.DDraw.Text(raycastHit.collider.name, raycastHit.point, Color.white, 60f);
		}
		return false;
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0000F19E File Offset: 0x0000D39E
	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, int layerMask, float padding = 0f)
	{
		return GamePhysics.LineOfSight(p0, p1, layerMask, padding) && GamePhysics.LineOfSight(p1, p2, layerMask, padding);
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0000F1B8 File Offset: 0x0000D3B8
	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int layerMask, float padding = 0f)
	{
		return GamePhysics.LineOfSight(p0, p1, layerMask, padding) && GamePhysics.LineOfSight(p1, p2, layerMask, padding) && GamePhysics.LineOfSight(p2, p3, layerMask, padding);
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x0000F1E1 File Offset: 0x0000D3E1
	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int layerMask, float padding = 0f)
	{
		return GamePhysics.LineOfSight(p0, p1, layerMask, padding) && GamePhysics.LineOfSight(p1, p2, layerMask, padding) && GamePhysics.LineOfSight(p2, p3, layerMask, padding) && GamePhysics.LineOfSight(p3, p4, layerMask, padding);
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x0000F218 File Offset: 0x0000D418
	public static bool Verify(RaycastHit hitInfo)
	{
		return GamePhysics.Verify(hitInfo.collider, hitInfo.point);
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0000F22D File Offset: 0x0000D42D
	public static bool Verify(Collider collider, Vector3 point)
	{
		return (!(collider is TerrainCollider) || !TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(point, 0.01f)) && collider.enabled;
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x000725F0 File Offset: 0x000707F0
	public static int HandleTerrainCollision(Vector3 position, int layerMask)
	{
		int num = 8388608;
		if ((layerMask & num) != 0 && TerrainMeta.Collision && TerrainMeta.Collision.GetIgnore(position, 0.01f))
		{
			layerMask &= ~num;
		}
		return layerMask;
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x0000F25D File Offset: 0x0000D45D
	public static void Sort(List<RaycastHit> hits)
	{
		hits.Sort((RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0000F284 File Offset: 0x0000D484
	public static void Sort(RaycastHit[] hits)
	{
		Array.Sort<RaycastHit>(hits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}
}
