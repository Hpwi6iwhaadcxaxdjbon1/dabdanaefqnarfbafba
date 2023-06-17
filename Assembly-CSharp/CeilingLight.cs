using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class CeilingLight : IOEntity
{
	// Token: 0x04000527 RID: 1319
	public float pushScale = 2f;

	// Token: 0x0600083B RID: 2107 RVA: 0x0004B1AC File Offset: 0x000493AC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CeilingLight.OnRpcMessage", 0.1f))
		{
			if (rpc == 101495390U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: ClientPhysPush ");
				}
				using (TimeWarning.New("ClientPhysPush", 0.1f))
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
							this.ClientPhysPush(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in ClientPhysPush", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00008B4B File Offset: 0x00006D4B
	public override void OnAttacked(HitInfo info)
	{
		if (info.Initiator == LocalPlayer.Entity)
		{
			this.PhysPush(info.attackNormal, info.HitPositionWorld);
		}
		base.OnAttacked(info);
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0004B2C8 File Offset: 0x000494C8
	[BaseEntity.RPC_Client]
	public void ClientPhysPush(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		if (LocalPlayer.Entity && num == LocalPlayer.Entity.net.ID)
		{
			return;
		}
		Vector3 attackNormal = msg.read.Vector3();
		Vector3 hitPositionWorld = msg.read.Vector3();
		this.PhysPush(attackNormal, hitPositionWorld);
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0004B320 File Offset: 0x00049520
	public void PhysPush(Vector3 attackNormal, Vector3 hitPositionWorld)
	{
		Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].AddForceAtPosition(attackNormal * this.pushScale, hitPositionWorld, 1);
		}
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0004B358 File Offset: 0x00049558
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && component.isKinematic)
		{
			component.detectCollisions = false;
		}
	}
}
