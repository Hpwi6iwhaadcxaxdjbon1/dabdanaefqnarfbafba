using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x0200086C RID: 2156
	[ConsoleSystem.Factory("layer")]
	public class Layer : ConsoleSystem
	{
		// Token: 0x06002F06 RID: 12038 RVA: 0x000E90E0 File Offset: 0x000E72E0
		[ClientVar]
		public static void show(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!entity)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			MainCamera.mainCamera.cullingMask |= LayerMask.GetMask(new string[]
			{
				@string
			});
		}

		// Token: 0x06002F07 RID: 12039 RVA: 0x000E9144 File Offset: 0x000E7344
		[ClientVar]
		public static void hide(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!entity)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			MainCamera.mainCamera.cullingMask &= ~LayerMask.GetMask(new string[]
			{
				@string
			});
		}

		// Token: 0x06002F08 RID: 12040 RVA: 0x000E91A8 File Offset: 0x000E73A8
		[ClientVar]
		public static void toggle(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!entity)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			MainCamera.mainCamera.cullingMask ^= LayerMask.GetMask(new string[]
			{
				@string
			});
		}

		// Token: 0x06002F09 RID: 12041 RVA: 0x000E920C File Offset: 0x000E740C
		[ClientVar]
		public static void culling(ConsoleSystem.Arg arg)
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!entity)
			{
				return;
			}
			if (!entity.IsAdmin && !entity.IsDeveloper)
			{
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			float @float = arg.GetFloat(1, -1f);
			if (@float < 0f)
			{
				return;
			}
			int num = LayerMask.NameToLayer(@string);
			if (num < 0)
			{
				return;
			}
			float[] layerCullDistances = MainCamera.mainCamera.layerCullDistances;
			layerCullDistances[num] = @float;
			MainCamera.mainCamera.layerCullDistances = layerCullDistances;
		}
	}
}
