using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000639 RID: 1593
public class UIPaintBox : MonoBehaviour
{
	// Token: 0x04001FA3 RID: 8099
	public UIPaintBox.OnBrushChanged onBrushChanged = new UIPaintBox.OnBrushChanged();

	// Token: 0x04001FA4 RID: 8100
	public Brush brush;

	// Token: 0x0600238E RID: 9102 RVA: 0x000BC9C0 File Offset: 0x000BABC0
	public void UpdateBrushSize(int size)
	{
		this.brush.brushSize = Vector2.one * (float)size;
		this.brush.spacing = Mathf.Clamp((float)size * 0.1f, 1f, 3f);
		this.OnChanged();
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x0001C246 File Offset: 0x0001A446
	public void UpdateBrushTexture(Texture2D tex)
	{
		this.brush.texture = tex;
		this.OnChanged();
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000BCA0C File Offset: 0x000BAC0C
	public void UpdateBrushColor(Color col)
	{
		this.brush.color.r = col.r;
		this.brush.color.g = col.g;
		this.brush.color.b = col.b;
		this.OnChanged();
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x0001C25A File Offset: 0x0001A45A
	public void UpdateBrushAlpha(float a)
	{
		this.brush.color.a = a;
		this.OnChanged();
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x0001C273 File Offset: 0x0001A473
	public void UpdateBrushEraser(bool b)
	{
		this.brush.erase = b;
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x0001C281 File Offset: 0x0001A481
	private void OnChanged()
	{
		this.onBrushChanged.Invoke(this.brush);
	}

	// Token: 0x0200063A RID: 1594
	[Serializable]
	public class OnBrushChanged : UnityEvent<Brush>
	{
	}
}
