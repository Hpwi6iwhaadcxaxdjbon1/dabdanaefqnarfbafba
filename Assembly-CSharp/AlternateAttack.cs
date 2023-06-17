using System;
using UnityEngine;

// Token: 0x02000770 RID: 1904
public class AlternateAttack : StateMachineBehaviour
{
	// Token: 0x04002479 RID: 9337
	public bool random;

	// Token: 0x0400247A RID: 9338
	public bool dontIncrement;

	// Token: 0x0400247B RID: 9339
	public string[] targetTransitions;

	// Token: 0x06002971 RID: 10609 RVA: 0x000D3178 File Offset: 0x000D1378
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.random)
		{
			string text = this.targetTransitions[Random.Range(0, this.targetTransitions.Length)];
			animator.Play(text, layerIndex, 0f);
			return;
		}
		int integer = animator.GetInteger("lastAttack");
		string text2 = this.targetTransitions[integer % this.targetTransitions.Length];
		animator.Play(text2, layerIndex, 0f);
		if (!this.dontIncrement)
		{
			animator.SetInteger("lastAttack", integer + 1);
		}
	}
}
