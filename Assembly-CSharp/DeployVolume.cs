using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000378 RID: 888
public abstract class DeployVolume : PrefabAttribute
{
	// Token: 0x0400139B RID: 5019
	public LayerMask layers = 537001984;

	// Token: 0x0400139C RID: 5020
	[InspectorFlags]
	public ColliderInfo.Flags ignore;

	// Token: 0x060016AE RID: 5806 RVA: 0x00013256 File Offset: 0x00011456
	protected override Type GetIndexedType()
	{
		return typeof(DeployVolume);
	}

	// Token: 0x060016AF RID: 5807
	protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

	// Token: 0x060016B0 RID: 5808
	protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

	// Token: 0x060016B1 RID: 5809 RVA: 0x00087E6C File Offset: 0x0008606C
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x00087E98 File Offset: 0x00086098
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, OBB test, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, test, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x00087EC8 File Offset: 0x000860C8
	public static bool CheckSphere(Vector3 pos, float radius, int layerMask, ColliderInfo.Flags ignore)
	{
		if (ignore == (ColliderInfo.Flags)0)
		{
			return GamePhysics.CheckSphere(pos, radius, layerMask, 0);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, 1);
		bool result = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x00087F00 File Offset: 0x00086100
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, ColliderInfo.Flags ignore)
	{
		if (ignore == (ColliderInfo.Flags)0)
		{
			return GamePhysics.CheckCapsule(start, end, radius, layerMask, 0);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, 1);
		bool result = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x00087F3C File Offset: 0x0008613C
	public static bool CheckOBB(OBB obb, int layerMask, ColliderInfo.Flags ignore)
	{
		if (ignore == (ColliderInfo.Flags)0)
		{
			return GamePhysics.CheckOBB(obb, layerMask, 0);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, 1);
		bool result = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x00087F74 File Offset: 0x00086174
	public static bool CheckBounds(Bounds bounds, int layerMask, ColliderInfo.Flags ignore)
	{
		if (ignore == (ColliderInfo.Flags)0)
		{
			return GamePhysics.CheckBounds(bounds, layerMask, 0);
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, 1);
		bool result = DeployVolume.CheckFlags(list, ignore);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x00087FAC File Offset: 0x000861AC
	private static bool CheckFlags(List<Collider> list, ColliderInfo.Flags ignore)
	{
		for (int i = 0; i < list.Count; i++)
		{
			ColliderInfo component = list[i].gameObject.GetComponent<ColliderInfo>();
			if (component == null)
			{
				return true;
			}
			if (!component.HasFlag(ignore))
			{
				return true;
			}
		}
		return false;
	}
}
