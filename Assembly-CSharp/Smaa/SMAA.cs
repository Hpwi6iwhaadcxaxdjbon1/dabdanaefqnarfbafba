using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020007E6 RID: 2022
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
	[ExecuteInEditMode]
	public class SMAA : MonoBehaviour
	{
		// Token: 0x0400280D RID: 10253
		public DebugPass DebugPass;

		// Token: 0x0400280E RID: 10254
		public QualityPreset Quality = QualityPreset.High;

		// Token: 0x0400280F RID: 10255
		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		// Token: 0x04002810 RID: 10256
		public bool UsePredication;

		// Token: 0x04002811 RID: 10257
		public Preset CustomPreset;

		// Token: 0x04002812 RID: 10258
		public PredicationPreset CustomPredicationPreset;

		// Token: 0x04002813 RID: 10259
		public Shader Shader;

		// Token: 0x04002814 RID: 10260
		public Texture2D AreaTex;

		// Token: 0x04002815 RID: 10261
		public Texture2D SearchTex;

		// Token: 0x04002816 RID: 10262
		protected Camera m_Camera;

		// Token: 0x04002817 RID: 10263
		protected Preset m_LowPreset;

		// Token: 0x04002818 RID: 10264
		protected Preset m_MediumPreset;

		// Token: 0x04002819 RID: 10265
		protected Preset m_HighPreset;

		// Token: 0x0400281A RID: 10266
		protected Preset m_UltraPreset;

		// Token: 0x0400281B RID: 10267
		protected Material m_Material;

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06002C24 RID: 11300 RVA: 0x000225F7 File Offset: 0x000207F7
		public Material Material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = new Material(this.Shader);
					this.m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_Material;
			}
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x000E1120 File Offset: 0x000DF320
		private void OnEnable()
		{
			if (this.AreaTex == null)
			{
				this.AreaTex = Resources.Load<Texture2D>("AreaTex");
			}
			if (this.SearchTex == null)
			{
				this.SearchTex = Resources.Load<Texture2D>("SearchTex");
			}
			this.m_Camera = base.GetComponent<Camera>();
		}

		// Token: 0x06002C26 RID: 11302 RVA: 0x0002262B File Offset: 0x0002082B
		private void Start()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				base.enabled = false;
				return;
			}
			if (!this.Shader || !this.Shader.isSupported)
			{
				base.enabled = false;
			}
			this.CreatePresets();
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x00022663 File Offset: 0x00020863
		private void OnDisable()
		{
			if (this.m_Material != null)
			{
				Object.DestroyImmediate(this.m_Material);
			}
		}

		// Token: 0x06002C28 RID: 11304 RVA: 0x000E1178 File Offset: 0x000DF378
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			int pixelWidth = this.m_Camera.pixelWidth;
			int pixelHeight = this.m_Camera.pixelHeight;
			Preset preset = this.CustomPreset;
			if (this.Quality == QualityPreset.Low)
			{
				preset = this.m_LowPreset;
			}
			else if (this.Quality == QualityPreset.Medium)
			{
				preset = this.m_MediumPreset;
			}
			else if (this.Quality == QualityPreset.High)
			{
				preset = this.m_HighPreset;
			}
			else if (this.Quality == QualityPreset.Ultra)
			{
				preset = this.m_UltraPreset;
			}
			int detectionMethod = (int)this.DetectionMethod;
			int pass = 4;
			int pass2 = 5;
			this.Material.SetTexture("_AreaTex", this.AreaTex);
			this.Material.SetTexture("_SearchTex", this.SearchTex);
			this.Material.SetTexture("_SourceTex", source);
			this.Material.SetVector("_Metrics", new Vector4(1f / (float)pixelWidth, 1f / (float)pixelHeight, (float)pixelWidth, (float)pixelHeight));
			this.Material.SetVector("_Params1", new Vector4(preset.Threshold, preset.DepthThreshold, (float)preset.MaxSearchSteps, (float)preset.MaxSearchStepsDiag));
			this.Material.SetVector("_Params2", new Vector2((float)preset.CornerRounding, preset.LocalContrastAdaptationFactor));
			Shader.DisableKeyword("USE_PREDICATION");
			if (this.DetectionMethod == EdgeDetectionMethod.Depth)
			{
				this.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
			}
			else if (this.UsePredication)
			{
				this.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
				Shader.EnableKeyword("USE_PREDICATION");
				this.Material.SetVector("_Params3", new Vector3(this.CustomPredicationPreset.Threshold, this.CustomPredicationPreset.Scale, this.CustomPredicationPreset.Strength));
			}
			Shader.DisableKeyword("USE_DIAG_SEARCH");
			Shader.DisableKeyword("USE_CORNER_DETECTION");
			if (preset.DiagDetection)
			{
				Shader.EnableKeyword("USE_DIAG_SEARCH");
			}
			if (preset.CornerDetection)
			{
				Shader.EnableKeyword("USE_CORNER_DETECTION");
			}
			RenderTexture renderTexture = this.TempRT(pixelWidth, pixelHeight);
			RenderTexture renderTexture2 = this.TempRT(pixelWidth, pixelHeight);
			this.Clear(renderTexture);
			this.Clear(renderTexture2);
			Graphics.Blit(source, renderTexture, this.Material, detectionMethod);
			if (this.DebugPass == DebugPass.Edges)
			{
				Graphics.Blit(renderTexture, destination);
			}
			else
			{
				Graphics.Blit(renderTexture, renderTexture2, this.Material, pass);
				if (this.DebugPass == DebugPass.Weights)
				{
					Graphics.Blit(renderTexture2, destination);
				}
				else
				{
					Graphics.Blit(renderTexture2, destination, this.Material, pass2);
				}
			}
			RenderTexture.ReleaseTemporary(renderTexture);
			RenderTexture.ReleaseTemporary(renderTexture2);
		}

		// Token: 0x06002C29 RID: 11305 RVA: 0x0002267E File Offset: 0x0002087E
		private void Clear(RenderTexture rt)
		{
			Graphics.Blit(rt, rt, this.Material, 0);
		}

		// Token: 0x06002C2A RID: 11306 RVA: 0x000E13FC File Offset: 0x000DF5FC
		private RenderTexture TempRT(int width, int height)
		{
			int depthBuffer = 0;
			return RenderTexture.GetTemporary(width, height, depthBuffer, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x000E1418 File Offset: 0x000DF618
		private void CreatePresets()
		{
			this.m_LowPreset = new Preset
			{
				Threshold = 0.15f,
				MaxSearchSteps = 4
			};
			this.m_LowPreset.DiagDetection = false;
			this.m_LowPreset.CornerDetection = false;
			this.m_MediumPreset = new Preset
			{
				Threshold = 0.1f,
				MaxSearchSteps = 8
			};
			this.m_MediumPreset.DiagDetection = false;
			this.m_MediumPreset.CornerDetection = false;
			this.m_HighPreset = new Preset
			{
				Threshold = 0.1f,
				MaxSearchSteps = 16,
				MaxSearchStepsDiag = 8,
				CornerRounding = 25
			};
			this.m_UltraPreset = new Preset
			{
				Threshold = 0.05f,
				MaxSearchSteps = 32,
				MaxSearchStepsDiag = 16,
				CornerRounding = 25
			};
		}
	}
}
