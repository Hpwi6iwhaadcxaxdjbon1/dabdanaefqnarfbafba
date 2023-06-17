using System;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public static class DecalComponentEx
{
	// Token: 0x06000FB4 RID: 4020 RVA: 0x0006B50C File Offset: 0x0006970C
	public static void ApplyDecalComponents(this Transform transform, DecalComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Apply(ref pos, ref rot, ref scale);
		}
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0006B534 File Offset: 0x00069734
	public static void ApplyDecalComponents(this Transform transform, DecalComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecalComponents(components, ref position, ref rotation, ref localScale);
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = localScale;
	}
}
