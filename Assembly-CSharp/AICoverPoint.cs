using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class AICoverPoint : BaseMonoBehaviour
{
	// Token: 0x040008C4 RID: 2244
	public float coverDot = 0.5f;

	// Token: 0x040008C5 RID: 2245
	private BaseEntity currentUser;

	// Token: 0x06000C01 RID: 3073 RVA: 0x0000B48D File Offset: 0x0000968D
	public bool InUse()
	{
		return this.currentUser != null;
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x0000B49B File Offset: 0x0000969B
	public bool IsUsedBy(BaseEntity user)
	{
		return this.InUse() && !(user == null) && user == this.currentUser;
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x0000B4BE File Offset: 0x000096BE
	public void SetUsedBy(BaseEntity user, float duration = 5f)
	{
		this.currentUser = user;
		base.Invoke(new Action(this.ClearUsed), duration);
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0000B4DA File Offset: 0x000096DA
	public void ClearUsed()
	{
		this.currentUser = null;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0005ADF8 File Offset: 0x00058FF8
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Vector3 vector = base.transform.position + Vector3.up * 1f;
		Gizmos.DrawCube(base.transform.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawLine(base.transform.position, vector);
		Vector3 normalized = (base.transform.forward + base.transform.right * this.coverDot * 1f).normalized;
		Vector3 normalized2 = (base.transform.forward + -base.transform.right * this.coverDot * 1f).normalized;
		Gizmos.DrawLine(vector, vector + normalized * 1f);
		Gizmos.DrawLine(vector, vector + normalized2 * 1f);
	}
}
