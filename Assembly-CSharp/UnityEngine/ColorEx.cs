using System;

namespace UnityEngine
{
	// Token: 0x0200082E RID: 2094
	public static class ColorEx
	{
		// Token: 0x06002D76 RID: 11638 RVA: 0x0002359F File Offset: 0x0002179F
		public static string ToHex(this Color32 color)
		{
			return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		}

		// Token: 0x06002D77 RID: 11639 RVA: 0x000E4940 File Offset: 0x000E2B40
		public static Color Parse(string str)
		{
			string[] array = str.Split(new char[]
			{
				' '
			});
			if (array.Length == 3)
			{
				return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
			}
			if (array.Length == 4)
			{
				return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
			}
			return Color.white;
		}
	}
}
