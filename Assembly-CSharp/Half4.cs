using System;
using UnityEngine;

// Token: 0x02000765 RID: 1893
public struct Half4
{
	// Token: 0x04002459 RID: 9305
	public ushort x;

	// Token: 0x0400245A RID: 9306
	public ushort y;

	// Token: 0x0400245B RID: 9307
	public ushort z;

	// Token: 0x0400245C RID: 9308
	public ushort w;

	// Token: 0x0600293F RID: 10559 RVA: 0x000D27F4 File Offset: 0x000D09F4
	public Half4(Vector4 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
		this.w = Mathf.FloatToHalf(vec.w);
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x0002024D File Offset: 0x0001E44D
	public static explicit operator Vector4(Half4 vec)
	{
		return new Vector4(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z), Mathf.HalfToFloat(vec.w));
	}
}
