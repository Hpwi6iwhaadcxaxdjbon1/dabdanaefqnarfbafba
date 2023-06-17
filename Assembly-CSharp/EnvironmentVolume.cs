using System;
using UnityEngine;

// Token: 0x02000387 RID: 903
public class EnvironmentVolume : MonoBehaviour
{
	// Token: 0x040013BE RID: 5054
	public bool StickyGizmos;

	// Token: 0x040013BF RID: 5055
	public EnvironmentType Type = EnvironmentType.Underground;

	// Token: 0x040013C0 RID: 5056
	public Vector3 Center = Vector3.zero;

	// Token: 0x040013C1 RID: 5057
	public Vector3 Size = Vector3.one;

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x060016EE RID: 5870 RVA: 0x00013508 File Offset: 0x00011708
	// (set) Token: 0x060016EF RID: 5871 RVA: 0x00013510 File Offset: 0x00011710
	public BoxCollider trigger { get; private set; }

	// Token: 0x060016F0 RID: 5872 RVA: 0x00013519 File Offset: 0x00011719
	protected void OnDrawGizmos()
	{
		if (this.StickyGizmos)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x00013529 File Offset: 0x00011729
	protected void OnDrawGizmosSelected()
	{
		if (!this.StickyGizmos)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x060016F2 RID: 5874 RVA: 0x0008888C File Offset: 0x00086A8C
	private void DrawGizmos()
	{
		Vector3 lossyScale = base.transform.lossyScale;
		Quaternion rotation = base.transform.rotation;
		Vector3 pos = base.transform.position + rotation * Vector3.Scale(lossyScale, this.Center);
		Vector3 size = Vector3.Scale(lossyScale, this.Size);
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		GizmosUtil.DrawCube(pos, size, rotation);
		GizmosUtil.DrawWireCube(pos, size, rotation);
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x00013539 File Offset: 0x00011739
	protected virtual void Awake()
	{
		this.UpdateTrigger();
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x00088910 File Offset: 0x00086B10
	public void UpdateTrigger()
	{
		if (!this.trigger)
		{
			this.trigger = base.gameObject.GetComponent<BoxCollider>();
		}
		if (!this.trigger)
		{
			this.trigger = base.gameObject.AddComponent<BoxCollider>();
		}
		this.trigger.isTrigger = true;
		this.trigger.center = this.Center;
		this.trigger.size = this.Size;
	}
}
