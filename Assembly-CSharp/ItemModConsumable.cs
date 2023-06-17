using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200044C RID: 1100
public class ItemModConsumable : MonoBehaviour
{
	// Token: 0x04001708 RID: 5896
	public int amountToConsume = 1;

	// Token: 0x04001709 RID: 5897
	public float conditionFractionToLose;

	// Token: 0x0400170A RID: 5898
	public List<ItemModConsumable.ConsumableEffect> effects = new List<ItemModConsumable.ConsumableEffect>();

	// Token: 0x0200044D RID: 1101
	[Serializable]
	public class ConsumableEffect
	{
		// Token: 0x0400170B RID: 5899
		public MetabolismAttribute.Type type;

		// Token: 0x0400170C RID: 5900
		public float amount;

		// Token: 0x0400170D RID: 5901
		public float time;

		// Token: 0x0400170E RID: 5902
		public float onlyIfHealthLessThan = 1f;
	}
}
