using System;
using UnityEngine;

// Token: 0x0200028B RID: 651
public class ScaleTransform : ScaleRenderer
{
	// Token: 0x04000F29 RID: 3881
	private Vector3 initialScale;

	// Token: 0x0600127F RID: 4735 RVA: 0x0000FE39 File Offset: 0x0000E039
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.myRenderer.transform.localScale = this.initialScale * scale;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x0000FE5E File Offset: 0x0000E05E
	public override void GatherInitialValues()
	{
		this.initialScale = this.myRenderer.transform.localScale;
		base.GatherInitialValues();
	}
}
