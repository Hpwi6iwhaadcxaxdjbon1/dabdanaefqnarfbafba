using System;
using UnityEngine;

// Token: 0x020005C3 RID: 1475
public class BoneInfoComponent : MonoBehaviour, IClientComponent
{
	// Token: 0x04001DAD RID: 7597
	[Header("Size Variation")]
	public Vector3 sizeVariation = Vector3.zero;

	// Token: 0x04001DAE RID: 7598
	public int sizeVariationSeed;
}
