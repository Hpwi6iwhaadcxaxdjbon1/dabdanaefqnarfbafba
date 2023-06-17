using System;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class BigWheelGame : SpinnerWheel
{
	// Token: 0x0400074D RID: 1869
	public HitNumber[] hitNumbers;

	// Token: 0x0400074E RID: 1870
	public GameObject indicator;

	// Token: 0x0400074F RID: 1871
	public GameObjectRef winEffect;

	// Token: 0x06000AFA RID: 2810 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool AllowPlayerSpins()
	{
		return false;
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool CanUpdateSign(BasePlayer player)
	{
		return false;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00056CA8 File Offset: 0x00054EA8
	public HitNumber GetCurrentHitType()
	{
		HitNumber result = null;
		float num = float.PositiveInfinity;
		foreach (HitNumber hitNumber in this.hitNumbers)
		{
			float num2 = Vector3.Distance(this.indicator.transform.position, hitNumber.transform.position);
			if (num2 < num)
			{
				result = hitNumber;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00056D08 File Offset: 0x00054F08
	[ContextMenu("LoadHitNumbers")]
	private void LoadHitNumbers()
	{
		HitNumber[] componentsInChildren = base.GetComponentsInChildren<HitNumber>();
		this.hitNumbers = componentsInChildren;
	}
}
