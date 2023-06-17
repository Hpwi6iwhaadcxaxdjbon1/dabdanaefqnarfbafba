using System;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x020008A3 RID: 2211
	public static class Controls
	{
		// Token: 0x04002A86 RID: 10886
		public static float labelWidth = 100f;

		// Token: 0x06002FC5 RID: 12229 RVA: 0x000EB654 File Offset: 0x000E9854
		public static float FloatSlider(string strLabel, float value, float low, float high, string format = "0.00")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			float num = float.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			}));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			float result = GUILayout.HorizontalSlider(num, low, high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06002FC6 RID: 12230 RVA: 0x000EB6C8 File Offset: 0x000E98C8
		public static int IntSlider(string strLabel, int value, int low, int high, string format = "0")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			float num = (float)int.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			}));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			int result = (int)GUILayout.HorizontalSlider(num, (float)low, (float)high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06002FC7 RID: 12231 RVA: 0x00024A68 File Offset: 0x00022C68
		public static string TextArea(string strName, string value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			string result = GUILayout.TextArea(value, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06002FC8 RID: 12232 RVA: 0x00024A9D File Offset: 0x00022C9D
		public static bool Checkbox(string strName, bool value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			bool result = GUILayout.Toggle(value, "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06002FC9 RID: 12233 RVA: 0x00024AD7 File Offset: 0x00022CD7
		public static bool Button(string strName)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool result = GUILayout.Button(strName, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}
	}
}
