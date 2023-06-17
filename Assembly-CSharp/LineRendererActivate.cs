using System;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class LineRendererActivate : MonoBehaviour, IClientComponent
{
	// Token: 0x06000BA7 RID: 2983 RVA: 0x0000B1CA File Offset: 0x000093CA
	private void OnEnable()
	{
		base.GetComponent<LineRenderer>().enabled = true;
	}
}
