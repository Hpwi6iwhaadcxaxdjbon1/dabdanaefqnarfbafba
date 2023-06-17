using System;
using UnityEngine;

// Token: 0x0200020A RID: 522
[ExecuteInEditMode]
public class LookAt : MonoBehaviour, IClientComponent
{
	// Token: 0x04000CDE RID: 3294
	public Transform target;

	// Token: 0x06000FFD RID: 4093 RVA: 0x0000E1C2 File Offset: 0x0000C3C2
	private void Update()
	{
		if (this.target == null)
		{
			return;
		}
		base.transform.LookAt(this.target);
	}
}
