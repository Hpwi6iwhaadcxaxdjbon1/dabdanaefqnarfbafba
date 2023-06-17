using System;
using UnityEngine;

// Token: 0x020004A5 RID: 1189
public class DecorFlip : DecorComponent
{
	// Token: 0x04001886 RID: 6278
	public DecorFlip.AxisType FlipAxis = DecorFlip.AxisType.Y;

	// Token: 0x06001BAE RID: 7086 RVA: 0x00098F78 File Offset: 0x00097178
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = SeedEx.Seed(pos, World.Seed) + 4U;
		if (SeedRandom.Value(ref num) > 0.5f)
		{
			return;
		}
		switch (this.FlipAxis)
		{
		case DecorFlip.AxisType.X:
		case DecorFlip.AxisType.Z:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.up) * rot;
			return;
		case DecorFlip.AxisType.Y:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.forward) * rot;
			return;
		default:
			return;
		}
	}

	// Token: 0x020004A6 RID: 1190
	public enum AxisType
	{
		// Token: 0x04001888 RID: 6280
		X,
		// Token: 0x04001889 RID: 6281
		Y,
		// Token: 0x0400188A RID: 6282
		Z
	}
}
