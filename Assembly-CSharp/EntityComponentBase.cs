using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x020002B2 RID: 690
public class EntityComponentBase : BaseMonoBehaviour
{
	// Token: 0x06001350 RID: 4944 RVA: 0x0000792A File Offset: 0x00005B2A
	protected virtual BaseEntity GetBaseEntity()
	{
		return null;
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void GetMenuOptions(List<Option> list)
	{
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06001352 RID: 4946 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool HasMenuOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}
}
