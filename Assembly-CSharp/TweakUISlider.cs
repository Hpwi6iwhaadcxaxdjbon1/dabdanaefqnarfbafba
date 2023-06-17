using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006D5 RID: 1749
public class TweakUISlider : MonoBehaviour
{
	// Token: 0x040022D2 RID: 8914
	public Slider sliderControl;

	// Token: 0x040022D3 RID: 8915
	public Text textControl;

	// Token: 0x040022D4 RID: 8916
	public string convarName = "effects.motionblur";

	// Token: 0x040022D5 RID: 8917
	internal ConsoleSystem.Command conVar;

	// Token: 0x060026C3 RID: 9923 RVA: 0x0001E3B8 File Offset: 0x0001C5B8
	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar != null)
		{
			this.UpdateSliderValue();
			this.UpdateTextValue();
			return;
		}
		Debug.LogWarning("Tweak Slider Convar Missing: " + this.convarName);
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x0001E3F5 File Offset: 0x0001C5F5
	protected void OnEnable()
	{
		this.UpdateSliderValue();
		this.UpdateTextValue();
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x0001E403 File Offset: 0x0001C603
	public void OnChanged()
	{
		this.UpdateConVar();
		this.UpdateTextValue();
		this.UpdateSliderValue();
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000CB248 File Offset: 0x000C9448
	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		float value = this.sliderControl.value;
		if (this.conVar.AsFloat == value)
		{
			return;
		}
		this.conVar.Set(value);
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000CB288 File Offset: 0x000C9488
	private void UpdateSliderValue()
	{
		if (this.conVar == null)
		{
			return;
		}
		float asFloat = this.conVar.AsFloat;
		if (this.sliderControl.value == asFloat)
		{
			return;
		}
		this.sliderControl.value = asFloat;
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000CB2C8 File Offset: 0x000C94C8
	private void UpdateTextValue()
	{
		if (this.sliderControl.wholeNumbers)
		{
			this.textControl.text = this.sliderControl.value.ToString("N0");
			return;
		}
		this.textControl.text = this.sliderControl.value.ToString("0.0");
	}
}
