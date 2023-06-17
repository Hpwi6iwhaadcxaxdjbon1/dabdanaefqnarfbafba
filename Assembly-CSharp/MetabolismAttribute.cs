using System;
using UnityEngine;

// Token: 0x020002D5 RID: 725
[Serializable]
public class MetabolismAttribute
{
	// Token: 0x04001014 RID: 4116
	public float startMin;

	// Token: 0x04001015 RID: 4117
	public float startMax;

	// Token: 0x04001016 RID: 4118
	public float min;

	// Token: 0x04001017 RID: 4119
	public float max;

	// Token: 0x04001018 RID: 4120
	public float value;

	// Token: 0x04001019 RID: 4121
	internal float lastValue;

	// Token: 0x0400101A RID: 4122
	internal float lastGreatFraction;

	// Token: 0x0400101B RID: 4123
	private const float greatInterval = 0.1f;

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060013BA RID: 5050 RVA: 0x00010C1E File Offset: 0x0000EE1E
	public float greatFraction
	{
		get
		{
			return Mathf.Floor(this.Fraction() / 0.1f) / 10f;
		}
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x00010C37 File Offset: 0x0000EE37
	public void Reset()
	{
		this.value = Mathf.Clamp(Random.Range(this.startMin, this.startMax), this.min, this.max);
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x00010C61 File Offset: 0x0000EE61
	public float Fraction()
	{
		return Mathf.InverseLerp(this.min, this.max, this.value);
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x00010C7A File Offset: 0x0000EE7A
	public float InverseFraction()
	{
		return 1f - this.Fraction();
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x00010C88 File Offset: 0x0000EE88
	public void Add(float val)
	{
		this.value = Mathf.Clamp(this.value + val, this.min, this.max);
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x00010CA9 File Offset: 0x0000EEA9
	public void Subtract(float val)
	{
		this.value = Mathf.Clamp(this.value - val, this.min, this.max);
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x00010CCA File Offset: 0x0000EECA
	public void Increase(float fTarget)
	{
		fTarget = Mathf.Clamp(fTarget, this.min, this.max);
		if (fTarget <= this.value)
		{
			return;
		}
		this.value = fTarget;
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x00010CF1 File Offset: 0x0000EEF1
	public void MoveTowards(float fTarget, float fRate)
	{
		if (fRate == 0f)
		{
			return;
		}
		this.value = Mathf.Clamp(Mathf.MoveTowards(this.value, fTarget, fRate), this.min, this.max);
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x00010D20 File Offset: 0x0000EF20
	public bool HasChanged()
	{
		bool result = this.lastValue != this.value;
		this.lastValue = this.value;
		return result;
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0007C328 File Offset: 0x0007A528
	public bool HasGreatlyChanged()
	{
		float greatFraction = this.greatFraction;
		bool result = this.lastGreatFraction != greatFraction;
		this.lastGreatFraction = greatFraction;
		return result;
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x00010D3F File Offset: 0x0000EF3F
	public void SetValue(float newValue)
	{
		this.lastValue = this.value;
		this.value = newValue;
	}

	// Token: 0x020002D6 RID: 726
	public enum Type
	{
		// Token: 0x0400101D RID: 4125
		Calories,
		// Token: 0x0400101E RID: 4126
		Hydration,
		// Token: 0x0400101F RID: 4127
		Heartrate,
		// Token: 0x04001020 RID: 4128
		Poison,
		// Token: 0x04001021 RID: 4129
		Radiation,
		// Token: 0x04001022 RID: 4130
		Bleeding,
		// Token: 0x04001023 RID: 4131
		Health,
		// Token: 0x04001024 RID: 4132
		HealthOverTime
	}
}
