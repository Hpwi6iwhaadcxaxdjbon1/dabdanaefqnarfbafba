using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006ED RID: 1773
public class GridLayoutGroupNeat : GridLayoutGroup
{
	// Token: 0x06002725 RID: 10021 RVA: 0x000CBEBC File Offset: 0x000CA0BC
	private float IdealCellWidth(float cellSize)
	{
		float num = base.rectTransform.rect.x + (float)(base.padding.left + base.padding.right) * 0.5f;
		float num2 = Mathf.Floor(num / cellSize);
		return num / num2 - this.m_Spacing.x;
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x000CBF14 File Offset: 0x000CA114
	public override void SetLayoutHorizontal()
	{
		Vector2 cellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(cellSize.x);
		base.SetLayoutHorizontal();
		this.m_CellSize = cellSize;
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000CBF4C File Offset: 0x000CA14C
	public override void SetLayoutVertical()
	{
		Vector2 cellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(cellSize.x);
		base.SetLayoutVertical();
		this.m_CellSize = cellSize;
	}
}
