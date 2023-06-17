using System;
using UnityEngine;

// Token: 0x02000764 RID: 1892
public struct Half3
{
	// Token: 0x04002456 RID: 9302
	public ushort x;

	// Token: 0x04002457 RID: 9303
	public ushort y;

	// Token: 0x04002458 RID: 9304
	public ushort z;

	// Token: 0x0600293D RID: 10557 RVA: 0x000201F0 File Offset: 0x0001E3F0
	public Half3(Vector3 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x00020225 File Offset: 0x0001E425
	public static explicit operator Vector3(Half3 vec)
	{
		return new Vector3(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z));
	}
}
