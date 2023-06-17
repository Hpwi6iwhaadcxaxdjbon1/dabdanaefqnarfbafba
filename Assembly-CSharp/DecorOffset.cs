using System;
using UnityEngine;

// Token: 0x020004A7 RID: 1191
public class DecorOffset : DecorComponent
{
	// Token: 0x0400188B RID: 6283
	public Vector3 MinOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x0400188C RID: 6284
	public Vector3 MaxOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x06001BB0 RID: 7088 RVA: 0x00099018 File Offset: 0x00097218
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = SeedEx.Seed(pos, World.Seed) + 1U;
		pos.x += scale.x * SeedRandom.Range(ref num, this.MinOffset.x, this.MaxOffset.x);
		pos.y += scale.y * SeedRandom.Range(ref num, this.MinOffset.y, this.MaxOffset.y);
		pos.z += scale.z * SeedRandom.Range(ref num, this.MinOffset.z, this.MaxOffset.z);
	}
}
