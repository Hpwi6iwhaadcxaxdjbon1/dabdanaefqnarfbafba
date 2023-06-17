using System;
using UnityEngine;

// Token: 0x020006FC RID: 1788
[Serializable]
public class MinMax
{
	// Token: 0x0400233C RID: 9020
	public float x;

	// Token: 0x0400233D RID: 9021
	public float y = 1f;

	// Token: 0x06002755 RID: 10069 RVA: 0x0001EA69 File Offset: 0x0001CC69
	public MinMax(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x0001EA8A File Offset: 0x0001CC8A
	public float Random()
	{
		return UnityEngine.Random.Range(this.x, this.y);
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x0001EA9D File Offset: 0x0001CC9D
	public float Lerp(float t)
	{
		return Mathf.Lerp(this.x, this.y, t);
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x0001EAB1 File Offset: 0x0001CCB1
	public float Lerp(float a, float b, float t)
	{
		return Mathf.Lerp(this.x, this.y, Mathf.InverseLerp(a, b, t));
	}
}
