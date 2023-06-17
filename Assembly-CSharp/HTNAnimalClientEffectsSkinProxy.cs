using System;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class HTNAnimalClientEffectsSkinProxy : MonoBehaviour
{
	// Token: 0x040008DA RID: 2266
	public HTNAnimal Body;

	// Token: 0x06000C30 RID: 3120 RVA: 0x0000B6C2 File Offset: 0x000098C2
	private void FrontLeftFootstep()
	{
		HTNAnimal body = this.Body;
		if (body == null)
		{
			return;
		}
		body.FrontLeftFootstep();
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0000B6D4 File Offset: 0x000098D4
	private void FrontRightFootstep()
	{
		HTNAnimal body = this.Body;
		if (body == null)
		{
			return;
		}
		body.FrontRightFootstep();
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0000B6E6 File Offset: 0x000098E6
	private void BackLeftFootstep()
	{
		HTNAnimal body = this.Body;
		if (body == null)
		{
			return;
		}
		body.BackLeftFootstep();
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0000B6F8 File Offset: 0x000098F8
	private void BackRightFootstep()
	{
		HTNAnimal body = this.Body;
		if (body == null)
		{
			return;
		}
		body.BackRightFootstep();
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x0000B70A File Offset: 0x0000990A
	public void DoEffect(string effect)
	{
		HTNAnimal body = this.Body;
		if (body == null)
		{
			return;
		}
		body.DoEffect(effect);
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x0000B71D File Offset: 0x0000991D
	public void PlaySound(string soundName)
	{
		HTNAnimal body = this.Body;
		if (body == null)
		{
			return;
		}
		body.PlaySound(soundName);
	}
}
