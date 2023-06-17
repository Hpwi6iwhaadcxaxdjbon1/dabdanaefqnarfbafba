using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000388 RID: 904
public static class EnvironmentVolumeEx
{
	// Token: 0x060016F6 RID: 5878 RVA: 0x00088988 File Offset: 0x00086B88
	public static bool CheckEnvironmentVolumes(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			OBB obb;
			obb..ctor(environmentVolume.transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
			obb.Transform(pos, scale, rot);
			if (EnvironmentManager.Check(obb, type))
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return true;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return false;
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x00013566 File Offset: 0x00011766
	public static bool CheckEnvironmentVolumes(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumes(transform.position, transform.rotation, transform.lossyScale, type);
	}
}
