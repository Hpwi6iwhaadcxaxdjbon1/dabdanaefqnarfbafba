using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020002CE RID: 718
public static class EntityLinkEx
{
	// Token: 0x060013A6 RID: 5030 RVA: 0x0007C0C0 File Offset: 0x0007A2C0
	public static void FreeLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			EntityLink entityLink = links[i];
			entityLink.Clear();
			Pool.Free<EntityLink>(ref entityLink);
		}
		links.Clear();
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0007C0FC File Offset: 0x0007A2FC
	public static void ClearLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			links[i].Clear();
		}
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x0007C128 File Offset: 0x0007A328
	public static void AddLinks(this List<EntityLink> links, BaseEntity entity, Socket_Base[] sockets)
	{
		foreach (Socket_Base socket in sockets)
		{
			EntityLink entityLink = Pool.Get<EntityLink>();
			entityLink.Setup(entity, socket);
			links.Add(entityLink);
		}
	}
}
