using System;
using Facepunch.Steamworks;
using Network;

// Token: 0x020002E7 RID: 743
public class LootContainer : StorageContainer
{
	// Token: 0x04001058 RID: 4184
	public bool destroyOnEmpty = true;

	// Token: 0x04001059 RID: 4185
	public LootSpawn lootDefinition;

	// Token: 0x0400105A RID: 4186
	public int maxDefinitionsToSpawn;

	// Token: 0x0400105B RID: 4187
	public float minSecondsBetweenRefresh = 3600f;

	// Token: 0x0400105C RID: 4188
	public float maxSecondsBetweenRefresh = 7200f;

	// Token: 0x0400105D RID: 4189
	public bool initialLootSpawn = true;

	// Token: 0x0400105E RID: 4190
	public float xpLootedScale = 1f;

	// Token: 0x0400105F RID: 4191
	public float xpDestroyedScale = 1f;

	// Token: 0x04001060 RID: 4192
	public bool BlockPlayerItemInput;

	// Token: 0x04001061 RID: 4193
	public int scrapAmount;

	// Token: 0x04001062 RID: 4194
	public string deathStat = "";

	// Token: 0x04001063 RID: 4195
	public LootContainer.spawnType SpawnType;

	// Token: 0x04001064 RID: 4196
	private bool localPlayerInvolved;

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060013E1 RID: 5089 RVA: 0x00010EE1 File Offset: 0x0000F0E1
	public bool shouldRefreshContents
	{
		get
		{
			return this.minSecondsBetweenRefresh > 0f && this.maxSecondsBetweenRefresh > 0f;
		}
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x00004771 File Offset: 0x00002971
	public override bool DisplayHealthInfo(BasePlayer player)
	{
		return this.ShowHealthInfo;
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x00010EFF File Offset: 0x0000F0FF
	public override void DoDestroyEffects(BaseNetworkable.DestroyMode mode, Message msg)
	{
		if (mode == BaseNetworkable.DestroyMode.Gib && this.localPlayerInvolved && !string.IsNullOrEmpty(this.deathStat))
		{
			Client.Instance.Stats.Add(this.deathStat, 1, true);
		}
		base.DoDestroyEffects(mode, msg);
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x00010F3A File Offset: 0x0000F13A
	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient && info.InitiatorPlayer == LocalPlayer.Entity)
		{
			this.localPlayerInvolved = true;
		}
		base.OnAttacked(info);
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x00010F64 File Offset: 0x0000F164
	public override void InitShared()
	{
		base.InitShared();
		this.localPlayerInvolved = false;
	}

	// Token: 0x020002E8 RID: 744
	public enum spawnType
	{
		// Token: 0x04001066 RID: 4198
		GENERIC,
		// Token: 0x04001067 RID: 4199
		PLAYER,
		// Token: 0x04001068 RID: 4200
		TOWN,
		// Token: 0x04001069 RID: 4201
		AIRDROP,
		// Token: 0x0400106A RID: 4202
		CRASHSITE,
		// Token: 0x0400106B RID: 4203
		ROADSIDE
	}

	// Token: 0x020002E9 RID: 745
	[Serializable]
	public struct LootSpawnSlot
	{
		// Token: 0x0400106C RID: 4204
		public LootSpawn definition;

		// Token: 0x0400106D RID: 4205
		public int numberToSpawn;

		// Token: 0x0400106E RID: 4206
		public float probability;
	}
}
