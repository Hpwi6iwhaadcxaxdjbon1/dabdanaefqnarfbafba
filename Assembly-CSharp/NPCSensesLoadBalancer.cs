using System;
using Apex.LoadBalancing;

// Token: 0x0200013F RID: 319
public sealed class NPCSensesLoadBalancer : LoadBalancer
{
	// Token: 0x040008D9 RID: 2265
	public static readonly ILoadBalancer NpcSensesLoadBalancer = new LoadBalancedQueue(50, 0.1f, 50, 4);

	// Token: 0x06000C2E RID: 3118 RVA: 0x0000B677 File Offset: 0x00009877
	private NPCSensesLoadBalancer()
	{
	}
}
