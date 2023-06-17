using System;
using Rust.Ai.HTN;
using UnityEngine;

// Token: 0x02000143 RID: 323
public abstract class BaseNpcDefinition : Definition<BaseNpcDefinition>
{
	// Token: 0x040008DE RID: 2270
	[Header("Domain")]
	public HTNDomain Domain;

	// Token: 0x040008DF RID: 2271
	[Header("Base Stats")]
	public BaseNpcDefinition.InfoStats Info;

	// Token: 0x040008E0 RID: 2272
	public BaseNpcDefinition.VitalStats Vitals;

	// Token: 0x040008E1 RID: 2273
	public BaseNpcDefinition.MovementStats Movement;

	// Token: 0x040008E2 RID: 2274
	public BaseNpcDefinition.SensoryStats Sensory;

	// Token: 0x040008E3 RID: 2275
	public BaseNpcDefinition.MemoryStats Memory;

	// Token: 0x040008E4 RID: 2276
	public BaseNpcDefinition.EngagementStats Engagement;

	// Token: 0x06000C45 RID: 3141 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void Loadout(HTNPlayer target)
	{
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnlyLoadoutWeapons(HTNPlayer target)
	{
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void StartVoices(HTNPlayer target)
	{
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void StopVoices(HTNPlayer target)
	{
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x0000792A File Offset: 0x00005B2A
	public virtual BaseCorpse OnCreateCorpse(HTNPlayer target)
	{
		return null;
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void Loadout(HTNAnimal target)
	{
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void StartVoices(HTNAnimal target)
	{
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void StopVoices(HTNAnimal target)
	{
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0000792A File Offset: 0x00005B2A
	public virtual BaseCorpse OnCreateCorpse(HTNAnimal target)
	{
		return null;
	}

	// Token: 0x02000144 RID: 324
	[Serializable]
	public class InfoStats
	{
		// Token: 0x040008E5 RID: 2277
		public BaseNpcDefinition.Family Family;

		// Token: 0x040008E6 RID: 2278
		public BaseNpcDefinition.Family[] Predators;

		// Token: 0x040008E7 RID: 2279
		public BaseNpcDefinition.Family[] Prey;

		// Token: 0x06000C4F RID: 3151 RVA: 0x0005B810 File Offset: 0x00059A10
		public BaseNpc.AiStatistics.FamilyEnum ToFamily(BaseNpcDefinition.Family family)
		{
			switch (family)
			{
			default:
				return BaseNpc.AiStatistics.FamilyEnum.Player;
			case BaseNpcDefinition.Family.Scientist:
				return BaseNpc.AiStatistics.FamilyEnum.Scientist;
			case BaseNpcDefinition.Family.Murderer:
				return BaseNpc.AiStatistics.FamilyEnum.Murderer;
			case BaseNpcDefinition.Family.Horse:
				return BaseNpc.AiStatistics.FamilyEnum.Horse;
			case BaseNpcDefinition.Family.Deer:
				return BaseNpc.AiStatistics.FamilyEnum.Deer;
			case BaseNpcDefinition.Family.Boar:
				return BaseNpc.AiStatistics.FamilyEnum.Boar;
			case BaseNpcDefinition.Family.Wolf:
				return BaseNpc.AiStatistics.FamilyEnum.Wolf;
			case BaseNpcDefinition.Family.Bear:
				return BaseNpc.AiStatistics.FamilyEnum.Bear;
			case BaseNpcDefinition.Family.Chicken:
				return BaseNpc.AiStatistics.FamilyEnum.Chicken;
			case BaseNpcDefinition.Family.Zombie:
				return BaseNpc.AiStatistics.FamilyEnum.Zombie;
			}
		}
	}

	// Token: 0x02000145 RID: 325
	[Serializable]
	public class VitalStats
	{
		// Token: 0x040008E8 RID: 2280
		public float HP = 100f;
	}

	// Token: 0x02000146 RID: 326
	[Serializable]
	public class MovementStats
	{
		// Token: 0x040008E9 RID: 2281
		public float DuckSpeed = 1.7f;

		// Token: 0x040008EA RID: 2282
		public float WalkSpeed = 2.8f;

		// Token: 0x040008EB RID: 2283
		public float RunSpeed = 5.5f;

		// Token: 0x040008EC RID: 2284
		public float TurnSpeed = 120f;

		// Token: 0x040008ED RID: 2285
		public float Acceleration = 12f;
	}

	// Token: 0x02000147 RID: 327
	[Serializable]
	public class SensoryStats
	{
		// Token: 0x040008EE RID: 2286
		public float VisionRange = 40f;

		// Token: 0x040008EF RID: 2287
		public float HearingRange = 20f;

		// Token: 0x040008F0 RID: 2288
		[Range(0f, 360f)]
		public float FieldOfView = 120f;

		// Token: 0x040008F1 RID: 2289
		private const float Inv180 = 0.0055555557f;

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000C53 RID: 3155 RVA: 0x0000B7F5 File Offset: 0x000099F5
		public float SqrVisionRange
		{
			get
			{
				return this.VisionRange * this.VisionRange;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x0000B804 File Offset: 0x00009A04
		public float SqrHearingRange
		{
			get
			{
				return this.HearingRange * this.HearingRange;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000C55 RID: 3157 RVA: 0x0000B813 File Offset: 0x00009A13
		public float FieldOfViewRadians
		{
			get
			{
				return (this.FieldOfView - 180f) * -0.0055555557f - 0.1f;
			}
		}
	}

	// Token: 0x02000148 RID: 328
	[Serializable]
	public class MemoryStats
	{
		// Token: 0x040008F2 RID: 2290
		public float ForgetTime = 30f;

		// Token: 0x040008F3 RID: 2291
		public float ForgetInRangeTime = 5f;

		// Token: 0x040008F4 RID: 2292
		public float NoSeeReturnToSpawnTime = 10f;

		// Token: 0x040008F5 RID: 2293
		public float ForgetAnimalInRangeTime = 5f;
	}

	// Token: 0x02000149 RID: 329
	[Serializable]
	public class EngagementStats
	{
		// Token: 0x040008F6 RID: 2294
		public float CloseRange = 2f;

		// Token: 0x040008F7 RID: 2295
		public float MediumRange = 20f;

		// Token: 0x040008F8 RID: 2296
		public float LongRange = 100f;

		// Token: 0x040008F9 RID: 2297
		public float AggroRange = 100f;

		// Token: 0x040008FA RID: 2298
		public float DeaggroRange = 150f;

		// Token: 0x040008FB RID: 2299
		public float Hostility = 1f;

		// Token: 0x040008FC RID: 2300
		public float Defensiveness = 1f;

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000C58 RID: 3160 RVA: 0x0000B88A File Offset: 0x00009A8A
		public float SqrCloseRange
		{
			get
			{
				return this.CloseRange * this.CloseRange;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000C59 RID: 3161 RVA: 0x0000B899 File Offset: 0x00009A99
		public float SqrMediumRange
		{
			get
			{
				return this.MediumRange * this.MediumRange;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x0000B8A8 File Offset: 0x00009AA8
		public float SqrLongRange
		{
			get
			{
				return this.LongRange * this.LongRange;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000C5B RID: 3163 RVA: 0x0000B8B7 File Offset: 0x00009AB7
		public float SqrAggroRange
		{
			get
			{
				return this.AggroRange * this.AggroRange;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x0000B8C6 File Offset: 0x00009AC6
		public float SqrDeaggroRange
		{
			get
			{
				return this.DeaggroRange * this.DeaggroRange;
			}
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x0000B8D5 File Offset: 0x00009AD5
		public float CloseRangeFirearm(AttackEntity ent)
		{
			if (!ent)
			{
				return this.CloseRange;
			}
			return this.CloseRange + ent.CloseRangeAddition;
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0000B8F3 File Offset: 0x00009AF3
		public float MediumRangeFirearm(AttackEntity ent)
		{
			if (!ent)
			{
				return this.MediumRange;
			}
			return this.MediumRange + ent.MediumRangeAddition;
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x0000B911 File Offset: 0x00009B11
		public float LongRangeFirearm(AttackEntity ent)
		{
			if (!ent)
			{
				return this.LongRange;
			}
			return this.LongRange + ent.LongRangeAddition;
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x0000B92F File Offset: 0x00009B2F
		public float SqrCloseRangeFirearm(AttackEntity ent)
		{
			float num = this.CloseRangeFirearm(ent);
			return num * num;
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x0000B93A File Offset: 0x00009B3A
		public float SqrMediumRangeFirearm(AttackEntity ent)
		{
			float num = this.MediumRangeFirearm(ent);
			return num * num;
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x0000B945 File Offset: 0x00009B45
		public float SqrLongRangeFirearm(AttackEntity ent)
		{
			float num = this.LongRangeFirearm(ent);
			return num * num;
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x0000B950 File Offset: 0x00009B50
		public float CenterOfCloseRange()
		{
			return this.CloseRange * 0.5f;
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x0000B95E File Offset: 0x00009B5E
		public float CenterOfCloseRangeFirearm(AttackEntity ent)
		{
			return this.CloseRangeFirearm(ent) * 0.5f;
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x0000B96D File Offset: 0x00009B6D
		public float SqrCenterOfCloseRange()
		{
			float num = this.CenterOfCloseRange();
			return num * num;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x0000B977 File Offset: 0x00009B77
		public float SqrCenterOfCloseRangeFirearm(AttackEntity ent)
		{
			float num = this.CenterOfCloseRangeFirearm(ent);
			return num * num;
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x0005B860 File Offset: 0x00059A60
		public float CenterOfMediumRange()
		{
			float num = this.MediumRange - this.CloseRange;
			return this.MediumRange - num * 0.5f;
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x0005B88C File Offset: 0x00059A8C
		public float CenterOfMediumRangeFirearm(AttackEntity ent)
		{
			float num = this.MediumRangeFirearm(ent);
			float num2 = num - this.CloseRangeFirearm(ent);
			return num - num2 * 0.5f;
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x0000B982 File Offset: 0x00009B82
		public float SqrCenterOfMediumRange()
		{
			float num = this.CenterOfMediumRange();
			return num * num;
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x0000B98C File Offset: 0x00009B8C
		public float SqrCenterOfMediumRangeFirearm(AttackEntity ent)
		{
			float num = this.CenterOfMediumRangeFirearm(ent);
			return num * num;
		}
	}

	// Token: 0x0200014A RID: 330
	[Serializable]
	public class RoamStats
	{
		// Token: 0x040008FD RID: 2301
		public float MaxRoamRange = 20f;

		// Token: 0x040008FE RID: 2302
		public float MinRoamDelay = 5f;

		// Token: 0x040008FF RID: 2303
		public float MaxRoamDelay = 10f;

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000C6C RID: 3180 RVA: 0x0000B997 File Offset: 0x00009B97
		public float SqrMaxRoamRange
		{
			get
			{
				return this.MaxRoamRange * this.MaxRoamRange;
			}
		}
	}

	// Token: 0x0200014B RID: 331
	public enum Family
	{
		// Token: 0x04000901 RID: 2305
		Player,
		// Token: 0x04000902 RID: 2306
		Scientist,
		// Token: 0x04000903 RID: 2307
		Murderer,
		// Token: 0x04000904 RID: 2308
		Horse,
		// Token: 0x04000905 RID: 2309
		Deer,
		// Token: 0x04000906 RID: 2310
		Boar,
		// Token: 0x04000907 RID: 2311
		Wolf,
		// Token: 0x04000908 RID: 2312
		Bear,
		// Token: 0x04000909 RID: 2313
		Chicken,
		// Token: 0x0400090A RID: 2314
		Zombie
	}
}
