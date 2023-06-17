using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B6 RID: 1462
[RequireComponent(typeof(WindZone))]
[ExecuteInEditMode]
public class WindZoneExManager : MonoBehaviour
{
	// Token: 0x04001D7F RID: 7551
	public float maxAccumMain = 4f;

	// Token: 0x04001D80 RID: 7552
	public float maxAccumTurbulence = 4f;

	// Token: 0x04001D81 RID: 7553
	public float globalMainScale = 1f;

	// Token: 0x04001D82 RID: 7554
	public float globalTurbulenceScale = 1f;

	// Token: 0x04001D83 RID: 7555
	public Transform testPosition;

	// Token: 0x04001D84 RID: 7556
	private const int MaxWindZones = 8;

	// Token: 0x04001D85 RID: 7557
	private const float MaxWindZoneDistanceToCamera = 1000f;

	// Token: 0x04001D86 RID: 7558
	private const float MaxWindZoneSqrDistanceToCamera = 1000000f;

	// Token: 0x04001D87 RID: 7559
	private static HashSet<WindZoneEx> registeredZones = new HashSet<WindZoneEx>();

	// Token: 0x04001D88 RID: 7560
	private static List<WindZoneExManager.CurrentZoneEntry> currentZones = new List<WindZoneExManager.CurrentZoneEntry>();

	// Token: 0x04001D89 RID: 7561
	private static Vector4[] windZoneInfoArray = new Vector4[8];

	// Token: 0x04001D8A RID: 7562
	private static Vector4[] windZoneParamArray = new Vector4[8];

