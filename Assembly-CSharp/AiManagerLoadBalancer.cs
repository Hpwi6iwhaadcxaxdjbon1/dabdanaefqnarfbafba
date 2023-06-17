using System;
using Apex.LoadBalancing;

// Token: 0x0200013D RID: 317
public sealed class AiManagerLoadBalancer : LoadBalancer
{
	// Token: 0x040008D7 RID: 2263
	public static readonly ILoadBalancer aiManagerLoadBalancer = new LoadBalancedQueue(1, 2.5f, 1, 4);

	// Token: 0x06000C2A RID: 3114 RVA: 0x0000B677 File Offset: 0x00009877
	private AiManagerLoadBalancer()
	{
	}
}
