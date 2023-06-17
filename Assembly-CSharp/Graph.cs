using System;
using UnityEngine;

// Token: 0x0200062A RID: 1578
public abstract class Graph : MonoBehaviour
{
	// Token: 0x04001F52 RID: 8018
	public Material Material;

	// Token: 0x04001F53 RID: 8019
	public int Resolution = 128;

	// Token: 0x04001F54 RID: 8020
	public Vector2 ScreenFill = new Vector2(0f, 0f);

	// Token: 0x04001F55 RID: 8021
	public Vector2 ScreenOrigin = new Vector2(0f, 0f);

	// Token: 0x04001F56 RID: 8022
	public Vector2 Pivot = new Vector2(0f, 0f);

	// Token: 0x04001F57 RID: 8023
	public Rect Area = new Rect(0f, 0f, 128f, 32f);

	// Token: 0x04001F58 RID: 8024
	internal float CurrentValue;

	// Token: 0x04001F59 RID: 8025
	private int index;

	// Token: 0x04001F5A RID: 8026
	private float[] values;

	// Token: 0x04001F5B RID: 8027
	private float max;

	// Token: 0x0600233B RID: 9019
	protected abstract float GetValue();

	// Token: 0x0600233C RID: 9020
	protected abstract Color GetColor(float value);

	// Token: 0x0600233D RID: 9021 RVA: 0x0001BEA9 File Offset: 0x0001A0A9
	protected Vector3 GetVertex(float x, float y)
	{
		return new Vector3(x, y, 0f);
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000BB1DC File Offset: 0x000B93DC
	protected void Update()
	{
		if (this.values == null || this.values.Length != this.Resolution)
		{
			this.values = new float[this.Resolution];
		}
		this.max = 0f;
		for (int i = 0; i < this.values.Length - 1; i++)
		{
			this.max = Mathf.Max(this.max, this.values[i] = this.values[i + 1]);
		}
		this.max = Mathf.Max(this.max, this.CurrentValue = (this.values[this.values.Length - 1] = this.GetValue()));
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000BB28C File Offset: 0x000B948C
	protected void OnGUI()
	{
		if (Event.current.type != 7)
		{
			return;
		}
		if (this.values == null || this.values.Length == 0)
		{
			return;
		}
		float num = Mathf.Max(this.Area.width, this.ScreenFill.x * (float)Screen.width);
		float num2 = Mathf.Max(this.Area.height, this.ScreenFill.y * (float)Screen.height);
		float num3 = this.Area.x - this.Pivot.x * num + this.ScreenOrigin.x * (float)Screen.width;
		float num4 = this.Area.y - this.Pivot.y * num2 + this.ScreenOrigin.y * (float)Screen.height;
		GL.PushMatrix();
		this.Material.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Begin(7);
		for (int i = 0; i < this.values.Length; i++)
		{
			float num5 = this.values[i];
			float num6 = num / (float)this.values.Length;
			float num7 = num2 * num5 / this.max;
			float num8 = num3 + (float)i * num6;
			float num9 = num4;
			GL.Color(this.GetColor(num5));
			GL.Vertex(this.GetVertex(num8 + 0f, num9 + num7));
			GL.Vertex(this.GetVertex(num8 + num6, num9 + num7));
			GL.Vertex(this.GetVertex(num8 + num6, num9 + 0f));
			GL.Vertex(this.GetVertex(num8 + 0f, num9 + 0f));
		}
		GL.End();
		GL.PopMatrix();
	}
}
