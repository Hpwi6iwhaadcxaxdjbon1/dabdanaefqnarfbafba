using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class TreadAnimator : MonoBehaviour, IClientComponent
{
	// Token: 0x040007DD RID: 2013
	public Animator mainBodyAnimator;

	// Token: 0x040007DE RID: 2014
	public Transform[] wheelBones;

	// Token: 0x040007DF RID: 2015
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x040007E0 RID: 2016
	public Vector3[] wheelBoneOrigin;

	// Token: 0x040007E1 RID: 2017
	public float wheelBoneDistMax = 0.26f;

	// Token: 0x040007E2 RID: 2018
	public Renderer treadRenderer;

	// Token: 0x040007E3 RID: 2019
	public Material leftTread;

	// Token: 0x040007E4 RID: 2020
	public Material rightTread;

	// Token: 0x040007E5 RID: 2021
	public TreadEffects treadEffects;

	// Token: 0x040007E6 RID: 2022
	private float angularVelocity;

	// Token: 0x040007E7 RID: 2023
	private Vector3 lastForward;

	// Token: 0x040007E8 RID: 2024
	private Vector3 currentVelocity = Vector3.zero;

	// Token: 0x040007E9 RID: 2025
	private Vector3 lastPos = Vector3.zero;

	// Token: 0x040007EA RID: 2026
	public float treadConstant = 0.14f;

	// Token: 0x040007EB RID: 2027
	public float wheelSpinConstant = 80f;

	// Token: 0x040007EC RID: 2028
	private float wheelAngle;

	// Token: 0x040007ED RID: 2029
	private float treadAmount;

	// Token: 0x040007EE RID: 2030
	public float traceLineMin = 0.55f;

	// Token: 0x040007EF RID: 2031
	public float traceLineMax = 0.79f;

	// Token: 0x040007F0 RID: 2032
	public float maxShockDist = 0.26f;

	// Token: 0x040007F1 RID: 2033
	private int cachedMask = -1;

	// Token: 0x06000B6B RID: 2923 RVA: 0x00058B38 File Offset: 0x00056D38
	public void Awake()
	{
		this.vecShocksOffsetPosition = new Vector3[this.wheelBones.Length];
		this.wheelBoneOrigin = new Vector3[this.wheelBones.Length];
		this.wheelBoneOrigin = new Vector3[this.wheelBones.Length];
		for (int i = 0; i < this.wheelBones.Length; i++)
		{
			this.wheelBoneOrigin[i] = base.transform.InverseTransformPoint(this.wheelBones[i].transform.position);
			this.vecShocksOffsetPosition[i] = this.wheelBones[i].localPosition;
		}
		for (int j = 0; j < this.treadRenderer.sharedMaterials.Length; j++)
		{
			Material material = this.treadRenderer.sharedMaterials[j];
			if (material.name.Contains("treads_left"))
			{
				this.leftTread = material;
			}
			else if (material.name.Contains("treads_right"))
			{
				this.rightTread = material;
			}
		}
		this.lastPos = base.transform.position;
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0000AF69 File Offset: 0x00009169
	public void Update()
	{
		this.CalculateVelocity();
		this.UpdateHeights();
		this.AnimateWheelsTreads();
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00058C40 File Offset: 0x00056E40
	public void CalculateVelocity()
	{
		float magnitude = (base.transform.position - this.lastPos).magnitude;
		Vector3 a = (base.transform.position - this.lastPos).normalized * magnitude;
		this.currentVelocity = Vector3.Lerp(this.currentVelocity, a / Time.deltaTime, Time.deltaTime * 10f);
		this.angularVelocity = Mathf.Lerp(this.angularVelocity, Vector3.Angle(this.lastForward, base.transform.forward) / Time.deltaTime, Time.deltaTime * 10f);
		this.lastPos = base.transform.position;
		this.lastForward = base.transform.forward;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0000AF7D File Offset: 0x0000917D
	public Vector3 GetCurrentVelocity()
	{
		return this.currentVelocity;
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0000AF85 File Offset: 0x00009185
	public float GetAngularSpeed()
	{
		return this.angularVelocity;
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00005B85 File Offset: 0x00003D85
	public float GetSpeed(bool rightSide)
	{
		return 1f;
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00058D14 File Offset: 0x00056F14
	private void AnimateWheelsTreads()
	{
		float num = Vector3.Dot(this.currentVelocity, base.transform.forward);
		float x = this.treadAmount = (this.treadAmount + Time.deltaTime * num * this.treadConstant * -1f) % 1f;
		this.leftTread.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.leftTread.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.leftTread.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		this.rightTread.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.rightTread.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.rightTread.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		if (num >= 0f)
		{
			this.wheelAngle = (this.wheelAngle + Time.deltaTime * num * this.wheelSpinConstant) % 360f;
		}
		else
		{
			this.wheelAngle += Time.deltaTime * num * this.wheelSpinConstant;
			if (this.wheelAngle <= 0f)
			{
				this.wheelAngle = 360f;
			}
		}
		this.mainBodyAnimator.SetFloat("wheel_spin", this.wheelAngle);
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00058E7C File Offset: 0x0005707C
	public void UpdateHeights()
	{
		Ray ray = default(Ray);
		if (this.cachedMask == -1)
		{
			this.cachedMask = LayerMask.GetMask(new string[]
			{
				"Terrain",
				"World",
				"Construction",
				"Debris",
				"Clutter",
				"Deployed",
				"Tree",
				"Default"
			});
		}
		int num = this.wheelBones.Length;
		for (int i = 0; i < num; i++)
		{
			ray.origin = base.transform.TransformPoint(this.wheelBoneOrigin[i]);
			ray.direction = base.transform.up * -1f;
			RaycastHit raycastHit;
			bool flag = Physics.SphereCast(ray, 0.25f, ref raycastHit, this.traceLineMax, this.cachedMask);
			Debug.DrawLine(ray.origin, flag ? raycastHit.point : (ray.origin + ray.direction * this.traceLineMax), flag ? Color.green : Color.red, Time.deltaTime * 1.5f);
			float num2;
			if (flag)
			{
				this.treadEffects.EnableTreadSmoke(i, true);
				num2 = raycastHit.distance - 0.13f;
			}
			else
			{
				this.treadEffects.EnableTreadSmoke(i, false);
				num2 = this.maxShockDist;
			}
			this.vecShocksOffsetPosition[i].y = Mathf.Lerp(this.vecShocksOffsetPosition[i].y, Mathf.Clamp(num2 * -1f, -this.maxShockDist, 0.1f), Time.deltaTime * 20f);
			this.wheelBones[i].localPosition = this.vecShocksOffsetPosition[i];
		}
	}
}
