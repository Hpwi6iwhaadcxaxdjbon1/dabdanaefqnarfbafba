using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class ModelConditionTest_RoofTop : ModelConditionTest
{
	// Token: 0x04000B94 RID: 2964
	private const string socket = "roof/sockets/wall-female";

	// Token: 0x04000B95 RID: 2965
	private const string socket_male = "roof/sockets/wall-male";

	// Token: 0x06000E6E RID: 3694 RVA: 0x00065238 File Offset: 0x00063438
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 4.5f, -3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x0006528C File Offset: 0x0006348C
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("roof/sockets/wall-female");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name == "roof/sockets/wall-male")
			{
				return false;
			}
		}
		return true;
	}
}
