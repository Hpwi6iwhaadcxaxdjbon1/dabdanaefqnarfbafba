using System;
using System.Runtime.InteropServices;
using Facepunch;
using UnityEngine;

// Token: 0x02000366 RID: 870
public static class SelfCheck
{
	// Token: 0x06001660 RID: 5728 RVA: 0x00086B64 File Offset: 0x00084D64
	public static bool Run()
	{
		if (FileSystem.Backend.isError)
		{
			return SelfCheck.Failed("Asset Bundle Error: " + FileSystem.Backend.loadingError);
		}
		if (FileSystem.Load<GameManifest>("Assets/manifest.asset", true) == null)
		{
			return SelfCheck.Failed("Couldn't load game manifest - verify your game content!");
		}
		if (!SelfCheck.TestRustNative())
		{
			return false;
		}
		if (CommandLine.HasSwitch("-force-feature-level-9-3"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-9-3");
		}
		if (CommandLine.HasSwitch("-force-feature-level-10-0"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-0");
		}
		return !CommandLine.HasSwitch("-force-feature-level-10-1") || SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-1");
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x00012DCF File Offset: 0x00010FCF
	private static bool Failed(string Message)
	{
		if (SingletonComponent<Bootstrap>.Instance)
		{
			SingletonComponent<Bootstrap>.Instance.messageString = "";
			SingletonComponent<Bootstrap>.Instance.ThrowError(Message);
		}
		Debug.LogError("SelfCheck Failed: " + Message);
		return false;
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x00086C04 File Offset: 0x00084E04
	private static bool TestRustNative()
	{
		try
		{
			if (!SelfCheck.RustNative_VersionCheck(5))
			{
				return SelfCheck.Failed("RustNative is wrong version!");
			}
		}
		catch (DllNotFoundException ex)
		{
			return SelfCheck.Failed("RustNative library couldn't load! " + ex.Message);
		}
		return true;
	}

	// Token: 0x06001663 RID: 5731
	[DllImport("RustNative")]
	private static extern bool RustNative_VersionCheck(int version);
}
