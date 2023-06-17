using System;
using UnityEngine;

// Token: 0x020004A8 RID: 1192
public class DecorRotate : DecorComponent
{
	// Token: 0x0400188D RID: 6285
	public Vector3 MinRotation = new Vector3(0f, -180f, 0f);

	// Token: 0x0400188E RID: 6286
	public Vector3 MaxRotation = new Vector3(0f, 180f, 0f);

	// Token: 0x06001BB2 RID: 7090 RVA: 0x000990C4 File Offset: 0x000972C4
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = SeedEx.Seed(pos, World.Seed) + 2U;
		float x = SeedRandom.Range(ref num, this.MinRotation.x, this.MaxRotation.x);
		float y = SeedRandom.Range(ref num, this.MinRotation.y, this.MaxRotation.y);
		float z = SeedRandom.Range(ref num, this.MinRotation.z, this.MaxRotation.z);
		rot = Quaternion.Euler(x, y, z) * rot;
	}
}
