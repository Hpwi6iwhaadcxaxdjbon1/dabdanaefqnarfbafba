using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002B4 RID: 692
public class EntityFlag_Toggle : EntityComponent<BaseEntity>, IOnPostNetworkUpdate, IOnSendNetworkUpdate, IPrefabPreProcess
{
	// Token: 0x04000FA9 RID: 4009
	public bool runClientside = true;

	// Token: 0x04000FAA RID: 4010
	public bool runServerside = true;

	// Token: 0x04000FAB RID: 4011
	public BaseEntity.Flags flag;

	// Token: 0x04000FAC RID: 4012
	[SerializeField]
	private UnityEvent onFlagEnabled = new UnityEvent();

	// Token: 0x04000FAD RID: 4013
	[SerializeField]
	private UnityEvent onFlagDisabled = new UnityEvent();

	// Token: 0x04000FAE RID: 4014
	internal bool hasRunOnce;

	// Token: 0x04000FAF RID: 4015
	internal bool lastHasFlag;

	// Token: 0x06001357 RID: 4951 RVA: 0x0007B570 File Offset: 0x00079770
	public void DoUpdate(BaseEntity entity)
	{
		bool flag = entity.HasFlag(this.flag);
		if (this.hasRunOnce && flag == this.lastHasFlag)
		{
			return;
		}
		this.hasRunOnce = true;
		this.lastHasFlag = flag;
		if (flag)
		{
			this.onFlagEnabled.Invoke();
			return;
		}
		this.onFlagDisabled.Invoke();
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x0001069F File Offset: 0x0000E89F
	public void OnPostNetworkUpdate(BaseEntity entity)
	{
		if (base.baseEntity != entity)
		{
			return;
		}
		if (!this.runClientside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x000106C0 File Offset: 0x0000E8C0
	public void OnSendNetworkUpdate(BaseEntity entity)
	{
		if (!this.runServerside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x000106D2 File Offset: 0x0000E8D2
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			process.RemoveComponent(this);
		}
	}
}
