using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200075F RID: 1887
public static class TransformUtil
{
	// Token: 0x06002929 RID: 10537 RVA: 0x00020011 File Offset: 0x0001E211
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, 100f, -1, ignoreTransform);
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x00020026 File Offset: 0x0001E226
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, range, -1, ignoreTransform);
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x000D2570 File Offset: 0x000D0770
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = default(RaycastHit);
		RaycastHit raycastHit;
		if (!Physics.Raycast(new Ray(startPos, Vector3.down), ref raycastHit, range, mask))
		{
			return false;
		}
		if (ignoreTransform != null && (raycastHit.collider.transform == ignoreTransform || raycastHit.collider.transform.IsChildOf(ignoreTransform)))
		{
			return TransformUtil.GetGroundInfo(startPos - new Vector3(0f, 0.01f, 0f), out hitOut, range, mask, ignoreTransform);
		}
		hitOut = raycastHit;
		return true;
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x00020037 File Offset: 0x0001E237
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, 100f, -1, ignoreTransform);
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x0002004D File Offset: 0x0001E24D
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, range, -1, ignoreTransform);
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x000D261C File Offset: 0x000D081C
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		foreach (RaycastHit raycastHit in Enumerable.OrderBy<RaycastHit, float>(Physics.RaycastAll(new Ray(startPos, Vector3.down), range, mask), (RaycastHit h) => h.distance))
		{
			if (!(ignoreTransform != null) || (!(raycastHit.collider.transform == ignoreTransform) && !raycastHit.collider.transform.IsChildOf(ignoreTransform)))
			{
				pos = raycastHit.point;
				normal = raycastHit.normal;
				return true;
			}
		}
		pos = startPos;
		normal = Vector3.up;
		return false;
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x00020060 File Offset: 0x0001E260
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, -1);
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x00020075 File Offset: 0x0001E275
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, -1);
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x000D2718 File Offset: 0x000D0918
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(startPos, Vector3.down), ref raycastHit, range, mask) && raycastHit.collider is TerrainCollider)
		{
			pos = raycastHit.point;
			normal = raycastHit.normal;
			return true;
		}
		pos = startPos;
		normal = Vector3.up;
		return false;
	}

	// Token: 0x06002932 RID: 10546 RVA: 0x00020086 File Offset: 0x0001E286
	public static Transform[] GetRootObjects()
	{
		return Enumerable.ToArray<Transform>(Enumerable.Where<Transform>(Object.FindObjectsOfType<Transform>(), (Transform x) => x.transform == x.transform.root));
	}
}
