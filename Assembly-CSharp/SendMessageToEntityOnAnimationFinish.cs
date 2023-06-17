using System;
using UnityEngine;

// Token: 0x020005BA RID: 1466
public class SendMessageToEntityOnAnimationFinish : StateMachineBehaviour
{
	// Token: 0x04001D93 RID: 7571
	public string messageToSendToEntity;

	// Token: 0x04001D94 RID: 7572
	public float repeatRate = 0.1f;

	// Token: 0x04001D95 RID: 7573
	private const float lastMessageSent = 0f;

	// Token: 0x060021BD RID: 8637 RVA: 0x000B6A1C File Offset: 0x000B4C1C
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (0f + this.repeatRate > Time.time)
		{
			return;
		}
		if (animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (stateInfo.normalizedTime < 1f)
		{
			return;
		}
		for (int i = 0; i < animator.layerCount; i++)
		{
			if (i != layerIndex)
			{
				if (animator.IsInTransition(i))
				{
					return;
				}
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
				if (currentAnimatorStateInfo.speed > 0f && currentAnimatorStateInfo.normalizedTime < 1f)
				{
					return;
				}
			}
		}
		BaseEntity baseEntity = animator.gameObject.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.SendMessage(this.messageToSendToEntity);
		}
	}
}
