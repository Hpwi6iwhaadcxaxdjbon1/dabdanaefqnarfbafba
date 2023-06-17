using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000841 RID: 2113
	[ConsoleSystem.Factory("ai")]
	public class AI : ConsoleSystem
	{
		// Token: 0x04002924 RID: 10532
		[ClientVar]
		public static bool groundAlign = true;

		// Token: 0x04002925 RID: 10533
		[ClientVar]
		public static float maxGroundAlignDist = 30f;

		// Token: 0x04002926 RID: 10534
		[ClientVar]
		public static bool debugVis = false;

		// Token: 0x04002927 RID: 10535
		[ClientVar]
		public static bool npc_no_foot_ik = true;

		// Token: 0x04002928 RID: 10536
		private static HitTest lookingAtNpcCache;

		// Token: 0x06002E17 RID: 11799 RVA: 0x000E7004 File Offset: 0x000E5204
		[ClientVar(AllowRunFromServer = true)]
		public static void aiDebug_lookat(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			if (LocalPlayer.Entity.lookingAtEntity)
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Client, "ai.aiDebug_toggle", new object[]
				{
					LocalPlayer.Entity.lookingAtEntity.net.ID
				});
			}
		}

		// Token: 0x06002E18 RID: 11800 RVA: 0x00023ADA File Offset: 0x00021CDA
		[ClientVar(AllowRunFromServer = true)]
		public static void aiDebug_LoadBalanceOverdueReport(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "ai.aiDebug_LoadBalanceOverdueReportServer", Array.Empty<object>());
		}

		// Token: 0x06002E19 RID: 11801 RVA: 0x00002ECE File Offset: 0x000010CE
		[ClientVar(AllowRunFromServer = true)]
		public static void selectNPCLookat(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002E1A RID: 11802 RVA: 0x000E7070 File Offset: 0x000E5270
		private static bool CheckLookingAtVisible(HitTest test, TraceInfo trace)
		{
			Vector3 origin = test.AttackRay.origin;
			Vector3 a = trace.point - origin;
			float magnitude = a.magnitude;
			if (magnitude < Mathf.Epsilon)
			{
				return true;
			}
			Vector3 direction = a / magnitude;
			RaycastHit hit;
			return !Physics.Raycast(new Ray(origin, direction), ref hit, magnitude + 0.01f, 1218519041) || hit.GetEntity() == trace.entity;
		}
	}
}
