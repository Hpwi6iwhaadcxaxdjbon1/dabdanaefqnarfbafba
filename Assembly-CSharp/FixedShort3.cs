using System;
using UnityEngine;

// Token: 0x02000761 RID: 1889
public struct FixedShort3
{
	// Token: 0x04002443 RID: 9283
	private const int FracBits = 10;

	// Token: 0x04002444 RID: 9284
	private const float MaxFrac = 1024f;

	// Token: 0x04002445 RID: 9285
	private const float RcpMaxFrac = 0.0009765625f;

	// Token: 0x04002446 RID: 9286
	public short x;

	// Token: 0x04002447 RID: 9287
	public short y;

	// Token: 0x04002448 RID: 9288
	public short z;

	// Token: 0x06002937 RID: 10551 RVA: 0x000200E3 File Offset: 0x0001E2E3
	public FixedShort3(Vector3 vec)
	{
		this.x = (short)(vec.x * 1024f);
		this.y = (short)(vec.y * 1024f);
		this.z = (short)(vec.z * 1024f);
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x0002011E File Offset: 0x0001E31E
	public static explicit operator Vector3(FixedShort3 vec)
	{
		return new Vector3((float)vec.x * 0.0009765625f, (float)vec.y * 0.0009765625f, (float)vec.z * 0.0009765625f);
	}
}
