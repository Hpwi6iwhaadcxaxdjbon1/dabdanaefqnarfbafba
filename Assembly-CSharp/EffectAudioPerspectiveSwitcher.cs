using System;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class EffectAudioPerspectiveSwitcher : MonoBehaviour, IEffect
{
	// Token: 0x06001221 RID: 4641 RVA: 0x000772AC File Offset: 0x000754AC
	public virtual void SetupEffect(Effect effect)
	{
		if (base.gameObject.GetComponentInParent<BaseViewModel>() && !BaseViewModel.HideViewmodel)
		{
			this.MakeFirstPerson();
			return;
		}
		if (effect.gameObject == null)
		{
			this.MakeThirdPerson();
			return;
		}
		if (effect.gameObject.GetComponentInParent<BaseViewModel>() && !BaseViewModel.HideViewmodel)
		{
			this.MakeFirstPerson();
			return;
		}
		this.MakeThirdPerson();
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x00077314 File Offset: 0x00075514
	private void MakeThirdPerson()
	{
		SoundPlayer component = base.gameObject.GetComponent<SoundPlayer>();
		if (component)
		{
			component.MakeThirdPerson();
		}
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x0007733C File Offset: 0x0007553C
	private void MakeFirstPerson()
	{
		SoundPlayer component = base.gameObject.GetComponent<SoundPlayer>();
		if (component)
		{
			component.MakeFirstPerson();
		}
	}
}
