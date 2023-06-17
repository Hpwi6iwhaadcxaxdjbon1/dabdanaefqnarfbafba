using System;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class Buoyancy : MonoBehaviour
{
	// Token: 0x040012B4 RID: 4788
	public BuoyancyPoint[] points;

	// Token: 0x040012B5 RID: 4789
	public GameObjectRef[] waterImpacts;

	// Token: 0x040012B6 RID: 4790
	public Rigidbody rigidBody;

	// Token: 0x040012B7 RID: 4791
	public float buoyancyScale = 1f;

	// Token: 0x040012B8 RID: 4792
	public float submergedFraction;

	// Token: 0x040012B9 RID: 4793
	public bool doEffects = true;

	// Token: 0x040012BA RID: 4794
	public bool clientSide;

	// Token: 0x040012BB RID: 4795
	public Action<bool> SubmergedChanged;

	// Token: 0x040012BC RID: 4796
	private Buoyancy.BuoyancyPointData[] pointData;

	// Token: 0x040012BD RID: 4797
	public float waveHeightScale = 0.5f;

	// Token: 0x060015CB RID: 5579 RVA: 0x00012670 File Offset: 0x00010870
	public static string DefaultWaterImpact()
	{
		return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x00085248 File Offset: 0x00083448
	public void FixedUpdate()
	{
		bool flag = this.submergedFraction > 0f;
		this.BuoyancyFixedUpdate();
		bool flag2 = this.submergedFraction > 0f;
		if (this.SubmergedChanged != null && flag != flag2)
		{
			this.SubmergedChanged.Invoke(flag2);
		}
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x00085290 File Offset: 0x00083490
	public Vector3 GetFlowDirection(float normX, float normZ)
	{
		if (TerrainMeta.WaterMap == null)
		{
			return Vector3.zero;
		}
		Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(normX, normZ);
		float num = Mathf.Clamp01(Mathf.Abs(normalFast.y));
		normalFast.y = 0f;
		Vector3Ex.FastRenormalize(normalFast, num);
		return normalFast;
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x000852E4 File Offset: 0x000834E4
	public void EnsurePointsInitialized()
	{
		if (this.points == null || this.points.Length == 0)
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				BuoyancyPoint buoyancyPoint = new GameObject("BuoyancyPoint")
				{
					transform = 
					{
						parent = component.gameObject.transform,
						localPosition = component.centerOfMass
					}
				}.AddComponent<BuoyancyPoint>();
				buoyancyPoint.buoyancyForce = component.mass * -Physics.gravity.y;
				buoyancyPoint.buoyancyForce *= 1.32f;
				buoyancyPoint.size = 0.2f;
				this.points = new BuoyancyPoint[1];
				this.points[0] = buoyancyPoint;
			}
		}
		if (this.pointData == null || this.pointData.Length != this.points.Length)
		{
			this.pointData = new Buoyancy.BuoyancyPointData[this.points.Length];
			for (int i = 0; i < this.points.Length; i++)
			{
				Transform transform = this.points[i].transform;
				Transform parent = transform.parent;
				transform.SetParent(base.transform);
				Vector3 localPosition = transform.localPosition;
				transform.SetParent(parent);
				this.pointData[i].transform = transform;
				this.pointData[i].localPosition = transform.localPosition;
				this.pointData[i].rootToPoint = localPosition;
			}
		}
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x0008544C File Offset: 0x0008364C
	public void BuoyancyFixedUpdate()
	{
		if (TerrainMeta.WaterMap == null)
		{
			return;
		}
		this.EnsurePointsInitialized();
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.rigidBody.IsSleeping())
		{
			return;
		}
		if (this.buoyancyScale == 0f)
		{
			return;
		}
		float time = Time.time;
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		for (int i = 0; i < this.pointData.Length; i++)
		{
			BuoyancyPoint buoyancyPoint = this.points[i];
			Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(this.pointData[i].rootToPoint);
			this.pointData[i].position = vector;
			this.pointData[i].normX = (vector.x - x) * x2;
			this.pointData[i].normZ = (vector.z - z) * z2;
		}
		int num = 0;
		for (int j = 0; j < this.points.Length; j++)
		{
			BuoyancyPoint buoyancyPoint2 = this.points[j];
			Vector3 position = this.pointData[j].position;
			Vector3 localPosition = this.pointData[j].localPosition;
			float normX = this.pointData[j].normX;
			float normZ = this.pointData[j].normZ;
			WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(position, normX, normZ);
			bool flag = false;
			if (position.y < buoyancyWaterInfo.surfaceLevel)
			{
				flag = true;
				num++;
				float currentDepth = buoyancyWaterInfo.currentDepth;
				float num2 = Mathf.InverseLerp(0f, buoyancyPoint2.size, currentDepth);
				float num3 = 1f + Mathf.PerlinNoise(buoyancyPoint2.randomOffset + time * buoyancyPoint2.waveFrequency, 0f) * buoyancyPoint2.waveScale;
				float num4 = buoyancyPoint2.buoyancyForce * this.buoyancyScale;
				Vector3 vector2 = new Vector3(0f, num3 * num2 * num4, 0f);
				Vector3 flowDirection = this.GetFlowDirection(normX, normZ);
				if (flowDirection.y < 0.9999f && flowDirection != Vector3.up)
				{
					num4 *= 0.25f;
					vector2.x += flowDirection.x * num4;
					vector2.y += flowDirection.y * num4;
					vector2.z += flowDirection.z * num4;
				}
				this.rigidBody.AddForceAtPosition(vector2, position, 0);
			}
			if (buoyancyPoint2.doSplashEffects && ((!buoyancyPoint2.wasSubmergedLastFrame && flag) || (!flag && buoyancyPoint2.wasSubmergedLastFrame)) && this.doEffects && this.rigidBody.GetRelativePointVelocity(localPosition).magnitude > 1f)
			{
				string strName = (this.waterImpacts != null && this.waterImpacts.Length != 0 && this.waterImpacts[0].isValid) ? this.waterImpacts[0].resourcePath : Buoyancy.DefaultWaterImpact();
				Vector3 b = new Vector3(Random.Range(-0.25f, 0.25f), 0f, Random.Range(-0.25f, 0.25f));
				if (this.clientSide)
				{
					Effect.client.Run(strName, position + b, Vector3.up, default(Vector3));
				}
				else
				{
					Effect.server.Run(strName, position + b, Vector3.up, null, false);
				}
				buoyancyPoint2.nexSplashTime = Time.time + 0.25f;
			}
			buoyancyPoint2.wasSubmergedLastFrame = flag;
		}
		if (this.points.Length != 0)
		{
			this.submergedFraction = (float)num / (float)this.points.Length;
		}
	}

	// Token: 0x0200033A RID: 826
	[Serializable]
	private struct BuoyancyPointData
	{
		// Token: 0x040012BE RID: 4798
		public Transform transform;

		// Token: 0x040012BF RID: 4799
		public Vector3 localPosition;

		// Token: 0x040012C0 RID: 4800
		public Vector3 rootToPoint;

		// Token: 0x040012C1 RID: 4801
		public float normX;

		// Token: 0x040012C2 RID: 4802
		public float normZ;

		// Token: 0x040012C3 RID: 4803
		public Vector3 position;
	}
}
