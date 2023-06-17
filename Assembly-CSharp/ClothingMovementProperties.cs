using System;
using UnityEngine;

// Token: 0x020003E5 RID: 997
[CreateAssetMenu(menuName = "Rust/Clothing Movement Properties")]
public class ClothingMovementProperties : ScriptableObject
{
	// Token: 0x04001558 RID: 5464
	public float speedReduction;

	// Token: 0x04001559 RID: 5465
	[Tooltip("If this piece of clothing is worn movement speed will be reduced by atleast this much")]
	public float minSpeedReduction;

	// Token: 0x0400155A RID: 5466
	public float waterSpeedBonus;
}
