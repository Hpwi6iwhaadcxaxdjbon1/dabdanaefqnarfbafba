using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000578 RID: 1400
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AtmosphereVolumeRenderer : MonoBehaviour
{
	// Token: 0x04001C07 RID: 7175
	public FogMode Mode = FogMode.ExponentialSquared;

	// Token: 0x04001C08 RID: 7176
	public bool DistanceFog = true;

	// Token: 0x04001C09 RID: 7177
	public bool HeightFog = true;

	// Token: 0x04001C0A RID: 7178
	public AtmosphereVolume Volume;

	// Token: 0x04001C0B RID: 7179
	private Camera targetCamera;

	// Token: 0x04001C0C RID: 7180
	private CommandBufferManager commandBufferManager;

	// Token: 0x04001C0D RID: 7181
	private CommandBufferDesc commandBufferDesc;

	// Token: 0x04001C0E RID: 7182
	private Material fogMaterial;

	// Token: 0x04001C0F RID: 7183
	private Vector4[] matrixArray = new Vector4[3];

	// Token: 0x04001C10 RID: 7184
	private static Mesh volumeMesh = null;

	// Token: 0x04001C11 RID: 7185
	private const int MaxVolumes = 1;

	// Token: 0x04001C12 RID: 7186
	private static HashSet<AtmosphereVolume> registeredVolumes = new HashSet<AtmosphereVolume>();

	// Token: 0x04001C13 RID: 7187
	private List<AtmosphereVolumeRenderer.CurrentVolumeEntry> currentVolumes = new List<AtmosphereVolumeRenderer.CurrentVolumeEntry>();

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001FF9 RID: 8185 RVA: 0x000194FF File Offset: 0x000176FF
	public Camera TargetCamera
	{
		get
		{
			if (this.targetCamera == null)
			{
				this.targetCamera = base.GetComponent<Camera>();
			}
			return this.targetCamera;
		}
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x00019521 File Offset: 0x00017721
	public static void Clear()
	{
		AtmosphereVolumeRenderer.registeredVolumes.Clear();
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x0001952D File Offset: 0x0001772D
	public static void Register(AtmosphereVolume volume)
	{
		AtmosphereVolumeRenderer.registeredVolumes.Add(volume);
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x0001953B File Offset: 0x0001773B
	public static void Unregister(AtmosphereVolume volume)
	{
		AtmosphereVolumeRenderer.registeredVolumes.Remove(volume);
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x00019549 File Offset: 0x00017749
	private void OnEnable()
	{
		this.CheckCommandBuffer();
		this.CheckMaterials();
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x00019557 File Offset: 0x00017757
	private void OnDisable()
	{
		this.CleanupCommandBuffer();
		this.CleanupMaterials();
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x00019565 File Offset: 0x00017765
	private void Update()
	{
		this.CheckCommandBuffer();
		this.CheckMaterials();
		this.UpdateAtmosphere();
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000AEB98 File Offset: 0x000ACD98
	private void CheckCommandBuffer()
	{
		if (this.commandBufferManager == null)
		{
			this.commandBufferManager = base.GetComponent<CommandBufferManager>();
		}
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferDesc = ((this.commandBufferDesc == null) ? new CommandBufferDesc(CameraEvent.AfterImageEffectsOpaque, 100, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer)) : this.commandBufferDesc);
			this.commandBufferManager.AddCommands(this.commandBufferDesc);
		}
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x00019579 File Offset: 0x00017779
	private void CleanupCommandBuffer()
	{
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferManager.RemoveCommands(this.commandBufferDesc);
			this.commandBufferManager = null;
		}
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000195AE File Offset: 0x000177AE
	private void CheckMaterials()
	{
		if (this.fogMaterial == null)
		{
			this.fogMaterial = new Material(Shader.Find("Hidden/FogVolume"))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000195DB File Offset: 0x000177DB
	private void CleanupMaterials()
	{
		if (this.fogMaterial != null)
		{
			Object.DestroyImmediate(this.fogMaterial);
			this.fogMaterial = null;
		}
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000195FD File Offset: 0x000177FD
	private void UpdateVolumeMesh()
	{
		if (AtmosphereVolumeRenderer.volumeMesh == null)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			AtmosphereVolumeRenderer.volumeMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			GameManager.DestroyImmediate(gameObject, false);
		}
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x000AEC18 File Offset: 0x000ACE18
	public void FindAndSortVolumes(Camera camera)
	{
		this.currentVolumes.Clear();
		Vector3 position = camera.transform.position;
		foreach (AtmosphereVolume atmosphereVolume in AtmosphereVolumeRenderer.registeredVolumes)
		{
			float num = Vector3.SqrMagnitude(atmosphereVolume.transform.position - position);
			float num2 = atmosphereVolume.MaxVisibleDistance * atmosphereVolume.MaxVisibleDistance;
			if (num < num2)
			{
				this.currentVolumes.Add(new AtmosphereVolumeRenderer.CurrentVolumeEntry(atmosphereVolume, num));
			}
		}
		this.currentVolumes.Sort((AtmosphereVolumeRenderer.CurrentVolumeEntry x, AtmosphereVolumeRenderer.CurrentVolumeEntry y) => x.distanceSqr.CompareTo(y.distanceSqr));
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x00019628 File Offset: 0x00017828
	public void UpdateAtmosphere()
	{
		Vector3 position = base.transform.position;
		this.FindAndSortVolumes(this.TargetCamera);
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x000AECE4 File Offset: 0x000ACEE4
	private void SetFrustumGlobalShaderParams(CommandBuffer cb, Camera camera)
	{
		Transform transform = camera.transform;
		float nearClipPlane = camera.nearClipPlane;
		float farClipPlane = camera.farClipPlane;
		float fieldOfView = camera.fieldOfView;
		float aspect = camera.aspect;
		float num = fieldOfView * 0.5f;
		Vector3 b = transform.right * nearClipPlane * Mathf.Tan(num * 0.017453292f) * aspect;
		Vector3 b2 = transform.up * nearClipPlane * Mathf.Tan(num * 0.017453292f);
		Vector3 vector = transform.forward * nearClipPlane - b + b2;
		float d = vector.magnitude * farClipPlane / nearClipPlane;
		vector.Normalize();
		vector *= d;
		Vector3 vector2 = transform.forward * nearClipPlane + b + b2;
		vector2.Normalize();
		vector2 *= d;
		Vector3 vector3 = transform.forward * nearClipPlane + b - b2;
		vector3.Normalize();
		vector3 *= d;
		Vector3 vector4 = transform.forward * nearClipPlane - b - b2;
		vector4.Normalize();
		vector4 *= d;
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetRow(0, vector);
		identity.SetRow(1, vector2);
		identity.SetRow(2, vector3);
		identity.SetRow(3, vector4);
		cb.SetGlobalMatrix("_FrustumCornersWS", identity);
	}

	// Token: 0x06002008 RID: 8200 RVA: 0x000AEE74 File Offset: 0x000AD074
	private bool SetGlobalShaderParams(CommandBuffer cb)
	{
		Debug.Assert(this.currentVolumes.Count > 0);
		Vector3 position = this.TargetCamera.transform.position;
		AtmosphereVolume volume = this.currentVolumes[0].volume;
		float num = Vector3.SqrMagnitude(volume.transform.position - position);
		float num2 = volume.MaxVisibleDistance * volume.MaxVisibleDistance;
		float num3 = Mathf.Clamp01((1f - Mathf.Clamp01(num / num2)) * 4f);
		bool flag = num3 > 0f;
		if (flag)
		{
			this.SetFrustumGlobalShaderParams(cb, this.TargetCamera);
			float num4 = volume.transform.position.y + volume.FogSettings.Height;
			float num5 = position.y - num4;
			float z = (num5 <= 0f) ? 1f : 0f;
			float time = (TOD_Sky.Instance != null) ? Mathf.Clamp01(TOD_Sky.Instance.SunZenith / 90f) : 0.5f;
			FogMode mode = this.Mode;
			float density = volume.FogSettings.Density;
			float startDistance = volume.FogSettings.StartDistance;
			int num6 = 100000;
			Color value = volume.FogSettings.ColorOverDaytime.Evaluate(time);
			float heightDensity = volume.FogSettings.HeightDensity;
			bool flag2 = mode == FogMode.Linear;
			float num7 = flag2 ? ((float)num6 - startDistance) : 0f;
			float num8 = (Mathf.Abs(num7) > 0.0001f) ? (1f / num7) : 0f;
			Vector4 value2;
			value2.x = density * 1.2011224f * num3;
			value2.y = density * 1.442695f * num3;
			value2.z = (flag2 ? (-num8) : 0f);
			value2.w = (flag2 ? ((float)num6 * num8) : 0f);
			cb.SetGlobalVector("_CameraWS", position);
			cb.SetGlobalVector("_HeightParams", new Vector4(num4, num5, z, heightDensity * 0.5f));
			cb.SetGlobalVector("_DistanceParams", new Vector4(-Mathf.Max(startDistance, 0f), 0f, 0f, 0f));
			cb.SetGlobalVector("_SceneFogParams", value2);
			cb.SetGlobalVector("_SceneFogMode", new Vector4((float)mode, 1f, (float)(this.DistanceFog ? 1 : 0), (float)(this.HeightFog ? 1 : 0)));
			cb.SetGlobalColor("_SceneFogColor", value);
			Matrix4x4 worldToLocalMatrix = volume.transform.worldToLocalMatrix;
			Vector3 position2 = volume.transform.position;
			float boundsAttenuationDecay = volume.BoundsAttenuationDecay;
			this.matrixArray[0] = worldToLocalMatrix.GetRow(0);
			this.matrixArray[1] = worldToLocalMatrix.GetRow(1);
			this.matrixArray[2] = worldToLocalMatrix.GetRow(2);
			cb.SetGlobalVectorArray("_VolumeWorldToLocal", this.matrixArray);
			cb.SetGlobalVector("_VolumeWorldPosition", position2);
			cb.SetGlobalFloat("_VolumeAttenuationDecay", Mathf.Max(1f, boundsAttenuationDecay));
		}
		return flag;
	}

	// Token: 0x06002009 RID: 8201 RVA: 0x00019642 File Offset: 0x00017842
	private void ClearGlobalShaderParams(CommandBuffer cb)
	{
		cb.SetGlobalVector("_SceneFogMode", Vector4.zero);
	}

	// Token: 0x0600200A RID: 8202 RVA: 0x000AF194 File Offset: 0x000AD394
	private void FillCommandBuffer(CommandBuffer cb)
	{
		int num = -1;
		if (this.DistanceFog && this.HeightFog)
		{
			num = 0;
		}
		else if (this.DistanceFog)
		{
			num = 1;
		}
		else if (this.HeightFog)
		{
			num = 2;
		}
		if (num >= 0 && this.currentVolumes.Count > 0 && this.SetGlobalShaderParams(cb))
		{
			this.UpdateVolumeMesh();
			cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
			cb.DrawMesh(AtmosphereVolumeRenderer.volumeMesh, this.currentVolumes[0].volume.transform.localToWorldMatrix, this.fogMaterial, 0, num);
			return;
		}
		this.ClearGlobalShaderParams(cb);
	}

	// Token: 0x02000579 RID: 1401
	private struct CurrentVolumeEntry
	{
		// Token: 0x04001C14 RID: 7188
		public AtmosphereVolume volume;

		// Token: 0x04001C15 RID: 7189
		public float distanceSqr;

		// Token: 0x0600200D RID: 8205 RVA: 0x0001969A File Offset: 0x0001789A
		public CurrentVolumeEntry(AtmosphereVolume volume, float distance)
		{
			this.volume = volume;
			this.distanceSqr = distance;
		}
	}
}
