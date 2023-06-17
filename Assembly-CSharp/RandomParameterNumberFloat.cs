using System;
using UnityEngine;

// Token: 0x02000779 RID: 1913
public class RandomParameterNumberFloat : StateMachineBehaviour
{
	// Token: 0x040024BA RID: 9402
	public string parameterName;

	// Token: 0x040024BB RID: 9403
	public int min;

	// Token: 0x040024BC RID: 9404
	public int max;

	// Token: 0x060029B0 RID: 10672 RVA: 0x000206B1 File Offset: 0x0001E8B1
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (string.IsNullOrEmpty(this.parameterName))
		{
			return;
		}
		animator.SetFloat(this.parameterName, Mathf.Floor(Random.Range((float)this.min, (float)this.max + 0.5f)));
	}
}
