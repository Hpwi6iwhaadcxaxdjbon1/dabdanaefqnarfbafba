using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000385 RID: 901
public class EnvironmentManager : SingletonComponent<EnvironmentManager>
{
	// Token: 0x060016E8 RID: 5864 RVA: 0x000887D4 File Offset: 0x000869D4
	public static EnvironmentType Get(OBB obb)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		GamePhysics.OverlapOBB<EnvironmentVolume>(obb, list, 262144, 2);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return environmentType;
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x00088820 File Offset: 0x00086A20
	public static EnvironmentType Get(Vector3 pos, ref List<EnvironmentVolume> list)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		GamePhysics.OverlapSphere<EnvironmentVolume>(pos, 0.01f, list, 262144, 2);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		return environmentType;
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x00088868 File Offset: 0x00086A68
	public static EnvironmentType Get(Vector3 pos)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		EnvironmentType result = EnvironmentManager.Get(pos, ref list);
		Pool.FreeList<EnvironmentVolume>(ref list);
		return result;
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x000134E6 File Offset: 0x000116E6
	public static bool Check(OBB obb, EnvironmentType type)
	{
		return (EnvironmentManager.Get(obb) & type) > (EnvironmentType)0;
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x000134F3 File Offset: 0x000116F3
	public static bool Check(Vector3 pos, EnvironmentType type)
	{
		return (EnvironmentManager.Get(pos) & type) > (EnvironmentType)0;
	}
}
