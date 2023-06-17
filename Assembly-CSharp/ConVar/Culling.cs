using System;

namespace ConVar
{
	// Token: 0x0200084C RID: 2124
	[ConsoleSystem.Factory("culling")]
	public class Culling : ConsoleSystem
	{
		// Token: 0x04002954 RID: 10580
		[ClientVar(Saved = true, Help = "How many times per second to check for visibility")]
		public static float entityUpdateRate = 5f;

		// Token: 0x04002955 RID: 10581
		[ClientVar(Saved = true, Help = "Entity of any kind will always be visible closer than this")]
		public static float entityMinCullDist = 15f;

		// Token: 0x04002956 RID: 10582
		[ClientVar(Saved = true, Help = "Minimum distance at which we start disabling animators for entities")]
		public static float entityMinAnimatorCullDist = 30f;

		// Token: 0x04002957 RID: 10583
		[ClientVar(Saved = true, Help = "Minimum distance at which we start disabling shadows for entities")]
		public static float entityMinShadowCullDist = 5f;

		// Token: 0x04002958 RID: 10584
		[ClientVar(Saved = true, Help = "Maximum distance to show any players in meters")]
		public static float entityMaxDist = 5000f;

		// Token: 0x04002959 RID: 10585
		private static bool _env = true;

		// Token: 0x0400295A RID: 10586
		[ClientVar(Saved = true, Help = "Minimum distance in meters to start culling environment")]
		public static float envMinDist = 10f;

		// Token: 0x0400295B RID: 10587
		[ClientVar(Help = "Default visibility for unknown data during reprojection; e.g. rotating camera fast.")]
		public static bool noDataVisible = false;

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06002E4E RID: 11854 RVA: 0x00023C56 File Offset: 0x00021E56
		// (set) Token: 0x06002E4F RID: 11855 RVA: 0x000E7828 File Offset: 0x000E5A28
		[ClientVar(Help = "Enable/Disable occlusion culling")]
		public static bool toggle
		{
			get
			{
				return OcclusionCulling.Enabled;
			}
			set
			{
				if (OcclusionCulling.Enabled != value)
				{
					OcclusionCulling.Enabled = value;
					if (!value)
					{
						OcclusionCulling.MakeAllVisible();
						foreach (BasePlayer basePlayer in BasePlayer.VisiblePlayerList)
						{
							basePlayer.MakeVisible();
						}
						foreach (BaseNpc baseNpc in BaseNpc.VisibleNpcList)
						{
							baseNpc.MakeVisible();
						}
					}
				}
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06002E50 RID: 11856 RVA: 0x00023C5D File Offset: 0x00021E5D
		// (set) Token: 0x06002E51 RID: 11857 RVA: 0x00023C64 File Offset: 0x00021E64
		[ClientVar(Saved = true, Help = "Culling safe mode; VERY SLOW and LEAKY... for debugging purposes only")]
		public static bool safeMode
		{
			get
			{
				return OcclusionCulling.SafeMode;
			}
			set
			{
				OcclusionCulling.SafeMode = value;
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06002E52 RID: 11858 RVA: 0x00023C6C File Offset: 0x00021E6C
		// (set) Token: 0x06002E53 RID: 11859 RVA: 0x00023C73 File Offset: 0x00021E73
		[ClientVar(Saved = true, Help = "Enable environment culling")]
		public static bool env
		{
			get
			{
				return Culling._env;
			}
			set
			{
				bool flag = value != Culling._env;
				Culling._env = value;
				if (flag)
				{
					LODComponent.ChangeCullingAll();
				}
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06002E54 RID: 11860 RVA: 0x00023C8D File Offset: 0x00021E8D
		// (set) Token: 0x06002E55 RID: 11861 RVA: 0x00023CAF File Offset: 0x00021EAF
		[ClientVar(ClientAdmin = true, Help = "Debug occludees 0=off, 1=dynamic, 2=static, 4=grid, 7=all (green:visible, red:culled)")]
		public static int debug
		{
			get
			{
				if (!(LocalPlayer.Entity != null) || !LocalPlayer.Entity.IsDeveloper)
				{
					return 0;
				}
				return (int)OcclusionCulling.DebugShow;
			}
			set
			{
				OcclusionCulling.DebugShow = (OcclusionCulling.DebugFilter)value;
			}
		}
	}
}
