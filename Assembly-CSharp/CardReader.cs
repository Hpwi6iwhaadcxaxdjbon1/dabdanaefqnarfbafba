using System;
using Network;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class CardReader : IOEntity
{
	// Token: 0x0400051E RID: 1310
	public float accessDuration = 10f;

	// Token: 0x0400051F RID: 1311
	public int accessLevel;

	// Token: 0x04000520 RID: 1312
	public GameObjectRef accessGrantedEffect;

	// Token: 0x04000521 RID: 1313
	public GameObjectRef accessDeniedEffect;

	// Token: 0x04000522 RID: 1314
	public GameObjectRef swipeEffect;

	// Token: 0x04000523 RID: 1315
	public Transform audioPosition;

	// Token: 0x04000524 RID: 1316
	public BaseEntity.Flags AccessLevel1 = BaseEntity.Flags.Reserved1;

	// Token: 0x04000525 RID: 1317
	public BaseEntity.Flags AccessLevel2 = BaseEntity.Flags.Reserved2;

	// Token: 0x04000526 RID: 1318
	public BaseEntity.Flags AccessLevel3 = BaseEntity.Flags.Reserved3;

	// Token: 0x06000837 RID: 2103 RVA: 0x0004B0CC File Offset: 0x000492CC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CardReader.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0004B110 File Offset: 0x00049310
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.accessLevel = info.msg.ioEntity.genericInt1;
			this.accessDuration = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0004B160 File Offset: 0x00049360
	public void ClientCardSwiped(Keycard card)
	{
		Effect.client.Run(this.swipeEffect.resourcePath, this.audioPosition.position, Vector3.up, default(Vector3));
		base.ServerRPC<uint>("ServerCardSwiped", card.net.ID);
	}
}
