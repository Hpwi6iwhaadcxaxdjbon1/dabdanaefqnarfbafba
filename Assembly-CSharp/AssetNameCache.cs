using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005C2 RID: 1474
public static class AssetNameCache
{
	// Token: 0x04001DAA RID: 7594
	private static Dictionary<Object, string> mixed = new Dictionary<Object, string>();

	// Token: 0x04001DAB RID: 7595
	private static Dictionary<Object, string> lower = new Dictionary<Object, string>();

	// Token: 0x04001DAC RID: 7596
	private static Dictionary<Object, string> upper = new Dictionary<Object, string>();

	// Token: 0x060021C6 RID: 8646 RVA: 0x000B6ABC File Offset: 0x000B4CBC
	private static string LookupName(Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string name;
		if (!AssetNameCache.mixed.TryGetValue(obj, ref name))
		{
			name = obj.name;
			AssetNameCache.mixed.Add(obj, name);
		}
		return name;
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x000B6AFC File Offset: 0x000B4CFC
	private static string LookupNameLower(Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.lower.TryGetValue(obj, ref text))
		{
			text = obj.name.ToLower();
			AssetNameCache.lower.Add(obj, text);
		}
		return text;
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x000B6B40 File Offset: 0x000B4D40
	private static string LookupNameUpper(Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.upper.TryGetValue(obj, ref text))
		{
			text = obj.name.ToUpper();
			AssetNameCache.upper.Add(obj, text);
		}
		return text;
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x0001ADEB File Offset: 0x00018FEB
	public static string GetName(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x0001ADF3 File Offset: 0x00018FF3
	public static string GetNameLower(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x0001ADFB File Offset: 0x00018FFB
	public static string GetNameUpper(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x0001ADEB File Offset: 0x00018FEB
	public static string GetName(this Material mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x0001ADF3 File Offset: 0x00018FF3
	public static string GetNameLower(this Material mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x0001ADFB File Offset: 0x00018FFB
	public static string GetNameUpper(this Material mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}
}
