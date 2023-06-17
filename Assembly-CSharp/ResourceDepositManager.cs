using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002DA RID: 730
public class ResourceDepositManager : BaseEntity
{
	// Token: 0x04001039 RID: 4153
	public static ResourceDepositManager _manager;

	// Token: 0x0400103A RID: 4154
	private const int resolution = 20;

	// Token: 0x0400103B RID: 4155
	public Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit> _deposits;

	// Token: 0x020002DB RID: 731
	[Serializable]
	public class ResourceDeposit
	{
		// Token: 0x0400103C RID: 4156
		public float lastSurveyTime = float.NegativeInfinity;

		// Token: 0x0400103D RID: 4157
		public Vector3 origin;

		// Token: 0x0400103E RID: 4158
		public List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry> _resources;

		// Token: 0x060013CE RID: 5070 RVA: 0x00010DB6 File Offset: 0x0000EFB6
		public ResourceDeposit()
		{
			this._resources = new List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry>();
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0007C508 File Offset: 0x0007A708
		public void Add(ItemDefinition type, float efficiency, int amount, float workNeeded, ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType, bool liquid = false)
		{
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry = new ResourceDepositManager.ResourceDeposit.ResourceDepositEntry();
			resourceDepositEntry.type = type;
			resourceDepositEntry.efficiency = efficiency;
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry2 = resourceDepositEntry;
			resourceDepositEntry.amount = amount;
			resourceDepositEntry2.startAmount = amount;
			resourceDepositEntry.spawnType = spawnType;
			resourceDepositEntry.workNeeded = workNeeded;
			resourceDepositEntry.isLiquid = liquid;
			this._resources.Add(resourceDepositEntry);
		}

		// Token: 0x020002DC RID: 732
		[Serializable]
		public enum surveySpawnType
		{
			// Token: 0x04001040 RID: 4160
			ITEM,
			// Token: 0x04001041 RID: 4161
			OIL,
			// Token: 0x04001042 RID: 4162
			WATER
		}

		// Token: 0x020002DD RID: 733
		[Serializable]
		public class ResourceDepositEntry
		{
			// Token: 0x04001043 RID: 4163
			public ItemDefinition type;

			// Token: 0x04001044 RID: 4164
			public float efficiency = 1f;

			// Token: 0x04001045 RID: 4165
			public int amount;

			// Token: 0x04001046 RID: 4166
			public int startAmount;

			// Token: 0x04001047 RID: 4167
			public float workNeeded = 1f;

			// Token: 0x04001048 RID: 4168
			public float workDone;

			// Token: 0x04001049 RID: 4169
			public ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType;

			// Token: 0x0400104A RID: 4170
			public bool isLiquid;

			// Token: 0x060013D0 RID: 5072 RVA: 0x00010DD4 File Offset: 0x0000EFD4
			public void Subtract(int subamount)
			{
				if (subamount <= 0)
				{
					return;
				}
				this.amount -= subamount;
				if (this.amount < 0)
				{
					this.amount = 0;
				}
			}
		}
	}
}
