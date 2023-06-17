using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000088 RID: 136
public class CollectibleEntity : BaseEntity, IPrefabPreProcess
{
	// Token: 0x04000547 RID: 1351
	public Translate.Phrase itemName;

	// Token: 0x04000548 RID: 1352
	public ItemAmount[] itemList;

	// Token: 0x04000549 RID: 1353
	public GameObjectRef pickupEffect;

	// Token: 0x0400054A RID: 1354
	public float xpScale = 1f;

	// Token: 0x0600085D RID: 2141 RVA: 0x0004BC5C File Offset: 0x00049E5C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CollectibleEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool ShouldLerp()
	{
		return false;
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0004BCA0 File Offset: 0x00049EA0
	public override List<Option> GetMenuItems(BasePlayer player)
	{
		List<Option> list = new List<Option>();
		list.Add(new Option
		{
			title = this.itemName.token,
			icon = "pickup",
			show = true,
			function = delegate(BasePlayer ply)
			{
				base.ServerRPC("Pickup");
			}
		});
		return list;
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x00008CD4 File Offset: 0x00006ED4
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			preProcess.RemoveComponent(base.GetComponent<Collider>());
		}
	}
}
