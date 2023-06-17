using System;
using UnityEngine.UI;

// Token: 0x020006F0 RID: 1776
public class NonDrawingGraphic : Graphic
{
	// Token: 0x06002730 RID: 10032 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void SetMaterialDirty()
	{
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void SetVerticesDirty()
	{
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x0001E8D7 File Offset: 0x0001CAD7
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
