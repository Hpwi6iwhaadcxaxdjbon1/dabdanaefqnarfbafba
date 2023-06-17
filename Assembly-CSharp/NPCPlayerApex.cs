using System;
using Apex.AI.Components;
using Apex.AI.Serialization;
using Rust.Ai;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class NPCPlayerApex : NPCPlayer
{
	// Token: 0x04000893 RID: 2195
	public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

	// Token: 0x04000894 RID: 2196
	public GameObjectRef RadioEffect;

	// Token: 0x04000895 RID: 2197
	public GameObjectRef DeathEffect;

	// Token: 0x04000896 RID: 2198
	public int agentTypeIndex;

	// Token: 0x04000897 RID: 2199
	public BaseNpc.AiStatistics Stats;

	// Token: 0x04000898 RID: 2200
	[SerializeField]
	private UtilityAIComponent utilityAiComponent;

	// Token: 0x04000899 RID: 2201
	public bool NewAI;

	// Token: 0x0400089A RID: 2202
	public bool NeverMove;

	// Token: 0x0400089B RID: 2203
	public bool IsMountableAgent;

	// Token: 0x0400089C RID: 2204
	public float WeaponSwitchFrequency = 5f;

	// Token: 0x0400089D RID: 2205
	public float ToolSwitchFrequency = 5f;

	// Token: 0x0400089E RID: 2206
	public WaypointSet WaypointSet;

	// Token: 0x0400089F RID: 2207
	[NonSerialized]
	public Transform[] LookAtInterestPointsStationary;

	// Token: 0x040008A0 RID: 2208
	[Header("NPC Player Senses")]
	public int ForgetUnseenEntityTime = 10;

	// Token: 0x040008A1 RID: 2209
	public float SensesTickRate = 0.5f;

	// Token: 0x040008A2 RID: 2210
	public float MaxDistanceToCover = 15f;

	// Token: 0x040008A3 RID: 2211
	public float MinDistanceToRetreatCover = 6f;

	// Token: 0x040008A4 RID: 2212
	[Header("NPC Player Senses Target Scoring")]
	public float VisionRangeScore = 1f;

	// Token: 0x040008A5 RID: 2213
	public float AggroRangeScore = 5f;

	// Token: 0x040008A6 RID: 2214
	public float LongRangeScore = 1f;

	// Token: 0x040008A7 RID: 2215
	public float MediumRangeScore = 5f;

	// Token: 0x040008A8 RID: 2216
	public float CloseRangeScore = 10f;

	// Token: 0x040008A9 RID: 2217
	[Header("Sensory")]
	[Tooltip("Only care about sensations from our active enemy target, and nobody else.")]
	public bool OnlyTargetSensations;

	// Token: 0x040008AA RID: 2218
	[Header("Sensory System")]
	public AIStorage SelectPlayerTargetUtility;

	// Token: 0x040008AB RID: 2219
	public AIStorage SelectPlayerTargetMountedUtility;

	// Token: 0x040008AC RID: 2220
	public AIStorage SelectEntityTargetsUtility;

	// Token: 0x040008AD RID: 2221
	public AIStorage SelectCoverTargetsUtility;

	// Token: 0x040008AE RID: 2222
	public AIStorage SelectEnemyHideoutUtility;

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0000B07A File Offset: 0x0000927A
	public override string Categorize()
	{
		return "scientist";
	}

	// Token: 0x02000131 RID: 305
	public enum WeaponTypeEnum : byte
	{
		// Token: 0x040008B0 RID: 2224
		None,
		// Token: 0x040008B1 RID: 2225
		CloseRange,
		// Token: 0x040008B2 RID: 2226
		MediumRange,
		// Token: 0x040008B3 RID: 2227
		LongRange
	}

	// Token: 0x02000132 RID: 306
	public enum EnemyRangeEnum : byte
	{
		// Token: 0x040008B5 RID: 2229
		CloseAttackRange,
		// Token: 0x040008B6 RID: 2230
		MediumAttackRange,
		// Token: 0x040008B7 RID: 2231
		LongAttackRange,
		// Token: 0x040008B8 RID: 2232
		OutOfRange
	}

	// Token: 0x02000133 RID: 307
	public enum EnemyEngagementRangeEnum : byte
	{
		// Token: 0x040008BA RID: 2234
		AggroRange,
		// Token: 0x040008BB RID: 2235
		DeaggroRange,
		// Token: 0x040008BC RID: 2236
		NeutralRange
	}

	// Token: 0x02000134 RID: 308
	public enum ToolTypeEnum : byte
	{
		// Token: 0x040008BE RID: 2238
		None,
		// Token: 0x040008BF RID: 2239
		Research,
		// Token: 0x040008C0 RID: 2240
		Lightsource
	}
}
