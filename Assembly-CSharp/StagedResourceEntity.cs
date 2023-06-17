using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class StagedResourceEntity : ResourceEntity
{
	// Token: 0x04000672 RID: 1650
	public List<StagedResourceEntity.ResourceStage> stages = new List<StagedResourceEntity.ResourceStage>();

	// Token: 0x04000673 RID: 1651
	public int stage;

	// Token: 0x04000674 RID: 1652
	public GameObjectRef changeStageEffect;

	// Token: 0x04000675 RID: 1653
	public GameObject gibSourceTest;

	// Token: 0x060009FA RID: 2554 RVA: 0x00053270 File Offset: 0x00051470
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StagedResourceEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 1435177709U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: ResourceUpdate ");
				}
				using (TimeWarning.New("ResourceUpdate", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage packet = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ResourceUpdate(packet);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in ResourceUpdate", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0005338C File Offset: 0x0005158C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		int num = info.msg.resource.stage;
		if (info.fromDisk && base.isServer)
		{
			this.health = this.startHealth;
			num = 0;
		}
		if (num != this.stage)
		{
			this.stage = num;
			this.UpdateStage();
		}
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00009E31 File Offset: 0x00008031
	public void RunChangeEffect()
	{
		if (base.isServer)
		{
			return;
		}
		if (this.stage == 0)
		{
			return;
		}
		if (this.changeStageEffect.isValid)
		{
			Effect.client.Run(this.changeStageEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero);
		}
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x00009E6E File Offset: 0x0000806E
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.UpdateStage();
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x000533F8 File Offset: 0x000515F8
	[BaseEntity.RPC_Client]
	private void ResourceUpdate(BaseEntity.RPCMessage packet)
	{
		BaseResource baseResource = BaseResource.Deserialize(packet.read);
		this.health = baseResource.health;
		this.stage = baseResource.stage;
		this.UpdateStage();
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x00009E7D File Offset: 0x0000807D
	public T GetStageComponent<T>() where T : Component
	{
		return this.stages[this.stage].instance.GetComponentInChildren<T>();
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00053430 File Offset: 0x00051630
	private void UpdateStage()
	{
		if (this.stages.Count == 0)
		{
			return;
		}
		this.RunChangeEffect();
		for (int i = 0; i < this.stages.Count; i++)
		{
			this.stages[i].instance.SetActive(i == this.stage);
		}
	}

	// Token: 0x020000BA RID: 186
	[Serializable]
	public class ResourceStage
	{
		// Token: 0x04000676 RID: 1654
		public float health;

		// Token: 0x04000677 RID: 1655
		public GameObject instance;
	}
}
