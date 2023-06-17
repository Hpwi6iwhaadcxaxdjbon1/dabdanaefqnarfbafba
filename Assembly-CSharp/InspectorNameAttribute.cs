using System;
using UnityEngine;

// Token: 0x020006FB RID: 1787
public class InspectorNameAttribute : PropertyAttribute
{
	// Token: 0x0400233B RID: 9019
	public string name;

	// Token: 0x06002754 RID: 10068 RVA: 0x0001EA5A File Offset: 0x0001CC5A
	public InspectorNameAttribute(string name)
	{
		this.name = name;
	}
}
