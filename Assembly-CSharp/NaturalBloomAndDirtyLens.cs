using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000426 RID: 1062
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Natural Bloom and Dirty Lens")]
[RequireComponent(typeof(Camera))]
public class NaturalBloomAndDirtyLens : MonoBehaviour
{
	// Token: 0x0400161D RID: 5661
	public Shader shader;

	// Token: 0x0400161E RID: 5662
	public Texture2D lensDirtTexture;

	// Token: 0x0400161F RID: 5663
	public float range = 10000f;

	// Token: 0x04001620 RID: 5664
	public float cutoff = 1f;

	// Token: 0x04001621 RID: 5665
	[Range(0f, 1f)]
	public float bloomIntensity = 0.05f;

	// Token: 0x04001622 RID: 5666
	[Range(0f, 1f)]
	public float lensDirtIntensity = 0.05f;

	// Token: 0x04001623 RID: 5667
	[Range(0f, 4f)]
	public float spread = 1f;

	// Token: 0x04001624 RID: 5668
	[Range(0f, 4f)]
	public int iterations = 1;

	// Token: 0x04001625 RID: 5669
	[Range(1f, 10f)]
	public int mips = 6;

	// Token: 0x04001626 RID: 5670
	public float[] mipWeights = new float[]
	{
		0.5f,
		0.6f,
		0.6f,
		0.45f,
		0.35f,
		0.23f
	};

	// Token: 0x04001627 RID: 5671
	public bool highPrecision;

	// Token: 0x04001628 RID: 5672
	public bool downscaleSource;

	// Token: 0x04001629 RID: 5673
	public bool debug;

	// Token: 0x0400162A RID: 5674
	private Material material;

	// Token: 0x0400162B RID: 5675
	private bool isSupported;

	// Token: 0x0400162C RID: 5676
	private float blurSize = 4f;

	// Token: 0x0400162D RID: 5677
	private static int[] paramID = new int[]
	{
		Shader.PropertyToID("_BloomRange"),
		Shader.PropertyToID("_BloomCutoff"),
		Shader.PropertyToID("_BloomIntensity"),
		Shader.PropertyToID("_LensDirtIntensity"),
		Shader.PropertyToID("_MipWeights"),
		Shader.PropertyToID("_LensDirt"),
		Shader.PropertyToID("_BlurSize")
	};

	// Token: 0x0400162E RID: 5678
	private static int[] sourceID = new int[]
	{
		Shader.PropertyToID("_Bloom0"),
		Shader.PropertyToID("_Bloom1"),
		Shader.PropertyToID("_Bloom2"),
		Shader.PropertyToID("_Bloom3"),
		Shader.PropertyToID("_Bloom4"),
		Shader.PropertyToID("_Bloom5")
	};

