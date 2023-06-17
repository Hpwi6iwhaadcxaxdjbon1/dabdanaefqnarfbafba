using System;
using System.Collections.Generic;
using Facepunch.Math;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000892 RID: 2194
	public static class Output
	{
		// Token: 0x04002A40 RID: 10816
		public static bool installed = false;

		// Token: 0x04002A41 RID: 10817
		public static List<Output.Entry> HistoryOutput = new List<Output.Entry>();

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06002F84 RID: 12164 RVA: 0x000EA060 File Offset: 0x000E8260
		// (remove) Token: 0x06002F85 RID: 12165 RVA: 0x000EA094 File Offset: 0x000E8294
		public static event Action<string, string, LogType> OnMessage;

		// Token: 0x06002F86 RID: 12166 RVA: 0x000247F0 File Offset: 0x000229F0
		public static void Install()
		{
			if (Output.installed)
			{
				return;
			}
			Application.logMessageReceived += Output.LogHandler;
			Output.installed = true;
		}

		// Token: 0x06002F87 RID: 12167 RVA: 0x000EA0C8 File Offset: 0x000E82C8
		internal static void LogHandler(string log, string stacktrace, LogType type)
		{
			if (Output.OnMessage == null)
			{
				return;
			}
			if (log.StartsWith("[Physics.PhysX] RigidBody::setRigidBodyFlag"))
			{
				return;
			}
			if (log.StartsWith("Skipped frame because GfxDevice"))
			{
				return;
			}
			if (log.StartsWith("Your current multi-scene setup has inconsistent Lighting"))
			{
				return;
			}
			if (log.Contains("HandleD3DDeviceLost"))
			{
				return;
			}
			if (log.Contains("ResetD3DDevice"))
			{
				return;
			}
			if (log.Contains("dev->Reset"))
			{
				return;
			}
			if (log.Contains("D3Dwindow device not lost anymore"))
			{
				return;
			}
			if (log.Contains("D3D device reset"))
			{
				return;
			}
			if (log.Contains("group < 0xfff"))
			{
				return;
			}
			if (log.Contains("Mesh can not have more than 65000 vert"))
			{
				return;
			}
			if (log.Contains("Trying to add (Layout Rebuilder for)"))
			{
				return;
			}
			if (log.Contains("Coroutine continue failure"))
			{
				return;
			}
			if (log.Contains("No texture data available to upload"))
			{
				return;
			}
			if (log.Contains("Trying to reload asset from disk that is not"))
			{
				return;
			}
			if (log.Contains("Unable to find shaders used for the terrain engine."))
			{
				return;
			}
			if (log.Contains("Canvas element contains more than 65535 vertices"))
			{
				return;
			}
			if (log.Contains("RectTransform.set_anchorMin"))
			{
				return;
			}
			if (log.Contains("FMOD failed to initialize the output device"))
			{
				return;
			}
			if (log.Contains("Cannot create FMOD::Sound"))
			{
				return;
			}
			if (log.Contains("invalid utf-16 sequence"))
			{
				return;
			}
			if (log.Contains("missing surrogate tail"))
			{
				return;
			}
			if (log.Contains("Failed to create agent because it is not close enough to the Nav"))
			{
				return;
			}
			if (log.Contains("user-provided triangle mesh descriptor is invalid"))
			{
				return;
			}
			if (log.Contains("Releasing render texture that is set as"))
			{
				return;
			}
			Output.OnMessage.Invoke(log, stacktrace, type);
			Output.HistoryOutput.Add(new Output.Entry
			{
				Message = log,
				Stacktrace = stacktrace,
				Type = type.ToString(),
				Time = Epoch.Current
			});
			while (Output.HistoryOutput.Count > 65536)
			{
				Output.HistoryOutput.RemoveAt(0);
			}
		}

		// Token: 0x02000893 RID: 2195
		public struct Entry
		{
			// Token: 0x04002A42 RID: 10818
			public string Message;

			// Token: 0x04002A43 RID: 10819
			public string Stacktrace;

			// Token: 0x04002A44 RID: 10820
			public string Type;

			// Token: 0x04002A45 RID: 10821
			public int Time;
		}
	}
}
