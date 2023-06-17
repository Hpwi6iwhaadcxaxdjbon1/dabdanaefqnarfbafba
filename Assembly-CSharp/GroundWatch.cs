using System;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class GroundWatch : MonoBehaviour, IServerComponent
{
	// Token: 0x0400142A RID: 5162
	public Vector3 groundPosition = Vector3.zero;

	// Token: 0x0400142B RID: 5163
	public LayerMask layers = 27328512;

	// Token: 0x0400142C RID: 5164
	public float radius = 0.1f;
}
