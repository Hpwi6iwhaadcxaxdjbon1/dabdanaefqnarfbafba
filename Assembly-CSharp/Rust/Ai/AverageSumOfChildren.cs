using System;
using System.Collections.Generic;
using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008B8 RID: 2232
	public class AverageSumOfChildren : CompositeQualifier
	{
		// Token: 0x04002AF8 RID: 11000
		[ApexSerialization]
		private bool normalize = true;

		// Token: 0x04002AF9 RID: 11001
		[ApexSerialization]
		private float postNormalizeMultiplier = 1f;

		// Token: 0x04002AFA RID: 11002
		[ApexSerialization]
		private float MaxAverageScore = 100f;

		// Token: 0x04002AFB RID: 11003
		[ApexSerialization]
		private bool FailIfAnyScoreZero = true;

		// Token: 0x0600302B RID: 12331 RVA: 0x000EC12C File Offset: 0x000EA32C
		public override float Score(IAIContext context, IList<IContextualScorer> scorers)
		{
			if (scorers.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < scorers.Count; i++)
			{
				float num2 = scorers[i].Score(context);
				if (this.FailIfAnyScoreZero && (num2 < 0f || Mathf.Approximately(num2, 0f)))
				{
					return 0f;
				}
				num += num2;
			}
			num /= (float)scorers.Count;
			if (this.normalize)
			{
				num /= this.MaxAverageScore;
				return num * this.postNormalizeMultiplier;
			}
			return num;
		}
	}
}
