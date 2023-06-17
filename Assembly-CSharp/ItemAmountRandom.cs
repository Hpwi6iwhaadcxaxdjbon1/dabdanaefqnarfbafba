using System;
using UnityEngine;

// Token: 0x0200046E RID: 1134
[Serializable]
public class ItemAmountRandom
{
	// Token: 0x04001780 RID: 6016
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x04001781 RID: 6017
	public AnimationCurve amount = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x06001A96 RID: 6806 RVA: 0x000160A5 File Offset: 0x000142A5
	public int RandomAmount()
	{
		return Mathf.RoundToInt(this.amount.Evaluate(Random.Range(0f, 1f)));
	}
}
