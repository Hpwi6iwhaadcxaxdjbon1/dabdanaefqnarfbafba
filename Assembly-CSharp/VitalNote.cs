using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006F3 RID: 1779
public class VitalNote : MonoBehaviour, IClientComponent
{
	// Token: 0x04002322 RID: 8994
	public VitalNote.Vital VitalType;

	// Token: 0x04002323 RID: 8995
	public FloatConditions showIf;

	// Token: 0x04002324 RID: 8996
	public Text valueText;

	// Token: 0x04002325 RID: 8997
	public Animator animator;

	// Token: 0x04002326 RID: 8998
	private float lastValue = float.NaN;

	// Token: 0x04002327 RID: 8999
	private bool show = true;

	// Token: 0x06002739 RID: 10041 RVA: 0x000CC298 File Offset: 0x000CA498
	private void Update()
	{
		if (LocalPlayer.Entity == null)
		{
			this.Hide();
			return;
		}
		PlayerMetabolism metabolism = LocalPlayer.Entity.metabolism;
		switch (this.VitalType)
		{
		case VitalNote.Vital.Comfort:
			this.UpdateShow(metabolism.comfort.Fraction() * 100f, "{0:0}%");
			return;
		case VitalNote.Vital.Radiation:
			this.UpdateShow(metabolism.radiation_poison.value, "{0:N0}");
			return;
		case VitalNote.Vital.Poison:
			this.UpdateShow(metabolism.poison.Fraction() * 100f, "{0:0}%");
			return;
		case VitalNote.Vital.Cold:
			this.UpdateShow(metabolism.temperature.value, "{0:0}°c");
			return;
		case VitalNote.Vital.Bleeding:
			this.UpdateShow(metabolism.bleeding.value, "{0:N0}");
			return;
		case VitalNote.Vital.Hot:
			this.UpdateShow(metabolism.temperature.value, "{0:0}°c");
			return;
		case VitalNote.Vital.Drowning:
			this.UpdateShow((1f - metabolism.oxygen.Fraction()) * 100f, "{0:N0}%");
			return;
		case VitalNote.Vital.Wet:
			this.UpdateShow(metabolism.wetness.Fraction() * 100f, "{0:N0}%");
			return;
		case VitalNote.Vital.Hygiene:
			this.UpdateShow(metabolism.dirtyness.value, "{0:N0}");
			return;
		case VitalNote.Vital.Starving:
			this.UpdateShow(metabolism.calories.value, "{0:N0}");
			return;
		case VitalNote.Vital.Dehydration:
			this.UpdateShow(metabolism.hydration.value, "{0:N0}");
			return;
		default:
			throw new ArgumentOutOfRangeException(this.VitalType.ToString());
		}
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000CC434 File Offset: 0x000CA634
	private void UpdateShow(float value, string format)
	{
		if (!float.IsNaN(this.lastValue))
		{
			value = Mathf.MoveTowards(this.lastValue, value, Time.deltaTime * 200f);
		}
		if (this.valueText && this.lastValue != value)
		{
			this.lastValue = value;
			this.valueText.text = string.Format(format, value);
		}
		if (this.showIf.AllTrue(value))
		{
			this.Show();
			return;
		}
		this.Hide();
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x0001E944 File Offset: 0x0001CB44
	private void Hide()
	{
		if (!this.show)
		{
			return;
		}
		this.show = false;
		this.animator.SetBool("show", false);
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x0001E967 File Offset: 0x0001CB67
	private void Show()
	{
		if (this.show)
		{
			return;
		}
		this.show = true;
		this.animator.SetBool("show", true);
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x0001E98A File Offset: 0x0001CB8A
	private void ClientConnected()
	{
		this.Hide();
		this.lastValue = float.NaN;
	}

	// Token: 0x020006F4 RID: 1780
	public enum Vital
	{
		// Token: 0x04002329 RID: 9001
		Comfort,
		// Token: 0x0400232A RID: 9002
		Radiation,
		// Token: 0x0400232B RID: 9003
		Poison,
		// Token: 0x0400232C RID: 9004
		Cold,
		// Token: 0x0400232D RID: 9005
		Bleeding,
		// Token: 0x0400232E RID: 9006
		Hot,
		// Token: 0x0400232F RID: 9007
		Drowning,
		// Token: 0x04002330 RID: 9008
		Wet,
		// Token: 0x04002331 RID: 9009
		Hygiene,
		// Token: 0x04002332 RID: 9010
		Starving,
		// Token: 0x04002333 RID: 9011
		Dehydration
	}
}
