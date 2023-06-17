using System;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class DevMovePlayer : BaseMonoBehaviour
{
	// Token: 0x04000E71 RID: 3697
	public BasePlayer player;

	// Token: 0x04000E72 RID: 3698
	public Transform[] Waypoints;

	// Token: 0x04000E73 RID: 3699
	public bool moveRandomly;

	// Token: 0x04000E74 RID: 3700
	public Vector3 destination = Vector3.zero;

	// Token: 0x04000E75 RID: 3701
	public Vector3 lookPoint = Vector3.zero;
}
