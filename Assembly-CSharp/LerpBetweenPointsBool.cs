using System;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class LerpBetweenPointsBool : MonoBehaviour, IClientComponent
{
	// Token: 0x04000FCD RID: 4045
	public Vector3 offsetPosLocal;

	// Token: 0x04000FCE RID: 4046
	public float speed;

	// Token: 0x04000FCF RID: 4047
	private Vector3 targetPos;

	// Token: 0x04000FD0 RID: 4048
	private Vector3 initialPos;

	// Token: 0x06001373 RID: 4979 RVA: 0x000107ED File Offset: 0x0000E9ED
	public void SetAtOffset(bool should)
	{
		this.targetPos = (should ? this.offsetPosLocal : this.initialPos);
		this.SetEnabled(true);
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x0007BD30 File Offset: 0x00079F30
	protected void Awake()
	{
		this.targetPos = (this.initialPos = base.transform.localPosition);
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x0007BD58 File Offset: 0x00079F58
	protected void Update()
	{
		base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, this.targetPos, Time.deltaTime * this.speed);
		if (base.transform.localPosition == this.targetPos)
		{
			this.SetEnabled(false);
		}
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x0007BDB4 File Offset: 0x00079FB4
	private void SetEnabled(bool state)
	{
		base.enabled = state;
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (baseEntity && !baseEntity.IsBusy())
		{
			base.gameObject.SendBatchingToggle(!state);
		}
	}
}
