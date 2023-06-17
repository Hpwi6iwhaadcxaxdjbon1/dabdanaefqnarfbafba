using System;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class ModelConditionTest_FoundationSide : ModelConditionTest
{
	// Token: 0x04000B86 RID: 2950
	private const string square_south = "foundation/sockets/foundation-top/1";

	// Token: 0x04000B87 RID: 2951
	private const string square_north = "foundation/sockets/foundation-top/3";

	// Token: 0x04000B88 RID: 2952
	private const string square_west = "foundation/sockets/foundation-top/2";

	// Token: 0x04000B89 RID: 2953
	private const string square_east = "foundation/sockets/foundation-top/4";

	// Token: 0x04000B8A RID: 2954
	private const string triangle_south = "foundation.triangle/sockets/foundation-top/1";

	// Token: 0x04000B8B RID: 2955
	private const string triangle_northwest = "foundation.triangle/sockets/foundation-top/2";

	// Token: 0x04000B8C RID: 2956
	private const string triangle_northeast = "foundation.triangle/sockets/foundation-top/3";

	// Token: 0x04000B8D RID: 2957
	private string socket = string.Empty;

	// Token: 0x06000E61 RID: 3681 RVA: 0x00064E80 File Offset: 0x00063080
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(1.5f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00064ED4 File Offset: 0x000630D4
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		Vector3 vector = this.worldRotation * Vector3.right;
		if (name.Contains("foundation.triangle"))
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/1";
			}
			if (vector.x < -0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/2";
			}
			if (vector.x > 0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/3";
				return;
			}
		}
		else
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/1";
			}
			if (vector.z > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/3";
			}
			if (vector.x < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/2";
			}
			if (vector.x > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/4";
			}
		}
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00064FA8 File Offset: 0x000631A8
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(this.socket);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null) && !(buildingBlock.blockDefinition.info.name.token == "foundation_steps"))
			{
				if (buildingBlock.grade == BuildingGrade.Enum.TopTier)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Metal)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Stone)
				{
					return false;
				}
			}
		}
		return true;
	}
}
