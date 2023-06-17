using System;
using Rust;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class DoorAnimEvents : MonoBehaviour, IClientComponent
{
	// Token: 0x04000E8D RID: 3725
	public GameObjectRef openStart;

	// Token: 0x04000E8E RID: 3726
	public GameObjectRef openEnd;

	// Token: 0x04000E8F RID: 3727
	public GameObjectRef closeStart;

	// Token: 0x04000E90 RID: 3728
	public GameObjectRef closeEnd;

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060011E0 RID: 4576 RVA: 0x0000F8FF File Offset: 0x0000DAFF
	public Animator animator
	{
		get
		{
			return base.GetComponent<Animator>();
		}
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x00076078 File Offset: 0x00074278
	private void DoorOpenStart()
	{
		if (Application.isLoading)
		{
			return;
		}
		if (!this.openStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		Effect.client.Run(this.openStart.resourcePath, base.gameObject);
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x000760DC File Offset: 0x000742DC
	private void DoorOpenEnd()
	{
		if (Application.isLoading)
		{
			return;
		}
		if (!this.openEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		Effect.client.Run(this.openEnd.resourcePath, base.gameObject);
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x00076140 File Offset: 0x00074340
	private void DoorCloseStart()
	{
		if (Application.isLoading)
		{
			return;
		}
		if (!this.closeStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		Effect.client.Run(this.closeStart.resourcePath, base.gameObject);
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x000761A4 File Offset: 0x000743A4
	private void DoorCloseEnd()
	{
		if (Application.isLoading)
		{
			return;
		}
		if (!this.closeEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		Effect.client.Run(this.closeEnd.resourcePath, base.gameObject);
	}
}
