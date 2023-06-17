using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020006A7 RID: 1703
public class PaintableImageGrid : UIBehaviour
{
	// Token: 0x040021EC RID: 8684
	public UIPaintableImage templateImage;

	// Token: 0x040021ED RID: 8685
	public int cols = 4;

	// Token: 0x040021EE RID: 8686
	public int rows = 4;

	// Token: 0x040021EF RID: 8687
	internal UIPaintableImage[,] images;

	// Token: 0x060025F4 RID: 9716 RVA: 0x000C7824 File Offset: 0x000C5A24
	public void Initialize()
	{
		if (this.images != null)
		{
			return;
		}
		this.images = new UIPaintableImage[this.cols, this.rows];
		int num = 0;
		for (int i = 0; i < this.cols; i++)
		{
			for (int j = 0; j < this.rows; j++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.templateImage.gameObject);
				gameObject.transform.SetParent(base.transform, false);
				gameObject.name = i + ", " + j;
				this.images[i, j] = gameObject.GetComponent<UIPaintableImage>();
				this.images[i, j].imageNumber = num++;
			}
		}
		this.templateImage.gameObject.SetActive(false);
		this.OnTransformParentChanged();
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x0001D9B2 File Offset: 0x0001BBB2
	protected override void Start()
	{
		base.Start();
		this.Initialize();
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000C78F8 File Offset: 0x000C5AF8
	public void Clear()
	{
		this.Initialize();
		for (int i = 0; i < this.cols; i++)
		{
			for (int j = 0; j < this.rows; j++)
			{
				this.images[i, j].Clear();
			}
		}
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x000C7940 File Offset: 0x000C5B40
	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		if (this.images == null)
		{
			return;
		}
		Rect rect = (base.transform.parent as RectTransform).rect;
		float num = rect.width / (float)this.cols;
		float num2 = rect.height / (float)this.rows;
		for (int i = 0; i < this.cols; i++)
		{
			for (int j = 0; j < this.rows; j++)
			{
				if (!(this.images[i, j] == null))
				{
					this.images[i, j].rectTransform.sizeDelta = new Vector2(num, num2);
					this.images[i, j].rectTransform.anchoredPosition = new Vector2((float)i * num, (float)j * num2 * -1f);
				}
			}
		}
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000C7A1C File Offset: 0x000C5C1C
	public void DrawTexture(Vector2 pos, Vector2 size, Texture2D texture, Color textureColor, UIPaintableImage.DrawMode mode)
	{
		if (this.images == null)
		{
			return;
		}
		Rect rect = (base.transform.parent as RectTransform).rect;
		float num = rect.width / (float)this.cols;
		float num2 = rect.height / (float)this.rows;
		pos.y = rect.height - pos.y;
		for (int i = 0; i < this.cols; i++)
		{
			for (int j = 0; j < this.rows; j++)
			{
				this.images[i, j].DrawTexture(new Vector2(pos.x - (float)i * num, pos.y - (float)j * num2), size, texture, textureColor, mode);
			}
		}
	}
}
