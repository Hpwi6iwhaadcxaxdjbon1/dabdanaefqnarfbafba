using System;
using UnityEngine;

// Token: 0x02000597 RID: 1431
[CreateAssetMenu(menuName = "Rust/Environment Volume Properties")]
public class EnvironmentVolumeProperties : ScriptableObject
{
	// Token: 0x04001C9A RID: 7322
	public int ReflectionQuality;

	// Token: 0x04001C9B RID: 7323
	public LayerMask ReflectionCullingFlags;

	// Token: 0x04001C9C RID: 7324
	[Horizontal(1, 0)]
	public EnvironmentMultiplier[] ReflectionMultipliers;

	// Token: 0x04001C9D RID: 7325
	[Horizontal(1, 0)]
	public EnvironmentMultiplier[] AmbientMultipliers;

	// Token: 0x060020B6 RID: 8374 RVA: 0x000B195C File Offset: 0x000AFB5C
	public float FindReflectionMultiplier(EnvironmentType type)
	{
		foreach (EnvironmentMultiplier environmentMultiplier in this.ReflectionMultipliers)
		{
			if ((type & environmentMultiplier.Type) != (EnvironmentType)0)
			{
				return environmentMultiplier.Multiplier;
			}
		}
		return 1f;
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000B1998 File Offset: 0x000AFB98
	public float FindAmbientMultiplier(EnvironmentType type)
	{
		foreach (EnvironmentMultiplier environmentMultiplier in this.AmbientMultipliers)
		{
			if ((type & environmentMultiplier.Type) != (EnvironmentType)0)
			{
				return environmentMultiplier.Multiplier;
			}
		}
		return 1f;
	}
}
