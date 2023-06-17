using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000876 RID: 2166
	[ConsoleSystem.Factory("physics")]
	public class Physics : ConsoleSystem
	{
		// Token: 0x040029BE RID: 10686
		[ClientVar(Help = "The collision detection mode that ragdolls should use")]
		public static int ragdollmode = 1;

		// Token: 0x06002F2B RID: 12075 RVA: 0x00024425 File Offset: 0x00022625
		internal static void ApplyRagdoll(Rigidbody rigidBody)
		{
			if (Physics.ragdollmode <= 0)
			{
				rigidBody.collisionDetectionMode = 0;
			}
			if (Physics.ragdollmode == 1)
			{
				rigidBody.collisionDetectionMode = 1;
			}
			if (Physics.ragdollmode == 2)
			{
				rigidBody.collisionDetectionMode = 2;
			}
			if (Physics.ragdollmode >= 3)
			{
				rigidBody.collisionDetectionMode = 3;
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06002F2C RID: 12076 RVA: 0x00024463 File Offset: 0x00022663
		// (set) Token: 0x06002F2D RID: 12077 RVA: 0x00024470 File Offset: 0x00022670
		[ServerVar(Help = "The amount of physics steps per second")]
		[ClientVar]
		public static float steps
		{
			get
			{
				return 1f / Time.fixedDeltaTime;
			}
			set
			{
				if (value < 30f)
				{
					value = 30f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.fixedDeltaTime = 1f / value;
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06002F2E RID: 12078 RVA: 0x0002449C File Offset: 0x0002269C
		// (set) Token: 0x06002F2F RID: 12079 RVA: 0x000244A9 File Offset: 0x000226A9
		[ServerVar(Help = "The slowest physics steps will operate")]
		[ClientVar]
		public static float minsteps
		{
			get
			{
				return 1f / Time.maximumDeltaTime;
			}
			set
			{
				if (value < 2f)
				{
					value = 2f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.maximumDeltaTime = 1f / value;
			}
		}
	}
}
