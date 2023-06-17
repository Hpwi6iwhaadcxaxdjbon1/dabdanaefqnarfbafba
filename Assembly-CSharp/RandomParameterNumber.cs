using System;
using UnityEngine;

// Token: 0x02000778 RID: 1912
public class RandomParameterNumber : StateMachineBehaviour
{
	// Token: 0x040024B7 RID: 9399
	public string parameterName;

	// Token: 0x040024B8 RID: 9400
	public int min;

	// Token: 0x040024B9 RID: 9401
	public int max;

	// Token: 0x060029AE RID: 10670 RVA: 0x00020692 File Offset: 0x0001E892
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetInteger(this.parameterName, Random.Range(this.min, this.max));
	}
}
