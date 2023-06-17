using System;
using UnityEngine;

// Token: 0x0200053E RID: 1342
public static class TerrainModifierEx
{
	// Token: 0x06001E33 RID: 7731 RVA: 0x000A5B54 File Offset: 0x000A3D54
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		foreach (TerrainModifier terrainModifier in modifiers)
		{
			Vector3 point = Vector3.Scale(terrainModifier.worldPosition, scale);
			Vector3 pos2 = pos + rot * point;
			float y = scale.y;
			terrainModifier.Apply(pos2, y);
		}
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x00017F3F File Offset: 0x0001613F
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers)
	{
		transform.ApplyTerrainModifiers(modifiers, transform.position, transform.rotation, transform.lossyScale);
	}
}
