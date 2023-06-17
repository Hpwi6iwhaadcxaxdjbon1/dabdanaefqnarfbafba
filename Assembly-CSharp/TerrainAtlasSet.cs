using System;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
[CreateAssetMenu(menuName = "Rust/Terrain Atlas Set")]
public class TerrainAtlasSet : ScriptableObject
{
	// Token: 0x040018E7 RID: 6375
	public const int SplatCount = 8;

	// Token: 0x040018E8 RID: 6376
	public const int SplatSize = 2048;

	// Token: 0x040018E9 RID: 6377
	public const int MaxSplatSize = 2047;

	// Token: 0x040018EA RID: 6378
	public const int SplatPadding = 256;

	// Token: 0x040018EB RID: 6379
	public const int AtlasSize = 8192;

	// Token: 0x040018EC RID: 6380
	public const int RegionSize = 2560;

	// Token: 0x040018ED RID: 6381
	public const int SplatsPerLine = 3;

	// Token: 0x040018EE RID: 6382
	public const int SourceTypeCount = 4;

	// Token: 0x040018EF RID: 6383
	public const int AtlasMipCount = 10;

	// Token: 0x040018F0 RID: 6384
	public static string[] sourceTypeNames = new string[]
	{
		"Albedo",
		"Normal",
		"Specular",
		"Height"
	};

	// Token: 0x040018F1 RID: 6385
	public static string[] sourceTypeNamesExt = new string[]
	{
		"Albedo (rgb)",
		"Normal (rgb)",
		"Specular (rgba)",
		"Height (gray)"
	};

	// Token: 0x040018F2 RID: 6386
	public static string[] sourceTypePostfix = new string[]
	{
		"_albedo",
		"_normal",
		"_specular",
		"_height"
	};

	// Token: 0x040018F3 RID: 6387
	public string[] splatNames;

	// Token: 0x040018F4 RID: 6388
	public bool[] albedoHighpass;

	// Token: 0x040018F5 RID: 6389
	public string[] albedoPaths;

	// Token: 0x040018F6 RID: 6390
	public Color[] defaultValues;

	// Token: 0x040018F7 RID: 6391
	public TerrainAtlasSet.SourceMapSet[] sourceMaps;

	// Token: 0x040018F8 RID: 6392
	public bool highQualityCompression = true;

	// Token: 0x040018F9 RID: 6393
	public bool generateTextureAtlases = true;

	// Token: 0x040018FA RID: 6394
	public bool generateTextureArrays;

	// Token: 0x040018FB RID: 6395
	public string splatSearchPrefix = "terrain_";

	// Token: 0x040018FC RID: 6396
	public string splatSearchFolder = "Assets/Content/Nature/Terrain";

	// Token: 0x040018FD RID: 6397
	public string albedoAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_atlas";

	// Token: 0x040018FE RID: 6398
	public string normalAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_atlas";

	// Token: 0x040018FF RID: 6399
	public string albedoArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_array";

	// Token: 0x04001900 RID: 6400
	public string normalArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_array";

	// Token: 0x06001BFB RID: 7163 RVA: 0x0009A544 File Offset: 0x00098744
	public void CheckReset()
	{
		if (this.splatNames == null)
		{
			this.splatNames = new string[]
			{
				"Dirt",
				"Snow",
				"Sand",
				"Rock",
				"Grass",
				"Forest",
				"Stones",
				"Gravel"
			};
		}
		else if (this.splatNames.Length != 8)
		{
			Array.Resize<string>(ref this.splatNames, 8);
		}
		if (this.albedoHighpass == null)
		{
			this.albedoHighpass = new bool[8];
		}
		else if (this.albedoHighpass.Length != 8)
		{
			Array.Resize<bool>(ref this.albedoHighpass, 8);
		}
		if (this.albedoPaths == null)
		{
			this.albedoPaths = new string[8];
		}
		else if (this.albedoPaths.Length != 8)
		{
			Array.Resize<string>(ref this.albedoPaths, 8);
		}
		if (this.defaultValues == null)
		{
			this.defaultValues = new Color[]
			{
				new Color(1f, 1f, 1f, 0.5f),
				new Color(0.5f, 0.5f, 1f, 0f),
				new Color(0.5f, 0.5f, 0.5f, 0.5f),
				Color.black
			};
		}
		else if (this.defaultValues.Length != 4)
		{
			Array.Resize<Color>(ref this.defaultValues, 4);
		}
		if (this.sourceMaps == null)
		{
			this.sourceMaps = new TerrainAtlasSet.SourceMapSet[4];
		}
		else if (this.sourceMaps.Length != 4)
		{
			Array.Resize<TerrainAtlasSet.SourceMapSet>(ref this.sourceMaps, 4);
		}
		for (int i = 0; i < 4; i++)
		{
			this.sourceMaps[i] = ((this.sourceMaps[i] != null) ? this.sourceMaps[i] : new TerrainAtlasSet.SourceMapSet());
			this.sourceMaps[i].CheckReset();
		}
	}

	// Token: 0x020004C1 RID: 1217
	public enum SourceType
	{
		// Token: 0x04001902 RID: 6402
		ALBEDO,
		// Token: 0x04001903 RID: 6403
		NORMAL,
		// Token: 0x04001904 RID: 6404
		SPECULAR,
		// Token: 0x04001905 RID: 6405
		HEIGHT,
		// Token: 0x04001906 RID: 6406
		COUNT
	}

	// Token: 0x020004C2 RID: 1218
	[Serializable]
	public class SourceMapSet
	{
		// Token: 0x04001907 RID: 6407
		public Texture2D[] maps;

		// Token: 0x06001BFE RID: 7166 RVA: 0x00016E81 File Offset: 0x00015081
		internal void CheckReset()
		{
			if (this.maps == null)
			{
				this.maps = new Texture2D[8];
				return;
			}
			if (this.maps.Length != 8)
			{
				Array.Resize<Texture2D>(ref this.maps, 8);
			}
		}
	}
}
