using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class AIMovePoint : MonoBehaviour
{
	// Token: 0x040008CC RID: 2252
	public float radius = 1f;

	// Token: 0x040008CD RID: 2253
	public float nextAvailableRoamTime;

	// Token: 0x040008CE RID: 2254
	public float nextAvailableEngagementTime;

	// Token: 0x040008CF RID: 2255
	public BaseEntity lastUser;

	// Token: 0x06000C12 RID: 3090 RVA: 0x0000B52B File Offset: 0x0000972B
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCircleY(base.transform.position, this.radius);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0000B54D File Offset: 0x0000974D
	public bool CanBeUsedBy(BaseEntity user)
	{
		return (user != null && this.lastUser == user) || this.IsUsed();
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0000B56E File Offset: 0x0000976E
	public bool IsUsed()
	{
		return this.IsUsedForRoaming() || this.IsUsedForEngagement();
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0000B580 File Offset: 0x00009780
	public void MarkUsedForRoam(float dur = 10f, BaseEntity user = null)
	{
		this.nextAvailableRoamTime = Time.time + dur;
		this.lastUser = user;
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x0000B596 File Offset: 0x00009796
	public void MarkUsedForEngagement(float dur = 5f, BaseEntity user = null)
	{
		this.nextAvailableEngagementTime = Time.time + dur;
		this.lastUser = user;
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x0000B5AC File Offset: 0x000097AC
	public bool IsUsedForRoaming()
	{
		return Time.time < this.nextAvailableRoamTime;
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0000B5BB File Offset: 0x000097BB
	public bool IsUsedForEngagement()
	{
		return Time.time < this.nextAvailableEngagementTime;
	}
}
