using System;
using UnityEngine;

// Token: 0x0200071D RID: 1821
public class RotateCameraAroundObject : MonoBehaviour
{
	// Token: 0x040023B6 RID: 9142
	public GameObject m_goObjectToRotateAround;

	// Token: 0x040023B7 RID: 9143
	public float m_flRotateSpeed = 10f;

	// Token: 0x060027E5 RID: 10213 RVA: 0x000CDEC0 File Offset: 0x000CC0C0
	private void FixedUpdate()
	{
		if (this.m_goObjectToRotateAround != null)
		{
			base.transform.LookAt(this.m_goObjectToRotateAround.transform.position + Vector3.up * 0.75f);
			base.transform.Translate(Vector3.right * this.m_flRotateSpeed * Time.deltaTime);
		}
	}
}
