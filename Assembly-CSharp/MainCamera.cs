using System;
using ConVar;
using Kino;
using Rust;
using Rust.Components.Camera;
using Smaa;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.CinematicEffects;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200020B RID: 523
[ExecuteInEditMode]
public class MainCamera : SingletonComponent<MainCamera>
{
	// Token: 0x04000CDF RID: 3295
	public static Camera mainCamera;

	// Token: 0x04000CE0 RID: 3296
	public DepthOfField dof;

	// Token: 0x04000CE1 RID: 3297
	public AmplifyOcclusionEffect ssao;

	// Token: 0x04000CE2 RID: 3298
	public Motion motionBlur;

	// Token: 0x04000CE3 RID: 3299
	public TOD_Rays shafts;

	// Token: 0x04000CE4 RID: 3300
	public TonemappingColorGrading tonemappingColorGrading;

	// Token: 0x04000CE5 RID: 3301
	public FXAA fxaa;

	// Token: 0x04000CE6 RID: 3302
	public SMAA smaa;

	// Token: 0x04000CE7 RID: 3303
	public PostProcessLayer post;

	// Token: 0x04000CE8 RID: 3304
	public CC_SharpenAndVignette sharpenAndVignette;

	// Token: 0x04000CE9 RID: 3305
	public SEScreenSpaceShadows contactShadows;

	// Token: 0x04000CEA RID: 3306
	public VisualizeTexelDensity visualizeTexelDensity;

	// Token: 0x04000CEB RID: 3307
	public EnvironmentVolumePropertiesCollection environmentVolumeProperties;

	// Token: 0x04000CEC RID: 3308
	private TOD_Scattering todScattering;

	// Token: 0x04000CED RID: 3309
	private float ambientLightDay = -1f;

	// Token: 0x04000CEE RID: 3310
	private float ambientLightNight = -1f;

	// Token: 0x04000CEF RID: 3311
	private float ambientLightMultiplier = 1f;

	// Token: 0x04000CF0 RID: 3312
	private float ambientLightMultiplierTarget = 1f;

	// Token: 0x04000CF1 RID: 3313
	private float skyReflectionDay = -1f;

	// Token: 0x04000CF2 RID: 3314
	private float skyReflectionNight = -1f;

	// Token: 0x04000CF3 RID: 3315
	private float skyReflectionMultiplier = 1f;

	// Token: 0x04000CF4 RID: 3316
	private float skyReflectionMultiplierTarget = 1f;

	// Token: 0x04000CF5 RID: 3317
	private LayerMask skyReflectionCullingFlags = 0;

	// Token: 0x04000CF6 RID: 3318
	private static SphericalHarmonicsL2[] lightProbe = new SphericalHarmonicsL2[1];

	// Token: 0x04000CF7 RID: 3319
	private float environmentTimestamp;

	// Token: 0x04000CF8 RID: 3320
	private float environmentTransitionSpeed = 1f;

	// Token: 0x04000CF9 RID: 3321
	private EnvironmentType environmentType;

	// Token: 0x04000CFA RID: 3322
	private bool waterVisible;

	// Token: 0x04000CFB RID: 3323
	private int screenWidth;

	// Token: 0x04000CFC RID: 3324
	private int screenHeight;

	// Token: 0x04000CFD RID: 3325
	private Vector3 lastPosition;

	// Token: 0x04000CFE RID: 3326
	private Quaternion lastRotation;

	// Token: 0x04000CFF RID: 3327
	private const float defaultDistance = 4096f;

	// Token: 0x04000D00 RID: 3328
	private const float defaultSqrDistance = 16777216f;

