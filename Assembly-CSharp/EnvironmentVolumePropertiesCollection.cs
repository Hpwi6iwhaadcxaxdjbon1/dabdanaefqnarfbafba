using System;
using UnityEngine;

// Token: 0x02000598 RID: 1432
[CreateAssetMenu(menuName = "Rust/Environment Volume Properties Collection")]
public class EnvironmentVolumePropertiesCollection : ScriptableObject
{
	// Token: 0x04001C9E RID: 7326
	public float TransitionSpeed = 1f;

	// Token: 0x04001C9F RID: 7327
	public EnvironmentVolumeProperties[] Properties;

	// Token: 0x060020B9 RID: 8377 RVA: 0x000B19D4 File Offset: 0x000AFBD4
	public EnvironmentVolumeProperties FindQuality(int quality)
	{
		foreach (EnvironmentVolumeProperties environmentVolumeProperties in this.Properties)
		{
			if (environmentVolumeProperties.ReflectionQuality == quality)
			{
				return environmentVolumeProperties;
			}
		}
		return null;
	}
}
