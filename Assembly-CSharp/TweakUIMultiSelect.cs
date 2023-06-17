using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006D3 RID: 1747
public class TweakUIMultiSelect : MonoBehaviour
{
	// Token: 0x040022CD RID: 8909
	public ToggleGroup toggleGroup;

	// Token: 0x040022CE RID: 8910
	public string convarName = "effects.motionblur";

	// Token: 0x040022CF RID: 8911
	internal ConsoleSystem.Command conVar;

	// Token: 0x060026BA RID: 9914 RVA: 0x0001E352 File Offset: 0x0001C552
	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar != null)
		{
			this.UpdateToggleGroup();
			return;
		}
		Debug.LogWarning("Tweak Slider Convar Missing: " + this.convarName);
	}

	// Token: 0x060026BB RID: 9915 RVA: 0x0001E389 File Offset: 0x0001C589
	protected void OnEnable()
	{
		this.UpdateToggleGroup();
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x0001E391 File Offset: 0x0001C591
	public void OnChanged()
	{
		this.UpdateConVar();
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000CB178 File Offset: 0x000C9378
	private void UpdateToggleGroup()
	{
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		foreach (Toggle toggle in this.toggleGroup.GetComponentsInChildren<Toggle>())
		{
			toggle.isOn = (toggle.name == @string);
		}
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000CB1C8 File Offset: 0x000C93C8
	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		Toggle toggle = Enumerable.FirstOrDefault<Toggle>(Enumerable.Where<Toggle>(this.toggleGroup.GetComponentsInChildren<Toggle>(), (Toggle x) => x.isOn));
		if (toggle == null)
		{
			return;
		}
		if (this.conVar.String == toggle.name)
		{
			return;
		}
		this.conVar.Set(toggle.name);
	}
}
