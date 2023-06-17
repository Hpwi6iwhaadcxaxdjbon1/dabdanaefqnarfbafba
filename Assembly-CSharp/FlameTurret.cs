using System;
using System.Collections.Generic;
using ConVar;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class FlameTurret : StorageContainer
{
	// Token: 0x04000597 RID: 1431
	public Transform upper;

	// Token: 0x04000598 RID: 1432
	public Vector3 aimDir;

	// Token: 0x04000599 RID: 1433
	public float arc = 45f;

	// Token: 0x0400059A RID: 1434
	public float triggeredDuration = 5f;

	// Token: 0x0400059B RID: 1435
	public float flameRange = 7f;

	// Token: 0x0400059C RID: 1436
	public float flameRadius = 4f;

	// Token: 0x0400059D RID: 1437
	public float fuelPerSec = 1f;

	// Token: 0x0400059E RID: 1438
	public Transform eyeTransform;

	// Token: 0x0400059F RID: 1439
	public List<DamageTypeEntry> damagePerSec;

	// Token: 0x040005A0 RID: 1440
	public GameObjectRef triggeredEffect;

	// Token: 0x040005A1 RID: 1441
	public GameObjectRef fireballPrefab;

	// Token: 0x040005A2 RID: 1442
	public GameObjectRef explosionEffect;

	// Token: 0x040005A3 RID: 1443
	public TargetTrigger trigger;

	// Token: 0x060008CB RID: 2251 RVA: 0x0004E154 File Offset: 0x0004C354
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FlameTurret.OnRpcMessage", 0.1f))
		{
			if (rpc == 1671178114U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_ReceiveAimDir ");
				}
				using (TimeWarning.New("CLIENT_ReceiveAimDir", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_ReceiveAimDir(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_ReceiveAimDir", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x00002FD4 File Offset: 0x000011D4
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00009135 File Offset: 0x00007335
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00009142 File Offset: 0x00007342
	public void Update()
	{
		if (base.isClient)
		{
			this.ClientThink();
		}
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00009152 File Offset: 0x00007352
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.IsTriggered();
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00009168 File Offset: 0x00007368
	public void ClientThink()
	{
		this.UpdateAiming();
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x00009170 File Offset: 0x00007370
	[BaseEntity.RPC_Client]
	public void CLIENT_ReceiveAimDir(BaseEntity.RPCMessage rpc)
	{
		this.aimDir = rpc.read.Vector3();
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0004E270 File Offset: 0x0004C470
	public void UpdateAiming()
	{
		if (base.isServer)
		{
			return;
		}
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(0f, this.aimDir.y, 0f);
		if (this.upper.transform.localRotation != quaternion)
		{
			this.upper.transform.localRotation = Quaternion.Lerp(this.upper.transform.localRotation, quaternion, UnityEngine.Time.deltaTime * 12f);
		}
	}
}
