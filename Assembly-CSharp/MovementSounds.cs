using System;
using UnityEngine;

// Token: 0x02000478 RID: 1144
public class MovementSounds : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x0400179B RID: 6043
	public SoundDefinition waterMovementDef;

	// Token: 0x0400179C RID: 6044
	public float waterMovementFadeInSpeed = 1f;

	// Token: 0x0400179D RID: 6045
	public float waterMovementFadeOutSpeed = 1f;

	// Token: 0x0400179E RID: 6046
	private Sound waterMovement;

	// Token: 0x0400179F RID: 6047
	private SoundModulation.Modulator waterGainMod;

	// Token: 0x040017A0 RID: 6048
	private Vector3 lastPos;

	// Token: 0x040017A1 RID: 6049
	public bool mute;

	// Token: 0x06001AB6 RID: 6838 RVA: 0x000161D5 File Offset: 0x000143D5
	public void OnParentDestroying()
	{
		this.waterMovement = null;
		this.waterGainMod = null;
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x00093734 File Offset: 0x00091934
	protected void Update()
	{
		float num = Vector3.Distance(this.lastPos, base.transform.position);
		if (this.waterMovement == null)
		{
			this.waterMovement = SoundManager.RequestSoundInstance(this.waterMovementDef, base.gameObject, default(Vector3), false);
		}
		if (this.waterMovement != null && this.waterGainMod == null)
		{
			this.waterGainMod = this.waterMovement.modulation.CreateModulator(SoundModulation.Parameter.Gain);
		}
		if (this.waterMovement != null && this.waterGainMod != null)
		{
			if (num > 0.1f && WaterLevel.Test(base.transform.position) && !this.mute)
			{
				if (!this.waterMovement.playing)
				{
					this.waterMovement.Play();
				}
				this.waterGainMod.value = Mathf.MoveTowards(this.waterGainMod.value, 1f, this.waterMovementFadeInSpeed * Time.deltaTime);
			}
			else
			{
				this.waterGainMod.value = Mathf.MoveTowards(this.waterGainMod.value, 0f, this.waterMovementFadeOutSpeed * Time.deltaTime);
				if (this.waterMovement.playing && this.waterGainMod.value <= 0.05f)
				{
					this.waterMovement.Stop();
				}
			}
		}
		this.lastPos = base.transform.position;
	}
}
