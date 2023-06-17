using System;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class ModelConditionTest_WallTriangleLeft : ModelConditionTest
{
	// Token: 0x04000B96 RID: 2966
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x04000B97 RID: 2967
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x04000B98 RID: 2968
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x04000B99 RID: 2969
	private const string socket_4 = "wall/sockets/stability/1";

	// Token: 0x04000B9A RID: 2970
	private const string socket = "wall/sockets/neighbour/1";

	// Token: 0x06000E73 RID: 3699 RVA: 0x000652E0 File Offset: 0x000634E0
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/stability/1"))
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink("wall/sockets/neighbour/1");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null) && !(buildingBlock.blockDefinition.info.name.token != "roof") && Vector3.Angle(ent.transform.forward, buildingBlock.transform.forward) <= 10f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x000653B8 File Offset: 0x000635B8
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0000D2FE File Offset: 0x0000B4FE
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleLeft.CheckCondition(ent);
	}
}
