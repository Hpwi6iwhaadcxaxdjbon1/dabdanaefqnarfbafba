using System;
using Facepunch;

namespace ConVar
{
	// Token: 0x0200086E RID: 2158
	public class Manifest
	{
		// Token: 0x06002F0F RID: 12047 RVA: 0x000242FD File Offset: 0x000224FD
		[ClientVar]
		[ServerVar]
		public static object PrintManifest()
		{
			return Application.Manifest;
		}

		// Token: 0x06002F10 RID: 12048 RVA: 0x00024304 File Offset: 0x00022504
		[ServerVar]
		[ClientVar]
		public static object PrintManifestRaw()
		{
			return Manifest.Contents;
		}
	}
}
