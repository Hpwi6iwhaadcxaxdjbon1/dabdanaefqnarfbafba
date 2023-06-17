using System;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class ModelConditionTest_WallTriangleRight : ModelConditionTest
{
	// Token: 0x04000B9B RID: 2971
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x04000B9C RID: 2972
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x04000B9D RID: 2973
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x04000B9E RID: 2974
	private const string socket_4 = "wall/sockets/stability/2";

	// Token: 0x04000B9F RID: 2975
	private const string socket = "wall/sockets/neighbour/1";

	// Token: 0x06000E77 RID: 3703 RVA: 0x000653DC File Offset: 0x000635DC
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/stability/2"))
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
			if (!(buildingBlock == null) && !(buildingBlock.blockDefinition.info.name.token != "roof") && Vector3.Angle(ent.transform.forward, -buildingBlock.transform.forward) <= 10f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x000653B8 File Offset: 0x000635B8
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0000D306 File Offset: 0x0000B506
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}
}
