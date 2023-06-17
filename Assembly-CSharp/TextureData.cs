using System;
using UnityEngine;

// Token: 0x0200049D RID: 1181
public struct TextureData
{
	// Token: 0x0400186C RID: 6252
	public int width;

	// Token: 0x0400186D RID: 6253
	public int height;

	// Token: 0x0400186E RID: 6254
	public Color32[] colors;

	// Token: 0x06001B66 RID: 7014 RVA: 0x000982E0 File Offset: 0x000964E0
	public TextureData(Texture2D tex)
	{
		if (tex != null)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.colors = tex.GetPixels32();
			return;
		}
		this.width = 0;
		this.height = 0;
		this.colors = null;
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x000167FA File Offset: 0x000149FA
	public Color32 GetColor(int x, int y)
	{
		return this.colors[y * this.width + x];
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x00016811 File Offset: 0x00014A11
	public int GetShort(int x, int y)
	{
		return (int)BitUtility.DecodeShort(this.GetColor(x, y));
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x00016820 File Offset: 0x00014A20
	public int GetInt(int x, int y)
	{
		return BitUtility.DecodeInt(this.GetColor(x, y));
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x0001682F File Offset: 0x00014A2F
	public float GetFloat(int x, int y)
	{
		return BitUtility.DecodeFloat(this.GetColor(x, y));
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x0001683E File Offset: 0x00014A3E
	public float GetHalf(int x, int y)
	{
		return BitUtility.Short2Float(this.GetShort(x, y));
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x0001684D File Offset: 0x00014A4D
	public Vector4 GetVector(int x, int y)
	{
		return BitUtility.DecodeVector(this.GetColor(x, y));
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x0001685C File Offset: 0x00014A5C
	public Vector3 GetNormal(int x, int y)
	{
		return BitUtility.DecodeNormal(this.GetColor(x, y));
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x00098330 File Offset: 0x00096530
	public Color32 GetInterpolatedColor(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Color a = this.GetColor(num3, num4);
		Color b = this.GetColor(x2, num4);
		Color a2 = this.GetColor(num3, y2);
		Color b2 = this.GetColor(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Color a3 = Color.Lerp(a, b, t);
		Color b3 = Color.Lerp(a2, b2, t);
		return Color.Lerp(a3, b3, t2);
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x0009840C File Offset: 0x0009660C
	public int GetInterpolatedInt(float x, float y)
	{
		float f = x * (float)(this.width - 1);
		float f2 = y * (float)(this.height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(f), 1, this.width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(f2), 1, this.height - 2);
		return this.GetInt(x2, y2);
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x00098464 File Offset: 0x00096664
	public int GetInterpolatedShort(float x, float y)
	{
		float f = x * (float)(this.width - 1);
		float f2 = y * (float)(this.height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(f), 1, this.width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(f2), 1, this.height - 2);
		return this.GetShort(x2, y2);
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000984BC File Offset: 0x000966BC
	public float GetInterpolatedFloat(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		float @float = this.GetFloat(num3, num4);
		float float2 = this.GetFloat(x2, num4);
		float float3 = this.GetFloat(num3, y2);
		float float4 = this.GetFloat(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		float a = Mathf.Lerp(@float, float2, t);
		float b = Mathf.Lerp(float3, float4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x0009857C File Offset: 0x0009677C
	public float GetInterpolatedHalf(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		float half = this.GetHalf(num3, num4);
		float half2 = this.GetHalf(x2, num4);
		float half3 = this.GetHalf(num3, y2);
		float half4 = this.GetHalf(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		float a = Mathf.Lerp(half, half2, t);
		float b = Mathf.Lerp(half3, half4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x0009863C File Offset: 0x0009683C
	public Vector4 GetInterpolatedVector(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Vector4 vector = this.GetVector(num3, num4);
		Vector4 vector2 = this.GetVector(x2, num4);
		Vector4 vector3 = this.GetVector(num3, y2);
		Vector4 vector4 = this.GetVector(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Vector4 a = Vector4.Lerp(vector, vector2, t);
		Vector4 b = Vector4.Lerp(vector3, vector4, t);
		return Vector4.Lerp(a, b, t2);
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000986FC File Offset: 0x000968FC
	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Vector3 normal = this.GetNormal(num3, num4);
		Vector3 normal2 = this.GetNormal(x2, num4);
		Vector3 normal3 = this.GetNormal(num3, y2);
		Vector3 normal4 = this.GetNormal(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Vector3 a = Vector3.Lerp(normal, normal2, t);
		Vector3 b = Vector3.Lerp(normal3, normal4, t);
		return Vector3.Lerp(a, b, t2);
	}
}
