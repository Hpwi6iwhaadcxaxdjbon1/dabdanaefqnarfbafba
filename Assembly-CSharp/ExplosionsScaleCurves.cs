using System;
using UnityEngine;

// Token: 0x020007A4 RID: 1956
public class ExplosionsScaleCurves : MonoBehaviour
{
	// Token: 0x04002602 RID: 9730
	public AnimationCurve ScaleCurveX = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04002603 RID: 9731
	public AnimationCurve ScaleCurveY = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04002604 RID: 9732
	public AnimationCurve ScaleCurveZ = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04002605 RID: 9733
	public Vector3 GraphTimeMultiplier = Vector3.one;

	// Token: 0x04002606 RID: 9734
	public Vector3 GraphScaleMultiplier = Vector3.one;

	// Token: 0x04002607 RID: 9735
	private float startTime;

	// Token: 0x04002608 RID: 9736
	private Transform t;

	// Token: 0x04002609 RID: 9737
	private float evalX;

	// Token: 0x0400260A RID: 9738
	private float evalY;

	// Token: 0x0400260B RID: 9739
	private float evalZ;

	// Token: 0x06002A78 RID: 10872 RVA: 0x0002105F File Offset: 0x0001F25F
	private void Awake()
	{
		this.t = base.transform;
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x0002106D File Offset: 0x0001F26D
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.evalX = 0f;
		this.evalY = 0f;
		this.evalZ = 0f;
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x000D8AF4 File Offset: 0x000D6CF4
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (num <= this.GraphTimeMultiplier.x)
		{
			this.evalX = this.ScaleCurveX.Evaluate(num / this.GraphTimeMultiplier.x) * this.GraphScaleMultiplier.x;
		}
		if (num <= this.GraphTimeMultiplier.y)
		{
			this.evalY = this.ScaleCurveY.Evaluate(num / this.GraphTimeMultiplier.y) * this.GraphScaleMultiplier.y;
		}
		if (num <= this.GraphTimeMultiplier.z)
		{
			this.evalZ = this.ScaleCurveZ.Evaluate(num / this.GraphTimeMultiplier.z) * this.GraphScaleMultiplier.z;
		}
		this.t.localScale = new Vector3(this.evalX, this.evalY, this.evalZ);
	}
}