	// Token: 0x04001D8B RID: 7563
	private static WindZoneExManager instance;

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x060021AD RID: 8621 RVA: 0x0001ACD8 File Offset: 0x00018ED8
	public static WindZoneExManager Instance
	{
		get
		{
			return WindZoneExManager.instance;
		}
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x0001ACDF File Offset: 0x00018EDF
	public static void Clear()
	{
		WindZoneExManager.registeredZones.Clear();
		WindZoneExManager.currentZones.Clear();
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x0001ACF5 File Offset: 0x00018EF5
	public static void Register(WindZoneEx zone)
	{
		WindZoneExManager.registeredZones.Add(zone);
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x0001AD03 File Offset: 0x00018F03
	public static void Unregister(WindZoneEx zone)
	{
		WindZoneExManager.registeredZones.Remove(zone);
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x0001AD11 File Offset: 0x00018F11
	private void Awake()
	{
		if (WindZoneExManager.instance == null)
		{
			WindZoneExManager.instance = this;
		}
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000B65A0 File Offset: 0x000B47A0
	private void Update()
	{
		Camera camera = (MainCamera.mainCamera != null) ? MainCamera.mainCamera : Camera.main;
		this.FindAndSortZones(camera);
		this.SetShaderGlobals();
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000B65D4 File Offset: 0x000B47D4
	public void FindAndSortZones(Camera camera)
	{
		WindZoneExManager.currentZones.Clear();
		Vector3 position = camera.transform.position;
		foreach (WindZoneEx windZoneEx in WindZoneExManager.registeredZones)
		{
			if (windZoneEx.Mode == 1)
			{
				float num = Vector3.SqrMagnitude(windZoneEx.transform.position - position);
				if (num < 1000000f)
				{
					WindZoneExManager.currentZones.Add(new WindZoneExManager.CurrentZoneEntry(windZoneEx, num));
				}
			}
		}
		WindZoneExManager.currentZones.Sort((WindZoneExManager.CurrentZoneEntry x, WindZoneExManager.CurrentZoneEntry y) => x.distanceSqr.CompareTo(y.distanceSqr));
		foreach (WindZoneEx windZoneEx2 in WindZoneExManager.registeredZones)
		{
			if (windZoneEx2.Mode == null)
			{
				WindZoneExManager.currentZones.Add(new WindZoneExManager.CurrentZoneEntry(windZoneEx2, 0f));
			}
		}
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000B66F4 File Offset: 0x000B48F4
	public void SetShaderGlobals()
	{
		int num = Mathf.Min(WindZoneExManager.currentZones.Count, 8);
		for (int i = 0; i < num; i++)
		{
			WindZoneExManager.windZoneInfoArray[i] = WindZoneExManager.currentZones[i].zone.PackInfo();
			WindZoneExManager.windZoneParamArray[i] = WindZoneExManager.currentZones[i].zone.PackParam();
		}
		Shader.SetGlobalVectorArray("windx_WindZoneInfoArray", WindZoneExManager.windZoneInfoArray);
		Shader.SetGlobalVectorArray("windx_WindZoneParamArray", WindZoneExManager.windZoneParamArray);
		Shader.SetGlobalInt("windx_WindZoneCount", num);
		Vector4 value = new Vector4(this.maxAccumMain, this.maxAccumTurbulence, 1f, 1f);
		Shader.SetGlobalVector("windx_WindGlobalParams", value);
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000B67B0 File Offset: 0x000B49B0
	private static Vector4 ComputeWindForce(float time, Vector4 zoneInfo, Vector4 zoneParam, Vector3 center)
	{
		Vector3 vector = new Vector3(zoneInfo.x, zoneInfo.y, zoneInfo.z);
		bool flag = zoneInfo.w == 0f;
		float w = zoneInfo.w;
		float x = zoneParam.x;
		float y = zoneParam.y;
		float z = zoneParam.z;
		float w2 = zoneParam.w;
		float num = time * 3.1415927f * z + center.x * 0.1f + center.z * 0.1f;
		float num2 = (Mathf.Cos(num) + Mathf.Cos(num * 0.375f) + Mathf.Cos(num * 0.05f)) * 0.333f;
		num2 = 1f + num2 * w2;
		Vector4 result = Vector4.zero;
		if (flag)
		{
			float d = num2;
			Vector3 vector2 = vector.normalized * x;
			result = new Vector4(vector2.x, vector2.y, vector2.z, y) * d;
		}
		else
		{
			float num3 = 1f - Vector3.Distance(vector, center) / w;
			if (num3 > 0f)
			{
				num3 *= num2;
				Vector3 vector3 = (center - vector).normalized * x;
				result = new Vector4(vector3.x, vector3.y, vector3.z, y) * num3;
			}
		}
		return result;
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000B6908 File Offset: 0x000B4B08
	public static Vector4 ComputeWindForceAtLocation(Vector3 location)
	{
		if (WindZoneExManager.instance != null)
		{
			float time = Time.time;
			Vector4 vector = Vector4.zero;
			int num = Mathf.Min(WindZoneExManager.currentZones.Count, 8);
			for (int i = 0; i < num; i++)
			{
				vector += WindZoneExManager.ComputeWindForce(time, WindZoneExManager.windZoneInfoArray[i], WindZoneExManager.windZoneParamArray[i], location);
			}
			float magnitude = new Vector3(vector.x, vector.y, vector.z).magnitude;
			if (magnitude >= 1E-05f)
			{
				float num2 = 1f / magnitude * Mathf.Min(WindZoneExManager.instance.maxAccumMain, magnitude * WindZoneExManager.instance.globalMainScale);
				vector.x *= num2;
				vector.y *= num2;
				vector.z *= num2;
				vector.w = Mathf.Min(WindZoneExManager.instance.maxAccumTurbulence, vector.w * WindZoneExManager.instance.globalTurbulenceScale);
			}
			return vector;
		}
		return Vector4.zero;
	}

	// Token: 0x020005B7 RID: 1463
	private enum TestMode
	{
		// Token: 0x04001D8D RID: 7565
		Disabled,
		// Token: 0x04001D8E RID: 7566
		Low
	}

	// Token: 0x020005B8 RID: 1464
	private struct CurrentZoneEntry
	{
		// Token: 0x04001D8F RID: 7567
		public WindZoneEx zone;

		// Token: 0x04001D90 RID: 7568
		public float distanceSqr;

		// Token: 0x060021B9 RID: 8633 RVA: 0x0001AD86 File Offset: 0x00018F86
		public CurrentZoneEntry(WindZoneEx zone, float distance)
		{
			this.zone = zone;
			this.distanceSqr = distance;
		}
	}
}
