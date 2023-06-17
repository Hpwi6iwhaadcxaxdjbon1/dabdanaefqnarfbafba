using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class XMasRefill : BaseEntity
{
	// Token: 0x040006B4 RID: 1716
	public GameObjectRef[] giftPrefabs;

	// Token: 0x040006B5 RID: 1717
	public List<BasePlayer> goodKids;

	// Token: 0x040006B6 RID: 1718
	public List<Stocking> stockings;

	// Token: 0x040006B7 RID: 1719
	public AudioSource bells;

	// Token: 0x06000A55 RID: 2645 RVA: 0x000550A4 File Offset: 0x000532A4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("XMasRefill.OnRpcMessage", 0.1f))
		{
			if (rpc == 2490240562U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: PlayBells ");
				}
				using (TimeWarning.New("PlayBells", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							this.PlayBells();
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in PlayBells", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0000A3CB File Offset: 0x000085CB
	[BaseEntity.RPC_Client]
	public void PlayBells()
	{
		this.bells.Play();
	}
}
