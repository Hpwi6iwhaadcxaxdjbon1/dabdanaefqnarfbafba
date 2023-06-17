using System;
using Rust;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;

// Token: 0x0200036A RID: 874
public class Climate : SingletonComponent<Climate>
{
	// Token: 0x0400134D RID: 4941
	private const float fadeAngle = 20f;

	// Token: 0x0400134E RID: 4942
	private const float defaultTemp = 15f;

	// Token: 0x0400134F RID: 4943
	private const int weatherDurationHours = 18;

	// Token: 0x04001350 RID: 4944
	private const int weatherFadeHours = 6;

	// Token: 0x04001351 RID: 4945
	[Range(0f, 1f)]
	public float BlendingSpeed = 1f;

	// Token: 0x04001352 RID: 4946
	[Range(1f, 9f)]
	public float FogMultiplier = 5f;

	// Token: 0x04001353 RID: 4947
	public float FogDarknessDistance = 200f;

	// Token: 0x04001354 RID: 4948
	public bool DebugLUTBlending;

	// Token: 0x04001355 RID: 4949
	public Climate.WeatherParameters Weather;

	// Token: 0x04001356 RID: 4950
	public Climate.ClimateParameters Arid;

	// Token: 0x04001357 RID: 4951
	public Climate.ClimateParameters Temperate;

	// Token: 0x04001358 RID: 4952
	public Climate.ClimateParameters Tundra;

	// Token: 0x04001359 RID: 4953
	public Climate.ClimateParameters Arctic;

	// Token: 0x0400135A RID: 4954
	private Climate.ClimateParameters[] climates;

	// Token: 0x0400135B RID: 4955
	private Climate.WeatherState state = new Climate.WeatherState
	{
		Clouds = 0f,
		Fog = 0f,
		Wind = 0f,
		Rain = 0f
	};

	// Token: 0x0400135C RID: 4956
	private Climate.WeatherState clamps = new Climate.WeatherState
	{
		Clouds = -1f,
		Fog = -1f,
		Wind = -1f,
		Rain = -1f
	};

	// Token: 0x0400135D RID: 4957
	public Climate.WeatherState Overrides = new Climate.WeatherState
	{
		Clouds = -1f,
		Fog = -1f,
		Wind = -1f,
		Rain = -1f
	};

	// Token: 0x0400135E RID: 4958
	private Camera cam;

	// Token: 0x0400135F RID: 4959
	private TonemappingColorGrading tonemappingColorGrading;

	// Token: 0x04001360 RID: 4960
	private TOD_Scattering scattering;

	// Token: 0x04001361 RID: 4961
	private WindZone windZone;

	// Token: 0x04001362 RID: 4962
	private const int LUTSize = 16;

	// Token: 0x04001363 RID: 4963
	private ClimateBlendTexture lut;

	// Token: 0x04001364 RID: 4964
	private ClimateBlendTexture prevLut;

	// Token: 0x04001365 RID: 4965
	private Texture2D prevSrcLut1;

	// Token: 0x04001366 RID: 4966
	private Texture2D prevDstLut1;

	// Token: 0x04001367 RID: 4967
	private Texture2D prevSrcLut2;

	// Token: 0x04001368 RID: 4968
	private Texture2D prevDstLut2;

	// Token: 0x04001369 RID: 4969
	private float prevLerpLut1 = -1f;

	// Token: 0x0400136A RID: 4970
	private float prevLerpLut2 = -1f;

	// Token: 0x0400136B RID: 4971
	private float prevLerp = -1f;

	// Token: 0x0400136C RID: 4972
	private float cycleBlendTime;

