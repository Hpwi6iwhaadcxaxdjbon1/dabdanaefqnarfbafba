using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class PlayerMetabolism : BaseMetabolism<BasePlayer>
{
	// Token: 0x0400064A RID: 1610
	public const float HotThreshold = 40f;

	// Token: 0x0400064B RID: 1611
	public const float ColdThreshold = 5f;

	// Token: 0x0400064C RID: 1612
	public MetabolismAttribute temperature = new MetabolismAttribute();

	// Token: 0x0400064D RID: 1613
	public MetabolismAttribute poison = new MetabolismAttribute();

	// Token: 0x0400064E RID: 1614
	public MetabolismAttribute radiation_level = new MetabolismAttribute();

	// Token: 0x0400064F RID: 1615
	public MetabolismAttribute radiation_poison = new MetabolismAttribute();

	// Token: 0x04000650 RID: 1616
	public MetabolismAttribute wetness = new MetabolismAttribute();

	// Token: 0x04000651 RID: 1617
	public MetabolismAttribute dirtyness = new MetabolismAttribute();

	// Token: 0x04000652 RID: 1618
	public MetabolismAttribute oxygen = new MetabolismAttribute();

	// Token: 0x04000653 RID: 1619
	public MetabolismAttribute bleeding = new MetabolismAttribute();

	// Token: 0x04000654 RID: 1620
	public MetabolismAttribute comfort = new MetabolismAttribute();

	// Token: 0x04000655 RID: 1621
	public MetabolismAttribute pending_health = new MetabolismAttribute();

	// Token: 0x04000656 RID: 1622
	public bool isDirty;

	// Token: 0x04000657 RID: 1623
	private float lastConsumeTime;

	// Token: 0x04000658 RID: 1624
	private float lastUpdateTime;

	// Token: 0x060009CC RID: 2508 RVA: 0x00052618 File Offset: 0x00050818
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerMetabolism.OnRpcMessage", 0.1f))
		{
			if (rpc == 858557799U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: UpdateMetabolism ");
				}
				using (TimeWarning.New("UpdateMetabolism", 0.1f))
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
							this.UpdateMetabolism(packet);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in UpdateMetabolism", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00052734 File Offset: 0x00050934
	public override void Reset()
	{
		base.Reset();
		this.poison.Reset();
		this.radiation_level.Reset();
		this.radiation_poison.Reset();
		this.temperature.Reset();
		this.oxygen.Reset();
		this.bleeding.Reset();
		this.wetness.Reset();
		this.dirtyness.Reset();
		this.comfort.Reset();
		this.pending_health.Reset();
		this.lastConsumeTime = float.NegativeInfinity;
		this.isDirty = true;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00009C0B File Offset: 0x00007E0B
	public bool CanConsume()
	{
		return (!this.owner || !this.owner.IsHeadUnderwater()) && UnityEngine.Time.time - this.lastConsumeTime > 1f;
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00009C3C File Offset: 0x00007E3C
	public void MarkConsumption()
	{
		this.lastConsumeTime = UnityEngine.Time.time;
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00009C49 File Offset: 0x00007E49
	public void ClientInit(BasePlayer owner)
	{
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.Reset();
		this.owner = owner;
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x000527C8 File Offset: 0x000509C8
	[BaseEntity.RPC_Client]
	public void UpdateMetabolism(BaseEntity.RPCMessage packet)
	{
		using (PlayerMetabolism playerMetabolism = PlayerMetabolism.Deserialize(packet.read))
		{
			this.Load(playerMetabolism);
		}
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00052804 File Offset: 0x00050A04
	public PlayerMetabolism Save()
	{
		PlayerMetabolism playerMetabolism = Pool.Get<PlayerMetabolism>();
		playerMetabolism.calories = this.calories.value;
		playerMetabolism.hydration = this.hydration.value;
		playerMetabolism.heartrate = this.heartrate.value;
		playerMetabolism.temperature = this.temperature.value;
		playerMetabolism.radiation_level = this.radiation_level.value;
		playerMetabolism.radiation_poisoning = this.radiation_poison.value;
		playerMetabolism.wetness = this.wetness.value;
		playerMetabolism.dirtyness = this.dirtyness.value;
		playerMetabolism.oxygen = this.oxygen.value;
		playerMetabolism.bleeding = this.bleeding.value;
		playerMetabolism.comfort = this.comfort.value;
		playerMetabolism.pending_health = this.pending_health.value;
		if (this.owner)
		{
			playerMetabolism.health = this.owner.Health();
		}
		return playerMetabolism;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00052904 File Offset: 0x00050B04
	public void Load(PlayerMetabolism s)
	{
		this.calories.SetValue(s.calories);
		this.hydration.SetValue(s.hydration);
		this.comfort.SetValue(s.comfort);
		this.heartrate.value = s.heartrate;
		this.temperature.value = s.temperature;
		this.radiation_level.value = s.radiation_level;
		this.radiation_poison.value = s.radiation_poisoning;
		this.wetness.value = s.wetness;
		this.dirtyness.value = s.dirtyness;
		this.oxygen.value = s.oxygen;
		this.bleeding.value = s.bleeding;
		this.pending_health.value = s.pending_health;
		if (this.owner)
		{
			this.owner.health = s.health;
			if (this.owner.isClient && this.owner == LocalPlayer.Entity)
			{
				this.OnLocalMetabolismUpdated();
			}
		}
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00052A20 File Offset: 0x00050C20
	public override MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
		case MetabolismAttribute.Type.Poison:
			return this.poison;
		case MetabolismAttribute.Type.Radiation:
			return this.radiation_poison;
		case MetabolismAttribute.Type.Bleeding:
			return this.bleeding;
		case MetabolismAttribute.Type.HealthOverTime:
			return this.pending_health;
		}
		return base.FindAttribute(type);
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00052A70 File Offset: 0x00050C70
	public void OnLocalMetabolismUpdated()
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime;
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.comfort.lastValue > 0f && this.comfort.value > 0f)
		{
			Analytics.ComfortDuration += num;
		}
		if (this.radiation_level.value > 0f)
		{
			Analytics.RadiationExposureDuration += num;
		}
		if (this.temperature.value < 5f)
		{
			Analytics.ColdExposureDuration += num;
		}
		else if (this.temperature.value > 40f)
		{
			Analytics.HotExposureDuration += num;
		}
		if (this.owner.TimeAwake > 3f)
		{
			if (this.hydration.value > this.hydration.lastValue)
			{
				Analytics.ConsumedWater += this.hydration.value - this.hydration.lastValue;
			}
			if (this.calories.value > this.calories.lastValue)
			{
				Analytics.ConsumedFood += this.calories.value - this.calories.lastValue;
			}
		}
	}
}
