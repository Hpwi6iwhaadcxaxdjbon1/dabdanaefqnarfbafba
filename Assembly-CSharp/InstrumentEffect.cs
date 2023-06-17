using System;

// Token: 0x020003A0 RID: 928
public class InstrumentEffect : SoundPlayer, IEffect
{
	// Token: 0x04001470 RID: 5232
	public float pitch;

	// Token: 0x04001471 RID: 5233
	private SoundModulation.Modulator pitchMod;

	// Token: 0x06001786 RID: 6022 RVA: 0x0008AE60 File Offset: 0x00089060
	public override void CreateSound()
	{
		base.CreateSound();
		if (base.sound != null)
		{
			this.pitchMod = base.sound.modulation.CreateModulator(SoundModulation.Parameter.Pitch);
			this.pitch = this.pitch / 2f + 0.25f;
			this.pitchMod.value += this.pitch;
		}
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x00013B76 File Offset: 0x00011D76
	public virtual void SetupEffect(Effect e)
	{
		this.pitch = e.scale;
	}
}
