using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008CB RID: 2251
	public class AiLocationManager : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x04002B3F RID: 11071
		public static List<AiLocationManager> Managers = new List<AiLocationManager>();

		// Token: 0x04002B40 RID: 11072
		[SerializeField]
		public AiLocationSpawner MainSpawner;

		// Token: 0x04002B41 RID: 11073
		[SerializeField]
		public AiLocationSpawner.SquadSpawnerLocation LocationWhenMainSpawnerIsNull = AiLocationSpawner.SquadSpawnerLocation.None;

		// Token: 0x04002B42 RID: 11074
		public Transform CoverPointGroup;

		// Token: 0x04002B43 RID: 11075
		public Transform PatrolPointGroup;

		// Token: 0x04002B44 RID: 11076
		public CoverPointVolume DynamicCoverPointVolume;

		// Token: 0x04002B45 RID: 11077
		public bool SnapCoverPointsToGround;

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x0600305E RID: 12382 RVA: 0x000253B4 File Offset: 0x000235B4
		public AiLocationSpawner.SquadSpawnerLocation LocationType
		{
			get
			{
				if (this.MainSpawner != null)
				{
					return this.MainSpawner.Location;
				}
				return this.LocationWhenMainSpawnerIsNull;
			}
		}
	}
}
