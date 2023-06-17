using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class ScaleBySpeed : MonoBehaviour
{
	// Token: 0x04000D9D RID: 3485
	public float minScale = 0.001f;

	// Token: 0x04000D9E RID: 3486
	public float maxScale = 1f;

	// Token: 0x04000D9F RID: 3487
	public float minSpeed;

	// Token: 0x04000DA0 RID: 3488
	public float maxSpeed = 1f;

	// Token: 0x04000DA1 RID: 3489
	public MonoBehaviour component;

	// Token: 0x04000DA2 RID: 3490
	public bool toggleComponent = true;

	// Token: 0x04000DA3 RID: 3491
	public bool onlyWhenSubmerged;

	// Token: 0x04000DA4 RID: 3492
	public float submergedThickness = 0.33f;

	// Token: 0x04000DA5 RID: 3493
	private Vector3 prevPosition = Vector3.zero;

	// Token: 0x060010B6 RID: 4278 RVA: 0x0000EB15 File Offset: 0x0000CD15
	private void Start()
	{
		this.prevPosition = base.transform.position;
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x00070AE4 File Offset: 0x0006ECE4
	private void Update()
	{
		Vector3 position = base.transform.position;
		float num = (position - this.prevPosition).sqrMagnitude;
		float num2 = this.minScale;
		bool enabled = WaterSystem.GetHeight(position) > position.y - this.submergedThickness;
		if (num > 0.0001f)
		{
			num = Mathf.Sqrt(num);
			float value = Mathf.Clamp(num, this.minSpeed, this.maxSpeed) / (this.maxSpeed - this.minSpeed);
			num2 = Mathf.Lerp(this.minScale, this.maxScale, Mathf.Clamp01(value));
			if (this.component != null && this.toggleComponent)
			{
				this.component.enabled = enabled;
			}
		}
		else if (this.component != null && this.toggleComponent)
		{
			this.component.enabled = false;
		}
		base.transform.localScale = new Vector3(num2, num2, num2);
		this.prevPosition = position;
	}
}
