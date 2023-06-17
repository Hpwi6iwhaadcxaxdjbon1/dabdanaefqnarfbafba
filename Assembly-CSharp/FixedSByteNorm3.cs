using System;
using UnityEngine;

// Token: 0x02000762 RID: 1890
public struct FixedSByteNorm3
{
	// Token: 0x04002449 RID: 9289
	private const int FracBits = 7;

	// Token: 0x0400244A RID: 9290
	private const float MaxFrac = 128f;

	// Token: 0x0400244B RID: 9291
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x0400244C RID: 9292
	public sbyte x;

	// Token: 0x0400244D RID: 9293
	public sbyte y;

	// Token: 0x0400244E RID: 9294
	public sbyte z;

	// Token: 0x06002939 RID: 10553 RVA: 0x0002014C File Offset: 0x0001E34C
	public FixedSByteNorm3(Vector3 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x00020187 File Offset: 0x0001E387
	public static explicit operator Vector3(FixedSByteNorm3 vec)
	{
		return new Vector3((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f);
	}
}
