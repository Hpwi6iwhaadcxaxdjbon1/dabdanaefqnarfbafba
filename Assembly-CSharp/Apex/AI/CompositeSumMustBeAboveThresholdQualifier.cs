using System;
using System.Collections.Generic;
using Apex.Serialization;
using UnityEngine;

namespace Apex.AI
{
	// Token: 0x020008A9 RID: 2217
	[AICategory("Composite Qualifiers")]
	[FriendlyName("Sum must be above threshold", "Scores 0 if sum is below threshold.")]
	public class CompositeSumMustBeAboveThresholdQualifier : CompositeQualifier
	{
		// Token: 0x04002A9E RID: 10910
		[ApexSerialization(defaultValue = 0f)]
		public float threshold;

		// Token: 0x06002FFC RID: 12284 RVA: 0x000EBB08 File Offset: 0x000E9D08
		public sealed override float Score(IAIContext context, IList<IContextualScorer> scorers)
		{
			float num = 0f;
			int count = scorers.Count;
			for (int i = 0; i < count; i++)
			{
				IContextualScorer contextualScorer = scorers[i];
				if (!contextualScorer.isDisabled)
				{
					float num2 = contextualScorer.Score(context);
					if (num2 < 0f)
					{
						Debug.LogWarning("SumMustBeAboveThreshold scorer does not support scores below 0!");
					}
					else
					{
						num += num2;
						if (num > this.threshold)
						{
							break;
						}
					}
				}
			}
			if (num <= this.threshold)
			{
				return 0f;
			}
			return num;
		}
	}
}
