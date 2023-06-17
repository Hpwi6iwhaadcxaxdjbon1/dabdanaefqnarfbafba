using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class DoorKnocker : BaseCombatEntity
{
	// Token: 0x0400057A RID: 1402
	public Animator knocker1;

	// Token: 0x0400057B RID: 1403
	public Animator knocker2;

	// Token: 0x060008B2 RID: 2226 RVA: 0x0004D8BC File Offset: 0x0004BABC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DoorKnocker.OnRpcMessage", 0.1f))
		{
			if (rpc == 2329670328U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: ClientKnock ");
				}
				using (TimeWarning.New("ClientKnock", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ClientKnock(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in ClientKnock", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0004D9D8 File Offset: 0x0004BBD8
	[BaseEntity.RPC_Client]
	public void ClientKnock(BaseEntity.RPCMessage msg)
	{
		Vector3 a = msg.read.Vector3();
		float num = Vector3.Distance(a, this.knocker1.transform.position);
		float num2 = Vector3.Distance(a, this.knocker2.transform.position);
		if (num < num2)
		{
			this.knocker1.SetTrigger("knock");
			return;
		}
		this.knocker2.SetTrigger("knock");
	}
}
