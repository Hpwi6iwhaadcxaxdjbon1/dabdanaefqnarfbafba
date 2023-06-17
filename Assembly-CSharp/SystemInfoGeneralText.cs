using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000254 RID: 596
public class SystemInfoGeneralText : MonoBehaviour
{
	// Token: 0x04000E5D RID: 3677
	public Text text;

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060011AD RID: 4525 RVA: 0x000750DC File Offset: 0x000732DC
	public static string currentInfo
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("System");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tName: ");
			stringBuilder.Append(SystemInfo.deviceName);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tOS:   ");
			stringBuilder.Append(SystemInfo.operatingSystem);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("CPU");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tModel:  ");
			stringBuilder.Append(SystemInfo.processorType);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tCores:  ");
			stringBuilder.Append(SystemInfo.processorCount);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory: ");
			stringBuilder.Append(SystemInfo.systemMemorySize);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("GPU");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tModel:  ");
			stringBuilder.Append(SystemInfo.graphicsDeviceName);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tAPI:    ");
			stringBuilder.Append(SystemInfo.graphicsDeviceVersion);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory: ");
			stringBuilder.Append(SystemInfo.graphicsMemorySize);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tSM:     ");
			stringBuilder.Append(SystemInfo.graphicsShaderLevel);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("Process");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory:   ");
			stringBuilder.Append(SystemInfoEx.systemMemoryUsed);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("Mono");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tCollects: ");
			stringBuilder.Append(GC.CollectionCount(0));
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory:   ");
			stringBuilder.Append(SystemInfoGeneralText.MB(GC.GetTotalMemory(false)));
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			if (World.Seed > 0U && World.Size > 0U)
			{
				stringBuilder.Append("World");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSeed:        ");
				stringBuilder.Append(World.Seed);
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSize:        ");
				stringBuilder.Append(SystemInfoGeneralText.KM2(World.Size));
				stringBuilder.Append(" km²");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tHeightmap:   ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tWatermap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.WaterMap ? TerrainMeta.WaterMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSplatmap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.SplatMap ? TerrainMeta.SplatMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tBiomemap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tTopologymap: ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tAlphamap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.AlphaMap ? TerrainMeta.AlphaMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			if (!string.IsNullOrEmpty(World.Checksum))
			{
				stringBuilder.AppendLine("Checksum");
				stringBuilder.Append('\t');
				stringBuilder.AppendLine(World.Checksum);
			}
			return stringBuilder.ToString();
		}
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x0000F762 File Offset: 0x0000D962
	protected void Update()
	{
		this.text.text = SystemInfoGeneralText.currentInfo;
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0000F774 File Offset: 0x0000D974
	private static long MB(long bytes)
	{
		return bytes / 1048576L;
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0000F77E File Offset: 0x0000D97E
	private static long MB(ulong bytes)
	{
		return SystemInfoGeneralText.MB((long)bytes);
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x0000F786 File Offset: 0x0000D986
	private static int KM2(float meters)
	{
		return Mathf.RoundToInt(meters * meters * 1E-06f);
	}
}
