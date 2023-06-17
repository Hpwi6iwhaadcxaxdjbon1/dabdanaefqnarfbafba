using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class LocalPositionAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x04000CD3 RID: 3283
	public Vector3 centerPosition;

	// Token: 0x04000CD4 RID: 3284
	public bool worldSpace;

	// Token: 0x04000CD5 RID: 3285
	public float scaleX = 1f;

	// Token: 0x04000CD6 RID: 3286
	public float timeScaleX = 1f;

	// Token: 0x04000CD7 RID: 3287
	public AnimationCurve movementX = new AnimationCurve();

	// Token: 0x04000CD8 RID: 3288
	public float scaleY = 1f;

	// Token: 0x04000CD9 RID: 3289
	public float timeScaleY = 1f;

	// Token: 0x04000CDA RID: 3290
	public AnimationCurve movementY = new AnimationCurve();

	// Token: 0x04000CDB RID: 3291
	public float scaleZ = 1f;

	// Token: 0x04000CDC RID: 3292
	public float timeScaleZ = 1f;

	// Token: 0x04000CDD RID: 3293
	public AnimationCurve movementZ = new AnimationCurve();

	// Token: 0x06000FFB RID: 4091 RVA: 0x0006C5D4 File Offset: 0x0006A7D4
	protected void Update()
	{
		if (MainCamera.SqrDistance(base.transform.position) > 10000f)
		{
			return;
		}
		float x = this.movementX.Evaluate(Time.time * this.timeScaleX % 1f) * this.scaleX;
		float y = this.movementY.Evaluate(Time.time * this.timeScaleY % 1f) * this.scaleY;
		float z = this.movementZ.Evaluate(Time.time * this.timeScaleZ % 1f) * this.scaleZ;
		if (this.worldSpace)
		{
			base.transform.localPosition = base.transform.InverseTransformPoint(base.transform.position + this.centerPosition) + new Vector3(x, y, z);
			return;
		}
		base.transform.localPosition = this.centerPosition + new Vector3(x, y, z);
	}
}
