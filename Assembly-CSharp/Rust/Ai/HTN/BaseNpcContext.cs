using System;
using System.Collections.Generic;
using Apex.AI;
using Apex.Ai.HTN;
using UnityEngine;

namespace Rust.Ai.HTN
{
	// Token: 0x020008D7 RID: 2263
	public abstract class BaseNpcContext : IHTNContext, IAIContext
	{
		// Token: 0x04002B82 RID: 11138
		public static List<Item> InventoryLookupCache = new List<Item>(10);

		// Token: 0x06003067 RID: 12391
		public abstract void StartDomainDecomposition();

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06003068 RID: 12392
		// (set) Token: 0x06003069 RID: 12393
		public abstract PlanResultType PlanResult { get; set; }

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x0600306A RID: 12394
		// (set) Token: 0x0600306B RID: 12395
		public abstract PlanStateType PlanState { get; set; }

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x0600306C RID: 12396
		// (set) Token: 0x0600306D RID: 12397
		public abstract Stack<PrimitiveTaskSelector> HtnPlan { get; set; }

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x0600306E RID: 12398
		// (set) Token: 0x0600306F RID: 12399
		public abstract int DecompositionScore { get; set; }

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06003070 RID: 12400
		// (set) Token: 0x06003071 RID: 12401
		public abstract Dictionary<Guid, Stack<IEffect>> AppliedEffects { get; set; }

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06003072 RID: 12402
		// (set) Token: 0x06003073 RID: 12403
		public abstract Dictionary<Guid, Stack<IEffect>> AppliedExpectedEffects { get; set; }

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06003074 RID: 12404
		public abstract byte[] WorldState { get; }

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06003075 RID: 12405
		public abstract byte[] PreviousWorldState { get; }

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06003076 RID: 12406
		// (set) Token: 0x06003077 RID: 12407
		public abstract bool IsWorldStateDirty { get; set; }

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06003078 RID: 12408
		public abstract Stack<WorldStateInfo>[] WorldStateChanges { get; }

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06003079 RID: 12409
		public abstract List<PrimitiveTaskSelector> DebugPlan { get; }

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x0600307A RID: 12410
		// (set) Token: 0x0600307B RID: 12411
		public abstract PrimitiveTaskSelector CurrentTask { get; set; }

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x0600307C RID: 12412
		// (set) Token: 0x0600307D RID: 12413
		public abstract NpcOrientation OrientationType { get; set; }

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x0600307E RID: 12414
		public abstract List<NpcPlayerInfo> PlayersInRange { get; }

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x0600307F RID: 12415
		public abstract List<NpcPlayerInfo> EnemyPlayersInRange { get; }

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06003080 RID: 12416
		public abstract List<NpcPlayerInfo> EnemyPlayersInLineOfSight { get; }

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06003081 RID: 12417
		public abstract List<NpcPlayerInfo> EnemyPlayersAudible { get; }

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06003082 RID: 12418
		public abstract List<NpcPlayerInfo> PlayersOutsideDetectionRange { get; }

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06003083 RID: 12419
		// (set) Token: 0x06003084 RID: 12420
		public abstract NpcPlayerInfo PrimaryEnemyPlayerInLineOfSight { get; set; }

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06003085 RID: 12421
		// (set) Token: 0x06003086 RID: 12422
		public abstract NpcPlayerInfo PrimaryEnemyPlayerAudible { get; set; }

		// Token: 0x06003087 RID: 12423
		public abstract NpcPlayerInfo GetPrimaryEnemyPlayerTarget();

		// Token: 0x06003088 RID: 12424
		public abstract bool HasPrimaryEnemyPlayerTarget();

		// Token: 0x06003089 RID: 12425
		public abstract Vector3 GetDirectionToPrimaryEnemyPlayerTargetBody();

		// Token: 0x0600308A RID: 12426
		public abstract Vector3 GetDirectionToPrimaryEnemyPlayerTargetHead();

		// Token: 0x0600308B RID: 12427
		public abstract Vector3 GetDirectionToMemoryOfPrimaryEnemyPlayerTarget();

		// Token: 0x0600308C RID: 12428
		public abstract Vector3 GetDirectionLookAround();

		// Token: 0x0600308D RID: 12429
		public abstract Vector3 GetDirectionLastAttackedDir();

		// Token: 0x0600308E RID: 12430
		public abstract Vector3 GetDirectionAudibleTarget();

		// Token: 0x0600308F RID: 12431
		public abstract Vector3 GetDirectionToAnimal();

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06003090 RID: 12432
		public abstract List<AnimalInfo> AnimalsInRange { get; }

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06003091 RID: 12433
		public abstract Vector3 BodyPosition { get; }

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06003092 RID: 12434
		public abstract BaseNpcMemory BaseMemory { get; }

		// Token: 0x06003093 RID: 12435
		public abstract void SetFact(byte fact, byte value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true);

		// Token: 0x06003094 RID: 12436
		public abstract byte GetFact(byte fact);

		// Token: 0x06003095 RID: 12437 RVA: 0x000EC738 File Offset: 0x000EA938
		public byte GetWorldState(byte fact)
		{
			byte result = this.WorldState[(int)fact];
			if (this.WorldStateChanges[(int)fact].Count > 0)
			{
				result = this.WorldStateChanges[(int)fact].Peek().Value;
			}
			return result;
		}

		// Token: 0x06003096 RID: 12438 RVA: 0x00002ECE File Offset: 0x000010CE
		public virtual void ResetState()
		{
		}
	}
}
