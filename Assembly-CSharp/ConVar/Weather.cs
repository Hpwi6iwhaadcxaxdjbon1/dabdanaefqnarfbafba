using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x0200088D RID: 2189
	[ConsoleSystem.Factory("weather")]
	public class Weather : ConsoleSystem
	{
		// Token: 0x06002F7A RID: 12154 RVA: 0x000E9E08 File Offset: 0x000E8008
		[ClientVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			float clouds = Climate.GetClouds(MainCamera.mainCamera.transform.position);
			float fog = Climate.GetFog(MainCamera.mainCamera.transform.position);
			float wind = Climate.GetWind(MainCamera.mainCamera.transform.position);
			float rain = Climate.GetRain(MainCamera.mainCamera.transform.position);
			string text = string.Empty;
			text = string.Concat(new object[]
			{
				text,
				"Clouds: ",
				Mathf.RoundToInt(100f * clouds),
				"%"
			});
			text += Environment.NewLine;
			text = string.Concat(new object[]
			{
				text,
				"Fog:    ",
				Mathf.RoundToInt(100f * fog),
				"%"
			});
			text += Environment.NewLine;
			text = string.Concat(new object[]
			{
				text,
				"Wind:   ",
				Mathf.RoundToInt(100f * wind),
				"%"
			});
			text += Environment.NewLine;
			text = string.Concat(new object[]
			{
				text,
				"Rain:   ",
				Mathf.RoundToInt(100f * rain),
				"%"
			});
			args.ReplyWith(text);
		}
	}
}
