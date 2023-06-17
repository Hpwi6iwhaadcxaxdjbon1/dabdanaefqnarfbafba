using System;
using UnityEngine;

// Token: 0x0200070D RID: 1805
public class DeferredAction
{
	// Token: 0x04002389 RID: 9097
	private Object sender;

	// Token: 0x0400238A RID: 9098
	private Action action;

	// Token: 0x0400238B RID: 9099
	private ActionPriority priority = ActionPriority.Medium;

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x060027A7 RID: 10151 RVA: 0x0001EF82 File Offset: 0x0001D182
	// (set) Token: 0x060027A8 RID: 10152 RVA: 0x0001EF8A File Offset: 0x0001D18A
	public bool Idle { get; private set; }

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x060027A9 RID: 10153 RVA: 0x0001EF93 File Offset: 0x0001D193
	public int Index
	{
		get
		{
			return (int)this.priority;
		}
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x0001EF9B File Offset: 0x0001D19B
	public DeferredAction(Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		this.sender = sender;
		this.action = action;
		this.priority = priority;
		this.Idle = true;
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x0001EFC6 File Offset: 0x0001D1C6
	public void Action()
	{
		if (this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		this.Idle = true;
		if (this.sender)
		{
			this.action.Invoke();
		}
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x0001EFFA File Offset: 0x0001D1FA
	public void Invoke()
	{
		if (!this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		LoadBalancer.Enqueue(this);
		this.Idle = false;
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x0001415A File Offset: 0x0001235A
	public static implicit operator bool(DeferredAction obj)
	{
		return obj != null;
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x0001F01C File Offset: 0x0001D21C
	public static void Invoke(Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		new DeferredAction(sender, action, priority).Invoke();
	}
}
