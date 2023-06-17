using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x020007DE RID: 2014
	[SelectionBase]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class VolumetricLightBeam : MonoBehaviour
	{
		// Token: 0x040027C8 RID: 10184
		public bool colorFromLight = true;

		// Token: 0x040027C9 RID: 10185
		public ColorMode colorMode;

		// Token: 0x040027CA RID: 10186
		[FormerlySerializedAs("colorValue")]
		[ColorUsage(true, true)]
		public Color color = Consts.FlatColor;

		// Token: 0x040027CB RID: 10187
		public Gradient colorGradient;

		// Token: 0x040027CC RID: 10188
		[Range(0f, 1f)]
		public float alphaInside = 1f;

		// Token: 0x040027CD RID: 10189
		[FormerlySerializedAs("alpha")]
		[Range(0f, 1f)]
		public float alphaOutside = 1f;

		// Token: 0x040027CE RID: 10190
		public BlendingMode blendingMode;

		// Token: 0x040027CF RID: 10191
		[FormerlySerializedAs("angleFromLight")]
		public bool spotAngleFromLight = true;

		// Token: 0x040027D0 RID: 10192
		[Range(0.1f, 179.9f)]
		public float spotAngle = 35f;

		// Token: 0x040027D1 RID: 10193
		[FormerlySerializedAs("radiusStart")]
		public float coneRadiusStart = 0.1f;

		// Token: 0x040027D2 RID: 10194
		public MeshType geomMeshType;

		// Token: 0x040027D3 RID: 10195
		[FormerlySerializedAs("geomSides")]
		public int geomCustomSides = 18;

		// Token: 0x040027D4 RID: 10196
		public int geomCustomSegments = 5;

		// Token: 0x040027D5 RID: 10197
		public bool geomCap;

		// Token: 0x040027D6 RID: 10198
		public bool fadeEndFromLight = true;

		// Token: 0x040027D7 RID: 10199
		public AttenuationEquation attenuationEquation = AttenuationEquation.Quadratic;

		// Token: 0x040027D8 RID: 10200
		[Range(0f, 1f)]
		public float attenuationCustomBlending = 0.5f;

		// Token: 0x040027D9 RID: 10201
		public float fadeStart;

		// Token: 0x040027DA RID: 10202
		public float fadeEnd = 3f;

		// Token: 0x040027DB RID: 10203
		public float depthBlendDistance = 2f;

		// Token: 0x040027DC RID: 10204
		public float cameraClippingDistance = 0.5f;

		// Token: 0x040027DD RID: 10205
		[Range(0f, 1f)]
		public float glareFrontal = 0.5f;

		// Token: 0x040027DE RID: 10206
		[Range(0f, 1f)]
		public float glareBehind = 0.5f;

		// Token: 0x040027DF RID: 10207
		[Obsolete("Use 'glareFrontal' instead")]
		public float boostDistanceInside = 0.5f;

		// Token: 0x040027E0 RID: 10208
		[Obsolete("This property has been merged with 'fresnelPow'")]
		public float fresnelPowInside = 6f;

		// Token: 0x040027E1 RID: 10209
		[FormerlySerializedAs("fresnelPowOutside")]
		public float fresnelPow = 8f;

		// Token: 0x040027E2 RID: 10210
		public bool noiseEnabled;

		// Token: 0x040027E3 RID: 10211
		[Range(0f, 1f)]
		public float noiseIntensity = 0.5f;

		// Token: 0x040027E4 RID: 10212
		public bool noiseScaleUseGlobal = true;

		// Token: 0x040027E5 RID: 10213
		[Range(0.01f, 2f)]
		public float noiseScaleLocal = 0.5f;

		// Token: 0x040027E6 RID: 10214
		public bool noiseVelocityUseGlobal = true;

		// Token: 0x040027E7 RID: 10215
		public Vector3 noiseVelocityLocal = Consts.NoiseVelocityDefault;

		// Token: 0x040027E8 RID: 10216
		private Plane m_PlaneWS;

		// Token: 0x040027E9 RID: 10217
		[SerializeField]
		private int pluginVersion = -1;

		// Token: 0x040027EA RID: 10218
		[FormerlySerializedAs("trackChangesDuringPlaytime")]
		[SerializeField]
		private bool _TrackChangesDuringPlaytime;

		// Token: 0x040027EB RID: 10219
		[SerializeField]
		private int _SortingLayerID;

		// Token: 0x040027EC RID: 10220
		[SerializeField]
		private int _SortingOrder;

		// Token: 0x040027ED RID: 10221
		private BeamGeometry m_BeamGeom;

		// Token: 0x040027EE RID: 10222
		private Coroutine m_CoPlaytimeUpdate;

		// Token: 0x040027EF RID: 10223
		private Light _CachedLight;

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06002BEE RID: 11246 RVA: 0x000221F8 File Offset: 0x000203F8
		public float coneAngle
		{
			get
			{
				return Mathf.Atan2(this.coneRadiusEnd - this.coneRadiusStart, this.fadeEnd) * 57.29578f * 2f;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06002BEF RID: 11247 RVA: 0x0002221E File Offset: 0x0002041E
		public float coneRadiusEnd
		{
			get
			{
				return this.fadeEnd * Mathf.Tan(this.spotAngle * 0.017453292f * 0.5f);
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06002BF0 RID: 11248 RVA: 0x000E0B58 File Offset: 0x000DED58
		public float coneVolume
		{
			get
			{
				float num = this.coneRadiusStart;
				float coneRadiusEnd = this.coneRadiusEnd;
				return 1.0471976f * (num * num + num * coneRadiusEnd + coneRadiusEnd * coneRadiusEnd) * this.fadeEnd;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06002BF1 RID: 11249 RVA: 0x000E0B8C File Offset: 0x000DED8C
		public float coneApexOffsetZ
		{
			get
			{
				float num = this.coneRadiusStart / this.coneRadiusEnd;
				if (num != 1f)
				{
					return this.fadeEnd * num / (1f - num);
				}
				return float.MaxValue;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06002BF2 RID: 11250 RVA: 0x0002223E File Offset: 0x0002043E
		// (set) Token: 0x06002BF3 RID: 11251 RVA: 0x0002225A File Offset: 0x0002045A
		public int geomSides
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSides;
				}
				return this.geomCustomSides;
			}
			set
			{
				this.geomCustomSides = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides.");
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06002BF4 RID: 11252 RVA: 0x0002226D File Offset: 0x0002046D
		// (set) Token: 0x06002BF5 RID: 11253 RVA: 0x00022289 File Offset: 0x00020489
		public int geomSegments
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSegments;
				}
				return this.geomCustomSegments;
			}
			set
			{
				this.geomCustomSegments = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments.");
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06002BF6 RID: 11254 RVA: 0x0002229C File Offset: 0x0002049C
		public float attenuationLerpLinearQuad
		{
			get
			{
				if (this.attenuationEquation == AttenuationEquation.Linear)
				{
					return 0f;
				}
				if (this.attenuationEquation == AttenuationEquation.Quadratic)
				{
					return 1f;
				}
				return this.attenuationCustomBlending;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06002BF7 RID: 11255 RVA: 0x000222C1 File Offset: 0x000204C1
		// (set) Token: 0x06002BF8 RID: 11256 RVA: 0x000222C9 File Offset: 0x000204C9
		public int sortingLayerID
		{
			get
			{
				return this._SortingLayerID;
			}
			set
			{
				this._SortingLayerID = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingLayerID = value;
				}
			}
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06002BF9 RID: 11257 RVA: 0x000222EB File Offset: 0x000204EB
		// (set) Token: 0x06002BFA RID: 11258 RVA: 0x000222F8 File Offset: 0x000204F8
		public string sortingLayerName
		{
			get
			{
				return SortingLayer.IDToName(this.sortingLayerID);
			}
			set
			{
				this.sortingLayerID = SortingLayer.NameToID(value);
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06002BFB RID: 11259 RVA: 0x00022306 File Offset: 0x00020506
		// (set) Token: 0x06002BFC RID: 11260 RVA: 0x0002230E File Offset: 0x0002050E
		public int sortingOrder
		{
			get
			{
				return this._SortingOrder;
			}
			set
			{
				this._SortingOrder = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingOrder = value;
				}
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06002BFD RID: 11261 RVA: 0x00022330 File Offset: 0x00020530
		// (set) Token: 0x06002BFE RID: 11262 RVA: 0x00022338 File Offset: 0x00020538
		public bool trackChangesDuringPlaytime
		{
			get
			{
				return this._TrackChangesDuringPlaytime;
			}
			set
			{
				this._TrackChangesDuringPlaytime = value;
				this.StartPlaytimeUpdateIfNeeded();
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06002BFF RID: 11263 RVA: 0x00022347 File Offset: 0x00020547
		public bool isCurrentlyTrackingChanges
		{
			get
			{
				return this.m_CoPlaytimeUpdate != null;
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06002C00 RID: 11264 RVA: 0x00022352 File Offset: 0x00020552
		public bool hasGeometry
		{
			get
			{
				return this.m_BeamGeom != null;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06002C01 RID: 11265 RVA: 0x00022360 File Offset: 0x00020560
		public Bounds bounds
		{
			get
			{
				if (!(this.m_BeamGeom != null))
				{
					return new Bounds(Vector3.zero, Vector3.zero);
				}
				return this.m_BeamGeom.meshRenderer.bounds;
			}
		}

		// Token: 0x06002C02 RID: 11266 RVA: 0x00022390 File Offset: 0x00020590
		public void SetClippingPlane(Plane planeWS)
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlane(planeWS);
			}
			this.m_PlaneWS = planeWS;
		}

		// Token: 0x06002C03 RID: 11267 RVA: 0x000223B2 File Offset: 0x000205B2
		public void SetClippingPlaneOff()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlaneOff();
			}
			this.m_PlaneWS = default(Plane);
		}

		// Token: 0x06002C04 RID: 11268 RVA: 0x000E0BC8 File Offset: 0x000DEDC8
		public bool IsColliderHiddenByDynamicOccluder(Collider collider)
		{
			Debug.Assert(collider, "You should pass a valid Collider to VLB.VolumetricLightBeam.IsColliderHiddenByDynamicOccluder");
			return this.m_PlaneWS.IsValid() && !GeometryUtility.TestPlanesAABB(new Plane[]
			{
				this.m_PlaneWS
			}, collider.bounds);
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06002C05 RID: 11269 RVA: 0x000223D8 File Offset: 0x000205D8
		public int blendingModeAsInt
		{
			get
			{
				return Mathf.Clamp((int)this.blendingMode, 0, Enum.GetValues(typeof(BlendingMode)).Length);
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06002C06 RID: 11270 RVA: 0x000E0C18 File Offset: 0x000DEE18
		public string meshStats
		{
			get
			{
				Mesh mesh = this.m_BeamGeom ? this.m_BeamGeom.coneMesh : null;
				if (mesh)
				{
					return string.Format("Cone angle: {0:0.0} degrees\nMesh: {1} vertices, {2} triangles", this.coneAngle, mesh.vertexCount, mesh.triangles.Length / 3);
				}
				return "no mesh available";
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06002C07 RID: 11271 RVA: 0x000223FA File Offset: 0x000205FA
		public int meshVerticesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.vertexCount;
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06002C08 RID: 11272 RVA: 0x0002242D File Offset: 0x0002062D
		public int meshTrianglesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.triangles.Length / 3;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06002C09 RID: 11273 RVA: 0x00022464 File Offset: 0x00020664
		private Light lightSpotAttached
		{
			get
			{
				if (this._CachedLight == null)
				{
					this._CachedLight = base.GetComponent<Light>();
				}
				if (this._CachedLight && this._CachedLight.type == LightType.Spot)
				{
					return this._CachedLight;
				}
				return null;
			}
		}

		// Token: 0x06002C0A RID: 11274 RVA: 0x000224A2 File Offset: 0x000206A2
		public float GetInsideBeamFactor(Vector3 posWS)
		{
			return this.GetInsideBeamFactorFromObjectSpacePos(base.transform.InverseTransformPoint(posWS));
		}

		// Token: 0x06002C0B RID: 11275 RVA: 0x000E0C80 File Offset: 0x000DEE80
		public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
		{
			if (posOS.z < 0f)
			{
				return -1f;
			}
			Vector2 normalized = new Vector2(posOS.xy().magnitude, posOS.z + this.coneApexOffsetZ).normalized;
			return Mathf.Clamp((Mathf.Abs(Mathf.Sin(this.coneAngle * 0.017453292f / 2f)) - Mathf.Abs(normalized.x)) / 0.1f, -1f, 1f);
		}

		// Token: 0x06002C0C RID: 11276 RVA: 0x000224B6 File Offset: 0x000206B6
		[Obsolete("Use 'GenerateGeometry()' instead")]
		public void Generate()
		{
			this.GenerateGeometry();
		}

		// Token: 0x06002C0D RID: 11277 RVA: 0x000E0D08 File Offset: 0x000DEF08
		public virtual void GenerateGeometry()
		{
			this.HandleBackwardCompatibility(this.pluginVersion, 1510);
			this.pluginVersion = 1510;
			this.ValidateProperties();
			if (this.m_BeamGeom == null)
			{
				Shader beamShader = Config.Instance.beamShader;
				if (!beamShader)
				{
					Debug.LogError("Invalid BeamShader set in VLB Config");
					return;
				}
				this.m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
				this.m_BeamGeom.Initialize(this, beamShader);
			}
			this.m_BeamGeom.RegenerateMesh();
			this.m_BeamGeom.visible = base.enabled;
		}

		// Token: 0x06002C0E RID: 11278 RVA: 0x000224BE File Offset: 0x000206BE
		public virtual void UpdateAfterManualPropertyChange()
		{
			this.ValidateProperties();
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.UpdateMaterialAndBounds();
			}
		}

		// Token: 0x06002C0F RID: 11279 RVA: 0x000224B6 File Offset: 0x000206B6
		private void Start()
		{
			this.GenerateGeometry();
		}

		// Token: 0x06002C10 RID: 11280 RVA: 0x000224DE File Offset: 0x000206DE
		private void OnEnable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = true;
			}
			this.StartPlaytimeUpdateIfNeeded();
		}

		// Token: 0x06002C11 RID: 11281 RVA: 0x000224FF File Offset: 0x000206FF
		private void OnDisable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = false;
			}
			this.m_CoPlaytimeUpdate = null;
		}

		// Token: 0x06002C12 RID: 11282 RVA: 0x00002ECE File Offset: 0x000010CE
		private void StartPlaytimeUpdateIfNeeded()
		{
		}

		// Token: 0x06002C13 RID: 11283 RVA: 0x00022521 File Offset: 0x00020721
		private IEnumerator CoPlaytimeUpdate()
		{
			while (this.trackChangesDuringPlaytime && base.enabled)
			{
				this.UpdateAfterManualPropertyChange();
				yield return null;
			}
			this.m_CoPlaytimeUpdate = null;
			yield break;
		}

		// Token: 0x06002C14 RID: 11284 RVA: 0x00022530 File Offset: 0x00020730
		private void OnDestroy()
		{
			this.DestroyBeam();
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x00022538 File Offset: 0x00020738
		private void DestroyBeam()
		{
			if (this.m_BeamGeom)
			{
				Object.DestroyImmediate(this.m_BeamGeom.gameObject);
			}
			this.m_BeamGeom = null;
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x000E0D9C File Offset: 0x000DEF9C
		private void AssignPropertiesFromSpotLight(Light lightSpot)
		{
			if (lightSpot && lightSpot.type == LightType.Spot)
			{
				if (this.fadeEndFromLight)
				{
					this.fadeEnd = lightSpot.range;
				}
				if (this.spotAngleFromLight)
				{
					this.spotAngle = lightSpot.spotAngle;
				}
				if (this.colorFromLight)
				{
					this.colorMode = ColorMode.Flat;
					this.color = lightSpot.color;
				}
			}
		}

		// Token: 0x06002C17 RID: 11287 RVA: 0x000E0DFC File Offset: 0x000DEFFC
		private void ClampProperties()
		{
			this.alphaInside = Mathf.Clamp01(this.alphaInside);
			this.alphaOutside = Mathf.Clamp01(this.alphaOutside);
			this.attenuationCustomBlending = Mathf.Clamp01(this.attenuationCustomBlending);
			this.fadeEnd = Mathf.Max(0.01f, this.fadeEnd);
			this.fadeStart = Mathf.Clamp(this.fadeStart, 0f, this.fadeEnd - 0.01f);
			this.spotAngle = Mathf.Clamp(this.spotAngle, 0.1f, 179.9f);
			this.coneRadiusStart = Mathf.Max(this.coneRadiusStart, 0f);
			this.depthBlendDistance = Mathf.Max(this.depthBlendDistance, 0f);
			this.cameraClippingDistance = Mathf.Max(this.cameraClippingDistance, 0f);
			this.geomCustomSides = Mathf.Clamp(this.geomCustomSides, 3, 256);
			this.geomCustomSegments = Mathf.Clamp(this.geomCustomSegments, 0, 64);
			this.fresnelPow = Mathf.Max(0f, this.fresnelPow);
			this.glareBehind = Mathf.Clamp01(this.glareBehind);
			this.glareFrontal = Mathf.Clamp01(this.glareFrontal);
			this.noiseIntensity = Mathf.Clamp(this.noiseIntensity, 0f, 1f);
		}

		// Token: 0x06002C18 RID: 11288 RVA: 0x0002255E File Offset: 0x0002075E
		private void ValidateProperties()
		{
			this.AssignPropertiesFromSpotLight(this.lightSpotAttached);
			this.ClampProperties();
		}

		// Token: 0x06002C19 RID: 11289 RVA: 0x00022572 File Offset: 0x00020772
		private void HandleBackwardCompatibility(int serializedVersion, int newVersion)
		{
			if (serializedVersion == -1)
			{
				return;
			}
			if (serializedVersion == newVersion)
			{
				return;
			}
			if (serializedVersion < 1301)
			{
				this.attenuationEquation = AttenuationEquation.Linear;
			}
			if (serializedVersion < 1501)
			{
				this.geomMeshType = MeshType.Custom;
				this.geomCustomSegments = 5;
			}
			Utils.MarkCurrentSceneDirty();
		}
	}
}
