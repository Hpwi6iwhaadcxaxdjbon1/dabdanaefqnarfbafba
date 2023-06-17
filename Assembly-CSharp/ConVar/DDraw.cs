using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x0200084E RID: 2126
	public class DDraw
	{
		// Token: 0x06002E5A RID: 11866 RVA: 0x000E7A90 File Offset: 0x000E5C90
		[ClientVar(AllowRunFromServer = true, ClientAdmin = true)]
		public static void line(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Color color = arg.GetColor(1, Color.white);
			Vector3 vector = arg.GetVector3(2, Vector3.zero);
			Vector3 vector2 = arg.GetVector3(3, Vector3.zero);
			DDraw.Line(vector, vector2, color, @float, true, true);
		}

		// Token: 0x06002E5B RID: 11867 RVA: 0x000E7ADC File Offset: 0x000E5CDC
		[ClientVar(AllowRunFromServer = true, ClientAdmin = true)]
		public static void arrow(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Color color = arg.GetColor(1, Color.white);
			Vector3 vector = arg.GetVector3(2, Vector3.zero);
			Vector3 vector2 = arg.GetVector3(3, Vector3.zero);
			float float2 = arg.GetFloat(4, 0f);
			DDraw.Arrow(vector, vector2, float2, color, @float);
		}

		// Token: 0x06002E5C RID: 11868 RVA: 0x000E7B34 File Offset: 0x000E5D34
		[ClientVar(AllowRunFromServer = true, ClientAdmin = true)]
		public static void sphere(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Color color = arg.GetColor(1, Color.white);
			Vector3 vector = arg.GetVector3(2, Vector3.zero);
			float float2 = arg.GetFloat(3, 0f);
			DDraw.Sphere(vector, float2, color, @float, true);
		}

		// Token: 0x06002E5D RID: 11869 RVA: 0x000E7B80 File Offset: 0x000E5D80
		[ClientVar(AllowRunFromServer = true, ClientAdmin = true)]
		public static void text(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Color color = arg.GetColor(1, Color.white);
			Vector3 vector = arg.GetVector3(2, Vector3.zero);
			DDraw.Text(arg.GetString(3, ""), vector, color, @float);
		}

		// Token: 0x06002E5E RID: 11870 RVA: 0x000E7BC8 File Offset: 0x000E5DC8
		[ClientVar(AllowRunFromServer = true, ClientAdmin = true)]
		public static void box(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Color color = arg.GetColor(1, Color.white);
			Vector3 vector = arg.GetVector3(2, Vector3.zero);
			float float2 = arg.GetFloat(3, 0.1f);
			DDraw.Box(vector, float2, color, @float, true);
		}
	}
}
