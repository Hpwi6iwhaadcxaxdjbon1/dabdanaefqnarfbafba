using System;
using UnityEngine;

// Token: 0x020007A2 RID: 1954
public class ExplosionsLightCurves : MonoBehaviour
{
	// Token: 0x040025FB RID: 9723
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040025FC RID: 9724
	public float GraphTimeMultiplier = 1f;

	// Token: 0x040025FD RID: 9725
	public float GraphIntensityMultiplier = 1f;

	// Token: 0x040025FE RID: 9726
	private bool canUpdate;

	// Token: 0x040025FF RID: 9727
	private float startTime;

	// Token: 0x04002600 RID: 9728
	private Light lightSource;

	// Token: 0x06002A71 RID: 10865 RVA: 0x00020FD2 File Offset: 0x0001F1D2
	private void Awake()
	{
		this.lightSource = base.GetComponent<Light>();
		this.lightSource.intensity = this.LightCurve.Evaluate(0f);
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x00020FFB File Offset: 0x0001F1FB
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06002A73 RID: 10867 RVA: 0x000D8A98 File Offset: 0x000D6C98
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float intensity = this.LightCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphIntensityMultiplier;
			this.lightSource.intensity = intensity;
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}
