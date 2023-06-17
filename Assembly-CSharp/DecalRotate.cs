using System;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class DecalRotate : DecalComponent
{
	// Token: 0x04000CA4 RID: 3236
	[MinMax(0f, 360f)]
	public MinMax range = new MinMax(0f, 360f);

	// Token: 0x06000FBD RID: 4029 RVA: 0x0000DF47 File Offset: 0x0000C147
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		rot *= Quaternion.Euler(0f, this.range.Random(), 0f);
	}
}
