using System;
using UnityEngine;

// Token: 0x0200027B RID: 635
public class EyeBlink : MonoBehaviour
{
	// Token: 0x04000ECB RID: 3787
	public Transform LeftEye;

	// Token: 0x04000ECC RID: 3788
	public Vector3 LeftEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x04000ECD RID: 3789
	public Transform RightEye;

	// Token: 0x04000ECE RID: 3790
	public Vector3 RightEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x04000ECF RID: 3791
	public Vector2 TimeWithoutBlinking = new Vector2(1f, 10f);

	// Token: 0x04000ED0 RID: 3792
	public float BlinkSpeed = 0.2f;

	// Token: 0x04000ED1 RID: 3793
	private Vector3 LeftEyeInitial;

	// Token: 0x04000ED2 RID: 3794
	private Vector3 RightEyeInitial;

	// Token: 0x04000ED3 RID: 3795
	private float BlinkCountdown = 4f;

	// Token: 0x0600123E RID: 4670 RVA: 0x0000FC3D File Offset: 0x0000DE3D
	private void Start()
	{
		this.LeftEyeInitial = this.LeftEye.localPosition;
		this.RightEyeInitial = this.RightEye.localPosition;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00077C58 File Offset: 0x00075E58
	private void LateUpdate()
	{
		if (this.BlinkCountdown <= this.BlinkSpeed)
		{
			float d = Mathf.Sin(Mathf.InverseLerp(this.BlinkSpeed, 0f, this.BlinkCountdown) * 3.1415927f);
			this.LeftEye.localPosition = this.LeftEyeInitial + this.LeftEyeOffset * d;
			this.RightEye.localPosition = this.RightEyeInitial + this.RightEyeOffset * d;
		}
		if (this.BlinkCountdown > 0f)
		{
			this.BlinkCountdown -= Time.deltaTime;
			return;
		}
		this.BlinkCountdown = Random.Range(this.TimeWithoutBlinking.x, this.TimeWithoutBlinking.y);
		this.LeftEye.localPosition = this.LeftEyeInitial;
		this.RightEye.localPosition = this.RightEyeInitial;
	}
}
