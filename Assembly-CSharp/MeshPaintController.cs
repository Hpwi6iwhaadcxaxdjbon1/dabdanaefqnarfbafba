using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200020F RID: 527
public class MeshPaintController : MonoBehaviour, IClientComponent
{
	// Token: 0x04000D17 RID: 3351
	public Camera pickerCamera;

	// Token: 0x04000D18 RID: 3352
	public Texture2D brushTexture;

	// Token: 0x04000D19 RID: 3353
	public Vector2 brushScale = new Vector2(8f, 8f);

	// Token: 0x04000D1A RID: 3354
	public Color brushColor = Color.white;

	// Token: 0x04000D1B RID: 3355
	public float brushSpacing = 2f;

	// Token: 0x04000D1C RID: 3356
	public RawImage brushImage;

	// Token: 0x04000D1D RID: 3357
	private Vector3 lastPosition;

	// Token: 0x04000D1E RID: 3358
	internal List<MeshPaintable> dirtyPaintables = new List<MeshPaintable>();

	// Token: 0x04000D1F RID: 3359
	internal bool drawingBlocked = true;

	// Token: 0x0600103F RID: 4159 RVA: 0x0000E62B File Offset: 0x0000C82B
	private void Awake()
	{
		this.drawingBlocked = true;
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x0006DE78 File Offset: 0x0006C078
	private void Update()
	{
		if (this.drawingBlocked)
		{
			if (Input.anyKey)
			{
				return;
			}
			this.drawingBlocked = false;
		}
		if (Buttons.Sprint.IsDown)
		{
			return;
		}
		if (Input.GetKey(KeyCode.Mouse0))
		{
			Vector3 mousePosition = Input.mousePosition;
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.Draw(mousePosition);
				this.lastPosition = mousePosition;
				this.ApplyPaintables();
				return;
			}
			float num = Vector3.Distance(this.lastPosition, mousePosition);
			if (Vector3.Distance(this.lastPosition, mousePosition) > this.brushSpacing)
			{
				Vector3 normalized = (mousePosition - this.lastPosition).normalized;
				for (float num2 = 0f; num2 <= num; num2 += this.brushSpacing)
				{
					this.lastPosition += normalized * this.brushSpacing;
					this.Draw(this.lastPosition);
				}
				this.ApplyPaintables();
			}
		}
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x0006DF60 File Offset: 0x0006C160
	private void Draw(Vector3 pos)
	{
		RaycastHit info;
		if (!Physics.Raycast(this.pickerCamera.ScreenPointToRay(pos), ref info, 512f))
		{
			return;
		}
		MeshPaintable component = info.collider.GetComponent<MeshPaintable>();
		if (component)
		{
			if (!component.ShouldHit(info))
			{
				return;
			}
			component.DrawTexture(info.textureCoord2, this.brushScale.x, this.brushScale.y, this.brushTexture, this.brushColor);
			if (!this.dirtyPaintables.Contains(component))
			{
				this.dirtyPaintables.Add(component);
			}
		}
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x0006DFF0 File Offset: 0x0006C1F0
	private void ApplyPaintables()
	{
		foreach (MeshPaintable meshPaintable in this.dirtyPaintables)
		{
			meshPaintable.Apply();
		}
		this.dirtyPaintables.Clear();
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x0006E04C File Offset: 0x0006C24C
	public void UpdateBrushSize(float fNewSize)
	{
		this.brushScale.x = fNewSize;
		this.brushScale.y = this.brushScale.x;
		this.brushSpacing = fNewSize / 4f;
		if (this.brushSpacing < 1f)
		{
			this.brushSpacing = 1f;
		}
		this.brushImage.rectTransform.sizeDelta = Vector2.one * fNewSize * 3f;
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x0000E634 File Offset: 0x0000C834
	public void UpdateBrushAlpha(float fAlpha)
	{
		this.brushColor.a = fAlpha;
		this.brushImage.color = this.brushColor;
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x0006E0C8 File Offset: 0x0006C2C8
	public void UpdateBrushColor(Color color)
	{
		this.brushColor.r = color.r;
		this.brushColor.g = color.g;
		this.brushColor.b = color.b;
		this.brushImage.color = this.brushColor;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x0000E653 File Offset: 0x0000C853
	public void UpdateBrushTexture(Texture texture)
	{
		this.brushTexture = (texture as Texture2D);
		this.brushImage.texture = this.brushTexture;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x0006E11C File Offset: 0x0006C31C
	public void Clear()
	{
		foreach (MeshPaintable meshPaintable in base.GetComponentsInChildren<MeshPaintable>())
		{
			meshPaintable.Clear(meshPaintable.clearColor);
		}
	}
}
