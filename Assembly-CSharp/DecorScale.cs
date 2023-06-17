using System;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
public class DecorScale : DecorComponent
{
	// Token: 0x0400188F RID: 6287
	public Vector3 MinScale = new Vector3(1f, 1f, 1f);

	// Token: 0x04001890 RID: 6288
	public Vector3 MaxScale = new Vector3(2f, 2f, 2f);

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00099158 File Offset: 0x00097358
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = SeedEx.Seed(pos, World.Seed) + 3U;
		float t = SeedRandom.Value(ref num);
		scale.x *= Mathf.Lerp(this.MinScale.x, this.MaxScale.x, t);
		scale.y *= Mathf.Lerp(this.MinScale.y, this.MaxScale.y, t);
		scale.z *= Mathf.Lerp(this.MinScale.z, this.MaxScale.z, t);
	}
}
