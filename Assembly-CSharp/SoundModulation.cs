using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class SoundModulation : MonoBehaviour, IClientComponent
{
	// Token: 0x04000AF3 RID: 2803
	private const int parameterCount = 4;

	// Token: 0x04000AF4 RID: 2804
	private Sound sound;

	// Token: 0x04000AF5 RID: 2805
	private List<List<SoundModulation.Modulator>> modulators = new List<List<SoundModulation.Modulator>>();

	// Token: 0x04000AF6 RID: 2806
	private List<float> modulationValues = new List<float>();

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00062A00 File Offset: 0x00060C00
	protected void Awake()
	{
		this.sound = base.GetComponent<Sound>();
		for (int i = 0; i < 4; i++)
		{
			this.modulationValues.Add(1f);
			this.modulators.Add(new List<SoundModulation.Modulator>());
		}
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00062A48 File Offset: 0x00060C48
	public void Init()
	{
		for (int i = 0; i < 4; i++)
		{
			this.modulators[i].Clear();
			this.modulationValues[i] = 1f;
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00062A84 File Offset: 0x00060C84
	public void CalculateValues()
	{
		this.modulationValues[0] = Mathf.Pow(this.sound.definition.volume * this.ModulationValue(SoundModulation.Parameter.Gain), Sound.volumeExponent);
		this.modulationValues[1] = this.sound.definition.pitch * this.ModulationValue(SoundModulation.Parameter.Pitch);
		this.modulationValues[3] = this.sound.initialMaxDistance * this.ModulationValue(SoundModulation.Parameter.MaxDistance);
		if (!this.sound.definition.useCustomSpreadCurve)
		{
			this.modulationValues[2] = this.sound.initialSpread * this.ModulationValue(SoundModulation.Parameter.Spread);
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00062B34 File Offset: 0x00060D34
	public void ApplyModulations(AudioSource source)
	{
		if (this.modulationValues[0] != source.volume)
		{
			source.volume = this.modulationValues[0];
		}
		if (this.modulationValues[1] != source.pitch)
		{
			source.pitch = this.modulationValues[1];
		}
		if (this.modulationValues[3] != source.maxDistance)
		{
			source.maxDistance = this.modulationValues[3];
		}
		if (this.modulationValues[2] != source.spread && !this.sound.definition.useCustomSpreadCurve)
		{
			source.spread = this.modulationValues[2];
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00062BEC File Offset: 0x00060DEC
	public SoundModulation.Modulator CreateModulator(SoundModulation.Parameter param)
	{
		SoundModulation.Modulator modulator = Pool.Get<SoundModulation.Modulator>();
		modulator.param = param;
		modulator.value = 1f;
		if (this.modulators.Count <= (int)param)
		{
			Debug.LogError("Invalid CreateModulator");
		}
		else
		{
			this.modulators[(int)param].Add(modulator);
		}
		return modulator;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0000CB4B File Offset: 0x0000AD4B
	public void AddModulator(SoundModulation.Modulator mod)
	{
		if (!this.modulators[(int)mod.param].Contains(mod))
		{
			this.modulators[(int)mod.param].Add(mod);
		}
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x0000CB7D File Offset: 0x0000AD7D
	public void RemoveModulator(SoundModulation.Modulator mod)
	{
		this.modulators[(int)mod.param].Remove(mod);
		Pool.Free<SoundModulation.Modulator>(ref mod);
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x00062C40 File Offset: 0x00060E40
	public float ModulationValue(SoundModulation.Parameter param)
	{
		float num = 1f;
		if (this.modulators.Count <= (int)param)
		{
			return num;
		}
		List<SoundModulation.Modulator> list = this.modulators[(int)param];
		if (list == null)
		{
			return num;
		}
		for (int i = 0; i < list.Count; i++)
		{
			num *= list[i].value;
		}
		return num;
	}

	// Token: 0x0200018C RID: 396
	public enum Parameter
	{
		// Token: 0x04000AF8 RID: 2808
		Gain,
		// Token: 0x04000AF9 RID: 2809
		Pitch,
		// Token: 0x04000AFA RID: 2810
		Spread,
		// Token: 0x04000AFB RID: 2811
		MaxDistance
	}

	// Token: 0x0200018D RID: 397
	[Serializable]
	public class Modulator
	{
		// Token: 0x04000AFC RID: 2812
		public SoundModulation.Parameter param;

		// Token: 0x04000AFD RID: 2813
		public float value = 1f;
	}
}
