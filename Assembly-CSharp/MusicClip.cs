using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class MusicClip : ScriptableObject
{
	// Token: 0x040009FE RID: 2558
	public AudioClip audioClip;

	// Token: 0x040009FF RID: 2559
	public int lengthInBars = 1;

	// Token: 0x04000A00 RID: 2560
	public int lengthInBarsWithTail;

	// Token: 0x04000A01 RID: 2561
	public List<float> fadeInPoints = new List<float>();

	// Token: 0x06000D08 RID: 3336 RVA: 0x0005ED04 File Offset: 0x0005CF04
	public float GetNextFadeInPoint(float currentClipTimeBars)
	{
		if (this.fadeInPoints.Count == 0)
		{
			return currentClipTimeBars;
		}
		float result = -1f;
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.fadeInPoints.Count; i++)
		{
			float num2 = this.fadeInPoints[i];
			float num3 = num2 - currentClipTimeBars;
			if (num2 > 0.01f && num3 > 0f && num3 < num)
			{
				num = num3;
				result = num2;
			}
		}
		return result;
	}
}
