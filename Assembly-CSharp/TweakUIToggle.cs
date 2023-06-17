using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006D6 RID: 1750
public class TweakUIToggle : MonoBehaviour
{
	// Token: 0x040022D6 RID: 8918
	public Toggle toggleControl;

	// Token: 0x040022D7 RID: 8919
	public string convarName = "effects.motionblur";

	// Token: 0x040022D8 RID: 8920
	public bool inverse;

	// Token: 0x040022D9 RID: 8921
	internal ConsoleSystem.Command conVar;

	// Token: 0x060026CA RID: 9930 RVA: 0x0001E42A File Offset: 0x0001C62A
	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar != null)
		{
			this.UpdateToggleState();
			return;
		}
		Debug.LogWarning("Tweak Toggle Convar Missing: " + this.convarName);
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x0001E461 File Offset: 0x0001C661
	protected void OnEnable()
	{
		this.UpdateToggleState();
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x0001E469 File Offset: 0x0001C669
	public void OnToggleChanged()
	{
		this.UpdateConVar();
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x000CB32C File Offset: 0x000C952C
	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.toggleControl.isOn;
		if (this.inverse)
		{
			flag = !flag;
		}
		if (this.conVar.AsBool == flag)
		{
			return;
		}
		this.conVar.Set(flag);
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x000CB378 File Offset: 0x000C9578
	private void UpdateToggleState()
	{
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.conVar.AsBool;
		if (this.inverse)
		{
			flag = !flag;
		}
		this.toggleControl.isOn = flag;
	}
}
