using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class LightEx : UpdateBehaviour, IClientComponent
{
	// Token: 0x04000CB8 RID: 3256
	public bool alterColor;

	// Token: 0x04000CB9 RID: 3257
	public float colorTimeScale = 1f;

	// Token: 0x04000CBA RID: 3258
	public Color colorA = Color.red;

	// Token: 0x04000CBB RID: 3259
	public Color colorB = Color.yellow;

	// Token: 0x04000CBC RID: 3260
	public AnimationCurve blendCurve = new AnimationCurve();

	// Token: 0x04000CBD RID: 3261
	public bool loopColor = true;

	// Token: 0x04000CBE RID: 3262
	public bool alterIntensity;

	// Token: 0x04000CBF RID: 3263
	public float intensityTimeScale = 1f;

	// Token: 0x04000CC0 RID: 3264
	public AnimationCurve intenseCurve = new AnimationCurve();

	// Token: 0x04000CC1 RID: 3265
	public float intensityCurveScale = 3f;

	// Token: 0x04000CC2 RID: 3266
	public bool loopIntensity = true;

	// Token: 0x04000CC3 RID: 3267
	public bool randomOffset;

	// Token: 0x04000CC4 RID: 3268
	private Light lightComponent;

	// Token: 0x04000CC5 RID: 3269
	private float time;

	// Token: 0x06000FF0 RID: 4080 RVA: 0x0000E150 File Offset: 0x0000C350
	protected void Awake()
	{
		this.lightComponent = base.GetComponent<Light>();
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x0000E15E File Offset: 0x0000C35E
	protected override void OnEnable()
	{
		base.OnEnable();
		this.time = (this.randomOffset ? Random.value : 0f);
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x0006BFB4 File Offset: 0x0006A1B4
	public override void DeltaUpdate(float deltaTime)
	{
		this.time += deltaTime;
		if (this.lightComponent == null)
		{
			return;
		}
		if (this.alterColor)
		{
			float num = this.time * this.colorTimeScale;
			if (this.loopColor)
			{
				num %= 1f;
			}
			Color color = Color.Lerp(this.colorA, this.colorB, this.blendCurve.Evaluate(num));
			color.a = 1f;
			this.lightComponent.color = color;
		}
		if (this.alterIntensity)
		{
			float num2 = this.time * this.intensityTimeScale;
			if (this.loopIntensity)
			{
				num2 %= 1f;
			}
			this.lightComponent.intensity = this.intenseCurve.Evaluate(num2) * this.intensityCurveScale;
			this.SetLightActive(this.lightComponent.intensity > 0f);
		}
		float num3 = Mathf.InverseLerp(10f, 1000f, MainCamera.Distance(base.transform.position));
		if (num3 > 0f)
		{
			base.Sleep(Mathf.Lerp(0f, 1f + Random.value, num3));
		}
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x0000E180 File Offset: 0x0000C380
	private void SetLightActive(bool isActive)
	{
		if (this.lightComponent.enabled != isActive)
		{
			this.lightComponent.enabled = isActive;
		}
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0000E19C File Offset: 0x0000C39C
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0006C0DC File Offset: 0x0006A2DC
	public static bool CheckConflict(GameObject go)
	{
		LightEx component = go.GetComponent<LightEx>();
		if (!component || !component.alterIntensity)
		{
			return false;
		}
		LightLOD component2 = go.GetComponent<LightLOD>();
		if (component2 && component2.ToggleLight)
		{
			Debug.LogError("Using LightEx.AlterIntensity and LightLOD.ToggleLight on the same light source is not permitted: " + go.transform.root.name, go.transform.root);
			return true;
		}
		if (go.GetComponent<AmbientLightLOD>())
		{
			Debug.LogError("Using LightEx.AlterIntensity and AmbientLightLOD on the same light source is not permitted: " + go.transform.root.name, go.transform.root);
			return true;
		}
		return false;
	}
}
