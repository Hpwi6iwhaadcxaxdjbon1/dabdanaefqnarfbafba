using System;
using UnityEngine;

// Token: 0x020003E8 RID: 1000
[CreateAssetMenu(menuName = "Rust/LazyAim Properties")]
public class LazyAimProperties : ScriptableObject
{
	// Token: 0x0400155F RID: 5471
	[Range(0f, 10f)]
	public float snapStrength = 6f;

	// Token: 0x04001560 RID: 5472
	[Range(0f, 45f)]
	public float deadzoneAngle = 1f;
}
