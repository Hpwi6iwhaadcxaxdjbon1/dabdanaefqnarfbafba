using System;
using System.Threading;
using Facepunch.Unity;
using UnityEngine;

namespace ConVar
{
	// Token: 0x0200084F RID: 2127
	[ConsoleSystem.Factory("debug")]
	public class Debugging : ConsoleSystem
	{
		// Token: 0x0400295C RID: 10588
		[ClientVar]
		[ServerVar]
		public static bool checktriggers = false;

		// Token: 0x0400295D RID: 10589
		[ClientVar]
		[ServerVar]
		public static bool callbacks = false;

		// Token: 0x0400295E RID: 10590
		[ClientVar(Help = "Draw colliders")]
		public static bool drawcolliders = false;

		// Token: 0x0400295F RID: 10591
		[ClientVar(Help = "Whether or not to update ambient light from environment volumes")]
		public static bool ambientvolumes = true;

		// Token: 0x04002960 RID: 10592
		[ClientVar]
		public static bool oninventorychanged = true;

		// Token: 0x06002E60 RID: 11872 RVA: 0x00023CB7 File Offset: 0x00021EB7
		[ClientVar]
		[ServerVar]
		public static void renderinfo(ConsoleSystem.Arg arg)
		{
			RenderInfo.GenerateReport();
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06002E62 RID: 11874 RVA: 0x00023CCB File Offset: 0x00021ECB
		// (set) Token: 0x06002E61 RID: 11873 RVA: 0x00023CBE File Offset: 0x00021EBE
		[ServerVar]
		[ClientVar]
		public static bool log
		{
			get
			{
				return Debug.unityLogger.logEnabled;
			}
			set
			{
				Debug.unityLogger.logEnabled = value;
			}
		}

		// Token: 0x06002E63 RID: 11875 RVA: 0x000E7C14 File Offset: 0x000E5E14
		[ServerVar]
		[ClientVar]
		public static void stall(ConsoleSystem.Arg arg)
		{
			float num = Mathf.Clamp(arg.GetFloat(0, 0f), 0f, 1f);
			arg.ReplyWith("Stalling for " + num + " seconds...");
			Thread.Sleep(Mathf.RoundToInt(num * 1000f));
		}

		// Token: 0x06002E64 RID: 11876 RVA: 0x000E7C6C File Offset: 0x000E5E6C
		[ClientVar(Help = "Print info about what the player is currently looking at", AllowRunFromServer = true)]
		public static void lookingat(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (entity == null)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			Debug.Log("Looking At: " + entity.lookingAt);
			Debug.Log("       Ent: " + entity.lookingAtEntity);
			Debug.Log("       Pos: " + entity.lookingAtPoint);
			Debug.Log("     Layer: " + (entity.lookingAt ? LayerMask.LayerToName(entity.lookingAt.layer) : ""));
			DDraw.Box(entity.lookingAtPoint, 0.1f, Color.yellow, 5f, true);
			DDraw.Text("ent: " + entity.lookingAtEntity, entity.lookingAtPoint, Color.yellow, 5f);
		}

		// Token: 0x06002E65 RID: 11877 RVA: 0x000E7D54 File Offset: 0x000E5F54
		[ClientVar(Help = "Enable debug mode on the entity we're currently looking at", AllowRunFromServer = true)]
		public static void lookingat_debug(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (entity == null)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "entity.debug_toggle", new object[]
			{
				LocalPlayer.Entity.lookingAtEntity.net.ID
			});
		}

		// Token: 0x06002E66 RID: 11878 RVA: 0x000E7DB4 File Offset: 0x000E5FB4
		[ClientVar(Help = "Enable debug camera mode", AllowRunFromServer = true)]
		public static void debugcamera(ConsoleSystem.Arg arg)
		{
			if (!Client.IsPlayingDemo)
			{
				BasePlayer entity = LocalPlayer.Entity;
				if (entity == null)
				{
					return;
				}
				if (!entity.IsAdmin && !entity.IsDeveloper)
				{
					return;
				}
			}
			if (!SingletonComponent<CameraMan>.Instance)
			{
				GameManager.client.CreatePrefab("assets/bundled/prefabs/system/debug/debug_camera.prefab", true);
			}
			else
			{
				SingletonComponent<CameraMan>.Instance.enabled = !SingletonComponent<CameraMan>.Instance.enabled;
			}
			if (!Client.IsPlayingDemo)
			{
				LocalPlayer.Entity.OnViewModeChanged();
			}
		}

		// Token: 0x06002E67 RID: 11879 RVA: 0x000E7E30 File Offset: 0x000E6030
		[ClientVar(Help = "Toggle admin no clipping", AllowRunFromServer = true)]
		public static void noclip(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (entity == null)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			entity.movement.adminCheat = !entity.movement.adminCheat;
			if (TerrainMeta.Collision)
			{
				TerrainMeta.Collision.Reset(entity.movement.GetCollider());
			}
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x000E7E98 File Offset: 0x000E6098
		[ClientVar(Help = "Unfreeze the player when in debug camera mode", AllowRunFromServer = true)]
		public static void debugcamera_unfreeze(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (entity == null)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			SingletonComponent<CameraMan>.Instance.TogglePlayerFreeze();
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06002E69 RID: 11881 RVA: 0x000E7ED0 File Offset: 0x000E60D0
		// (set) Token: 0x06002E6A RID: 11882 RVA: 0x000E7EFC File Offset: 0x000E60FC
		[ClientVar(Help = "Whether or not to update the sky reflection probe")]
		public static bool skyreflection
		{
			get
			{
				TOD_Sky instance = TOD_Sky.Instance;
				return instance && instance.Reflection.Mode == 1;
			}
			set
			{
				BasePlayer entity = LocalPlayer.Entity;
				if (entity == null)
				{
					return;
				}
				if (!entity.IsAdmin && !entity.IsDeveloper)
				{
					return;
				}
				TOD_Sky instance = TOD_Sky.Instance;
				if (!instance)
				{
					return;
				}
				instance.Reflection.Mode = (value ? 1 : 0);
			}
		}

		// Token: 0x06002E6B RID: 11883 RVA: 0x000E7F4C File Offset: 0x000E614C
		[ClientVar(Help = "Print the current morph cache memory footprint.")]
		public static void morphCacheMem(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith((MorphCache.TotalMemoryFootprint / 1024L).ToString() + " KB");
		}
	}
}
