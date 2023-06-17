using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000505 RID: 1285
public class GenerateHeight : ProceduralComponent
{
	// Token: 0x06001DAC RID: 7596
	[DllImport("RustNative", EntryPoint = "generate_height")]
	public static extern void Native_GenerateHeight(short[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle);

	// Token: 0x06001DAD RID: 7597 RVA: 0x000A1988 File Offset: 0x0009FB88
	public override void Process(uint seed)
	{
		short[] dst = TerrainMeta.HeightMap.dst;
		int res = TerrainMeta.HeightMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		GenerateHeight.Native_GenerateHeight(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle);
	}
}
