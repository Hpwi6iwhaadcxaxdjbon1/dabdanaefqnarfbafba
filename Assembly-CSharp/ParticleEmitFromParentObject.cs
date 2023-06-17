using System;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class ParticleEmitFromParentObject : MonoBehaviour
{
	// Token: 0x04000F02 RID: 3842
	public string bonename;

	// Token: 0x04000F03 RID: 3843
	private Bounds bounds;

	// Token: 0x04000F04 RID: 3844
	private Transform bone;

	// Token: 0x04000F05 RID: 3845
	private BaseEntity entity;

	// Token: 0x04000F06 RID: 3846
	private float lastBoundsUpdate;

	// Token: 0x06001265 RID: 4709 RVA: 0x00078960 File Offset: 0x00076B60
	private void OnEnable()
	{
		if (this.bonename != null && base.transform.parent != null)
		{
			PlayerModel component = base.transform.parent.gameObject.GetComponent<PlayerModel>();
			if (component)
			{
				this.bone = component.FindBone(this.bonename);
			}
		}
		this.entity = base.gameObject.ToBaseEntity();
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x000789CC File Offset: 0x00076BCC
	private void UpdateRenderBounds(float delay = 0f)
	{
		if (Time.realtimeSinceStartup < this.lastBoundsUpdate + delay)
		{
			return;
		}
		this.lastBoundsUpdate = Time.realtimeSinceStartup;
		if (this.entity)
		{
			this.bounds = this.entity.WorldSpaceBounds().ToBounds();
		}
		else if (base.transform.parent)
		{
			this.bounds = base.transform.parent.WorkoutRenderBounds();
		}
		this.bounds.center = this.bounds.center - base.transform.parent.position;
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x00078A6C File Offset: 0x00076C6C
	private void Update()
	{
		if (base.transform.parent == null)
		{
			return;
		}
		if (this.bone != null)
		{
			base.transform.position = this.bone.position;
			base.transform.rotation = this.bone.rotation;
			return;
		}
		this.UpdateRenderBounds(0.5f);
		base.transform.localPosition = this.bounds.center;
		base.transform.rotation = Quaternion.identity;
		base.transform.localScale = this.bounds.size;
	}
}
