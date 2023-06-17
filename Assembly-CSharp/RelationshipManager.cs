using System;

// Token: 0x020003EF RID: 1007
public class RelationshipManager : BaseEntity
{
	// Token: 0x04001570 RID: 5488
	[NonSerialized]
	public static int clientMaxTeamSize;

	// Token: 0x0600190B RID: 6411 RVA: 0x00014D96 File Offset: 0x00012F96
	public static bool TeamsEnabled()
	{
		return RelationshipManager.clientMaxTeamSize > 0;
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x00014DA0 File Offset: 0x00012FA0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.relationshipManager != null)
		{
			RelationshipManager.clientMaxTeamSize = info.msg.relationshipManager.maxTeamSize;
		}
	}
}
