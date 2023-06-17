using System;
using UnityEngine;

// Token: 0x020003E4 RID: 996
[CreateAssetMenu(menuName = "Rust/Armor Properties")]
public class ArmorProperties : ScriptableObject
{
	// Token: 0x04001557 RID: 5463
	[InspectorFlags]
	public HitArea area;

	// Token: 0x060018F6 RID: 6390 RVA: 0x00014CF8 File Offset: 0x00012EF8
	public bool Contains(HitArea hitArea)
	{
		return (this.area & hitArea) > (HitArea)0;
	}
}
