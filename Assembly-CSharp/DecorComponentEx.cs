using System;
using UnityEngine;

// Token: 0x020004A3 RID: 1187
public static class DecorComponentEx
{
	// Token: 0x06001BA9 RID: 7081 RVA: 0x00098D08 File Offset: 0x00096F08
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Apply(ref pos, ref rot, ref scale);
		}
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x00098D30 File Offset: 0x00096F30
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = localScale;
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x00098D74 File Offset: 0x00096F74
	public static void ApplyDecorComponentsScaleOnly(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.localScale = localScale;
	}
}
