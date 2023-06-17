using System;
using UnityEngine;

// Token: 0x0200079A RID: 1946
[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
[AddComponentMenu("KriptoFX/Explosion_Bloom")]
[RequireComponent(typeof(Camera))]
public class Explosion_Bloom : MonoBehaviour
{
	// Token: 0x040025CC RID: 9676
	[SerializeField]
	public Explosion_Bloom.Settings settings = Explosion_Bloom.Settings.defaultSettings;

	// Token: 0x040025CD RID: 9677
	[SerializeField]
	[HideInInspector]
	private Shader m_Shader;

	// Token: 0x040025CE RID: 9678
	private Material m_Material;

	// Token: 0x040025CF RID: 9679
	private const int kMaxIterations = 16;

	// Token: 0x040025D0 RID: 9680
	private RenderTexture[] m_blurBuffer1 = new RenderTexture[16];

	// Token: 0x040025D1 RID: 9681
	private RenderTexture[] m_blurBuffer2 = new RenderTexture[16];

	// Token: 0x040025D2 RID: 9682
	private int m_Threshold;

	// Token: 0x040025D3 RID: 9683
	private int m_Curve;

	// Token: 0x040025D4 RID: 9684
	private int m_PrefilterOffs;

	// Token: 0x040025D5 RID: 9685
	private int m_SampleScale;

	// Token: 0x040025D6 RID: 9686
	private int m_Intensity;

	// Token: 0x040025D7 RID: 9687
	private int m_BaseTex;

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06002A4A RID: 10826 RVA: 0x00020D15 File Offset: 0x0001EF15
	public Shader shader
	{
		get
		{
			if (this.m_Shader == null)
			{
				this.m_Shader = Shader.Find("Hidden/KriptoFX/PostEffects/Explosion_Bloom");
			}
			return this.m_Shader;
		}
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06002A4B RID: 10827 RVA: 0x00020D3B File Offset: 0x0001EF3B
	public Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = Explosion_Bloom.CheckShaderAndCreateMaterial(this.shader);
			}
			return this.m_Material;
		}
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x000D8094 File Offset: 0x000D6294
	public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
	{
		if (s == null || !s.isSupported)
		{
			Debug.LogWarningFormat("Missing shader for image effect {0}", new object[]
			{
				effect
			});
			return false;
		}
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		return true;
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x00020D62 File Offset: 0x0001EF62
	public static Material CheckShaderAndCreateMaterial(Shader s)
	{
		if (s == null || !s.isSupported)
		{
			return null;
		}
		return new Material(s)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06002A4E RID: 10830 RVA: 0x00020D85 File Offset: 0x0001EF85
	public static bool supportsDX11
	{
		get
		{
			return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
		}
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x000D8128 File Offset: 0x000D6328
	private void Awake()
	{
		this.m_Threshold = Shader.PropertyToID("_Threshold");
		this.m_Curve = Shader.PropertyToID("_Curve");
		this.m_PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
		this.m_SampleScale = Shader.PropertyToID("_SampleScale");
		this.m_Intensity = Shader.PropertyToID("_Intensity");
		this.m_BaseTex = Shader.PropertyToID("_BaseTex");
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x00020D97 File Offset: 0x0001EF97
	private void OnEnable()
	{
		if (!Explosion_Bloom.IsSupported(this.shader, true, false, this))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x00020DB0 File Offset: 0x0001EFB0
	private void OnDisable()
	{
		if (this.m_Material != null)
		{
			Object.DestroyImmediate(this.m_Material);
		}
		this.m_Material = null;
	}

	// Token: 0x06002A52 RID: 10834 RVA: 0x000D8198 File Offset: 0x000D6398
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		bool isMobilePlatform = Application.isMobilePlatform;
		int num = source.width;
		int num2 = source.height;
		if (!this.settings.highQuality)
		{
			num /= 2;
			num2 /= 2;
		}
		RenderTextureFormat format = isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
		float num3 = Mathf.Log((float)num2, 2f) + this.settings.radius - 8f;
		int num4 = (int)num3;
		int num5 = Mathf.Clamp(num4, 1, 16);
		float thresholdLinear = this.settings.thresholdLinear;
		this.material.SetFloat(this.m_Threshold, thresholdLinear);
		float num6 = thresholdLinear * this.settings.softKnee + 1E-05f;
		Vector3 v = new Vector3(thresholdLinear - num6, num6 * 2f, 0.25f / num6);
		this.material.SetVector(this.m_Curve, v);
		bool flag = !this.settings.highQuality && this.settings.antiFlicker;
		this.material.SetFloat(this.m_PrefilterOffs, flag ? -0.5f : 0f);
		this.material.SetFloat(this.m_SampleScale, 0.5f + num3 - (float)num4);
		this.material.SetFloat(this.m_Intensity, Mathf.Max(0f, this.settings.intensity));
		RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, format);
		Graphics.Blit(source, temporary, this.material, this.settings.antiFlicker ? 1 : 0);
		RenderTexture renderTexture = temporary;
		for (int i = 0; i < num5; i++)
		{
			this.m_blurBuffer1[i] = RenderTexture.GetTemporary(renderTexture.width / 2, renderTexture.height / 2, 0, format);
			Graphics.Blit(renderTexture, this.m_blurBuffer1[i], this.material, (i == 0) ? (this.settings.antiFlicker ? 3 : 2) : 4);
			renderTexture = this.m_blurBuffer1[i];
		}
		for (int j = num5 - 2; j >= 0; j--)
		{
			RenderTexture renderTexture2 = this.m_blurBuffer1[j];
			this.material.SetTexture(this.m_BaseTex, renderTexture2);
			this.m_blurBuffer2[j] = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, format);
			Graphics.Blit(renderTexture, this.m_blurBuffer2[j], this.material, this.settings.highQuality ? 6 : 5);
			renderTexture = this.m_blurBuffer2[j];
		}
		int num7 = 7;
		num7 += (this.settings.highQuality ? 1 : 0);
		this.material.SetTexture(this.m_BaseTex, source);
		Graphics.Blit(renderTexture, destination, this.material, num7);
		for (int k = 0; k < 16; k++)
		{
			if (this.m_blurBuffer1[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer1[k]);
			}
			if (this.m_blurBuffer2[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer2[k]);
			}
			this.m_blurBuffer1[k] = null;
			this.m_blurBuffer2[k] = null;
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x0200079B RID: 1947
	[Serializable]
	public struct Settings
	{
		// Token: 0x040025D8 RID: 9688
		[Tooltip("Filters out pixels under this level of brightness.")]
		[SerializeField]
		public float threshold;

		// Token: 0x040025D9 RID: 9689
		[Range(0f, 1f)]
		[Tooltip("Makes transition between under/over-threshold gradual.")]
		[SerializeField]
		public float softKnee;

		// Token: 0x040025DA RID: 9690
		[Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
		[Range(1f, 7f)]
		[SerializeField]
		public float radius;

		// Token: 0x040025DB RID: 9691
		[Tooltip("Blend factor of the result image.")]
		[SerializeField]
		public float intensity;

		// Token: 0x040025DC RID: 9692
		[Tooltip("Controls filter quality and buffer resolution.")]
		[SerializeField]
		public bool highQuality;

		// Token: 0x040025DD RID: 9693
		[Tooltip("Reduces flashing noise with an additional filter.")]
		[SerializeField]
		public bool antiFlicker;

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06002A55 RID: 10837 RVA: 0x00020E08 File Offset: 0x0001F008
		// (set) Token: 0x06002A54 RID: 10836 RVA: 0x00020DFF File Offset: 0x0001EFFF
		public float thresholdGamma
		{
			get
			{
				return Mathf.Max(0f, this.threshold);
			}
			set
			{
				this.threshold = value;
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06002A57 RID: 10839 RVA: 0x00020E28 File Offset: 0x0001F028
		// (set) Token: 0x06002A56 RID: 10838 RVA: 0x00020E1A File Offset: 0x0001F01A
		public float thresholdLinear
		{
			get
			{
				return Mathf.GammaToLinearSpace(this.thresholdGamma);
			}
			set
			{
				this.threshold = Mathf.LinearToGammaSpace(value);
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06002A58 RID: 10840 RVA: 0x000D84B0 File Offset: 0x000D66B0
		public static Explosion_Bloom.Settings defaultSettings
		{
			get
			{
				return new Explosion_Bloom.Settings
				{
					threshold = 2f,
					softKnee = 0f,
					radius = 7f,
					intensity = 0.7f,
					highQuality = true,
					antiFlicker = true
				};
			}
		}
	}
}
