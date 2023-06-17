using System;
using UnityEngine;

// Token: 0x020007A0 RID: 1952
public class ExplosionsBillboard : MonoBehaviour
{
	// Token: 0x040025F2 RID: 9714
	public Camera Camera;

	// Token: 0x040025F3 RID: 9715
	public bool Active = true;

	// Token: 0x040025F4 RID: 9716
	public bool AutoInitCamera = true;

	// Token: 0x040025F5 RID: 9717
	private GameObject myContainer;

	// Token: 0x040025F6 RID: 9718
	private Transform t;

	// Token: 0x040025F7 RID: 9719
	private Transform camT;

	// Token: 0x040025F8 RID: 9720
	private Transform contT;

	// Token: 0x06002A6A RID: 10858 RVA: 0x000D893C File Offset: 0x000D6B3C
	private void Awake()
	{
		if (this.AutoInitCamera)
		{
			this.Camera = Camera.main;
			this.Active = true;
		}
		this.t = base.transform;
		Vector3 localScale = this.t.parent.transform.localScale;
		localScale.z = localScale.x;
		this.t.parent.transform.localScale = localScale;
		this.camT = this.Camera.transform;
		Transform parent = this.t.parent;
		this.myContainer = new GameObject
		{
			name = "Billboard_" + this.t.gameObject.name
		};
		this.contT = this.myContainer.transform;
		this.contT.position = this.t.position;
		this.t.parent = this.myContainer.transform;
		this.contT.parent = parent;
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x000D8A3C File Offset: 0x000D6C3C
	private void Update()
	{
		if (this.Active)
		{
			this.contT.LookAt(this.contT.position + this.camT.rotation * Vector3.back, this.camT.rotation * Vector3.up);
		}
	}
}
