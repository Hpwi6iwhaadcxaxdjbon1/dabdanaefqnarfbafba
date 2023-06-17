using System;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class AverageVelocity
{
	// Token: 0x04000BE7 RID: 3047
	private Vector3 pos;

	// Token: 0x04000BE8 RID: 3048
	private float time;

	// Token: 0x04000BE9 RID: 3049
	private float lastEntry;

	// Token: 0x04000BEA RID: 3050
	private float averageSpeed;

	// Token: 0x04000BEB RID: 3051
	private Vector3 averageVelocity;

	// Token: 0x06000EDC RID: 3804 RVA: 0x00066B70 File Offset: 0x00064D70
	public void Record(Vector3 newPos)
	{
		float num = Time.time - this.time;
		if (num < 0.1f)
		{
			return;
		}
		if (this.pos.sqrMagnitude > 0f)
		{
			Vector3 a = newPos - this.pos;
			this.averageVelocity = a * (1f / num);
			this.averageSpeed = this.averageVelocity.magnitude;
		}
		this.time = Time.time;
		this.pos = newPos;
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000EDD RID: 3805 RVA: 0x0000D69E File Offset: 0x0000B89E
	public float Speed
	{
		get
		{
			return this.averageSpeed;
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0000D6A6 File Offset: 0x0000B8A6
	public Vector3 Average
	{
		get
		{
			return this.averageVelocity;
		}
	}
}
