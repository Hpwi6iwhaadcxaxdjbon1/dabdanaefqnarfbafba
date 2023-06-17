using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000153 RID: 339
[CreateAssetMenu(menuName = "Rust/Ambience Definition")]
public class AmbienceDefinition : ScriptableObject
{
	// Token: 0x04000922 RID: 2338
	[Header("Sound")]
	public List<SoundDefinition> sounds;

	// Token: 0x04000923 RID: 2339
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange stingFrequency = new AmbienceDefinition.ValueRange(15f, 30f);

	// Token: 0x04000924 RID: 2340
	[InspectorFlags]
	[Header("Environment")]
	public TerrainBiome.Enum biomes = -1;

	// Token: 0x04000925 RID: 2341
	[InspectorFlags]
	public TerrainTopology.Enum topologies = -1;

	// Token: 0x04000926 RID: 2342
	public EnvironmentType environmentType = EnvironmentType.Underground;

	// Token: 0x04000927 RID: 2343
	public bool useEnvironmentType;

	// Token: 0x04000928 RID: 2344
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x04000929 RID: 2345
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange rain = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x0400092A RID: 2346
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange wind = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x0400092B RID: 2347
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange snow = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x02000154 RID: 340
	[Serializable]
	public class ValueRange
	{
		// Token: 0x0400092C RID: 2348
		public float min;

		// Token: 0x0400092D RID: 2349
		public float max;

		// Token: 0x06000C79 RID: 3193 RVA: 0x0000BA59 File Offset: 0x00009C59
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
