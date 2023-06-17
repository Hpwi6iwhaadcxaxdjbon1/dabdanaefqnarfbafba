using System;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class ModelConditionTest_RoofRight : ModelConditionTest
{
	// Token: 0x04000B92 RID: 2962
	private const string socket = "roof/sockets/neighbour/1";

	// Token: 0x04000B93 RID: 2963
	private const string socket_female = "roof/sockets/neighbour/2";

	// Token: 0x06000E6B RID: 3691 RVA: 0x00065190 File Offset: 0x00063390
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(-3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x000651E4 File Offset: 0x000633E4
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("roof/sockets/neighbour/1");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name == "roof/sockets/neighbour/2")
			{
				return false;
			}
		}
		return true;
	}
}
