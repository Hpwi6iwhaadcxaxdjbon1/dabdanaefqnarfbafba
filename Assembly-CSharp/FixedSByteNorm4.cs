using System;
using UnityEngine;

// Token: 0x02000763 RID: 1891
public struct FixedSByteNorm4
{
	// Token: 0x0400244F RID: 9295
	private const int FracBits = 7;

	// Token: 0x04002450 RID: 9296
	private const float MaxFrac = 128f;

	// Token: 0x04002451 RID: 9297
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x04002452 RID: 9298
	public sbyte x;

	// Token: 0x04002453 RID: 9299
	public sbyte y;

	// Token: 0x04002454 RID: 9300
	public sbyte z;

	// Token: 0x04002455 RID: 9301
	public sbyte w;

	// Token: 0x0600293B RID: 10555 RVA: 0x000D2798 File Offset: 0x000D0998
	public FixedSByteNorm4(Vector4 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
		this.w = (sbyte)(vec.w * 128f);
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000201B5 File Offset: 0x0001E3B5
	public static explicit operator Vector4(FixedSByteNorm4 vec)
	{
		return new Vector4((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f, (float)vec.w * 0.0078125f);
	}
}
