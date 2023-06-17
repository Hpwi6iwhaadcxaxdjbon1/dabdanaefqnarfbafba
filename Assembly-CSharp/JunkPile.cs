using System;
using ConVar;
using Network;
using Rust.Ai.HTN.ScientistJunkpile;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class JunkPile : BaseEntity
{
	// Token: 0x0400060F RID: 1551
	public GameObjectRef sinkEffect;

	// Token: 0x04000610 RID: 1552
	public SpawnGroup[] spawngroups;

	// Token: 0x04000611 RID: 1553
	public ScientistJunkpileSpawner npcSpawnGroup;

	// Token: 0x04000612 RID: 1554
	private const float lifetimeMinutes = 30f;

	// Token: 0x04000613 RID: 1555
	private float sunkAmount;

	// Token: 0x06000956 RID: 2390 RVA: 0x00050050 File Offset: 0x0004E250
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("JunkPile.OnRpcMessage", 0.1f))
		{
			if (rpc == 1187385725U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_StartSink ");
				}
				using (TimeWarning.New("CLIENT_StartSink", 0.1f))
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
							this.CLIENT_StartSink(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_StartSink", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x0005016C File Offset: 0x0004E36C
	[BaseEntity.RPC_Client]
	public void CLIENT_StartSink(BaseEntity.RPCMessage msg)
	{
		Effect.client.Run(this.sinkEffect.resourcePath, base.transform.position, base.transform.up, default(Vector3));
		base.InvokeRepeating(new Action(this.SinkThink), 0.75f, 0.01f);
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x000501C4 File Offset: 0x0004E3C4
	public void SinkThink()
	{
		if (this.sunkAmount >= 10f)
		{
			return;
		}
		float num = 0.5f * UnityEngine.Time.deltaTime;
		this.sunkAmount += num;
		base.transform.position -= new Vector3(0f, num, 0f);
	}
}
