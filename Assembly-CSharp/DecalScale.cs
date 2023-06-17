using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class DecalScale : DecalComponent
{
	// Token: 0x04000CA5 RID: 3237
	[MinMax(0f, 2f)]
	public MinMax range = new MinMax(0.9f, 1.1f);

	// Token: 0x06000FBF RID: 4031 RVA: 0x0006B63C File Offset: 0x0006983C
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		float num = this.range.Random();
		scale.x = this.localScale.x * num;
		scale.z = this.localScale.z * num;
	}
}
