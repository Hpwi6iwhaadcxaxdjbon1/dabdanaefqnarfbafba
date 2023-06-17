using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class TreeMarker : BaseEntity
{
	// Token: 0x04000007 RID: 7
	public GameObjectRef hitEffect;

	// Token: 0x04000008 RID: 8
	public SoundDefinition hitEffectSound;

	// Token: 0x04000009 RID: 9
	public GameObjectRef spawnEffect;

	// Token: 0x0400000A RID: 10
	public DeferredDecal myDecal;

	// Token: 0x0400000B RID: 11
	private Vector3 initialPosition;

	// Token: 0x0600000F RID: 15 RVA: 0x00002CFA File Offset: 0x00000EFA
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.initialPosition = base.transform.position;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002D14 File Offset: 0x00000F14
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		this.UpdatePositioning();
	}

	// Token: 0x06000011 RID: 17 RVA: 0x000267E4 File Offset: 0x000249E4
	[BaseEntity.RPC_Client]
	public void MarkerHit(BaseEntity.RPCMessage msg)
	{
		if (MainCamera.Distance(base.transform.position) > 30f)
		{
			return;
		}
		Effect.client.Run(this.hitEffect.resourcePath, base.transform.position, base.transform.up, default(Vector3));
		int num = msg.read.Int32();
		Sound sound = SoundManager.PlayOneshot(this.hitEffectSound, null, false, base.transform.position);
		if (sound != null)
		{
			SoundModulation.Modulator modulator = sound.modulation.CreateModulator(SoundModulation.Parameter.Pitch);
			float num2 = Mathf.Clamp01((float)num / 8f);
			modulator.value = 0.75f + 2f * num2;
		}
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00026894 File Offset: 0x00024A94
	public void UpdatePositioning()
	{
		Vector3 vector = -base.transform.up;
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(this.initialPosition + vector * -0.5f, vector), 0.05f, ref raycastHit, 3f, 1073741824))
		{
			base.transform.position = raycastHit.point;
			base.transform.rotation = QuaternionEx.LookRotationNormal(raycastHit.normal, Vector3.zero);
			Effect.client.Run(this.spawnEffect.resourcePath, base.transform.position, raycastHit.normal, default(Vector3));
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00026940 File Offset: 0x00024B40
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeMarker.OnRpcMessage", 0.1f))
		{
			if (rpc == 903533166U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: MarkerHit ");
				}
				using (TimeWarning.New("MarkerHit", 0.1f))
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
							this.MarkerHit(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in MarkerHit", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}
