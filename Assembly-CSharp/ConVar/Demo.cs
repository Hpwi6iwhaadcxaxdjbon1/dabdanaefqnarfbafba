using System;
using System.IO;
using Network;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000853 RID: 2131
	public class Demo
	{
		// Token: 0x04002966 RID: 10598
		public static bool IsTimeDemo;

		// Token: 0x04002967 RID: 10599
		[ClientVar]
		public static float timescale = 1f;

		// Token: 0x06002E78 RID: 11896 RVA: 0x000E7F80 File Offset: 0x000E6180
		[ClientVar(ClientAdmin = true)]
		public static string record(string filename)
		{
			if (Net.cl == null || !Net.cl.IsConnected())
			{
				return "Not connected to a server";
			}
			if (!Directory.Exists("demos"))
			{
				Directory.CreateDirectory("demos");
			}
			if (Net.cl.IsRecording)
			{
				return "You're already recording a demo";
			}
			DemoHeader demoHeader = new DemoHeader
			{
				version = 1U,
				level = Application.loadedLevelName,
				levelSeed = World.Seed,
				levelSize = World.Size,
				checksum = World.Checksum,
				localclient = Client.Steam.SteamId,
				position = MainCamera.position,
				rotation = MainCamera.forward,
				levelUrl = World.Url
			};
			string text = "demos/" + filename + ".dem";
			Net.cl.StartRecording(text, demoHeader.ToProtoBytes());
			ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet(), "server.snapshot", Array.Empty<object>());
			return "Recording to " + text;
		}

		// Token: 0x06002E79 RID: 11897 RVA: 0x00023D76 File Offset: 0x00021F76
		[ClientVar]
		public static string stop()
		{
			if (Client.IsPlayingDemo)
			{
				SingletonComponent<Client>.Instance.StopPlayingDemo(false);
				return "Stopping demo playing";
			}
			if (Net.cl.IsRecording)
			{
				Net.cl.StopRecording();
				return "Stopping demo recording";
			}
			return null;
		}

		// Token: 0x06002E7A RID: 11898 RVA: 0x000E8084 File Offset: 0x000E6284
		[ClientVar]
		public static string play(string filename)
		{
			if (Net.cl.IsConnected())
			{
				return "Disconnect before playing a demo";
			}
			if (Client.IsPlayingDemo)
			{
				return "Already playing a demo";
			}
			if (Net.cl.IsRecording)
			{
				return "Can't' play when recording a demo";
			}
			string text = "demos/" + filename + ".dem";
			if (!File.Exists(text))
			{
				text = Application.streamingAssetsPath + "/" + filename + ".dem";
			}
			byte[] array = Net.cl.StartPlayback(text);
			if (array == null)
			{
				return "Can't play demo " + text;
			}
			DemoHeader demoHeader = DemoHeader.Deserialize(array);
			if (demoHeader == null)
			{
				Net.cl.StopPlayback();
				return "Error reading demo header";
			}
			Demo.IsTimeDemo = false;
			Time.timeScale = Demo.timescale;
			SingletonComponent<Client>.Instance.StartPlayingDemo(text, demoHeader);
			return "Playing " + text;
		}

		// Token: 0x06002E7B RID: 11899 RVA: 0x00023DAD File Offset: 0x00021FAD
		[ClientVar]
		public static string timedemo(string filename)
		{
			if (Net.cl.IsConnected())
			{
				return "Disconnect before playing a demo";
			}
			string result = Demo.play(filename);
			Demo.IsTimeDemo = true;
			Time.timeScale = 1f;
			return result;
		}
	}
}
