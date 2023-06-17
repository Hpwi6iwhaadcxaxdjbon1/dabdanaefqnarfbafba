using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000632 RID: 1586
public class MapGrid : MonoBehaviour
{
	// Token: 0x04001F78 RID: 8056
	public Text coordinatePrefab;

	// Token: 0x04001F79 RID: 8057
	public int gridCellSize = 200;

	// Token: 0x04001F7A RID: 8058
	public float lineThickness = 0.3f;

	// Token: 0x04001F7B RID: 8059
	public CanvasGroup group;

	// Token: 0x04001F7C RID: 8060
	public float visibleAlphaLevel = 0.6f;

	// Token: 0x04001F7D RID: 8061
	public RawImage TargetImage;

	// Token: 0x04001F7E RID: 8062
	public bool show;

	// Token: 0x04001F7F RID: 8063
	private bool initialized;

	// Token: 0x06002364 RID: 9060 RVA: 0x000BBC5C File Offset: 0x000B9E5C
	public void SetGridVisible(bool isVis)
	{
		float num = isVis ? this.visibleAlphaLevel : 0f;
		this.TargetImage.materialForRendering.SetInt("_ShowGrid", isVis ? 1 : 0);
		if (this.group.alpha == num)
		{
			return;
		}
		this.group.alpha = num;
		this.InitializeGrid();
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x0001C019 File Offset: 0x0001A219
	public bool IsGridVisible()
	{
		return this.group.alpha >= this.visibleAlphaLevel;
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x0001C031 File Offset: 0x0001A231
	public void ClearGrid()
	{
		base.transform.DestroyChildren();
		this.initialized = false;
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000BBCB8 File Offset: 0x000B9EB8
	public void InitializeGrid()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.TargetImage.materialForRendering.SetFloat("_LineSpread", (float)this.gridCellSize);
		uint size = World.Size;
		int num = Mathf.CeilToInt(size / (float)this.gridCellSize);
		Vector3 vector = new Vector3((float)(-(float)((ulong)size)) * 0.5f, 0f, size * 0.5f);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Text text = Object.Instantiate<Text>(this.coordinatePrefab);
				text.transform.SetParent(base.transform, false);
				Vector2 v = MapInterface.WorldPosToImagePos(new Vector3(vector.x + (float)(i * this.gridCellSize), 0f, vector.z - (float)(j * this.gridCellSize)));
				text.rectTransform.localPosition = v;
				text.text = (this.NumberToLetter(i) ?? "") + j.ToString();
			}
		}
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x000BBDD4 File Offset: 0x000B9FD4
	public string NumberToLetter(int num)
	{
		int num2 = Mathf.FloorToInt((float)(num / 26));
		int num3 = num % 26;
		string text = "";
		if (num2 > 0)
		{
			for (int i = 0; i < num2; i++)
			{
				text += Convert.ToChar(65 + i).ToString();
			}
		}
		return text + Convert.ToChar(65 + num3).ToString();
	}
}
