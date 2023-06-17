using System;
using UnityEngine;

// Token: 0x0200017D RID: 381
[CreateAssetMenu(menuName = "Rust/Reverb Settings")]
public class ReverbSettings : ScriptableObject
{
	// Token: 0x04000A5C RID: 2652
	[Range(-10000f, 0f)]
	public int room;

	// Token: 0x04000A5D RID: 2653
	[Range(-10000f, 0f)]
	public int roomHF;

	// Token: 0x04000A5E RID: 2654
	[Range(-10000f, 0f)]
	public int roomLF;

	// Token: 0x04000A5F RID: 2655
	[Range(0.1f, 20f)]
	public float decayTime;

	// Token: 0x04000A60 RID: 2656
	[Range(0.1f, 2f)]
	public float decayHFRatio;

	// Token: 0x04000A61 RID: 2657
	[Range(-10000f, 1000f)]
	public int reflections;

	// Token: 0x04000A62 RID: 2658
	[Range(0f, 0.3f)]
	public float reflectionsDelay;

	// Token: 0x04000A63 RID: 2659
	[Range(-10000f, 2000f)]
	public int reverb;

	// Token: 0x04000A64 RID: 2660
	[Range(0f, 0.1f)]
	public float reverbDelay;

	// Token: 0x04000A65 RID: 2661
	[Range(1000f, 20000f)]
	public float HFReference;

	// Token: 0x04000A66 RID: 2662
	[Range(20f, 1000f)]
	public float LFReference;

	// Token: 0x04000A67 RID: 2663
	[Range(0f, 100f)]
	public float diffusion;

	// Token: 0x04000A68 RID: 2664
	[Range(0f, 100f)]
	public float density;
}
