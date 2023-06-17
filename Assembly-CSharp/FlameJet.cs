using System;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class FlameJet : MonoBehaviour
{
	// Token: 0x04000857 RID: 2135
	public LineRenderer line;

	// Token: 0x04000858 RID: 2136
	public float tesselation = 0.025f;

	// Token: 0x04000859 RID: 2137
	private float length;

	// Token: 0x0400085A RID: 2138
	public float maxLength = 2f;

	// Token: 0x0400085B RID: 2139
	public float drag;

	// Token: 0x0400085C RID: 2140
	private int numSegments;

	// Token: 0x0400085D RID: 2141
	public bool on;

	// Token: 0x0400085E RID: 2142
	private Vector3[] lastWorldSegments;

	// Token: 0x0400085F RID: 2143
	private Vector3[] currentSegments;

	// Token: 0x04000860 RID: 2144
	public Color startColor;

	// Token: 0x04000861 RID: 2145
	public Color endColor;

	// Token: 0x04000862 RID: 2146
	public Color currentColor;

	// Token: 0x06000BCE RID: 3022 RVA: 0x0000B32C File Offset: 0x0000952C
	private void Initialize()
	{
		this.currentColor = this.startColor;
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0000B33A File Offset: 0x0000953A
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0000B342 File Offset: 0x00009542
	public void LateUpdate()
	{
		this.UpdateLine();
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0000B34A File Offset: 0x0000954A
	public void SetOn(bool isOn)
	{
		this.on = isOn;
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0000B353 File Offset: 0x00009553
	private float curve(float x)
	{
		return x * x;
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0005AA78 File Offset: 0x00058C78
	private void UpdateLine()
	{
		this.currentColor.a = Mathf.Lerp(this.currentColor.a, this.on ? 1f : 0f, Time.deltaTime * 40f);
		this.line.SetColors(this.currentColor, this.endColor);
		this.tesselation = 0.1f;
		this.numSegments = Mathf.CeilToInt(this.maxLength / this.tesselation);
		float num = this.maxLength / (float)this.numSegments;
		Vector3[] array = new Vector3[this.numSegments];
		for (int i = 0; i < array.Length; i++)
		{
			float x = 0f;
			float y = 0f;
			if (this.lastWorldSegments != null && this.lastWorldSegments[i] != Vector3.zero)
			{
				Vector3 a = base.transform.InverseTransformPoint(this.lastWorldSegments[i]);
				float f = (float)i / (float)array.Length;
				Vector3 vector = Vector3.Lerp(a, Vector3.zero, Time.deltaTime * this.drag);
				vector = Vector3.Lerp(Vector3.zero, vector, Mathf.Sqrt(f));
				x = vector.x;
				y = vector.y;
			}
			if (i == 0)
			{
				y = (x = 0f);
			}
			Vector3 vector2 = new Vector3(x, y, (float)i * num);
			array[i] = vector2;
			if (this.lastWorldSegments == null)
			{
				this.lastWorldSegments = new Vector3[this.numSegments];
			}
			this.lastWorldSegments[i] = base.transform.TransformPoint(vector2);
		}
		this.line.SetVertexCount(this.numSegments);
		this.line.SetPositions(array);
	}
}
