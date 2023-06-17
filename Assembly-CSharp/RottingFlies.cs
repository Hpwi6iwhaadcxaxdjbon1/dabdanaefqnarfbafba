using System;
using Rust;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class RottingFlies : MonoBehaviour, IClientComponent
{
	// Token: 0x04000F10 RID: 3856
	public GameObjectRef effect;

	// Token: 0x04000F11 RID: 3857
	public SoundDefinition soundDef;

	// Token: 0x04000F12 RID: 3858
	public Transform rootBone;

	// Token: 0x04000F13 RID: 3859
	private GameObject particleInstance;

	// Token: 0x04000F14 RID: 3860
	private Sound sound;

	// Token: 0x0600126B RID: 4715 RVA: 0x00078BB8 File Offset: 0x00076DB8
	private void OnEnable()
	{
		if (MainCamera.Distance(base.transform.position) > 100f)
		{
			return;
		}
		this.particleInstance = EffectLibrary.CreateEffect(this.effect.resourcePath, this.rootBone.position, Quaternion.LookRotation(Vector3.up));
		this.sound = SoundManager.RequestSoundInstance(this.soundDef, this.particleInstance, default(Vector3), false);
		if (this.sound != null)
		{
			this.sound.FadeInAndPlay(10f);
		}
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x00078C48 File Offset: 0x00076E48
	private void Update()
	{
		if (this.particleInstance != null)
		{
			this.particleInstance.transform.position = Vector3.Lerp(this.particleInstance.transform.position, this.rootBone.position, Time.deltaTime * 10f);
		}
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x00078CA0 File Offset: 0x00076EA0
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.sound != null)
		{
			this.sound.FadeOutAndRecycle(0.1f);
			this.sound = null;
		}
		if (this.particleInstance != null)
		{
			GameManager.client.Retire(this.particleInstance);
			this.particleInstance = null;
		}
	}
}
