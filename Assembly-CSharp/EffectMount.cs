using System;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class EffectMount : EntityComponent<BaseEntity>, IClientComponent
{
	// Token: 0x04000820 RID: 2080
	public GameObject effectPrefab;

	// Token: 0x04000821 RID: 2081
	public GameObject spawnedEffect;

	// Token: 0x04000822 RID: 2082
	public GameObject mountBone;

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00059C4C File Offset: 0x00057E4C
	public void SetOn(bool isOn)
	{
		if (this.spawnedEffect)
		{
			GameManager.Destroy(this.spawnedEffect, 0f);
		}
		this.spawnedEffect = null;
		if (isOn)
		{
			this.spawnedEffect = Object.Instantiate<GameObject>(this.effectPrefab);
			this.spawnedEffect.transform.rotation = this.mountBone.transform.rotation;
			this.spawnedEffect.transform.position = this.mountBone.transform.position;
			this.spawnedEffect.transform.parent = this.mountBone.transform;
			this.spawnedEffect.SetActive(true);
		}
	}
}
