using System;
using UnityEngine;

// Token: 0x02000398 RID: 920
public class HideUntilMobile : EntityComponent<BaseEntity>
{
	// Token: 0x0400142D RID: 5165
	public GameObject[] visuals;

	// Token: 0x0400142E RID: 5166
	private Vector3 startPos;

	// Token: 0x0600176B RID: 5995 RVA: 0x0008A6A0 File Offset: 0x000888A0
	protected void Awake()
	{
		if (base.baseEntity.isServer)
		{
			return;
		}
		this.startPos = base.baseEntity.transform.position;
		this.SetVis(false);
		base.Invoke(new Action(this.ForceVisible), 0.5f);
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x0008A6F0 File Offset: 0x000888F0
	private void Update()
	{
		if (base.baseEntity.isServer)
		{
			return;
		}
		if (Vector3.Distance(base.baseEntity.transform.position, this.startPos) > 0.01f)
		{
			this.SetVis(true);
			GameManager.Destroy(this, 0f);
		}
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x00013AB7 File Offset: 0x00011CB7
	public void ForceVisible()
	{
		this.SetVis(true);
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x0008A740 File Offset: 0x00088940
	public void SetVis(bool isVisible)
	{
		if (this.visuals == null)
		{
			return;
		}
		GameObject[] array = this.visuals;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(isVisible);
		}
	}
}
