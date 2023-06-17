using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020004EE RID: 1262
public class TerrainQuality : TerrainExtension
{
	// Token: 0x040019B2 RID: 6578
	internal float HeightMapError;

	// Token: 0x040019B3 RID: 6579
	internal float BaseMapDistance;

	// Token: 0x040019B4 RID: 6580
	internal int TerrainShaderLod;

	// Token: 0x040019B5 RID: 6581
	private ConsoleSystem.Command terrain_quality;

	// Token: 0x040019B6 RID: 6582
	private ConsoleSystem.Command graphics_shaderlod;

	// Token: 0x06001D5D RID: 7517 RVA: 0x000178E5 File Offset: 0x00015AE5
	public override void Setup()
	{
		if (this.terrain_quality == null)
		{
			this.FindCommand();
		}
		this.TerrainQuality_OnValueChanged(this.terrain_quality);
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x00017901 File Offset: 0x00015B01
	protected void OnEnable()
	{
		if (this.terrain_quality == null)
		{
			this.FindCommand();
		}
		this.terrain_quality.OnValueChanged += new Action<ConsoleSystem.Command>(this.TerrainQuality_OnValueChanged);
		this.graphics_shaderlod.OnValueChanged += new Action<ConsoleSystem.Command>(this.TerrainQuality_OnValueChanged);
	}

	// Token: 0x06001D5F RID: 7519 RVA: 0x000A02B4 File Offset: 0x0009E4B4
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.terrain_quality == null)
		{
			this.FindCommand();
		}
		this.terrain_quality.OnValueChanged -= new Action<ConsoleSystem.Command>(this.TerrainQuality_OnValueChanged);
		this.graphics_shaderlod.OnValueChanged -= new Action<ConsoleSystem.Command>(this.TerrainQuality_OnValueChanged);
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x0001793F File Offset: 0x00015B3F
	private void FindCommand()
	{
		this.terrain_quality = ConsoleSystem.Index.Client.Find("terrain.quality");
		this.graphics_shaderlod = ConsoleSystem.Index.Client.Find("graphics.shaderlod");
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x000A0308 File Offset: 0x0009E508
	private void TerrainQuality_OnValueChanged(ConsoleSystem.Command obj)
	{
		if (this.config == null)
		{
			return;
		}
		float quality = Terrain.quality;
		this.HeightMapError = (float)Mathf.FloorToInt(Mathf.Lerp(this.config.HeightMapErrorMax, this.config.HeightMapErrorMin, quality / 100f));
		this.BaseMapDistance = (float)Mathf.FloorToInt(Mathf.Lerp(this.config.BaseMapDistanceMin, this.config.BaseMapDistanceMax, quality / 100f));
		this.TerrainShaderLod = Mathf.FloorToInt(Mathf.Lerp(this.config.ShaderLodMin, this.config.ShaderLodMax, quality / 100f));
		this.terrain.heightmapPixelError = this.HeightMapError;
		this.terrain.basemapDistance = this.BaseMapDistance;
	}
}
