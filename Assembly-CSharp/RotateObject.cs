using System;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class RotateObject : MonoBehaviour
{
	// Token: 0x04000E59 RID: 3673
	public float rotateSpeed_X = 1f;

	// Token: 0x04000E5A RID: 3674
	public float rotateSpeed_Y = 1f;

	// Token: 0x04000E5B RID: 3675
	public float rotateSpeed_Z = 1f;

	// Token: 0x060011A6 RID: 4518 RVA: 0x00074FEC File Offset: 0x000731EC
	private void Update()
	{
		base.transform.Rotate(Vector3.up, Time.deltaTime * this.rotateSpeed_X);
		base.transform.Rotate(base.transform.forward, Time.deltaTime * this.rotateSpeed_Y);
		base.transform.Rotate(base.transform.right, Time.deltaTime * this.rotateSpeed_Z);
	}
}
