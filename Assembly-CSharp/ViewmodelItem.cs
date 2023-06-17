using System;
using UnityEngine;

// Token: 0x02000237 RID: 567
public class ViewmodelItem : MonoBehaviour
{
	// Token: 0x04000DDA RID: 3546
	private bool vOneRun;

	// Token: 0x04000DDB RID: 3547
	private bool bWasOn;

	// Token: 0x04000DDC RID: 3548
	private Animator viewmodelAnimator;

	// Token: 0x06001105 RID: 4357 RVA: 0x0000EF52 File Offset: 0x0000D152
	private void OnEnable()
	{
		this.viewmodelAnimator = base.GetComponentInChildren<Animator>();
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x00072244 File Offset: 0x00070444
	private void Update()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		Item item = this.FindItem();
		if (item == null)
		{
			return;
		}
		if (item.IsOn() != this.bWasOn || !this.vOneRun)
		{
			this.bWasOn = item.IsOn();
			this.OnOffStateChanged();
		}
		this.vOneRun = true;
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x0007229C File Offset: 0x0007049C
	private void OnOffStateChanged()
	{
		if (this.bWasOn)
		{
			base.SendMessage("OnItemOn", SendMessageOptions.DontRequireReceiver);
			this.viewmodelAnimator.SetBool("on", true);
			return;
		}
		base.SendMessage("OnItemOff", SendMessageOptions.DontRequireReceiver);
		this.viewmodelAnimator.SetBool("on", false);
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0000EF60 File Offset: 0x0000D160
	private Item FindItem()
	{
		LocalPlayer.Entity == null;
		return null;
	}
}
