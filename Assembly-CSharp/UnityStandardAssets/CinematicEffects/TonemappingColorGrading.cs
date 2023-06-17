using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x02000809 RID: 2057
	[ExecuteInEditMode]
	[ImageEffectAllowedInSceneView]
	[AddComponentMenu("Image Effects/Cinematic/Tonemapping and Color Grading")]
	public class TonemappingColorGrading : MonoBehaviour
	{
		// Token: 0x04002854 RID: 10324
		[TonemappingColorGrading.SettingsGroup]
		[SerializeField]
		private TonemappingColorGrading.EyeAdaptationSettings m_EyeAdaptation = TonemappingColorGrading.EyeAdaptationSettings.defaultSettings;

		// Token: 0x04002855 RID: 10325
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.TonemappingSettings m_Tonemapping = TonemappingColorGrading.TonemappingSettings.defaultSettings;

		// Token: 0x04002856 RID: 10326
		[TonemappingColorGrading.SettingsGroup]
		[SerializeField]
		private TonemappingColorGrading.ColorGradingSettings m_ColorGrading = TonemappingColorGrading.ColorGradingSettings.defaultSettings;

		// Token: 0x04002857 RID: 10327
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.LUTSettings m_Lut = TonemappingColorGrading.LUTSettings.defaultSettings;

		// Token: 0x04002858 RID: 10328
		private Texture2D m_IdentityLut;

		// Token: 0x04002859 RID: 10329
		private RenderTexture m_InternalLut;

		// Token: 0x0400285A RID: 10330
		private Texture2D m_CurveTexture;

		// Token: 0x0400285B RID: 10331
		private Texture2D m_TonemapperCurve;

		// Token: 0x0400285C RID: 10332
		private float m_TonemapperCurveRange;

		// Token: 0x0400285D RID: 10333
		[SerializeField]
		private Shader m_Shader;

		// Token: 0x0400285E RID: 10334
		private Material m_Material;

		// Token: 0x04002861 RID: 10337
		private bool m_Dirty = true;

		// Token: 0x04002862 RID: 10338
		private bool m_TonemapperDirty = true;

		// Token: 0x04002863 RID: 10339
		private RenderTexture m_SmallAdaptiveRt;

		// Token: 0x04002864 RID: 10340
		private RenderTextureFormat m_AdaptiveRtFormat;

		// Token: 0x04002865 RID: 10341
		private RenderTexture[] rts;

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06002CE7 RID: 11495 RVA: 0x000230B5 File Offset: 0x000212B5
		// (set) Token: 0x06002CE8 RID: 11496 RVA: 0x000230BD File Offset: 0x000212BD
		public TonemappingColorGrading.EyeAdaptationSettings eyeAdaptation
		{
			get
			{
				return this.m_EyeAdaptation;
			}
			set
			{
				this.m_EyeAdaptation = value;
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06002CE9 RID: 11497 RVA: 0x000230C6 File Offset: 0x000212C6
		// (set) Token: 0x06002CEA RID: 11498 RVA: 0x000230CE File Offset: 0x000212CE
		public TonemappingColorGrading.TonemappingSettings tonemapping
		{
			get
			{
				return this.m_Tonemapping;
			}
			set
			{
				this.m_Tonemapping = value;
				this.SetTonemapperDirty();
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06002CEB RID: 11499 RVA: 0x000230DD File Offset: 0x000212DD
		// (set) Token: 0x06002CEC RID: 11500 RVA: 0x000230E5 File Offset: 0x000212E5
		public TonemappingColorGrading.ColorGradingSettings colorGrading
		{
			get
			{
				return this.m_ColorGrading;
			}
			set
			{
				this.m_ColorGrading = value;
				this.SetDirty();
			}
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06002CED RID: 11501 RVA: 0x000230F4 File Offset: 0x000212F4
		// (set) Token: 0x06002CEE RID: 11502 RVA: 0x000230FC File Offset: 0x000212FC
		public TonemappingColorGrading.LUTSettings lut
		{
			get
			{
				return this.m_Lut;
			}
			set
			{
				this.m_Lut = value;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06002CEF RID: 11503 RVA: 0x000E1CF4 File Offset: 0x000DFEF4
		private Texture2D identityLut
		{
			get
			{
				if (this.m_IdentityLut == null || this.m_IdentityLut.height != this.lutSize)
				{
					Object.DestroyImmediate(this.m_IdentityLut);
					this.m_IdentityLut = TonemappingColorGrading.GenerateIdentityLut(this.lutSize);
				}
				return this.m_IdentityLut;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06002CF0 RID: 11504 RVA: 0x000E1D44 File Offset: 0x000DFF44
		private RenderTexture internalLutRt
		{
			get
			{
				if (this.m_InternalLut == null || !this.m_InternalLut.IsCreated() || this.m_InternalLut.height != this.lutSize)
				{
					Object.DestroyImmediate(this.m_InternalLut);
					this.m_InternalLut = new RenderTexture(this.lutSize * this.lutSize, this.lutSize, 0, RenderTextureFormat.ARGB32)
					{
						name = "Internal LUT",
						filterMode = FilterMode.Bilinear,
						anisoLevel = 0,
						hideFlags = HideFlags.DontSave
					};
				}
				return this.m_InternalLut;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06002CF1 RID: 11505 RVA: 0x000E1DD4 File Offset: 0x000DFFD4
		private Texture2D curveTexture
		{
			get
			{
				if (this.m_CurveTexture == null)
				{
					this.m_CurveTexture = new Texture2D(256, 1, TextureFormat.ARGB32, false, true)
					{
						name = "Curve texture",
						wrapMode = TextureWrapMode.Clamp,
						filterMode = FilterMode.Bilinear,
						anisoLevel = 0,
						hideFlags = HideFlags.DontSave
					};
				}
				return this.m_CurveTexture;
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06002CF2 RID: 11506 RVA: 0x000E1E34 File Offset: 0x000E0034
		private Texture2D tonemapperCurve
		{
			get
			{
				if (this.m_TonemapperCurve == null)
				{
					TextureFormat textureFormat = TextureFormat.RGB24;
					if (SystemInfo.SupportsTextureFormat(TextureFormat.RFloat))
					{
						textureFormat = TextureFormat.RFloat;
					}
					else if (SystemInfo.SupportsTextureFormat(TextureFormat.RHalf))
					{
						textureFormat = TextureFormat.RHalf;
					}
					this.m_TonemapperCurve = new Texture2D(256, 1, textureFormat, false, true)
					{
						name = "Tonemapper curve texture",
						wrapMode = TextureWrapMode.Clamp,
						filterMode = FilterMode.Bilinear,
						anisoLevel = 0,
						hideFlags = HideFlags.DontSave
					};
				}
				return this.m_TonemapperCurve;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06002CF3 RID: 11507 RVA: 0x00023105 File Offset: 0x00021305
		public Shader shader
		{
			get
			{
				if (this.m_Shader == null)
				{
					this.m_Shader = Shader.Find("Hidden/TonemappingColorGrading");
				}
				return this.m_Shader;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06002CF4 RID: 11508 RVA: 0x0002312B File Offset: 0x0002132B
		public Material material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(this.shader);
				}
				return this.m_Material;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06002CF5 RID: 11509 RVA: 0x00023152 File Offset: 0x00021352
		public bool isGammaColorSpace
		{
			get
			{
				return QualitySettings.activeColorSpace == ColorSpace.Gamma;
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06002CF6 RID: 11510 RVA: 0x0002315C File Offset: 0x0002135C
		public int lutSize
		{
			get
			{
				return (int)this.colorGrading.precision;
			}
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06002CF7 RID: 11511 RVA: 0x00023169 File Offset: 0x00021369
		// (set) Token: 0x06002CF8 RID: 11512 RVA: 0x00023171 File Offset: 0x00021371
		public bool validRenderTextureFormat { get; private set; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06002CF9 RID: 11513 RVA: 0x0002317A File Offset: 0x0002137A
		// (set) Token: 0x06002CFA RID: 11514 RVA: 0x00023182 File Offset: 0x00021382
		public bool validUserLutSize { get; private set; }

		// Token: 0x06002CFB RID: 11515 RVA: 0x0002318B File Offset: 0x0002138B
		public void SetDirty()
		{
			this.m_Dirty = true;
		}

		// Token: 0x06002CFC RID: 11516 RVA: 0x00023194 File Offset: 0x00021394
		public void SetTonemapperDirty()
		{
			this.m_TonemapperDirty = true;
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x0002319D File Offset: 0x0002139D
		private void OnEnable()
		{
			if (!ImageEffectHelper.IsSupported(this.shader, false, true, this))
			{
				base.enabled = false;
				return;
			}
			this.SetDirty();
			this.SetTonemapperDirty();
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x000E1EB0 File Offset: 0x000E00B0
		private void OnDisable()
		{
			if (this.m_Material != null)
			{
				Object.DestroyImmediate(this.m_Material);
			}
			if (this.m_IdentityLut != null)
			{
				Object.DestroyImmediate(this.m_IdentityLut);
			}
			if (this.m_InternalLut != null)
			{
				Object.DestroyImmediate(this.internalLutRt);
			}
			if (this.m_SmallAdaptiveRt != null)
			{
				Object.DestroyImmediate(this.m_SmallAdaptiveRt);
			}
			if (this.m_CurveTexture != null)
			{
				Object.DestroyImmediate(this.m_CurveTexture);
			}
			if (this.m_TonemapperCurve != null)
			{
				Object.DestroyImmediate(this.m_TonemapperCurve);
			}
			this.m_Material = null;
			this.m_IdentityLut = null;
			this.m_InternalLut = null;
			this.m_SmallAdaptiveRt = null;
			this.m_CurveTexture = null;
			this.m_TonemapperCurve = null;
		}

		// Token: 0x06002CFF RID: 11519 RVA: 0x000231C3 File Offset: 0x000213C3
		private void OnValidate()
		{
			this.SetDirty();
			this.SetTonemapperDirty();
		}

		// Token: 0x06002D00 RID: 11520 RVA: 0x000E1F80 File Offset: 0x000E0180
		private static Texture2D GenerateIdentityLut(int dim)
		{
			Color[] array = new Color[dim * dim * dim];
			float num = 1f / ((float)dim - 1f);
			for (int i = 0; i < dim; i++)
			{
				for (int j = 0; j < dim; j++)
				{
					for (int k = 0; k < dim; k++)
					{
						array[i + j * dim + k * dim * dim] = new Color((float)i * num, Mathf.Abs((float)k * num), (float)j * num, 1f);
					}
				}
			}
			Texture2D texture2D = new Texture2D(dim * dim, dim, TextureFormat.RGB24, false, true);
			texture2D.name = "Identity LUT";
			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.anisoLevel = 0;
			texture2D.hideFlags = HideFlags.DontSave;
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x06002D01 RID: 11521 RVA: 0x000231D1 File Offset: 0x000213D1
		private float StandardIlluminantY(float x)
		{
			return 2.87f * x - 3f * x * x - 0.27509508f;
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x000E2038 File Offset: 0x000E0238
		private Vector3 CIExyToLMS(float x, float y)
		{
			float num = 1f;
			float num2 = num * x / y;
			float num3 = num * (1f - x - y) / y;
			float x2 = 0.7328f * num2 + 0.4296f * num - 0.1624f * num3;
			float y2 = -0.7036f * num2 + 1.6975f * num + 0.0061f * num3;
			float z = 0.003f * num2 + 0.0136f * num + 0.9834f * num3;
			return new Vector3(x2, y2, z);
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x000E20B0 File Offset: 0x000E02B0
		private Vector3 GetWhiteBalance()
		{
			float temperatureShift = this.colorGrading.basics.temperatureShift;
			float tint = this.colorGrading.basics.tint;
			float x = 0.31271f - temperatureShift * ((temperatureShift < 0f) ? 0.1f : 0.05f);
			float y = this.StandardIlluminantY(x) + tint * 0.05f;
			Vector3 vector = new Vector3(0.949237f, 1.03542f, 1.08728f);
			Vector3 vector2 = this.CIExyToLMS(x, y);
			return new Vector3(vector.x / vector2.x, vector.y / vector2.y, vector.z / vector2.z);
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x000E2160 File Offset: 0x000E0360
		private static Color NormalizeColor(Color c)
		{
			float num = (c.r + c.g + c.b) / 3f;
			if (Mathf.Approximately(num, 0f))
			{
				return new Color(1f, 1f, 1f, 1f);
			}
			return new Color
			{
				r = c.r / num,
				g = c.g / num,
				b = c.b / num,
				a = 1f
			};
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x000E21F4 File Offset: 0x000E03F4
		private void GenerateLiftGammaGain(out Color lift, out Color gamma, out Color gain)
		{
			Color color = TonemappingColorGrading.NormalizeColor(this.colorGrading.colorWheels.shadows);
			Color color2 = TonemappingColorGrading.NormalizeColor(this.colorGrading.colorWheels.midtones);
			Color color3 = TonemappingColorGrading.NormalizeColor(this.colorGrading.colorWheels.highlights);
			float num = (color.r + color.g + color.b) / 3f;
			float num2 = (color2.r + color2.g + color2.b) / 3f;
			float num3 = (color3.r + color3.g + color3.b) / 3f;
			float r = (color.r - num) * 0.1f;
			float g = (color.g - num) * 0.1f;
			float b = (color.b - num) * 0.1f;
			float b2 = Mathf.Pow(2f, (color2.r - num2) * 0.5f);
			float b3 = Mathf.Pow(2f, (color2.g - num2) * 0.5f);
			float b4 = Mathf.Pow(2f, (color2.b - num2) * 0.5f);
			float r2 = Mathf.Pow(2f, (color3.r - num3) * 0.5f);
			float g2 = Mathf.Pow(2f, (color3.g - num3) * 0.5f);
			float b5 = Mathf.Pow(2f, (color3.b - num3) * 0.5f);
			float r3 = 1f / Mathf.Max(0.01f, b2);
			float g3 = 1f / Mathf.Max(0.01f, b3);
			float b6 = 1f / Mathf.Max(0.01f, b4);
			lift = new Color(r, g, b);
			gamma = new Color(r3, g3, b6);
			gain = new Color(r2, g2, b5);
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x000E23D8 File Offset: 0x000E05D8
		private void GenCurveTexture()
		{
			AnimationCurve master = this.colorGrading.curves.master;
			AnimationCurve red = this.colorGrading.curves.red;
			AnimationCurve green = this.colorGrading.curves.green;
			AnimationCurve blue = this.colorGrading.curves.blue;
			Color[] array = new Color[256];
			for (float num = 0f; num <= 1f; num += 0.003921569f)
			{
				float a = Mathf.Clamp(master.Evaluate(num), 0f, 1f);
				float r = Mathf.Clamp(red.Evaluate(num), 0f, 1f);
				float g = Mathf.Clamp(green.Evaluate(num), 0f, 1f);
				float b = Mathf.Clamp(blue.Evaluate(num), 0f, 1f);
				array[(int)Mathf.Floor(num * 255f)] = new Color(r, g, b, a);
			}
			this.curveTexture.SetPixels(array);
			this.curveTexture.Apply();
		}

		// Token: 0x06002D07 RID: 11527 RVA: 0x000231EA File Offset: 0x000213EA
		private bool CheckUserLut()
		{
			this.validUserLutSize = (this.lut.texture.height == (int)Mathf.Sqrt((float)this.lut.texture.width));
			return this.validUserLutSize;
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x000E24F8 File Offset: 0x000E06F8
		private bool CheckSmallAdaptiveRt()
		{
			if (this.m_SmallAdaptiveRt != null)
			{
				return false;
			}
			this.m_AdaptiveRtFormat = RenderTextureFormat.ARGBHalf;
			if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf))
			{
				this.m_AdaptiveRtFormat = RenderTextureFormat.RGHalf;
			}
			this.m_SmallAdaptiveRt = new RenderTexture(1, 1, 0, this.m_AdaptiveRtFormat);
			this.m_SmallAdaptiveRt.hideFlags = HideFlags.DontSave;
			return true;
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x000E2550 File Offset: 0x000E0750
		[ImageEffectTransformsToLDR]
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.material.shaderKeywords = null;
			RenderTexture renderTexture = null;
			if (this.eyeAdaptation.enabled)
			{
				bool flag = this.CheckSmallAdaptiveRt();
				int num = (source.width < source.height) ? source.width : source.height;
				int num2 = num | num >> 1;
				int num3 = num2 | num2 >> 2;
				int num4 = num3 | num3 >> 4;
				int num5 = num4 | num4 >> 8;
				int num6 = num5 | num5 >> 16;
				int num7 = num6 - (num6 >> 1);
				renderTexture = RenderTexture.GetTemporary(num7, num7, 0, this.m_AdaptiveRtFormat);
				Graphics.Blit(source, renderTexture);
				int num8 = (int)Mathf.Log((float)renderTexture.width, 2f);
				int num9 = 2;
				if (this.rts == null || this.rts.Length != num8)
				{
					this.rts = new RenderTexture[num8];
				}
				for (int i = 0; i < num8; i++)
				{
					this.rts[i] = RenderTexture.GetTemporary(renderTexture.width / num9, renderTexture.width / num9, 0, this.m_AdaptiveRtFormat);
					num9 <<= 1;
				}
				RenderTexture source2 = this.rts[num8 - 1];
				Graphics.Blit(renderTexture, this.rts[0], this.material, 1);
				for (int j = 0; j < num8 - 1; j++)
				{
					Graphics.Blit(this.rts[j], this.rts[j + 1]);
					source2 = this.rts[j + 1];
				}
				this.m_SmallAdaptiveRt.MarkRestoreExpected();
				this.material.SetFloat("_AdaptationSpeed", Mathf.Max(this.eyeAdaptation.speed, 0.001f));
				Graphics.Blit(source2, this.m_SmallAdaptiveRt, this.material, flag ? 3 : 2);
				this.material.SetFloat("_MiddleGrey", this.eyeAdaptation.middleGrey);
				this.material.SetFloat("_AdaptationMin", Mathf.Pow(2f, this.eyeAdaptation.min));
				this.material.SetFloat("_AdaptationMax", Mathf.Pow(2f, this.eyeAdaptation.max));
				this.material.SetTexture("_LumTex", this.m_SmallAdaptiveRt);
			}
			bool flag2 = this.lut.enabled && this.lut.texture != null && this.CheckUserLut();
			int num10 = 4;
			if (this.tonemapping.enabled)
			{
				if (this.tonemapping.tonemapper == TonemappingColorGrading.Tonemapper.Curve)
				{
					if (this.m_TonemapperDirty)
					{
						float num11 = 1f;
						if (this.tonemapping.curve.length > 0)
						{
							num11 = this.tonemapping.curve[this.tonemapping.curve.length - 1].time;
							for (float num12 = 0f; num12 <= 1f; num12 += 0.003921569f)
							{
								float num13 = this.tonemapping.curve.Evaluate(num12 * num11);
								this.tonemapperCurve.SetPixel(Mathf.FloorToInt(num12 * 255f), 0, new Color(num13, num13, num13));
							}
							this.tonemapperCurve.Apply();
						}
						this.m_TonemapperCurveRange = 1f / num11;
						this.m_TonemapperDirty = false;
					}
					this.material.SetFloat("_ToneCurveRange", this.m_TonemapperCurveRange);
					this.material.SetTexture("_ToneCurve", this.tonemapperCurve);
				}
				else if (this.tonemapping.tonemapper == TonemappingColorGrading.Tonemapper.Neutral)
				{
					float num14 = this.tonemapping.neutralBlackIn * 20f + 1f;
					float num15 = this.tonemapping.neutralBlackOut * 10f + 1f;
					float num16 = this.tonemapping.neutralWhiteIn / 20f;
					float num17 = 1f - this.tonemapping.neutralWhiteOut / 20f;
					float t = num14 / num15;
					float t2 = num16 / num17;
					float y = Mathf.Max(0f, Mathf.LerpUnclamped(0.57f, 0.37f, t));
					float z = Mathf.LerpUnclamped(0.01f, 0.24f, t2);
					float w = Mathf.Max(0f, Mathf.LerpUnclamped(0.02f, 0.2f, t));
					this.material.SetVector("_NeutralTonemapperParams1", new Vector4(0.2f, y, z, w));
					this.material.SetVector("_NeutralTonemapperParams2", new Vector4(0.02f, 0.3f, this.tonemapping.neutralWhiteLevel, this.tonemapping.neutralWhiteClip / 10f));
				}
				this.material.SetFloat("_Exposure", this.tonemapping.exposure);
				num10 = (int)(num10 + (this.tonemapping.tonemapper + 1) * TonemappingColorGrading.Tonemapper.Hable);
			}
			num10 += (flag2 ? 1 : 0);
			if (this.colorGrading.enabled)
			{
				if (this.m_Dirty || !this.m_InternalLut.IsCreated())
				{
					Color c;
					Color c2;
					Color c3;
					this.GenerateLiftGammaGain(out c, out c2, out c3);
					this.GenCurveTexture();
					this.material.SetVector("_WhiteBalance", this.GetWhiteBalance());
					this.material.SetVector("_Lift", c);
					this.material.SetVector("_Gamma", c2);
					this.material.SetVector("_Gain", c3);
					this.material.SetVector("_ContrastGainGamma", new Vector3(this.colorGrading.basics.contrast, this.colorGrading.basics.gain, 1f / this.colorGrading.basics.gamma));
					this.material.SetFloat("_Vibrance", this.colorGrading.basics.vibrance);
					this.material.SetVector("_HSV", new Vector4(this.colorGrading.basics.hue, this.colorGrading.basics.saturation, this.colorGrading.basics.value));
					this.material.SetVector("_ChannelMixerRed", this.colorGrading.channelMixer.channels[0]);
					this.material.SetVector("_ChannelMixerGreen", this.colorGrading.channelMixer.channels[1]);
					this.material.SetVector("_ChannelMixerBlue", this.colorGrading.channelMixer.channels[2]);
					this.material.SetTexture("_CurveTex", this.curveTexture);
					this.internalLutRt.MarkRestoreExpected();
					Graphics.Blit(this.identityLut, this.internalLutRt, this.material, 0);
					this.m_Dirty = false;
				}
				this.material.SetTexture("_InternalLutTex", this.internalLutRt);
				this.material.SetVector("_InternalLutParams", new Vector3(1f / (float)this.internalLutRt.width, 1f / (float)this.internalLutRt.height, (float)this.internalLutRt.height - 1f));
			}
			if (flag2)
			{
				this.material.SetTexture("_UserLutTex", this.lut.texture);
				this.material.SetVector("_UserLutParams", new Vector4(1f / (float)this.lut.texture.width, 1f / (float)this.lut.texture.height, (float)this.lut.texture.height - 1f, this.lut.contribution));
			}
			GL.sRGBWrite = true;
			Graphics.Blit(source, destination, this.material, num10);
			if (this.eyeAdaptation.enabled)
			{
				for (int k = 0; k < this.rts.Length; k++)
				{
					RenderTexture.ReleaseTemporary(this.rts[k]);
				}
				RenderTexture.ReleaseTemporary(renderTexture);
			}
		}

		// Token: 0x06002D0A RID: 11530 RVA: 0x000E2D4C File Offset: 0x000E0F4C
		public Texture2D BakeLUT()
		{
			Texture2D texture2D = new Texture2D(this.internalLutRt.width, this.internalLutRt.height, TextureFormat.RGB24, false, true);
			RenderTexture.active = this.internalLutRt;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), 0, 0);
			RenderTexture.active = null;
			return texture2D;
		}

		// Token: 0x0200080A RID: 2058
		[AttributeUsage(256)]
		public class SettingsGroup : Attribute
		{
		}

		// Token: 0x0200080B RID: 2059
		public class IndentedGroup : PropertyAttribute
		{
		}

		// Token: 0x0200080C RID: 2060
		public class ChannelMixer : PropertyAttribute
		{
		}

		// Token: 0x0200080D RID: 2061
		public class ColorWheelGroup : PropertyAttribute
		{
			// Token: 0x04002866 RID: 10342
			public int minSizePerWheel = 60;

			// Token: 0x04002867 RID: 10343
			public int maxSizePerWheel = 150;

			// Token: 0x06002D0F RID: 11535 RVA: 0x00023221 File Offset: 0x00021421
			public ColorWheelGroup()
			{
			}

			// Token: 0x06002D10 RID: 11536 RVA: 0x0002323C File Offset: 0x0002143C
			public ColorWheelGroup(int minSizePerWheel, int maxSizePerWheel)
			{
				this.minSizePerWheel = minSizePerWheel;
				this.maxSizePerWheel = maxSizePerWheel;
			}
		}

		// Token: 0x0200080E RID: 2062
		public class Curve : PropertyAttribute
		{
			// Token: 0x04002868 RID: 10344
			public Color color = Color.white;

			// Token: 0x06002D11 RID: 11537 RVA: 0x00023265 File Offset: 0x00021465
			public Curve()
			{
			}

			// Token: 0x06002D12 RID: 11538 RVA: 0x00023278 File Offset: 0x00021478
			public Curve(float r, float g, float b, float a)
			{
				this.color = new Color(r, g, b, a);
			}
		}

		// Token: 0x0200080F RID: 2063
		[Serializable]
		public struct EyeAdaptationSettings
		{
			// Token: 0x04002869 RID: 10345
			public bool enabled;

			// Token: 0x0400286A RID: 10346
			[Tooltip("Midpoint Adjustment.")]
			[Min(0f)]
			public float middleGrey;

			// Token: 0x0400286B RID: 10347
			[Tooltip("The lowest possible exposure value; adjust this value to modify the brightest areas of your level.")]
			public float min;

			// Token: 0x0400286C RID: 10348
			[Tooltip("The highest possible exposure value; adjust this value to modify the darkest areas of your level.")]
			public float max;

			// Token: 0x0400286D RID: 10349
			[Tooltip("Speed of linear adaptation. Higher is faster.")]
			[Min(0f)]
			public float speed;

			// Token: 0x0400286E RID: 10350
			[Tooltip("Displays a luminosity helper in the GameView.")]
			public bool showDebug;

			// Token: 0x17000365 RID: 869
			// (get) Token: 0x06002D13 RID: 11539 RVA: 0x000E2E00 File Offset: 0x000E1000
			public static TonemappingColorGrading.EyeAdaptationSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.EyeAdaptationSettings
					{
						enabled = false,
						showDebug = false,
						middleGrey = 0.5f,
						min = -3f,
						max = 3f,
						speed = 1.5f
					};
				}
			}
		}

		// Token: 0x02000810 RID: 2064
		public enum Tonemapper
		{
			// Token: 0x04002870 RID: 10352
			ACES,
			// Token: 0x04002871 RID: 10353
			Curve,
			// Token: 0x04002872 RID: 10354
			Hable,
			// Token: 0x04002873 RID: 10355
			HejlDawson,
			// Token: 0x04002874 RID: 10356
			Photographic,
			// Token: 0x04002875 RID: 10357
			Reinhard,
			// Token: 0x04002876 RID: 10358
			Neutral
		}

		// Token: 0x02000811 RID: 2065
		[Serializable]
		public struct TonemappingSettings
		{
			// Token: 0x04002877 RID: 10359
			public bool enabled;

			// Token: 0x04002878 RID: 10360
			[Tooltip("Tonemapping technique to use. ACES is the recommended one.")]
			public TonemappingColorGrading.Tonemapper tonemapper;

			// Token: 0x04002879 RID: 10361
			[Min(0f)]
			[Tooltip("Adjusts the overall exposure of the scene.")]
			public float exposure;

			// Token: 0x0400287A RID: 10362
			[Tooltip("Custom tonemapping curve.")]
			public AnimationCurve curve;

			// Token: 0x0400287B RID: 10363
			[Range(-0.1f, 0.1f)]
			public float neutralBlackIn;

			// Token: 0x0400287C RID: 10364
			[Range(1f, 20f)]
			public float neutralWhiteIn;

			// Token: 0x0400287D RID: 10365
			[Range(-0.09f, 0.1f)]
			public float neutralBlackOut;

			// Token: 0x0400287E RID: 10366
			[Range(1f, 19f)]
			public float neutralWhiteOut;

			// Token: 0x0400287F RID: 10367
			[Range(0.1f, 20f)]
			public float neutralWhiteLevel;

			// Token: 0x04002880 RID: 10368
			[Range(1f, 10f)]
			public float neutralWhiteClip;

			// Token: 0x17000366 RID: 870
			// (get) Token: 0x06002D14 RID: 11540 RVA: 0x000E2E58 File Offset: 0x000E1058
			public static TonemappingColorGrading.TonemappingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.TonemappingSettings
					{
						enabled = false,
						tonemapper = TonemappingColorGrading.Tonemapper.Neutral,
						exposure = 1f,
						curve = TonemappingColorGrading.CurvesSettings.defaultCurve,
						neutralBlackIn = 0.02f,
						neutralWhiteIn = 10f,
						neutralBlackOut = 0f,
						neutralWhiteOut = 10f,
						neutralWhiteLevel = 5.3f,
						neutralWhiteClip = 10f
					};
				}
			}
		}

		// Token: 0x02000812 RID: 2066
		[Serializable]
		public struct LUTSettings
		{
			// Token: 0x04002881 RID: 10369
			public bool enabled;

			// Token: 0x04002882 RID: 10370
			[Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
			public Texture texture;

			// Token: 0x04002883 RID: 10371
			[Range(0f, 1f)]
			[Tooltip("Blending factor.")]
			public float contribution;

			// Token: 0x17000367 RID: 871
			// (get) Token: 0x06002D15 RID: 11541 RVA: 0x000E2EE0 File Offset: 0x000E10E0
			public static TonemappingColorGrading.LUTSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.LUTSettings
					{
						enabled = false,
						texture = null,
						contribution = 1f
					};
				}
			}
		}

		// Token: 0x02000813 RID: 2067
		[Serializable]
		public struct ColorWheelsSettings
		{
			// Token: 0x04002884 RID: 10372
			[ColorUsage(false)]
			public Color shadows;

			// Token: 0x04002885 RID: 10373
			[ColorUsage(false)]
			public Color midtones;

			// Token: 0x04002886 RID: 10374
			[ColorUsage(false)]
			public Color highlights;

			// Token: 0x17000368 RID: 872
			// (get) Token: 0x06002D16 RID: 11542 RVA: 0x000E2F14 File Offset: 0x000E1114
			public static TonemappingColorGrading.ColorWheelsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorWheelsSettings
					{
						shadows = Color.white,
						midtones = Color.white,
						highlights = Color.white
					};
				}
			}
		}

		// Token: 0x02000814 RID: 2068
		[Serializable]
		public struct BasicsSettings
		{
			// Token: 0x04002887 RID: 10375
			[Tooltip("Sets the white balance to a custom color temperature.")]
			[Range(-2f, 2f)]
			public float temperatureShift;

			// Token: 0x04002888 RID: 10376
			[Range(-2f, 2f)]
			[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
			public float tint;

			// Token: 0x04002889 RID: 10377
			[Space]
			[Tooltip("Shift the hue of all colors.")]
			[Range(-0.5f, 0.5f)]
			public float hue;

			// Token: 0x0400288A RID: 10378
			[Tooltip("Pushes the intensity of all colors.")]
			[Range(0f, 2f)]
			public float saturation;

			// Token: 0x0400288B RID: 10379
			[Tooltip("Adjusts the saturation so that clipping is minimized as colors approach full saturation.")]
			[Range(-1f, 1f)]
			public float vibrance;

			// Token: 0x0400288C RID: 10380
			[Range(0f, 10f)]
			[Tooltip("Brightens or darkens all colors.")]
			public float value;

			// Token: 0x0400288D RID: 10381
			[Space]
			[Range(0f, 2f)]
			[Tooltip("Expands or shrinks the overall range of tonal values.")]
			public float contrast;

			// Token: 0x0400288E RID: 10382
			[Tooltip("Contrast gain curve. Controls the steepness of the curve.")]
			[Range(0.01f, 5f)]
			public float gain;

			// Token: 0x0400288F RID: 10383
			[Tooltip("Applies a pow function to the source.")]
			[Range(0.01f, 5f)]
			public float gamma;

			// Token: 0x17000369 RID: 873
			// (get) Token: 0x06002D17 RID: 11543 RVA: 0x000E2F50 File Offset: 0x000E1150
			public static TonemappingColorGrading.BasicsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.BasicsSettings
					{
						temperatureShift = 0f,
						tint = 0f,
						contrast = 1f,
						hue = 0f,
						saturation = 1f,
						value = 1f,
						vibrance = 0f,
						gain = 1f,
						gamma = 1f
					};
				}
			}
		}

		// Token: 0x02000815 RID: 2069
		[Serializable]
		public struct ChannelMixerSettings
		{
			// Token: 0x04002890 RID: 10384
			public int currentChannel;

			// Token: 0x04002891 RID: 10385
			public Vector3[] channels;

			// Token: 0x1700036A RID: 874
			// (get) Token: 0x06002D18 RID: 11544 RVA: 0x000E2FD4 File Offset: 0x000E11D4
			public static TonemappingColorGrading.ChannelMixerSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ChannelMixerSettings
					{
						currentChannel = 0,
						channels = new Vector3[]
						{
							new Vector3(1f, 0f, 0f),
							new Vector3(0f, 1f, 0f),
							new Vector3(0f, 0f, 1f)
						}
					};
				}
			}
		}

		// Token: 0x02000816 RID: 2070
		[Serializable]
		public struct CurvesSettings
		{
			// Token: 0x04002892 RID: 10386
			[TonemappingColorGrading.Curve]
			public AnimationCurve master;

			// Token: 0x04002893 RID: 10387
			[TonemappingColorGrading.Curve(1f, 0f, 0f, 1f)]
			public AnimationCurve red;

			// Token: 0x04002894 RID: 10388
			[TonemappingColorGrading.Curve(0f, 1f, 0f, 1f)]
			public AnimationCurve green;

			// Token: 0x04002895 RID: 10389
			[TonemappingColorGrading.Curve(0f, 1f, 1f, 1f)]
			public AnimationCurve blue;

			// Token: 0x1700036B RID: 875
			// (get) Token: 0x06002D19 RID: 11545 RVA: 0x000E3050 File Offset: 0x000E1250
			public static TonemappingColorGrading.CurvesSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.CurvesSettings
					{
						master = TonemappingColorGrading.CurvesSettings.defaultCurve,
						red = TonemappingColorGrading.CurvesSettings.defaultCurve,
						green = TonemappingColorGrading.CurvesSettings.defaultCurve,
						blue = TonemappingColorGrading.CurvesSettings.defaultCurve
					};
				}
			}

			// Token: 0x1700036C RID: 876
			// (get) Token: 0x06002D1A RID: 11546 RVA: 0x000E3098 File Offset: 0x000E1298
			public static AnimationCurve defaultCurve
			{
				get
				{
					return new AnimationCurve(new Keyframe[]
					{
						new Keyframe(0f, 0f, 1f, 1f),
						new Keyframe(1f, 1f, 1f, 1f)
					});
				}
			}
		}

		// Token: 0x02000817 RID: 2071
		public enum ColorGradingPrecision
		{
			// Token: 0x04002897 RID: 10391
			Normal = 16,
			// Token: 0x04002898 RID: 10392
			High = 32
		}

		// Token: 0x02000818 RID: 2072
		[Serializable]
		public struct ColorGradingSettings
		{
			// Token: 0x04002899 RID: 10393
			public bool enabled;

			// Token: 0x0400289A RID: 10394
			[Tooltip("Internal LUT precision. \"Normal\" is 256x16, \"High\" is 1024x32. Prefer \"Normal\" on mobile devices.")]
			public TonemappingColorGrading.ColorGradingPrecision precision;

			// Token: 0x0400289B RID: 10395
			[TonemappingColorGrading.ColorWheelGroup]
			[Space]
			public TonemappingColorGrading.ColorWheelsSettings colorWheels;

			// Token: 0x0400289C RID: 10396
			[Space]
			[TonemappingColorGrading.IndentedGroup]
			public TonemappingColorGrading.BasicsSettings basics;

			// Token: 0x0400289D RID: 10397
			[TonemappingColorGrading.ChannelMixer]
			[Space]
			public TonemappingColorGrading.ChannelMixerSettings channelMixer;

			// Token: 0x0400289E RID: 10398
			[TonemappingColorGrading.IndentedGroup]
			[Space]
			public TonemappingColorGrading.CurvesSettings curves;

			// Token: 0x0400289F RID: 10399
			[Space]
			[Tooltip("Use dithering to try and minimize color banding in dark areas.")]
			public bool useDithering;

			// Token: 0x040028A0 RID: 10400
			[Tooltip("Displays the generated LUT in the top left corner of the GameView.")]
			public bool showDebug;

			// Token: 0x1700036D RID: 877
			// (get) Token: 0x06002D1B RID: 11547 RVA: 0x000E30F0 File Offset: 0x000E12F0
			public static TonemappingColorGrading.ColorGradingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorGradingSettings
					{
						enabled = false,
						useDithering = false,
						showDebug = false,
						precision = TonemappingColorGrading.ColorGradingPrecision.Normal,
						colorWheels = TonemappingColorGrading.ColorWheelsSettings.defaultSettings,
						basics = TonemappingColorGrading.BasicsSettings.defaultSettings,
						channelMixer = TonemappingColorGrading.ChannelMixerSettings.defaultSettings,
						curves = TonemappingColorGrading.CurvesSettings.defaultSettings
					};
				}
			}

			// Token: 0x06002D1C RID: 11548 RVA: 0x0002329B File Offset: 0x0002149B
			internal void Reset()
			{
				this.curves = TonemappingColorGrading.CurvesSettings.defaultSettings;
			}
		}

		// Token: 0x02000819 RID: 2073
		private enum Pass
		{
			// Token: 0x040028A2 RID: 10402
			LutGen,
			// Token: 0x040028A3 RID: 10403
			AdaptationLog,
			// Token: 0x040028A4 RID: 10404
			AdaptationExpBlend,
			// Token: 0x040028A5 RID: 10405
			AdaptationExp,
			// Token: 0x040028A6 RID: 10406
			TonemappingOff,
			// Token: 0x040028A7 RID: 10407
			TonemappingOff_LUT,
			// Token: 0x040028A8 RID: 10408
			TonemappingACES,
			// Token: 0x040028A9 RID: 10409
			TonemappingACES_LUT,
			// Token: 0x040028AA RID: 10410
			TonemappingCurve,
			// Token: 0x040028AB RID: 10411
			TonemappingCurve_LUT,
			// Token: 0x040028AC RID: 10412
			TonemappingHable,
			// Token: 0x040028AD RID: 10413
			TonemappingHable_LUT,
			// Token: 0x040028AE RID: 10414
			TonemappingHejlDawson,
			// Token: 0x040028AF RID: 10415
			TonemappingHejlDawson_LUT,
			// Token: 0x040028B0 RID: 10416
			TonemappingPhotographic,
			// Token: 0x040028B1 RID: 10417
			TonemappingPhotographic_LUT,
			// Token: 0x040028B2 RID: 10418
			TonemappingReinhard,
			// Token: 0x040028B3 RID: 10419
			TonemappingReinhard_LUT,
			// Token: 0x040028B4 RID: 10420
			TonemappingNeutral,
			// Token: 0x040028B5 RID: 10421
			TonemappingNeutral_LUT,
			// Token: 0x040028B6 RID: 10422
			AdaptationDebug
		}
	}
}
