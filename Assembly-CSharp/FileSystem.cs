using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

// Token: 0x02000292 RID: 658
public static class FileSystem
{
	// Token: 0x04000F42 RID: 3906
	public static bool LogDebug;

	// Token: 0x04000F43 RID: 3907
	public static bool LogTime;

	// Token: 0x04000F44 RID: 3908
	public static FileSystemBackend Backend;

	// Token: 0x0600129A RID: 4762 RVA: 0x0000FFA9 File Offset: 0x0000E1A9
	public static GameObject[] LoadPrefabs(string folder)
	{
		return FileSystem.Backend.LoadPrefabs(folder);
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x0000FFB6 File Offset: 0x0000E1B6
	public static GameObject LoadPrefab(string filePath)
	{
		return FileSystem.Backend.LoadPrefab(filePath);
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x0000FFC3 File Offset: 0x0000E1C3
	public static string[] FindAll(string folder, string search = "")
	{
		return FileSystem.Backend.FindAll(folder, search);
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x0000FFD1 File Offset: 0x0000E1D1
	public static T[] LoadAll<T>(string folder, string search = "") where T : Object
	{
		if (!StringEx.IsLower(folder))
		{
			folder = folder.ToLower();
		}
		return FileSystem.Backend.LoadAll<T>(folder, search);
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x0007945C File Offset: 0x0007765C
	public static T Load<T>(string filePath, bool complain = true) where T : Object
	{
		if (!StringEx.IsLower(filePath))
		{
			filePath = filePath.ToLower();
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		if (FileSystem.LogDebug)
		{
			File.AppendAllText("filesystem_debug.csv", string.Format("{0}\n", filePath));
		}
		T t = FileSystem.Backend.Load<T>(filePath);
		if (complain && t == null)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"[FileSystem] Not Found: ",
				filePath,
				" (",
				typeof(T),
				")"
			}));
		}
		if (FileSystem.LogTime)
		{
			File.AppendAllText("filesystem.csv", string.Format("{0},{1}\n", filePath, stopwatch.Elapsed.TotalMilliseconds));
		}
		return t;
	}
}
