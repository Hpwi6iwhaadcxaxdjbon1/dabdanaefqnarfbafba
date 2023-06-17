using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class ModelConditionTest_RoofBottom : ModelConditionTest
{
	// Token: 0x04000B8E RID: 2958
	private const string socket = "roof/sockets/wall-male";

	// Token: 0x04000B8F RID: 2959
	private const string socket_female = "roof/sockets/wall-female";

	// Token: 0x06000E65 RID: 3685 RVA: 0x00065040 File Offset: 0x00063240
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x00065094 File Offset: 0x00063294
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("roof/sockets/wall-male");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name == "roof/sockets/wall-female")
			{
				return false;
			}
		}
		return true;
	}
}
