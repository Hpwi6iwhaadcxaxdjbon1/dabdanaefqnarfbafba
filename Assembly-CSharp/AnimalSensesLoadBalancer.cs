using System;
using Apex.LoadBalancing;

// Token: 0x0200013E RID: 318
public sealed class AnimalSensesLoadBalancer : LoadBalancer
{
	// Token: 0x040008D8 RID: 2264
	public static readonly ILoadBalancer animalSensesLoadBalancer = new LoadBalancedQueue(300, 0.1f, 50, 4);

	// Token: 0x06000C2C RID: 3116 RVA: 0x0000B677 File Offset: 0x00009877
	private AnimalSensesLoadBalancer()
	{
	}
}
