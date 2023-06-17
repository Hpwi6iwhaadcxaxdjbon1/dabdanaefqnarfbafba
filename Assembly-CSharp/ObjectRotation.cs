using System;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
public class ObjectRotation : MonoBehaviour
{
	// Token: 0x040021EB RID: 8683
	public Camera cam;

	// Token: 0x060025F2 RID: 9714 RVA: 0x000C7674 File Offset: 0x000C5874
	private void Update()
	{
		if (!Buttons.Sprint.IsDown)
		{
			if (Buttons.Attack2.IsDown)
			{
				float num = 3f;
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				int index = 0;
				ref Vector3 ptr = ref eulerAngles;
				eulerAngles[index] = (ptr[0] = ptr[0] + Input.GetAxisRaw("Mouse Y") * 1f * num);
				int index2 = 1;
				ptr = ref eulerAngles;
				eulerAngles[index2] = (ptr[1] = ptr[1] + Input.GetAxisRaw("Mouse X") * num * -1f);
				eulerAngles[2] = 0f;
				base.transform.rotation = Quaternion.Euler(eulerAngles);
			}
		}
		else if (Buttons.Attack2.IsDown)
		{
			float d = 0.01f;
			Vector3 vector = Vector3.zero;
			vector += Vector3.right * Input.GetAxisRaw("Mouse X") * 1f * d;
			vector += Vector3.up * Input.GetAxisRaw("Mouse Y") * 1f * d;
			base.transform.position += this.cam.transform.rotation * vector;
		}
		float num2 = Input.GetAxis("Mouse ScrollWheel") * -0.1f;
		this.cam.fieldOfView = Mathf.Clamp(this.cam.fieldOfView + num2, 10f, 90f);
	}
}
