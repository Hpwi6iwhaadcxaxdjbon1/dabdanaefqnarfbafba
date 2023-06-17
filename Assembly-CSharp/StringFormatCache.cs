using System;
using System.Collections.Generic;

// Token: 0x020005F2 RID: 1522
public static class StringFormatCache
{
	// Token: 0x04001EA4 RID: 7844
	private static Dictionary<StringFormatCache.Key1, string> dict1 = new Dictionary<StringFormatCache.Key1, string>();

	// Token: 0x04001EA5 RID: 7845
	private static Dictionary<StringFormatCache.Key2, string> dict2 = new Dictionary<StringFormatCache.Key2, string>();

	// Token: 0x04001EA6 RID: 7846
	private static Dictionary<StringFormatCache.Key3, string> dict3 = new Dictionary<StringFormatCache.Key3, string>();

	// Token: 0x04001EA7 RID: 7847
	private static Dictionary<StringFormatCache.Key4, string> dict4 = new Dictionary<StringFormatCache.Key4, string>();

	// Token: 0x06002232 RID: 8754 RVA: 0x000B7D84 File Offset: 0x000B5F84
	public static string Get(string format, string value1)
	{
		StringFormatCache.Key1 key = new StringFormatCache.Key1(format, value1);
		string text;
		if (!StringFormatCache.dict1.TryGetValue(key, ref text))
		{
			text = string.Format(format, value1);
			StringFormatCache.dict1.Add(key, text);
		}
		return text;
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000B7DC0 File Offset: 0x000B5FC0
	public static string Get(string format, string value1, string value2)
	{
		StringFormatCache.Key2 key = new StringFormatCache.Key2(format, value1, value2);
		string text;
		if (!StringFormatCache.dict2.TryGetValue(key, ref text))
		{
			text = string.Format(format, value1, value2);
			StringFormatCache.dict2.Add(key, text);
		}
		return text;
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000B7DFC File Offset: 0x000B5FFC
	public static string Get(string format, string value1, string value2, string value3)
	{
		StringFormatCache.Key3 key = new StringFormatCache.Key3(format, value1, value2, value3);
		string text;
		if (!StringFormatCache.dict3.TryGetValue(key, ref text))
		{
			text = string.Format(format, value1, value2, value3);
			StringFormatCache.dict3.Add(key, text);
		}
		return text;
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x000B7E3C File Offset: 0x000B603C
	public static string Get(string format, string value1, string value2, string value3, string value4)
	{
		StringFormatCache.Key4 key = new StringFormatCache.Key4(format, value1, value2, value3, value4);
		string text;
		if (!StringFormatCache.dict4.TryGetValue(key, ref text))
		{
			text = string.Format(format, new object[]
			{
				value1,
				value2,
				value3,
				value4
			});
			StringFormatCache.dict4.Add(key, text);
		}
		return text;
	}

	// Token: 0x020005F3 RID: 1523
	private struct Key1 : IEquatable<StringFormatCache.Key1>
	{
		// Token: 0x04001EA8 RID: 7848
		public string format;

		// Token: 0x04001EA9 RID: 7849
		public string value1;

		// Token: 0x06002237 RID: 8759 RVA: 0x0001B21D File Offset: 0x0001941D
		public Key1(string format, string value1)
		{
			this.format = format;
			this.value1 = value1;
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x0001B22D File Offset: 0x0001942D
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode();
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x0001B246 File Offset: 0x00019446
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key1 && this.Equals((StringFormatCache.Key1)other);
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x0001B25E File Offset: 0x0001945E
		public bool Equals(StringFormatCache.Key1 other)
		{
			return this.format == other.format && this.value1 == other.value1;
		}
	}

	// Token: 0x020005F4 RID: 1524
	private struct Key2 : IEquatable<StringFormatCache.Key2>
	{
		// Token: 0x04001EAA RID: 7850
		public string format;

		// Token: 0x04001EAB RID: 7851
		public string value1;

		// Token: 0x04001EAC RID: 7852
		public string value2;

		// Token: 0x0600223B RID: 8763 RVA: 0x0001B286 File Offset: 0x00019486
		public Key2(string format, string value1, string value2)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x0001B29D File Offset: 0x0001949D
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode();
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x0001B2C2 File Offset: 0x000194C2
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key2 && this.Equals((StringFormatCache.Key2)other);
		}

		// Token: 0x0600223E RID: 8766 RVA: 0x0001B2DA File Offset: 0x000194DA
		public bool Equals(StringFormatCache.Key2 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2;
		}
	}

	// Token: 0x020005F5 RID: 1525
	private struct Key3 : IEquatable<StringFormatCache.Key3>
	{
		// Token: 0x04001EAD RID: 7853
		public string format;

		// Token: 0x04001EAE RID: 7854
		public string value1;

		// Token: 0x04001EAF RID: 7855
		public string value2;

		// Token: 0x04001EB0 RID: 7856
		public string value3;

		// Token: 0x0600223F RID: 8767 RVA: 0x0001B315 File Offset: 0x00019515
		public Key3(string format, string value1, string value2, string value3)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x0001B334 File Offset: 0x00019534
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode();
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x0001B365 File Offset: 0x00019565
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key3 && this.Equals((StringFormatCache.Key3)other);
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x000B7E90 File Offset: 0x000B6090
		public bool Equals(StringFormatCache.Key3 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3;
		}
	}

	// Token: 0x020005F6 RID: 1526
	private struct Key4 : IEquatable<StringFormatCache.Key4>
	{
		// Token: 0x04001EB1 RID: 7857
		public string format;

		// Token: 0x04001EB2 RID: 7858
		public string value1;

		// Token: 0x04001EB3 RID: 7859
		public string value2;

		// Token: 0x04001EB4 RID: 7860
		public string value3;

		// Token: 0x04001EB5 RID: 7861
		public string value4;

		// Token: 0x06002243 RID: 8771 RVA: 0x0001B37D File Offset: 0x0001957D
		public Key4(string format, string value1, string value2, string value3, string value4)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
			this.value4 = value4;
		}

		// Token: 0x06002244 RID: 8772 RVA: 0x0001B3A4 File Offset: 0x000195A4
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode() ^ this.value4.GetHashCode();
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x0001B3E1 File Offset: 0x000195E1
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key4 && this.Equals((StringFormatCache.Key4)other);
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x000B7EEC File Offset: 0x000B60EC
		public bool Equals(StringFormatCache.Key4 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3 && this.value4 == other.value4;
		}
	}
}