	// Token: 0x0600199C RID: 6556 RVA: 0x000152A1 File Offset: 0x000134A1
	private void Start()
	{
		this.isSupported = (SystemInfo.supportsImageEffects && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf));
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x000152B9 File Offset: 0x000134B9
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.material != null)
		{
			Object.DestroyImmediate(this.material);
			this.material = null;
		}
	}

	// Token: 0x0600199E RID: 6558 RVA: 0x00090A3C File Offset: 0x0008EC3C
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.isSupported)
		{
			UnityEngine.Graphics.Blit(source, destination);
			return;
		}
		if (!this.material)
		{
			this.material = new Material(this.shader);
		}
		this.material.hideFlags = HideFlags.HideAndDontSave;
		this.material.SetFloat(NaturalBloomAndDirtyLens.paramID[0], this.range);
		this.material.SetFloat(NaturalBloomAndDirtyLens.paramID[1], this.cutoff);
		this.material.SetFloat(NaturalBloomAndDirtyLens.paramID[2], Mathf.Exp(this.bloomIntensity) - 1f);
		this.material.SetFloat(NaturalBloomAndDirtyLens.paramID[3], Effects.lensdirt ? (Mathf.Exp(this.lensDirtIntensity) - 1f) : 0f);
		source.filterMode = FilterMode.Bilinear;
		RenderTextureFormat format = RenderTextureFormat.ARGBHalf;
		bool flag = false;
		if (!this.highPrecision)
		{
			if (SystemInfoEx.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float))
			{
				format = RenderTextureFormat.RGB111110Float;
			}
			else
			{
				format = RenderTextureFormat.ARGB32;
				flag = true;
			}
		}
		int num = flag ? 0 : 5;
		int num2 = source.width / (this.downscaleSource ? 2 : 1);
		int num3 = source.height / (this.downscaleSource ? 2 : 1);
		RenderTexture temporary = RenderTexture.GetTemporary(num2, num3, 0, format);
		UnityEngine.Graphics.Blit(source, temporary, this.material, num + 4);
		int num4 = num2 / 2;
		int num5 = num3 / 2;
		RenderTexture source2 = temporary;
		if (!Effects.bloom)
		{
			for (int i = 0; i < 3; i++)
			{
				if (num4 > 0 && num5 > 0)
				{
					RenderTexture temporary2 = RenderTexture.GetTemporary(num4, num5, 0, format);
					RenderTexture temporary3 = RenderTexture.GetTemporary(num4, num5, 0, format);
					temporary2.filterMode = FilterMode.Bilinear;
					temporary3.filterMode = FilterMode.Bilinear;
					UnityEngine.Graphics.Blit(source2, temporary2, this.material, num + 1);
					source2 = temporary2;
					for (int j = 0; j < this.iterations; j++)
					{
						this.material.SetFloat(NaturalBloomAndDirtyLens.paramID[6], (this.blurSize * 0.5f + (float)j) * this.spread);
						UnityEngine.Graphics.Blit(temporary2, temporary3, this.material, num + 2);
						UnityEngine.Graphics.Blit(temporary3, temporary2, this.material, num + 3);
					}
					this.material.SetTexture(NaturalBloomAndDirtyLens.sourceID[i * 2], temporary2);
					this.material.SetTexture(NaturalBloomAndDirtyLens.sourceID[i * 2 + 1], temporary2);
					RenderTexture.ReleaseTemporary(temporary2);
					RenderTexture.ReleaseTemporary(temporary3);
					num4 /= 4;
					num5 /= 4;
				}
			}
		}
		else
		{
			for (int k = 0; k < this.mips; k++)
			{
				if (num4 > 0 && num5 > 0)
				{
					RenderTexture temporary4 = RenderTexture.GetTemporary(num4, num5, 0, format);
					RenderTexture temporary5 = RenderTexture.GetTemporary(num4, num5, 0, format);
					temporary4.filterMode = FilterMode.Bilinear;
					temporary5.filterMode = FilterMode.Bilinear;
					UnityEngine.Graphics.Blit(source2, temporary4, this.material, num + 1);
					source2 = temporary4;
					float num6;
					if (k > 1)
					{
						num6 = 1f * this.spread;
					}
					else
					{
						num6 = 0.5f * this.spread;
					}
					if (k == 2)
					{
						num6 = 0.75f * this.spread;
					}
					for (int l = 0; l < this.iterations; l++)
					{
						this.material.SetFloat(NaturalBloomAndDirtyLens.paramID[6], (this.blurSize * 0.5f + (float)l) * num6);
						UnityEngine.Graphics.Blit(temporary4, temporary5, this.material, num + 2);
						UnityEngine.Graphics.Blit(temporary5, temporary4, this.material, num + 3);
					}
					this.material.SetTexture(NaturalBloomAndDirtyLens.sourceID[k], temporary4);
					RenderTexture.ReleaseTemporary(temporary4);
					RenderTexture.ReleaseTemporary(temporary5);
					num4 /= 2;
					num5 /= 2;
				}
			}
		}
		this.material.SetFloatArray(NaturalBloomAndDirtyLens.paramID[4], this.mipWeights);
		this.material.SetTexture(NaturalBloomAndDirtyLens.paramID[5], this.lensDirtTexture);
		if (this.debug)
		{
			UnityEngine.Graphics.Blit(Texture2D.blackTexture, destination, this.material, num);
		}
		else
		{
			UnityEngine.Graphics.Blit(source, destination, this.material, num);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x02000427 RID: 1063
	private static class Param
	{
		// Token: 0x0400162F RID: 5679
		public const int _BloomRange = 0;

		// Token: 0x04001630 RID: 5680
		public const int _BloomCutoff = 1;

		// Token: 0x04001631 RID: 5681
		public const int _BloomIntensity = 2;

		// Token: 0x04001632 RID: 5682
		public const int _LensDirtIntensity = 3;

		// Token: 0x04001633 RID: 5683
		public const int _MipWeights = 4;

		// Token: 0x04001634 RID: 5684
		public const int _LensDirt = 5;

		// Token: 0x04001635 RID: 5685
		public const int _BlurSize = 6;
	}
}
