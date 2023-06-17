using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200027E RID: 638
public class FirstPersonEffect : MonoBehaviour, IEffect
{
	// Token: 0x04000EE9 RID: 3817
	public bool isGunShot;

	// Token: 0x0600124A RID: 4682 RVA: 0x00078144 File Offset: 0x00076344
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

	// Token: 0x0600124B RID: 4683 RVA: 0x000781AC File Offset: 0x000763AC
	private void MakeThirdPerson()
	{
		List<AudioSource> list = Pool.GetList<AudioSource>();
		base.GetComponentsInChildren<AudioSource>(list);
		foreach (AudioSource audioSource in list)
		{
			audioSource.spatialBlend = 1f;
			if (this.isGunShot && MainCamera.isValid)
			{
				float num = MainCamera.Distance(base.transform.position);
				float num2 = Mathf.InverseLerp(0f, audioSource.maxDistance, num * 4f);
				audioSource.pitch = 1f - num2 * 0.5f;
			}
		}
		Pool.FreeList<AudioSource>(ref list);
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x00078260 File Offset: 0x00076460
	private void MakeFirstPerson()
	{
		List<AudioSource> list = Pool.GetList<AudioSource>();
		base.GetComponentsInChildren<AudioSource>(list);
		foreach (AudioSource audioSource in list)
		{
			audioSource.spatialBlend = 0f;
		}
		Pool.FreeList<AudioSource>(ref list);
	}
}
