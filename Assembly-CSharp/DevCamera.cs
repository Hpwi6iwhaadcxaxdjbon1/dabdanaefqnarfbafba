using System;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class DevCamera : MonoBehaviour
{
	// Token: 0x04000E63 RID: 3683
	public float movementScale = 1f;

	// Token: 0x04000E64 RID: 3684
	private Vector3 view;

	// Token: 0x04000E65 RID: 3685
	private Vector3 velocity;

	// Token: 0x04000E66 RID: 3686
	private float zoom = 0.5f;

	// Token: 0x060011B4 RID: 4532 RVA: 0x00075570 File Offset: 0x00073770
	private void Start()
	{
		base.transform.position = MainCamera.mainCamera.transform.position;
		this.view = MainCamera.mainCamera.transform.rotation.eulerAngles;
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x0000F7B7 File Offset: 0x0000D9B7
	private void Update()
	{
		if (Buttons.Attack2.IsDown)
		{
			this.DoFreeMovement();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			return;
		}
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x000755B4 File Offset: 0x000737B4
	private void DoFreeMovement()
	{
		if (Buttons.Attack.IsDown)
		{
			this.zoom = Mathf.Clamp(this.zoom + Input.GetAxisRaw("Mouse Y") * -0.033f, 0.01f, 1f);
		}
		else if (Buttons.Duck.IsDown)
		{
			this.view.z = this.view.z + Input.GetAxisRaw("Mouse X") * 1.5f;
		}
		else
		{
			this.view.y = this.view.y + Input.GetAxisRaw("Mouse X") * 3f * this.zoom;
			this.view.x = this.view.x - Input.GetAxisRaw("Mouse Y") * 3f * this.zoom;
		}
		base.transform.rotation = Quaternion.Euler(this.view);
		Vector3 vector = Vector3.zero;
		if (Buttons.Forward.IsDown)
		{
			vector += Vector3.forward;
		}
		if (Buttons.Backward.IsDown)
		{
			vector += Vector3.back;
		}
		if (Buttons.Left.IsDown)
		{
			vector += Vector3.left;
		}
		if (Buttons.Right.IsDown)
		{
			vector += Vector3.right;
		}
		vector.Normalize();
		if (Buttons.Sprint.IsDown)
		{
			vector *= 3f;
		}
		this.velocity += base.transform.rotation * vector * 0.05f;
		this.velocity = Vector3.Lerp(this.velocity, Vector3.zero, 0.2f);
		base.transform.position += this.velocity * this.movementScale;
		MainCamera.mainCamera.transform.position = base.transform.position;
		MainCamera.mainCamera.transform.rotation = base.transform.rotation;
		MainCamera.mainCamera.fieldOfView = Mathf.Lerp(1f, 120f, this.zoom);
	}
}
