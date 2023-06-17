using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class ModelConditionTest_RoofLeft : ModelConditionTest
{
	// Token: 0x04000B90 RID: 2960
	private const string socket = "roof/sockets/neighbour/2";

	// Token: 0x04000B91 RID: 2961
	private const string socket_female = "roof/sockets/neighbour/1";

	// Token: 0x06000E68 RID: 3688 RVA: 0x000650E8 File Offset: 0x000632E8
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0006513C File Offset: 0x0006333C
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("roof/sockets/neighbour/2");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name == "roof/sockets/neighbour/1")
			{
				return false;
			}
		}
		return true;
	}
}
