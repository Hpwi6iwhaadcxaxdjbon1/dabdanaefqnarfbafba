using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class DropBox : Mailbox
{
	// Token: 0x06000A5D RID: 2653 RVA: 0x0000A411 File Offset: 0x00008611
	public override bool PlayerIsOwner(BasePlayer player)
	{
		return this.PlayerBehind(player);
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00055358 File Offset: 0x00053558
	public bool PlayerBehind(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.3f;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00042690 File Offset: 0x00040890
	public bool PlayerInfront(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}
}
