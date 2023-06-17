using System;
using UnityEngine;

// Token: 0x020006F5 RID: 1781
public class VitalRadial : MonoBehaviour
{
	// Token: 0x0600273F RID: 10047 RVA: 0x0001E9B7 File Offset: 0x0001CBB7
	private void Awake()
	{
		Debug.LogWarning("VitalRadial is obsolete " + base.transform.GetRecursiveName(""), base.gameObject);
	}
}
