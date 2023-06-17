using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005C4 RID: 1476
[CreateAssetMenu(menuName = "Rust/Building Grade")]
public class BuildingGrade : ScriptableObject
{
	// Token: 0x04001DAF RID: 7599
	public BuildingGrade.Enum type;

	// Token: 0x04001DB0 RID: 7600
	public float baseHealth;

	// Token: 0x04001DB1 RID: 7601
	public List<ItemAmount> baseCost;

	// Token: 0x04001DB2 RID: 7602
	public PhysicMaterial physicMaterial;

	// Token: 0x04001DB3 RID: 7603
	public ProtectionProperties damageProtecton;

	// Token: 0x04001DB4 RID: 7604
	public BaseEntity.Menu.Option upgradeMenu;

	// Token: 0x020005C5 RID: 1477
	public enum Enum
	{
		// Token: 0x04001DB6 RID: 7606
		None = -1,
		// Token: 0x04001DB7 RID: 7607
		Twigs,
		// Token: 0x04001DB8 RID: 7608
		Wood,
		// Token: 0x04001DB9 RID: 7609
		Stone,
		// Token: 0x04001DBA RID: 7610
		Metal,
		// Token: 0x04001DBB RID: 7611
		TopTier,
		// Token: 0x04001DBC RID: 7612
		Count
	}
}
