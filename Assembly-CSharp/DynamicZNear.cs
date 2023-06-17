using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class DynamicZNear : MonoBehaviour
{
	// Token: 0x04000CA6 RID: 3238
	public float minimum = 0.05f;

	// Token: 0x04000CA7 RID: 3239
	public float maximum = 1f;

	// Token: 0x04000CA8 RID: 3240
	private Camera cam;

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0000DFAE File Offset: 0x0000C1AE
	protected void Awake()
	{
		this.cam = base.GetComponent<Camera>();
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x0006B67C File Offset: 0x0006987C
	protected void LateUpdate()
	{
		float num = this.WorkoutZNear();
		if (this.cam.nearClipPlane == num)
		{
			return;
		}
		if (this.cam.nearClipPlane > num)
		{
			this.cam.nearClipPlane = num;
			return;
		}
		this.cam.nearClipPlane = Mathf.MoveTowards(this.cam.nearClipPlane, num, Time.deltaTime * 0.1f);
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x0006B6E4 File Offset: 0x000698E4
	private float WorkoutZNear()
	{
		if (BaseViewModel.ActiveModel && !BaseViewModel.HideViewmodel)
		{
			return this.minimum;
		}
		float max = this.maximum;
		max = Mathf.Clamp(this.ScreenTest(0f, 0f), this.minimum, max);
		max = Mathf.Clamp(this.ScreenTest(1f, 0f), this.minimum, max);
		max = Mathf.Clamp(this.ScreenTest(1f, 1f), this.minimum, max);
		max = Mathf.Clamp(this.ScreenTest(0f, 1f), this.minimum, max);
		return Mathf.Clamp(this.ScreenTest(0.5f, 0.5f), this.minimum, max);
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0006B7A4 File Offset: 0x000699A4
	private float ScreenTest(float x, float y)
	{
		float num = this.maximum * 2f;
		Ray ray = this.cam.ScreenPointToRay(new Vector3((float)Screen.width * x, (float)Screen.height * y));
		ray.origin = base.transform.position;
		RaycastHit raycastHit;
		if (!Physics.SphereCast(ray, 0.1f, ref raycastHit, num))
		{
			return this.maximum;
		}
		return this.minimum;
	}
}
