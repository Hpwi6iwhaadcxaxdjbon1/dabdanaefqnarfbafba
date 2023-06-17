using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006D1 RID: 1745
public class TweakUIDropdown : MonoBehaviour
{
	// Token: 0x040022C1 RID: 8897
	public Button Left;

	// Token: 0x040022C2 RID: 8898
	public Button Right;

	// Token: 0x040022C3 RID: 8899
	public Text Current;

	// Token: 0x040022C4 RID: 8900
	public Image BackgroundImage;

	// Token: 0x040022C5 RID: 8901
	public TweakUIDropdown.NameValue[] nameValues;

	// Token: 0x040022C6 RID: 8902
	public string convarName = "effects.motionblur";

	// Token: 0x040022C7 RID: 8903
	public bool assignImageColor;

	// Token: 0x040022C8 RID: 8904
	internal ConsoleSystem.Command conVar;

	// Token: 0x040022C9 RID: 8905
	public int currentValue;

	// Token: 0x060026B2 RID: 9906 RVA: 0x0001E2F8 File Offset: 0x0001C4F8
	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar == null)
		{
			Debug.LogWarning("TweakUIDropDown Convar Missing: " + this.convarName);
			return;
		}
		this.UpdateState();
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x0001E32F File Offset: 0x0001C52F
	protected void OnEnable()
	{
		this.UpdateState();
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x0001E337 File Offset: 0x0001C537
	public void OnValueChanged()
	{
		this.UpdateConVar();
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x000CB010 File Offset: 0x000C9210
	public void ChangeValue(int change)
	{
		this.currentValue += change;
		if (this.currentValue < 0)
		{
			this.currentValue = 0;
		}
		if (this.currentValue > this.nameValues.Length - 1)
		{
			this.currentValue = this.nameValues.Length - 1;
		}
		this.Left.interactable = (this.currentValue > 0);
		this.Right.interactable = (this.currentValue < this.nameValues.Length - 1);
		this.UpdateConVar();
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x000CB094 File Offset: 0x000C9294
	private void UpdateConVar()
	{
		TweakUIDropdown.NameValue nameValue = this.nameValues[this.currentValue];
		if (this.conVar == null)
		{
			return;
		}
		if (this.conVar.String == nameValue.value)
		{
			return;
		}
		this.conVar.Set(nameValue.value);
		this.UpdateState();
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x000CB0E8 File Offset: 0x000C92E8
	private void UpdateState()
	{
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		for (int i = 0; i < this.nameValues.Length; i++)
		{
			if (!(this.nameValues[i].value != @string))
			{
				this.Current.text = this.nameValues[i].label.translated;
				this.currentValue = i;
				if (this.assignImageColor)
				{
					this.BackgroundImage.color = this.nameValues[i].imageColor;
				}
			}
		}
	}

	// Token: 0x020006D2 RID: 1746
	[Serializable]
	public class NameValue
	{
		// Token: 0x040022CA RID: 8906
		public string value;

		// Token: 0x040022CB RID: 8907
		public Color imageColor;

		// Token: 0x040022CC RID: 8908
		public Translate.Phrase label;
	}
}
