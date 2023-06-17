using System;
using UnityEngine;

// Token: 0x02000777 RID: 1911
public class RagdollEditor : SingletonComponent<RagdollEditor>
{
	// Token: 0x040024B3 RID: 9395
	private Vector3 view;

	// Token: 0x040024B4 RID: 9396
	private Rigidbody grabbedRigid;

	// Token: 0x040024B5 RID: 9397
	private Vector3 grabPos;

	// Token: 0x040024B6 RID: 9398
	private Vector3 grabOffset;

	// Token: 0x060029A6 RID: 10662 RVA: 0x00020626 File Offset: 0x0001E826
	private void OnGUI()
	{
		GUI.Box(new Rect((float)Screen.width * 0.5f - 2f, (float)Screen.height * 0.5f - 2f, 4f, 4f), "");
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x00020665 File Offset: 0x0001E865
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000D3E70 File Offset: 0x000D2070
	private void Update()
	{
		Camera.main.fieldOfView = 75f;
		if (Input.GetKey(KeyCode.Mouse1))
		{
			this.view.y = this.view.y + Input.GetAxisRaw("Mouse X") * 3f;
			this.view.x = this.view.x - Input.GetAxisRaw("Mouse Y") * 3f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		Camera.main.transform.rotation = Quaternion.Euler(this.view);
		Vector3 vector = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vector += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += Vector3.right;
		}
		Camera.main.transform.position += base.transform.rotation * vector * 0.05f;
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.StartGrab();
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			this.StopGrab();
		}
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x0002066D File Offset: 0x0001E86D
	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			this.UpdateGrab();
		}
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000D3FC0 File Offset: 0x000D21C0
	private void StartGrab()
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(base.transform.position, base.transform.forward, ref raycastHit, 100f))
		{
			return;
		}
		this.grabbedRigid = raycastHit.collider.GetComponent<Rigidbody>();
		if (this.grabbedRigid == null)
		{
			return;
		}
		this.grabPos = this.grabbedRigid.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
		this.grabOffset = base.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000D4058 File Offset: 0x000D2258
	private void UpdateGrab()
	{
		if (this.grabbedRigid == null)
		{
			return;
		}
		Vector3 a = base.transform.TransformPoint(this.grabOffset);
		Vector3 vector = this.grabbedRigid.transform.TransformPoint(this.grabPos);
		Vector3 vector2 = a - vector;
		this.grabbedRigid.AddForceAtPosition(vector2 * 100f, vector, 5);
		DDraw.Line(vector, vector + vector2, Color.green, 0.01f, true, true);
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x00020681 File Offset: 0x0001E881
	private void StopGrab()
	{
		this.grabbedRigid = null;
	}
}
