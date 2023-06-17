using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000844 RID: 2116
	[ConsoleSystem.Factory("batching")]
	public class Batching : ConsoleSystem
	{
		// Token: 0x04002931 RID: 10545
		[ServerVar]
		[ClientVar]
		public static bool colliders = false;

		// Token: 0x04002932 RID: 10546
		[ServerVar]
		[ClientVar]
		public static bool collider_threading = true;

		// Token: 0x04002933 RID: 10547
		[ServerVar]
		[ClientVar]
		public static int collider_capacity = 30000;

		// Token: 0x04002934 RID: 10548
		[ClientVar]
		[ServerVar]
		public static int collider_vertices = 1000;

		// Token: 0x04002935 RID: 10549
		[ClientVar]
		[ServerVar]
		public static int collider_submeshes = 1;

		// Token: 0x04002936 RID: 10550
		[ClientVar]
		public static bool renderers = true;

		// Token: 0x04002937 RID: 10551
		[ClientVar]
		public static bool renderer_threading = true;

		// Token: 0x04002938 RID: 10552
		[ClientVar]
		public static int renderer_capacity = 30000;

		// Token: 0x04002939 RID: 10553
		[ClientVar]
		public static int renderer_vertices = 1000;

		// Token: 0x0400293A RID: 10554
		[ClientVar]
		public static int renderer_submeshes = 1;

		// Token: 0x0400293B RID: 10555
		[ServerVar]
		[ClientVar]
		public static int verbose = 0;

		// Token: 0x06002E22 RID: 11810 RVA: 0x000E7198 File Offset: 0x000E5398
		[ClientVar]
		public static void refresh_renderers(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			RendererBatch[] array = Object.FindObjectsOfType<RendererBatch>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Refresh();
			}
			if (SingletonComponent<RendererGrid>.Instance)
			{
				SingletonComponent<RendererGrid>.Instance.Refresh();
			}
		}

		// Token: 0x06002E23 RID: 11811 RVA: 0x000E71F4 File Offset: 0x000E53F4
		[ClientVar]
		public static void print_renderers(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (SingletonComponent<RendererGrid>.Instance)
			{
				stringBuilder.AppendFormat("Mesh Renderer Batching: {0:N0}/{0:N0}", SingletonComponent<RendererGrid>.Instance.BatchedMeshCount(), SingletonComponent<RendererGrid>.Instance.MeshCount());
				stringBuilder.AppendLine();
			}
			args.ReplyWith(stringBuilder.ToString());
		}
	}
}