	// Token: 0x04000D01 RID: 3329
	public static MainCamera.DepthOfFieldSettings depthOfField;

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000FFF RID: 4095 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
	public static bool isValid
	{
		get
		{
			return MainCamera.mainCamera != null && MainCamera.mainCamera.enabled;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06001000 RID: 4096 RVA: 0x0000E1FF File Offset: 0x0000C3FF
	// (set) Token: 0x06001001 RID: 4097 RVA: 0x0000E210 File Offset: 0x0000C410
	public static Vector3 position
	{
		get
		{
			return MainCamera.mainCamera.transform.position;
		}
		set
		{
			MainCamera.mainCamera.transform.position = value;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06001002 RID: 4098 RVA: 0x0000E222 File Offset: 0x0000C422
	// (set) Token: 0x06001003 RID: 4099 RVA: 0x0000E233 File Offset: 0x0000C433
	public static Vector3 forward
	{
		get
		{
			return MainCamera.mainCamera.transform.forward;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.forward = value;
			}
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06001004 RID: 4100 RVA: 0x0000E253 File Offset: 0x0000C453
	// (set) Token: 0x06001005 RID: 4101 RVA: 0x0000E264 File Offset: 0x0000C464
	public static Vector3 right
	{
		get
		{
			return MainCamera.mainCamera.transform.right;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.right = value;
			}
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06001006 RID: 4102 RVA: 0x0000E284 File Offset: 0x0000C484
	// (set) Token: 0x06001007 RID: 4103 RVA: 0x0000E295 File Offset: 0x0000C495
	public static Vector3 up
	{
		get
		{
			return MainCamera.mainCamera.transform.up;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.up = value;
			}
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06001008 RID: 4104 RVA: 0x0000E2B5 File Offset: 0x0000C4B5
	public static Quaternion rotation
	{
		get
		{
			return MainCamera.mainCamera.transform.rotation;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06001009 RID: 4105 RVA: 0x0000E2C6 File Offset: 0x0000C4C6
	public static Ray Ray
	{
		get
		{
			return new Ray(MainCamera.position, MainCamera.forward);
		}
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x0600100A RID: 4106 RVA: 0x0006C740 File Offset: 0x0006A940
	public static RaycastHit Raycast
	{
		get
		{
			RaycastHit result;
			Physics.Raycast(MainCamera.Ray, ref result, 1024f, 229731073);
			return result;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x0600100B RID: 4107 RVA: 0x0000E2D7 File Offset: 0x0000C4D7
	public static SphericalHarmonicsL2[] LightProbe
	{
		get
		{
			return MainCamera.lightProbe;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x0600100C RID: 4108 RVA: 0x0000E2DE File Offset: 0x0000C4DE
	public static bool isWaterVisible
	{
		get
		{
			return !(SingletonComponent<MainCamera>.Instance != null) || SingletonComponent<MainCamera>.Instance.waterVisible;
		}
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x0000E2F9 File Offset: 0x0000C4F9
	public static bool InEnvironment(EnvironmentType type)
	{
		return !(SingletonComponent<MainCamera>.Instance == null) && (SingletonComponent<MainCamera>.Instance.environmentType & type) > (EnvironmentType)0;
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x0000E319 File Offset: 0x0000C519
	protected override void Awake()
	{
		base.Awake();
		MainCamera.mainCamera = base.GetComponent<Camera>();
		this.todScattering = base.GetComponent<TOD_Scattering>();
		MainCamera.UpdateLightProbe();
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x0000E33D File Offset: 0x0000C53D
	private void OnEnable()
	{
		MainCamera.mainCamera = base.GetComponent<Camera>();
		this.todScattering = base.GetComponent<TOD_Scattering>();
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
		ConVar.Graphics.af = ConVar.Graphics.af;
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x0000E376 File Offset: 0x0000C576
	public static void ClearCameraFrustumCorners()
	{
		Shader.SetGlobalMatrix("_FrustumNearCorners", Matrix4x4.identity);
		Shader.SetGlobalMatrix("_FrustumRayCorners", Matrix4x4.identity);
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x0006C768 File Offset: 0x0006A968
	public static void ComputeCameraFrustumCorners(Camera camera, out Matrix4x4 nearCorners, out Matrix4x4 rayCorners)
	{
		Vector3 position = camera.transform.position;
		Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
		Matrix4x4 lhs = Matrix4x4.Inverse(GL.GetGPUProjectionMatrix(camera.projectionMatrix, false) * worldToCameraMatrix);
		Vector4 vector = lhs * new Vector4(-1f, 1f, 1f, 1f);
		Vector4 vector2 = lhs * new Vector4(1f, 1f, 1f, 1f);
		Vector4 vector3 = lhs * new Vector4(1f, -1f, 1f, 1f);
		Vector4 vector4 = lhs * new Vector4(-1f, -1f, 1f, 1f);
		Vector4 vector5 = lhs * new Vector4(-1f, 1f, 0f, 1f);
		Vector4 vector6 = lhs * new Vector4(1f, 1f, 0f, 1f);
		Vector4 vector7 = lhs * new Vector4(1f, -1f, 0f, 1f);
		Vector4 vector8 = lhs * new Vector4(-1f, -1f, 0f, 1f);
		nearCorners = Matrix4x4.identity;
		nearCorners.SetRow(0, vector / vector.w);
		nearCorners.SetRow(1, vector2 / vector2.w);
		nearCorners.SetRow(2, vector3 / vector3.w);
		nearCorners.SetRow(3, vector4 / vector4.w);
		rayCorners = Matrix4x4.identity;
		rayCorners.SetRow(0, vector5 / vector5.w - position);
		rayCorners.SetRow(1, vector6 / vector6.w - position);
		rayCorners.SetRow(2, vector7 / vector7.w - position);
		rayCorners.SetRow(3, vector8 / vector8.w - position);
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x0006C9C8 File Offset: 0x0006ABC8
	public void UpdateCameraFrustumCorners(Camera camera, CommandBuffer cb = null)
	{
		Matrix4x4 value;
		Matrix4x4 value2;
		MainCamera.ComputeCameraFrustumCorners(camera, out value, out value2);
		Shader.SetGlobalMatrix("_FrustumNearCorners", value);
		Shader.SetGlobalMatrix("_FrustumRayCorners", value2);
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x0006C9F8 File Offset: 0x0006ABF8
	private void UpdateDepthOfField()
	{
		if (this.dof)
		{
			using (TimeWarning.New("DepthOfFieldFocusPoint.Evaluate", 0.1f))
			{
				DepthOfFieldFocusPoint depthOfFieldFocusPoint = DepthOfFieldFocusPoint.Evaluate(MainCamera.mainCamera);
				if (depthOfFieldFocusPoint != null)
				{
					MainCamera.depthOfField.wants = true;
					MainCamera.depthOfField.blurSize = 7f;
					MainCamera.depthOfField.focalSize = 0.2f;
					MainCamera.depthOfField.focalDistance = MainCamera.Distance(depthOfFieldFocusPoint.FocusPoint);
					if (BaseViewModel.ActiveModel != null)
					{
						MainCamera.depthOfField.focalDistance = 0.2f;
						MainCamera.depthOfField.focalSize = 0.6f;
						MainCamera.depthOfField.blurSize = 12f;
					}
				}
			}
			if (MainCamera.depthOfField.wants)
			{
				this.dof.enabled = true;
				this.dof.focalLength = MainCamera.depthOfField.focalDistance;
				this.dof.focalSize = MainCamera.depthOfField.focalSize;
				this.dof.aperture = 9f;
				this.dof.maxBlurSize = MainCamera.depthOfField.blurSize;
				this.dof.highResolution = true;
				this.dof.blurSampleCount = 2;
			}
			else
			{
				if (ConVar.Graphics.dof)
				{
					using (TimeWarning.New("ConVar.Graphics.dof", 0.1f))
					{
						this.dof.enabled = true;
						RaycastHit raycastHit;
						if (Physics.Raycast(new Ray(MainCamera.position, MainCamera.forward), ref raycastHit, 128f))
						{
							this.dof.focalLength = Mathf.Lerp(this.dof.focalLength, raycastHit.distance, 5f * UnityEngine.Time.deltaTime);
							using (TimeWarning.New("Find Eyes", 0.1f))
							{
								BaseEntity baseEntity = raycastHit.collider.gameObject.ToBaseEntity();
								if (baseEntity)
								{
									Transform eyeTransform = baseEntity.GetEyeTransform();
									if (eyeTransform)
									{
										this.dof.focalLength = MainCamera.Distance(eyeTransform.position);
									}
								}
								goto IL_23B;
							}
						}
						this.dof.focalLength = Mathf.Lerp(this.dof.focalLength, 128f, 2f * UnityEngine.Time.deltaTime);
						IL_23B:
						this.dof.focalSize = 0.4f;
						this.dof.aperture = ConVar.Graphics.dof_aper;
						this.dof.maxBlurSize = ConVar.Graphics.dof_blur;
						this.dof.highResolution = true;
						this.dof.blurSampleCount = 2;
						goto IL_29B;
					}
				}
				this.dof.enabled = false;
			}
		}
		IL_29B:
		MainCamera.depthOfField.wants = false;
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x0000E396 File Offset: 0x0000C596
	private void ResetColorGrading()
	{
		this.tonemappingColorGrading.enabled = false;
		this.tonemappingColorGrading.enabled = true;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x0006CCF8 File Offset: 0x0006AEF8
	private void UpdateColorGrading()
	{
		if (this.tonemappingColorGrading != null)
		{
			bool enabled = this.tonemappingColorGrading.lut.enabled;
			bool flag = true;
			if (this.tonemappingColorGrading.enabled && (this.screenWidth != Screen.width || this.screenHeight != Screen.height))
			{
				if (Application.isPlaying)
				{
					base.Invoke(new Action(this.ResetColorGrading), 0.5f);
				}
				this.screenWidth = Screen.width;
				this.screenHeight = Screen.height;
			}
			if ((flag && !enabled) || (!flag && enabled))
			{
				TonemappingColorGrading.LUTSettings lut = this.tonemappingColorGrading.lut;
				lut.enabled = flag;
				lut.texture = (flag ? lut.texture : null);
				this.tonemappingColorGrading.lut = lut;
			}
		}
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x0006CDC8 File Offset: 0x0006AFC8
	private void UpdateAntiAliasing()
	{
		if (this.fxaa != null)
		{
			this.fxaa.enabled = (Effects.antialiasing == 1);
		}
		if (this.smaa != null)
		{
			this.smaa.enabled = (Effects.antialiasing == 2);
		}
		if (this.post != null)
		{
			this.post.enabled = (Effects.antialiasing == 3);
		}
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x0006CE38 File Offset: 0x0006B038
	private void UpdateSharpenAndVignette()
	{
		if (this.sharpenAndVignette != null)
		{
			this.sharpenAndVignette.enabled = (Effects.sharpen || Effects.vignet);
			this.sharpenAndVignette.applySharpen = Effects.sharpen;
			this.sharpenAndVignette.applyVignette = Effects.vignet;
		}
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x0000E3B0 File Offset: 0x0000C5B0
	private void UpdateSSAO()
	{
		if (this.ssao != null)
		{
			this.ssao.enabled = Effects.ao;
		}
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x0000E3D0 File Offset: 0x0000C5D0
	private void UpdateMotionBlur()
	{
		if (this.motionBlur != null)
		{
			this.motionBlur.enabled = Effects.motionblur;
		}
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x0000E3F0 File Offset: 0x0000C5F0
	private void UpdateShafts()
	{
		if (this.shafts != null)
		{
			this.shafts.enabled = Effects.shafts;
		}
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0000E410 File Offset: 0x0000C610
	private void UpdateWaterVisibility()
	{
		this.waterVisible = true;
		if (WaterSystem.Collision != null)
		{
			this.waterVisible = !WaterSystem.Collision.GetIgnore(base.transform.position, 0.5f);
		}
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x0000E449 File Offset: 0x0000C649
	private void UpdateDitherMask()
	{
		KeywordUtil.EnsureKeywordState("TEMPORAL_ALPHA_TO_COVERAGE", Effects.antialiasing >= 3);
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0006CE90 File Offset: 0x0006B090
	private void UpdateShadows()
	{
		if (this.contactShadows != null)
		{
			this.contactShadows.enabled = ConVar.Graphics.contactshadows;
			if (this.contactShadows.enabled && this.contactShadows.sun == null)
			{
				this.contactShadows.sun = TOD_Sky.Instance.Components.LightSource;
			}
		}
		if (SingletonComponent<FoliageGrid>.Instance != null && SingletonComponent<FoliageGrid>.Instance.Initialized)
		{
			ShadowCastingMode foliageShadows = SingletonComponent<FoliageGrid>.Instance.FoliageShadows;
			SingletonComponent<FoliageGrid>.Instance.FoliageShadows = (ConVar.Graphics.grassshadows ? ShadowCastingMode.On : ShadowCastingMode.Off);
			if (foliageShadows != SingletonComponent<FoliageGrid>.Instance.FoliageShadows)
			{
				FoliageGrid.RefreshAll(true);
			}
		}
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0006CF40 File Offset: 0x0006B140
	private void UpdateVisualizeTexelDensity()
	{
		if (this.visualizeTexelDensity != null)
		{
			this.visualizeTexelDensity.enabled = (ConVar.Graphics.showtexeldensity != 0);
			this.visualizeTexelDensity.texelsPerMeter = 128 << ConVar.Graphics.showtexeldensity;
			this.visualizeTexelDensity.showHUD = (!MainMenuSystem.isOpen && !UICrafting.isOpen && !UIInventory.isOpen && !DeveloperTools.isOpen);
		}
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x0006CFB4 File Offset: 0x0006B1B4
	private void OnPreCull()
	{
		if (Effects.antialiasing < 3)
		{
			MainCamera.mainCamera.nonJitteredProjectionMatrix = MainCamera.mainCamera.projectionMatrix;
		}
		if (Effects.motionblur)
		{
			ObjectMotionVectorFix.RestoreObjectMotionVectors();
		}
		if (this.todScattering != null && this.todScattering.enabled)
		{
			this.UpdateCameraFrustumCorners(MainCamera.mainCamera, null);
			return;
		}
		MainCamera.ClearCameraFrustumCorners();
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x0006D018 File Offset: 0x0006B218
	private void OnPreRender()
	{
		this.UpdateDepthOfField();
		this.UpdateSSAO();
		this.UpdateShafts();
		this.UpdateMotionBlur();
		this.UpdateColorGrading();
		this.UpdateAntiAliasing();
		this.UpdateSharpenAndVignette();
		this.UpdateShadows();
		this.UpdateDitherMask();
		this.UpdateVisualizeTexelDensity();
		MainCamera.UpdateLightProbe();
		if (Effects.motionblur || Effects.antialiasing == 3 || Effects.ao)
		{
			MainCamera.mainCamera.depthTextureMode |= DepthTextureMode.Depth;
			MainCamera.mainCamera.depthTextureMode |= DepthTextureMode.MotionVectors;
		}
		else
		{
			MainCamera.mainCamera.depthTextureMode &= ~DepthTextureMode.MotionVectors;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (instance)
		{
			this.UpdateAmbientLight(instance);
			this.UpdateSkyReflection(instance);
			this.UpdateAmbientProbe();
		}
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x0006D0D8 File Offset: 0x0006B2D8
	private void UpdateAmbientLight(TOD_Sky sky)
	{
		if (this.ambientLightDay < 0f)
		{
			this.ambientLightDay = sky.Day.AmbientMultiplier;
		}
		if (this.ambientLightNight < 0f)
		{
			this.ambientLightNight = sky.Night.AmbientMultiplier;
		}
		if (Debugging.ambientvolumes || !LocalPlayer.Entity || !LocalPlayer.Entity.IsDeveloper)
		{
			this.ambientLightMultiplier = Mathf.MoveTowards(this.ambientLightMultiplier, this.ambientLightMultiplierTarget, UnityEngine.Time.deltaTime * this.environmentTransitionSpeed);
		}
		else
		{
			this.ambientLightMultiplier = 1f;
		}
		sky.Day.AmbientMultiplier = this.ambientLightDay * this.ambientLightMultiplier;
		sky.Night.AmbientMultiplier = this.ambientLightNight * this.ambientLightMultiplier;
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0006D1A0 File Offset: 0x0006B3A0
	private void UpdateSkyReflection(TOD_Sky sky)
	{
		if (this.skyReflectionDay < 0f)
		{
			this.skyReflectionDay = sky.Day.ReflectionMultiplier;
		}
		if (this.skyReflectionNight < 0f)
		{
			this.skyReflectionNight = sky.Night.ReflectionMultiplier;
		}
		if (Debugging.ambientvolumes || !LocalPlayer.Entity || !LocalPlayer.Entity.IsDeveloper)
		{
			this.skyReflectionMultiplier = Mathf.MoveTowards(this.skyReflectionMultiplier, this.skyReflectionMultiplierTarget, UnityEngine.Time.deltaTime * this.environmentTransitionSpeed);
		}
		else
		{
			this.skyReflectionMultiplier = 1f;
		}
		sky.Day.ReflectionMultiplier = this.skyReflectionDay * this.skyReflectionMultiplier;
		sky.Night.ReflectionMultiplier = this.skyReflectionNight * this.skyReflectionMultiplier;
		sky.Reflection.CullingMask = this.skyReflectionCullingFlags;
		RenderSettings.reflectionIntensity = Mathf.Lerp(sky.Night.ReflectionMultiplier, sky.Day.ReflectionMultiplier, sky.LerpValue);
		if (sky.Probe != null)
		{
			sky.Probe.intensity = RenderSettings.reflectionIntensity;
			Shader.SetGlobalTexture("global_SkyReflection", sky.Probe.texture);
			Shader.SetGlobalVector("global_SkyReflection_HDR", new Vector2(Mathf.GammaToLinearSpace(sky.Probe.intensity), 1f));
			return;
		}
		if (sky.ProbeEx != null)
		{
			ReflectionProbeEx reflectionProbeEx = (ReflectionProbeEx)sky.ProbeEx;
			reflectionProbeEx.clearFlags = TOD_Sky.Instance.Reflection.ClearFlags;
			reflectionProbeEx.timeSlicing = (TOD_Sky.Instance.Reflection.TimeSlicing != ReflectionProbeTimeSlicingMode.NoTimeSlicing);
			reflectionProbeEx.resolution = Mathf.ClosestPowerOfTwo(TOD_Sky.Instance.Reflection.Resolution);
			if (TOD_Sky.Instance.Components.Camera != null)
			{
				reflectionProbeEx.background = TOD_Sky.Instance.Components.Camera.BackgroundColor;
				reflectionProbeEx.nearClip = TOD_Sky.Instance.Components.Camera.NearClipPlane;
				reflectionProbeEx.farClip = TOD_Sky.Instance.Components.Camera.FarClipPlane;
			}
			Shader.SetGlobalTexture("global_SkyReflection", reflectionProbeEx.Texture);
			Shader.SetGlobalVector("global_SkyReflection_HDR", new Vector2(Mathf.GammaToLinearSpace(RenderSettings.reflectionIntensity), 1f));
		}
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x0000E460 File Offset: 0x0000C660
	private static void UpdateLightProbe()
	{
		LightProbes.GetInterpolatedProbe(Vector3.zero, null, out MainCamera.lightProbe[0]);
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x0006D3FC File Offset: 0x0006B5FC
	private void UpdateAmbientProbe()
	{
		SphericalHarmonicsL2 sphericalHarmonicsL = RenderSettings.ambientProbe;
		if (RenderSettings.ambientMode == AmbientMode.Flat)
		{
			sphericalHarmonicsL = default(SphericalHarmonicsL2);
			sphericalHarmonicsL.AddAmbientLight(RenderSettings.ambientLight.linear * RenderSettings.ambientIntensity);
		}
		else if (RenderSettings.ambientMode == AmbientMode.Trilight)
		{
			Color a = RenderSettings.ambientSkyColor.linear * RenderSettings.ambientIntensity;
			Color color = RenderSettings.ambientEquatorColor.linear * RenderSettings.ambientIntensity;
			Color a2 = RenderSettings.ambientGroundColor.linear * RenderSettings.ambientIntensity;
			sphericalHarmonicsL = default(SphericalHarmonicsL2);
			sphericalHarmonicsL.AddAmbientLight(color);
			sphericalHarmonicsL.AddDirectionalLight(Vector3.up, a - color, 0.5f);
			sphericalHarmonicsL.AddDirectionalLight(Vector3.down, a2 - color, 0.5f);
		}
		Shader.SetGlobalVector("ambient_SHAr", new Vector4(sphericalHarmonicsL[0, 3], sphericalHarmonicsL[0, 1], sphericalHarmonicsL[0, 2], sphericalHarmonicsL[0, 0] - sphericalHarmonicsL[0, 6]));
		Shader.SetGlobalVector("ambient_SHAg", new Vector4(sphericalHarmonicsL[1, 3], sphericalHarmonicsL[1, 1], sphericalHarmonicsL[1, 2], sphericalHarmonicsL[1, 0] - sphericalHarmonicsL[1, 6]));
		Shader.SetGlobalVector("ambient_SHAb", new Vector4(sphericalHarmonicsL[2, 3], sphericalHarmonicsL[2, 1], sphericalHarmonicsL[2, 2], sphericalHarmonicsL[2, 0] - sphericalHarmonicsL[2, 6]));
		Shader.SetGlobalVector("ambient_SHBr", new Vector4(sphericalHarmonicsL[0, 4], sphericalHarmonicsL[0, 5], sphericalHarmonicsL[0, 6] * 3f, sphericalHarmonicsL[0, 7]));
		Shader.SetGlobalVector("ambient_SHBg", new Vector4(sphericalHarmonicsL[1, 4], sphericalHarmonicsL[1, 5], sphericalHarmonicsL[1, 6] * 3f, sphericalHarmonicsL[1, 7]));
		Shader.SetGlobalVector("ambient_SHBb", new Vector4(sphericalHarmonicsL[2, 4], sphericalHarmonicsL[2, 5], sphericalHarmonicsL[2, 6] * 3f, sphericalHarmonicsL[2, 7]));
		Shader.SetGlobalVector("ambient_SHC", new Vector4(sphericalHarmonicsL[0, 8], sphericalHarmonicsL[1, 8], sphericalHarmonicsL[2, 8], 1f));
		Shader.SetGlobalFloat("global_MainLightingAtten", RenderSettings.ambientIntensity);
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x0006D678 File Offset: 0x0006B878
	private void LateUpdate()
	{
		MainCamera.mainCamera.nearClipPlane = this.GetIdealNearClipPlane();
		this.UpdateWaterVisibility();
		if ((Debugging.ambientvolumes || !LocalPlayer.Entity || !LocalPlayer.Entity.IsDeveloper) && UnityEngine.Time.realtimeSinceStartup - this.environmentTimestamp > 1f)
		{
			Vector3 position = MainCamera.mainCamera.transform.position;
			this.environmentType = EnvironmentManager.Get(position);
			if ((this.environmentType & EnvironmentType.Building) == (EnvironmentType)0 && Physics.Raycast(position, Vector3.up, 100f, 2097152))
			{
				this.environmentType |= EnvironmentType.PlayerConstruction;
			}
			this.environmentTransitionSpeed = 1f;
			this.skyReflectionCullingFlags = 0;
			this.ambientLightMultiplierTarget = 0f;
			this.skyReflectionMultiplierTarget = 0f;
			if (this.environmentVolumeProperties != null)
			{
				this.environmentTransitionSpeed = this.environmentVolumeProperties.TransitionSpeed;
				EnvironmentVolumeProperties environmentVolumeProperties = this.environmentVolumeProperties.FindQuality(Reflection.quality);
				if (environmentVolumeProperties != null)
				{
					this.skyReflectionCullingFlags = environmentVolumeProperties.ReflectionCullingFlags;
					this.ambientLightMultiplierTarget = environmentVolumeProperties.FindAmbientMultiplier(this.environmentType);
					this.skyReflectionMultiplierTarget = environmentVolumeProperties.FindReflectionMultiplier(this.environmentType);
				}
			}
			this.environmentTimestamp = UnityEngine.Time.realtimeSinceStartup;
		}
		float num = Vector3.Distance(MainCamera.mainCamera.transform.position, this.lastPosition) / UnityEngine.Time.deltaTime;
		float num2 = Quaternion.Angle(MainCamera.mainCamera.transform.rotation, this.lastRotation) / UnityEngine.Time.deltaTime;
		if (num > 0.01f || num2 > 0.01f)
		{
			Rust.GC.Pause(1f);
		}
		this.lastPosition = MainCamera.mainCamera.transform.position;
		this.lastRotation = MainCamera.mainCamera.transform.rotation;
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0006D844 File Offset: 0x0006BA44
	private float GetIdealNearClipPlane()
	{
		if ((float)Screen.height > 0f && (float)(Screen.width / Screen.height) > 1.7f)
		{
			return 0.1f - Mathf.InverseLerp(1.7f, 4f, (float)(Screen.width / Screen.height)) * 0.099f;
		}
		if (LocalPlayer.Entity && LocalPlayer.Entity.currentViewMode == BasePlayer.CameraMode.Eyes)
		{
			return 0.02f;
		}
		return 0.05f;
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x0000E478 File Offset: 0x0000C678
	public static float Distance(Vector3 pos)
	{
		if (MainCamera.mainCamera == null)
		{
			return 4096f;
		}
		return Vector3.Distance(MainCamera.mainCamera.transform.position, pos);
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x0006D8C0 File Offset: 0x0006BAC0
	public static float SqrDistance(Vector3 pos)
	{
		if (MainCamera.mainCamera == null)
		{
			return 16777216f;
		}
		return (MainCamera.mainCamera.transform.position - pos).sqrMagnitude;
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x0006D900 File Offset: 0x0006BB00
	public static float Distance(BaseEntity ent)
	{
		if (MainCamera.mainCamera == null)
		{
			return 4096f;
		}
		if (!ent.IsValid())
		{
			return 4096f;
		}
		return Vector3.Distance(MainCamera.mainCamera.transform.position, ent.transform.position);
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x0000E4A2 File Offset: 0x0000C6A2
	public static float Distance2D(Vector3 pos)
	{
		if (MainCamera.mainCamera == null)
		{
			return 4096f;
		}
		return Vector3Ex.Distance2D(MainCamera.mainCamera.transform.position, pos);
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x0000E4CC File Offset: 0x0000C6CC
	public static float SqrDistance2D(Vector3 pos)
	{
		if (MainCamera.mainCamera == null)
		{
			return 16777216f;
		}
		return Vector3Ex.SqrMagnitude2D(MainCamera.mainCamera.transform.position - pos);
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x0006D950 File Offset: 0x0006BB50
	public static float Distance2D(BaseEntity ent)
	{
		if (MainCamera.mainCamera == null)
		{
			return 4096f;
		}
		if (!ent.IsValid())
		{
			return 4096f;
		}
		return Vector3Ex.Distance2D(MainCamera.mainCamera.transform.position, ent.transform.position);
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x0006D9A0 File Offset: 0x0006BBA0
	public static HitTest Trace(float maxDistance, BaseEntity IgnoreEntity = null, float radius = 0f)
	{
		if (MainCamera.mainCamera == null)
		{
			return null;
		}
		HitTest hitTest = new HitTest();
		hitTest.AttackRay = new Ray(MainCamera.mainCamera.transform.position, MainCamera.mainCamera.transform.forward);
		hitTest.MaxDistance = maxDistance;
		hitTest.ignoreEntity = IgnoreEntity;
		hitTest.Radius = radius;
		hitTest.Forgiveness = radius;
		hitTest.type = HitTest.Type.Generic;
		if (!GameTrace.Trace(hitTest, 1269916417))
		{
			return null;
		}
		return hitTest;
	}

	// Token: 0x0200020C RID: 524
	public struct DepthOfFieldSettings
	{
		// Token: 0x04000D02 RID: 3330
		public bool wants;

		// Token: 0x04000D03 RID: 3331
		public float focalDistance;

		// Token: 0x04000D04 RID: 3332
		public float focalSize;

		// Token: 0x04000D05 RID: 3333
		public float blurSize;
	}
}
