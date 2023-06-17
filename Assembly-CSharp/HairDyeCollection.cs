using System;
using UnityEngine;

// Token: 0x020005D3 RID: 1491
[CreateAssetMenu(menuName = "Rust/Hair Dye Collection")]
public class HairDyeCollection : ScriptableObject
{
	// Token: 0x04001DFB RID: 7675
	public Texture capMask;

	// Token: 0x04001DFC RID: 7676
	public bool applyCap;

	// Token: 0x04001DFD RID: 7677
	public HairDye[] Variations;

	// Token: 0x060021F4 RID: 8692 RVA: 0x0001AF57 File Offset: 0x00019157
	public HairDye Get(float seed)
	{
		if (this.Variations.Length != 0)
		{
			return this.Variations[Mathf.Clamp(Mathf.FloorToInt(seed * (float)this.Variations.Length), 0, this.Variations.Length - 1)];
		}
		return null;
	}
}