	// Token: 0x0600167F RID: 5759 RVA: 0x00086E04 File Offset: 0x00085004
	protected void Start()
	{
		if (!(this.cam = MainCamera.mainCamera))
		{
			Debug.LogError("'Main Camera' not found.");
			base.enabled = false;
			return;
		}
		if (!(this.tonemappingColorGrading = this.cam.GetComponentInChildren<TonemappingColorGrading>()))
		{
			Debug.LogError("'Tonemapping Color Grading' not found.");
			base.enabled = false;
			return;
		}
		if (!(this.scattering = this.cam.GetComponent<TOD_Scattering>()))
		{
			Debug.LogError("'Scattering' not found.");
			base.enabled = false;
			return;
		}
		if (!(this.windZone = Object.FindObjectOfType<WindZone>()))
		{
			Debug.LogError("'Wind Zone' not found.");
			base.enabled = false;
			return;
		}
		this.lut = new ClimateBlendTexture(256, 16, true);
		this.prevLut = new ClimateBlendTexture(256, 16, true);
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x00012FF9 File Offset: 0x000111F9
	protected override void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		base.OnDestroy();
		if (this.lut != null)
		{
			this.lut.Dispose();
		}
		if (this.prevLut != null)
		{
			this.prevLut.Dispose();
		}
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x00086EE0 File Offset: 0x000850E0
	protected void Update()
	{
		if (!TerrainMeta.BiomeMap || !TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		long num = 36000000000L;
		long num2 = (long)((ulong)World.Seed + (ulong)instance.Cycle.Ticks);
		long num3 = 18L * num;
		long num4 = 6L * num;
		long num5 = num2 / num3;
		float t = Mathf.InverseLerp(0f, (float)num4, (float)(num2 % num3));
		Climate.WeatherState weatherState = this.GetWeatherState((uint)(num5 % (long)((ulong)-1)));
		Climate.WeatherState weatherState2 = this.GetWeatherState((uint)((num5 + 1L) % (long)((ulong)-1)));
		this.state = Climate.WeatherState.Fade(weatherState, weatherState2, t);
		this.state.Override(this.Overrides);
		Climate.ClimateParameters climateParameters;
		Climate.ClimateParameters climateParameters2;
		float num6 = this.FindBlendParameters(this.cam.transform.position, out climateParameters, out climateParameters2);
		float num7 = LoadingScreen.isOpen ? 1f : (this.BlendingSpeed * Time.deltaTime);
		if (this.lut.CheckLostData() || this.prevLut.CheckLostData())
		{
			num7 = 1f;
		}
		float a;
		float b;
		float t2 = climateParameters.FogDensity.FindBlendParameters(instance, out a, out b);
		this.clamps.Fog = Mathf.Lerp(this.clamps.Fog, Mathf.Lerp(a, b, t2), num7);
		float a2;
		float b2;
		float t3 = climateParameters.AerialDensity.FindBlendParameters(instance, out a2, out b2);
		float b3 = 0f;
		float num8 = Mathf.Lerp(a2, b2, t3);
		float num9 = Mathf.Lerp(1f, this.FogMultiplier, instance.Atmosphere.Fogginess);
		if (MainCamera.InEnvironment(EnvironmentType.Underground))
		{
			num8 = 0f;
		}
		if (instance.IsNight && instance.SunVisibility <= Mathf.Epsilon && instance.MoonVisibility <= Mathf.Epsilon)
		{
			b3 = this.FogDarknessDistance;
		}
		this.scattering.StartDistance = Mathf.Lerp(this.scattering.StartDistance, b3, num7);
		this.scattering.GlobalDensity = Mathf.Lerp(this.scattering.GlobalDensity, num9 * num8, num7);
		instance.Atmosphere.Fogginess = Climate.GetFog(this.cam.transform.position);
		instance.Clouds.Coverage = Climate.GetClouds(this.cam.transform.position);
		instance.Clouds.Opacity = Climate.GetCloudOpacity(this.cam.transform.position);
		this.windZone.windMain = Mathf.Lerp(0.2f, 0.5f, Climate.GetWind(this.cam.transform.position));
		Texture2D texture2D;
		Texture2D texture2D2;
		float num10 = climateParameters.LUT.FindBlendParameters(instance, out texture2D, out texture2D2);
		Texture2D texture2D3;
		Texture2D texture2D4;
		float num11 = climateParameters2.LUT.FindBlendParameters(instance, out texture2D3, out texture2D4);
		bool flag = this.prevSrcLut1 != texture2D || this.prevDstLut1 != texture2D2 || this.prevLerpLut1 != num10;
		bool flag2 = this.prevSrcLut2 != texture2D3 || this.prevDstLut2 != texture2D4 || this.prevLerpLut2 != num11;
		bool flag3 = this.prevLerp != num6;
		if (this.cycleBlendTime >= 1f && (flag || flag2 || flag3))
		{
			ClimateBlendTexture.Swap(ref this.lut, ref this.prevLut);
			this.prevSrcLut1 = texture2D;
			this.prevDstLut1 = texture2D2;
			this.prevSrcLut2 = texture2D3;
			this.prevDstLut2 = texture2D4;
			this.prevLerpLut1 = num10;
			this.prevLerpLut2 = num11;
			this.prevLerp = num6;
			this.cycleBlendTime = 0f;
		}
		else
		{
			this.cycleBlendTime += num7;
		}
		this.cycleBlendTime = Mathf.Min(1f, this.cycleBlendTime);
		this.lut.Blend(texture2D, texture2D2, num10, texture2D3, texture2D4, num11, num6, this.prevLut, this.cycleBlendTime);
		if (LoadingScreen.isOpen)
		{
			Graphics.CopyTexture(this.lut, this.prevLut);
		}
		TonemappingColorGrading.LUTSettings lutsettings = this.tonemappingColorGrading.lut;
		lutsettings.texture = this.lut;
		this.tonemappingColorGrading.lut = lutsettings;
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x0001302F File Offset: 0x0001122F
	public static float GetClouds(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Clouds, SingletonComponent<Climate>.Instance.state.Clouds);
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x00013066 File Offset: 0x00011266
	public static float GetCloudOpacity(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 1f;
		}
		return Mathf.InverseLerp(0.9f, 0.8f, Climate.GetFog(position));
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x0001308F File Offset: 0x0001128F
	public static float GetFog(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Fog, SingletonComponent<Climate>.Instance.state.Fog);
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x000130C6 File Offset: 0x000112C6
	public static float GetWind(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Wind, SingletonComponent<Climate>.Instance.state.Wind);
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x0008730C File Offset: 0x0008550C
	public static float GetRain(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		float t = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 1) : 0f;
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f;
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Rain, SingletonComponent<Climate>.Instance.state.Rain) * Mathf.Lerp(1f, 0.5f, t) * (1f - num);
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x000873A8 File Offset: 0x000855A8
	public static float GetSnow(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f;
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Rain, SingletonComponent<Climate>.Instance.state.Rain) * num;
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x0008740C File Offset: 0x0008560C
	public static float GetTemperature(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 15f;
		}
		if (!TOD_Sky.Instance)
		{
			return 15f;
		}
		Climate.ClimateParameters climateParameters;
		Climate.ClimateParameters climateParameters2;
		float t = SingletonComponent<Climate>.Instance.FindBlendParameters(position, out climateParameters, out climateParameters2);
		if (climateParameters == null || climateParameters2 == null)
		{
			return 15f;
		}
		float hour = TOD_Sky.Instance.Cycle.Hour;
		float a = climateParameters.Temperature.Evaluate(hour);
		float b = climateParameters2.Temperature.Evaluate(hour);
		return Mathf.Lerp(a, b, t);
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x0008748C File Offset: 0x0008568C
	private Climate.WeatherState GetWeatherState(uint seed)
	{
		SeedRandom.Wanghash(ref seed);
		bool flag = SeedRandom.Value(ref seed) < this.Weather.CloudChance;
		bool flag2 = SeedRandom.Value(ref seed) < this.Weather.FogChance;
		bool flag3 = SeedRandom.Value(ref seed) < this.Weather.RainChance;
		bool flag4 = SeedRandom.Value(ref seed) < this.Weather.StormChance;
		float num = flag ? SeedRandom.Value(ref seed) : 0f;
		float num2 = (float)(flag2 ? 1 : 0);
		float num3 = (float)(flag3 ? 1 : 0);
		float wind = flag4 ? SeedRandom.Value(ref seed) : 0f;
		if (num3 > 0f)
		{
			num3 = Mathf.Max(num3, 0.5f);
			num2 = Mathf.Max(num2, num3);
			num = Mathf.Max(num, num3);
		}
		return new Climate.WeatherState
		{
			Clouds = num,
			Fog = num2,
			Wind = wind,
			Rain = num3
		};
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x00087584 File Offset: 0x00085784
	private float FindBlendParameters(Vector3 pos, out Climate.ClimateParameters src, out Climate.ClimateParameters dst)
	{
		if (this.climates == null)
		{
			this.climates = new Climate.ClimateParameters[]
			{
				this.Arid,
				this.Temperate,
				this.Tundra,
				this.Arctic
			};
		}
		if (TerrainMeta.BiomeMap == null)
		{
			src = null;
			dst = null;
			return 0.5f;
		}
		int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
		int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
		src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		dst = this.climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
		return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
	}

	// Token: 0x0200036B RID: 875
	[Serializable]
	public class ClimateParameters
	{
		// Token: 0x0400136D RID: 4973
		public AnimationCurve Temperature;

		// Token: 0x0400136E RID: 4974
		[Horizontal(4, -1)]
		public Climate.Float4 AerialDensity;

		// Token: 0x0400136F RID: 4975
		[Horizontal(4, -1)]
		public Climate.Float4 FogDensity;

		// Token: 0x04001370 RID: 4976
		[Horizontal(4, -1)]
		public Climate.Texture2D4 LUT;
	}

	// Token: 0x0200036C RID: 876
	[Serializable]
	public class WeatherParameters
	{
		// Token: 0x04001371 RID: 4977
		[Range(0f, 1f)]
		public float RainChance = 0.5f;

		// Token: 0x04001372 RID: 4978
		[Range(0f, 1f)]
		public float FogChance = 0.5f;

		// Token: 0x04001373 RID: 4979
		[Range(0f, 1f)]
		public float CloudChance = 0.5f;

		// Token: 0x04001374 RID: 4980
		[Range(0f, 1f)]
		public float StormChance = 0.5f;
	}

	// Token: 0x0200036D RID: 877
	public struct WeatherState
	{
		// Token: 0x04001375 RID: 4981
		public float Clouds;

		// Token: 0x04001376 RID: 4982
		public float Fog;

		// Token: 0x04001377 RID: 4983
		public float Wind;

		// Token: 0x04001378 RID: 4984
		public float Rain;

		// Token: 0x0600168E RID: 5774 RVA: 0x0008773C File Offset: 0x0008593C
		public static Climate.WeatherState Fade(Climate.WeatherState a, Climate.WeatherState b, float t)
		{
			return new Climate.WeatherState
			{
				Clouds = Mathf.SmoothStep(a.Clouds, b.Clouds, t),
				Fog = Mathf.SmoothStep(a.Fog, b.Fog, t),
				Wind = Mathf.SmoothStep(a.Wind, b.Wind, t),
				Rain = Mathf.SmoothStep(a.Rain, b.Rain, t)
			};
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x000877B8 File Offset: 0x000859B8
		public void Override(Climate.WeatherState other)
		{
			if (other.Clouds >= 0f)
			{
				this.Clouds = Mathf.Clamp01(other.Clouds);
			}
			if (other.Fog >= 0f)
			{
				this.Fog = Mathf.Clamp01(other.Fog);
			}
			if (other.Wind >= 0f)
			{
				this.Wind = Mathf.Clamp01(other.Wind);
			}
			if (other.Rain >= 0f)
			{
				this.Rain = Mathf.Clamp01(other.Rain);
			}
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x00087840 File Offset: 0x00085A40
		public void Max(Climate.WeatherState other)
		{
			this.Clouds = Mathf.Max(this.Clouds, other.Clouds);
			this.Fog = Mathf.Max(this.Fog, other.Fog);
			this.Wind = Mathf.Max(this.Wind, other.Wind);
			this.Rain = Mathf.Max(this.Rain, other.Rain);
		}
	}

	// Token: 0x0200036E RID: 878
	public class Value4<T>
	{
		// Token: 0x04001379 RID: 4985
		public T Dawn;

		// Token: 0x0400137A RID: 4986
		public T Noon;

		// Token: 0x0400137B RID: 4987
		public T Dusk;

		// Token: 0x0400137C RID: 4988
		public T Night;

		// Token: 0x06001691 RID: 5777 RVA: 0x000878AC File Offset: 0x00085AAC
		public float FindBlendParameters(TOD_Sky sky, out T src, out T dst)
		{
			float num = Mathf.Abs(sky.SunriseTime - sky.Cycle.Hour);
			float num2 = Mathf.Abs(sky.SunsetTime - sky.Cycle.Hour);
			float num3 = (180f - sky.SunZenith) / 180f;
			float num4 = 0.11111111f;
			if (num < num2)
			{
				if (num3 < 0.5f)
				{
					src = this.Night;
					dst = this.Dawn;
					return Mathf.InverseLerp(0.5f - num4, 0.5f, num3);
				}
				src = this.Dawn;
				dst = this.Noon;
				return Mathf.InverseLerp(0.5f, 0.5f + num4, num3);
			}
			else
			{
				if (num3 > 0.5f)
				{
					src = this.Noon;
					dst = this.Dusk;
					return Mathf.InverseLerp(0.5f + num4, 0.5f, num3);
				}
				src = this.Dusk;
				dst = this.Night;
				return Mathf.InverseLerp(0.5f, 0.5f - num4, num3);
			}
		}
	}

	// Token: 0x0200036F RID: 879
	[Serializable]
	public class Float4 : Climate.Value4<float>
	{
	}

	// Token: 0x02000370 RID: 880
	[Serializable]
	public class Color4 : Climate.Value4<Color>
	{
	}

	// Token: 0x02000371 RID: 881
	[Serializable]
	public class Texture2D4 : Climate.Value4<Texture2D>
	{
	}
}
