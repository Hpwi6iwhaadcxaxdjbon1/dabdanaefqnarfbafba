using System;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class DecorTransform : DecorComponent
{
	// Token: 0x04001891 RID: 6289
	public Vector3 Position = new Vector3(0f, 0f, 0f);

	// Token: 0x04001892 RID: 6290
	public Vector3 Rotation = new Vector3(0f, 0f, 0f);

	// Token: 0x04001893 RID: 6291
	public Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x06001BB8 RID: 7096 RVA: 0x000991F4 File Offset: 0x000973F4
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos += rot * Vector3.Scale(scale, this.Position);
		rot = Quaternion.Euler(this.Rotation) * rot;
		scale = Vector3.Scale(scale, this.Scale);
	}
}
