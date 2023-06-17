using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x020007C9 RID: 1993
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class BeamGeometry : MonoBehaviour
	{
		// Token: 0x0400272D RID: 10029
		private VolumetricLightBeam m_Master;

		// Token: 0x0400272E RID: 10030
		private Matrix4x4 m_ColorGradientMatrix;

		// Token: 0x0400272F RID: 10031
		private MeshType m_CurrentMeshType;

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06002B7C RID: 11132 RVA: 0x00021BDA File Offset: 0x0001FDDA
		// (set) Token: 0x06002B7D RID: 11133 RVA: 0x00021BE2 File Offset: 0x0001FDE2
		public MeshRenderer meshRenderer { get; private set; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06002B7E RID: 11134 RVA: 0x00021BEB File Offset: 0x0001FDEB
		// (set) Token: 0x06002B7F RID: 11135 RVA: 0x00021BF3 File Offset: 0x0001FDF3
		public MeshFilter meshFilter { get; private set; }

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06002B80 RID: 11136 RVA: 0x00021BFC File Offset: 0x0001FDFC
		// (set) Token: 0x06002B81 RID: 11137 RVA: 0x00021C04 File Offset: 0x0001FE04
		public Material material { get; private set; }

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06002B82 RID: 11138 RVA: 0x00021C0D File Offset: 0x0001FE0D
		// (set) Token: 0x06002B83 RID: 11139 RVA: 0x00021C15 File Offset: 0x0001FE15
		public Mesh coneMesh { get; private set; }

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06002B84 RID: 11140 RVA: 0x00021C1E File Offset: 0x0001FE1E
		// (set) Token: 0x06002B85 RID: 11141 RVA: 0x00021C2B File Offset: 0x0001FE2B
		public bool visible
		{
			get
			{
				return this.meshRenderer.enabled;
			}
			set
			{
				this.meshRenderer.enabled = value;
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06002B86 RID: 11142 RVA: 0x00021C39 File Offset: 0x0001FE39
		// (set) Token: 0x06002B87 RID: 11143 RVA: 0x00021C46 File Offset: 0x0001FE46
		public int sortingLayerID
		{
			get
			{
				return this.meshRenderer.sortingLayerID;
			}
			set
			{
				this.meshRenderer.sortingLayerID = value;
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06002B88 RID: 11144 RVA: 0x00021C54 File Offset: 0x0001FE54
		// (set) Token: 0x06002B89 RID: 11145 RVA: 0x00021C61 File Offset: 0x0001FE61
		public int sortingOrder
		{
			get
			{
				return this.meshRenderer.sortingOrder;
			}
			set
			{
				this.meshRenderer.sortingOrder = value;
			}
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x00021C6F File Offset: 0x0001FE6F
		private void Start()
		{
			if (!this.m_Master)
			{
				Object.DestroyImmediate(base.gameObject);
			}
		}

		// Token: 0x06002B8B RID: 11147 RVA: 0x00021C89 File Offset: 0x0001FE89
		private void OnDestroy()
		{
			if (this.material)
			{
				Object.DestroyImmediate(this.material);
				this.material = null;
			}
		}

		// Token: 0x06002B8C RID: 11148 RVA: 0x00021CAA File Offset: 0x0001FEAA
		private static bool IsUsingCustomRenderPipeline()
		{
			return RenderPipelineManager.currentPipeline != null || GraphicsSettings.renderPipelineAsset != null;
		}

		// Token: 0x06002B8D RID: 11149 RVA: 0x00021CC0 File Offset: 0x0001FEC0
		private void OnEnable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipeline.beginCameraRendering += new Action<Camera>(this.OnBeginCameraRendering);
			}
		}

		// Token: 0x06002B8E RID: 11150 RVA: 0x00021CDA File Offset: 0x0001FEDA
		private void OnDisable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipeline.beginCameraRendering -= new Action<Camera>(this.OnBeginCameraRendering);
			}
		}

		// Token: 0x06002B8F RID: 11151 RVA: 0x000DEDD4 File Offset: 0x000DCFD4
		public void Initialize(VolumetricLightBeam master, Shader shader)
		{
			HideFlags proceduralObjectsHideFlags = Consts.ProceduralObjectsHideFlags;
			this.m_Master = master;
			base.transform.SetParent(master.transform, false);
			this.material = new Material(shader);
			this.material.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer = base.gameObject.GetOrAddComponent<MeshRenderer>();
			this.meshRenderer.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer.material = this.material;
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			if (SortingLayer.IsValid(this.m_Master.sortingLayerID))
			{
				this.sortingLayerID = this.m_Master.sortingLayerID;
			}
			else
			{
				Debug.LogError(string.Format("Beam '{0}' has an invalid sortingLayerID ({1}). Please fix it by setting a valid layer.", Utils.GetPath(this.m_Master.transform), this.m_Master.sortingLayerID));
			}
			this.sortingOrder = this.m_Master.sortingOrder;
			this.meshFilter = base.gameObject.GetOrAddComponent<MeshFilter>();
			this.meshFilter.hideFlags = proceduralObjectsHideFlags;
			base.gameObject.hideFlags = proceduralObjectsHideFlags;
		}

		// Token: 0x06002B90 RID: 11152 RVA: 0x000DEEF8 File Offset: 0x000DD0F8
		public void RegenerateMesh()
		{
			Debug.Assert(this.m_Master);
			base.gameObject.layer = Config.Instance.geometryLayerID;
			base.gameObject.tag = Config.Instance.geometryTag;
			if (this.coneMesh && this.m_CurrentMeshType == MeshType.Custom)
			{
				Object.DestroyImmediate(this.coneMesh);
			}
			this.m_CurrentMeshType = this.m_Master.geomMeshType;
			MeshType geomMeshType = this.m_Master.geomMeshType;
			if (geomMeshType != MeshType.Shared)
			{
				if (geomMeshType == MeshType.Custom)
				{
					this.coneMesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, this.m_Master.geomCustomSides, this.m_Master.geomCustomSegments, this.m_Master.geomCap);
					this.coneMesh.hideFlags = Consts.ProceduralObjectsHideFlags;
					this.meshFilter.mesh = this.coneMesh;
				}
				else
				{
					Debug.LogError("Unsupported MeshType");
				}
			}
			else
			{
				this.coneMesh = GlobalMesh.mesh;
				this.meshFilter.sharedMesh = this.coneMesh;
			}
			this.UpdateMaterialAndBounds();
		}

		// Token: 0x06002B91 RID: 11153 RVA: 0x000DF010 File Offset: 0x000DD210
		private void ComputeLocalMatrix()
		{
			float num = Mathf.Max(this.m_Master.coneRadiusStart, this.m_Master.coneRadiusEnd);
			base.transform.localScale = new Vector3(num, num, this.m_Master.fadeEnd);
		}

		// Token: 0x06002B92 RID: 11154 RVA: 0x000DF058 File Offset: 0x000DD258
		public void UpdateMaterialAndBounds()
		{
			Debug.Assert(this.m_Master);
			this.material.renderQueue = Config.Instance.geometryRenderQueue;
			float f = this.m_Master.coneAngle * 0.017453292f / 2f;
			this.material.SetVector("_ConeSlopeCosSin", new Vector2(Mathf.Cos(f), Mathf.Sin(f)));
			Vector2 v = new Vector2(Mathf.Max(this.m_Master.coneRadiusStart, 0.0001f), Mathf.Max(this.m_Master.coneRadiusEnd, 0.0001f));
			this.material.SetVector("_ConeRadius", v);
			float value = Mathf.Sign(this.m_Master.coneApexOffsetZ) * Mathf.Max(Mathf.Abs(this.m_Master.coneApexOffsetZ), 0.0001f);
			this.material.SetFloat("_ConeApexOffsetZ", value);
			if (this.m_Master.colorMode == ColorMode.Gradient)
			{
				Utils.FloatPackingPrecision floatPackingPrecision = Utils.GetFloatPackingPrecision();
				this.material.EnableKeyword((floatPackingPrecision == Utils.FloatPackingPrecision.High) ? "VLB_COLOR_GRADIENT_MATRIX_HIGH" : "VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.m_ColorGradientMatrix = this.m_Master.colorGradient.SampleInMatrix((int)floatPackingPrecision);
			}
			else
			{
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_HIGH");
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.material.SetColor("_ColorFlat", this.m_Master.color);
			}
			if (Consts.BlendingMode_AlphaAsBlack[this.m_Master.blendingModeAsInt])
			{
				this.material.EnableKeyword("ALPHA_AS_BLACK");
			}
			else
			{
				this.material.DisableKeyword("ALPHA_AS_BLACK");
			}
			this.material.SetInt("_BlendSrcFactor", (int)Consts.BlendingMode_SrcFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetInt("_BlendDstFactor", (int)Consts.BlendingMode_DstFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetFloat("_AlphaInside", this.m_Master.alphaInside);
			this.material.SetFloat("_AlphaOutside", this.m_Master.alphaOutside);
			this.material.SetFloat("_AttenuationLerpLinearQuad", this.m_Master.attenuationLerpLinearQuad);
			this.material.SetFloat("_DistanceFadeStart", this.m_Master.fadeStart);
			this.material.SetFloat("_DistanceFadeEnd", this.m_Master.fadeEnd);
			this.material.SetFloat("_DistanceCamClipping", this.m_Master.cameraClippingDistance);
			this.material.SetFloat("_FresnelPow", Mathf.Max(0.001f, this.m_Master.fresnelPow));
			this.material.SetFloat("_GlareBehind", this.m_Master.glareBehind);
			this.material.SetFloat("_GlareFrontal", this.m_Master.glareFrontal);
			this.material.SetFloat("_DrawCap", (float)(this.m_Master.geomCap ? 1 : 0));
			if (this.m_Master.depthBlendDistance > 0f)
			{
				this.material.EnableKeyword("VLB_DEPTH_BLEND");
				this.material.SetFloat("_DepthBlendDistance", this.m_Master.depthBlendDistance);
			}
			else
			{
				this.material.DisableKeyword("VLB_DEPTH_BLEND");
			}
			if (this.m_Master.noiseEnabled && this.m_Master.noiseIntensity > 0f && Noise3D.isSupported)
			{
				Noise3D.LoadIfNeeded();
				this.material.EnableKeyword("VLB_NOISE_3D");
				this.material.SetVector("_NoiseLocal", new Vector4(this.m_Master.noiseVelocityLocal.x, this.m_Master.noiseVelocityLocal.y, this.m_Master.noiseVelocityLocal.z, this.m_Master.noiseScaleLocal));
				this.material.SetVector("_NoiseParam", new Vector3(this.m_Master.noiseIntensity, this.m_Master.noiseVelocityUseGlobal ? 1f : 0f, this.m_Master.noiseScaleUseGlobal ? 1f : 0f));
			}
			else
			{
				this.material.DisableKeyword("VLB_NOISE_3D");
			}
			this.ComputeLocalMatrix();
		}

		// Token: 0x06002B93 RID: 11155 RVA: 0x000DF4B8 File Offset: 0x000DD6B8
		public void SetClippingPlane(Plane planeWS)
		{
			Vector3 normal = planeWS.normal;
			this.material.EnableKeyword("VLB_CLIPPING_PLANE");
			this.material.SetVector("_ClippingPlaneWS", new Vector4(normal.x, normal.y, normal.z, planeWS.distance));
		}

		// Token: 0x06002B94 RID: 11156 RVA: 0x00021CF4 File Offset: 0x0001FEF4
		public void SetClippingPlaneOff()
		{
			this.material.DisableKeyword("VLB_CLIPPING_PLANE");
		}

		// Token: 0x06002B95 RID: 11157 RVA: 0x00021D06 File Offset: 0x0001FF06
		private void OnBeginCameraRendering(Camera cam)
		{
			this.UpdateCameraRelatedProperties(cam);
		}

		// Token: 0x06002B96 RID: 11158 RVA: 0x000DF50C File Offset: 0x000DD70C
		private void OnWillRenderObject()
		{
			if (!BeamGeometry.IsUsingCustomRenderPipeline())
			{
				Camera current = Camera.current;
				if (current != null)
				{
					this.UpdateCameraRelatedProperties(current);
				}
			}
		}

		// Token: 0x06002B97 RID: 11159 RVA: 0x000DF538 File Offset: 0x000DD738
		private void UpdateCameraRelatedProperties(Camera cam)
		{
			if (cam && this.m_Master)
			{
				if (this.material)
				{
					Vector3 vector = this.m_Master.transform.InverseTransformPoint(cam.transform.position);
					this.material.SetVector("_CameraPosObjectSpace", vector);
					Vector3 normalized = base.transform.InverseTransformDirection(cam.transform.forward).normalized;
					float w = cam.orthographic ? -1f : this.m_Master.GetInsideBeamFactorFromObjectSpacePos(vector);
					this.material.SetVector("_CameraParams", new Vector4(normalized.x, normalized.y, normalized.z, w));
					if (this.m_Master.colorMode == ColorMode.Gradient)
					{
						this.material.SetMatrix("_ColorGradientMatrix", this.m_ColorGradientMatrix);
					}
				}
				if (this.m_Master.depthBlendDistance > 0f)
				{
					cam.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
		}
	}
}
