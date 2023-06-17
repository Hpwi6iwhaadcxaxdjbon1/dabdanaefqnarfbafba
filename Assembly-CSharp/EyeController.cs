using System;
using UnityEngine;

// Token: 0x0200027C RID: 636
public class EyeController : MonoBehaviour
{
	// Token: 0x04000ED4 RID: 3796
	public const float MaxLookDot = 0.8f;

	// Token: 0x04000ED5 RID: 3797
	public bool debug;

	// Token: 0x04000ED6 RID: 3798
	public Transform LeftEye;

	// Token: 0x04000ED7 RID: 3799
	public Transform RightEye;

	// Token: 0x04000ED8 RID: 3800
	public Transform EyeTransform;

	// Token: 0x04000ED9 RID: 3801
	public Vector3 Fudge = new Vector3(0f, 90f, 0f);

	// Token: 0x04000EDA RID: 3802
	public Vector3 FlickerRange;

	// Token: 0x04000EDB RID: 3803
	private Transform Focus;

	// Token: 0x04000EDC RID: 3804
	private float FocusUpdateTime;

	// Token: 0x04000EDD RID: 3805
	private Vector3 Flicker;

	// Token: 0x04000EDE RID: 3806
	private Vector3 FlickerTarget;

	// Token: 0x04000EDF RID: 3807
	private float TimeToUpdateFlicker;

	// Token: 0x04000EE0 RID: 3808
	private float FlickerSpeed;

	// Token: 0x06001241 RID: 4673 RVA: 0x00077DB0 File Offset: 0x00075FB0
	public void UpdateEyes()
	{
		Vector3 vector = this.EyeTransform.position + this.EyeTransform.forward * 100f;
		Vector3 vector2 = vector;
		this.UpdateFocus(vector);
		this.UpdateFlicker();
		if (this.Focus != null)
		{
			vector2 = this.Focus.position;
			Vector3 vector3 = this.EyeTransform.position - vector;
			Vector3 vector4 = this.EyeTransform.position - vector2;
			if (Vector3.Dot(vector3.normalized, vector4.normalized) < 0.8f)
			{
				this.Focus = null;
			}
		}
		this.UpdateEye(this.LeftEye, vector2);
		this.UpdateEye(this.RightEye, vector2);
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00077E6C File Offset: 0x0007606C
	private void UpdateEye(Transform eye, Vector3 LookAt)
	{
		eye.rotation = Quaternion.LookRotation((LookAt - eye.position).normalized, this.EyeTransform.up) * Quaternion.Euler(this.Fudge) * Quaternion.Euler(this.Flicker);
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00077EC4 File Offset: 0x000760C4
	private void UpdateFlicker()
	{
		this.TimeToUpdateFlicker -= Time.deltaTime;
		this.Flicker = Vector3.Lerp(this.Flicker, this.FlickerTarget, Time.deltaTime * this.FlickerSpeed);
		if (this.TimeToUpdateFlicker < 0f)
		{
			this.TimeToUpdateFlicker = Random.Range(0.2f, 2f);
			this.FlickerTarget = new Vector3(Random.Range(-this.FlickerRange.x, this.FlickerRange.x), Random.Range(-this.FlickerRange.y, this.FlickerRange.y), Random.Range(-this.FlickerRange.z, this.FlickerRange.z)) * (this.Focus ? 0.01f : 1f);
			this.FlickerSpeed = Random.Range(10f, 30f);
		}
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00002ECE File Offset: 0x000010CE
	private void UpdateFocus(Vector3 defaultLookAtPos)
	{
	}
}
