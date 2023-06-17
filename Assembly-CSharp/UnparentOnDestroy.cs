using System;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class UnparentOnDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x04000DD6 RID: 3542
	public float destroyAfterSeconds = 1f;

	// Token: 0x060010F6 RID: 4342 RVA: 0x0000EDA2 File Offset: 0x0000CFA2
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		GameManager.Destroy(base.gameObject, (this.destroyAfterSeconds <= 0f) ? 1f : this.destroyAfterSeconds);
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x0000EDD5 File Offset: 0x0000CFD5
	protected void OnValidate()
	{
		if (this.destroyAfterSeconds <= 0f)
		{
			this.destroyAfterSeconds = 1f;
		}
	}
}
