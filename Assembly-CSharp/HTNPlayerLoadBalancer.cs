using System;
using Apex.LoadBalancing;

// Token: 0x02000142 RID: 322
public sealed class HTNPlayerLoadBalancer : LoadBalancer
{
	// Token: 0x040008DD RID: 2269
	public static readonly ILoadBalancer HTNPlayerBalancer = new LoadBalancedQueue(50, 0.1f, 50, 4);

	// Token: 0x06000C43 RID: 3139 RVA: 0x0000B677 File Offset: 0x00009877
	private HTNPlayerLoadBalancer()
	{
	}
}
