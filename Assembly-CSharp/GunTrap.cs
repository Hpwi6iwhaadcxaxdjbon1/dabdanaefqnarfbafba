using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class GunTrap : StorageContainer
{
	// Token: 0x040005A4 RID: 1444
	public GameObjectRef gun_fire_effect;

	// Token: 0x040005A5 RID: 1445
	public GameObjectRef bulletEffect;

	// Token: 0x040005A6 RID: 1446
	public GameObjectRef triggeredEffect;

	// Token: 0x040005A7 RID: 1447
	public Transform muzzlePos;

	// Token: 0x040005A8 RID: 1448
	public Transform eyeTransform;

	// Token: 0x040005A9 RID: 1449
	public int numPellets = 15;

	// Token: 0x040005AA RID: 1450
	public int aimCone = 30;

	// Token: 0x040005AB RID: 1451
	public float sensorRadius = 1.25f;

	// Token: 0x040005AC RID: 1452
	public ItemDefinition ammoType;

	// Token: 0x040005AD RID: 1453
	public TargetTrigger trigger;

	// Token: 0x060008D4 RID: 2260 RVA: 0x0004E300 File Offset: 0x0004C500
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("GunTrap.OnRpcMessage", 0.1f))
		{
			if (rpc == 1127653975U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_FireGun ");
				}
				using (TimeWarning.New("CLIENT_FireGun", 0.1f))
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
							this.CLIENT_FireGun(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_FireGun", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x00004723 File Offset: 0x00002923
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x000091C2 File Offset: 0x000073C2
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0004E41C File Offset: 0x0004C61C
	[BaseEntity.RPC_Client]
	public void CLIENT_FireGun(BaseEntity.RPCMessage rpc)
	{
		Vector3 a = rpc.read.Vector3();
		float d = 100f;
		Vector3 position = this.muzzlePos.position;
		Vector3 normalized = (a - position).normalized;
		GameObject gameObject = GameManager.client.CreatePrefab(this.bulletEffect.resourcePath, position + normalized * 1f, Quaternion.LookRotation(normalized), false);
		if (gameObject == null)
		{
			return;
		}
		Projectile component = gameObject.GetComponent<Projectile>();
		if (component)
		{
			component.clientsideEffect = true;
			component.owner = null;
			component.seed = 0;
			component.InitializeVelocity(normalized * d);
		}
		gameObject.SetActive(true);
	}

	// Token: 0x02000097 RID: 151
	public static class GunTrapFlags
	{
		// Token: 0x040005AE RID: 1454
		public const BaseEntity.Flags Triggered = BaseEntity.Flags.Reserved1;
	}
}
