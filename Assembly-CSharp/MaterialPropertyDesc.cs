using System;
using UnityEngine;

// Token: 0x020005A3 RID: 1443
public struct MaterialPropertyDesc
{
	// Token: 0x04001CDE RID: 7390
	public int nameID;

	// Token: 0x04001CDF RID: 7391
	public Type type;

	// Token: 0x0600211A RID: 8474 RVA: 0x0001A518 File Offset: 0x00018718
	public MaterialPropertyDesc(string name, Type type)
	{
		this.nameID = Shader.PropertyToID(name);
		this.type = type;
	}
}
