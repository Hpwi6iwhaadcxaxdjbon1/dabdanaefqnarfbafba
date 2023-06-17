using System;
using UnityEngine;

// Token: 0x02000719 RID: 1817
public class ParticleSystemPlayer : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x060027CC RID: 10188 RVA: 0x0001F0C3 File Offset: 0x0001D2C3
	protected void OnEnable()
	{
		base.GetComponent<ParticleSystem>().enableEmission = true;
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x0001F0D1 File Offset: 0x0001D2D1
	public void OnParentDestroying()
	{
		base.GetComponent<ParticleSystem>().enableEmission = false;
	}
}
