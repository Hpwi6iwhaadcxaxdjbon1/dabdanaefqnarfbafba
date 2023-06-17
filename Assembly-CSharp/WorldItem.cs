using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C2 RID: 194
public class WorldItem : BaseEntity
{
	// Token: 0x040006B2 RID: 1714
	[Header("WorldItem")]
	public bool allowPickup = true;

	// Token: 0x040006B3 RID: 1715
	[NonSerialized]
	public Item item;

	// Token: 0x06000A43 RID: 2627 RVA: 0x00054B24 File Offset: 0x00052D24
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WorldItem.OnRpcMessage", 0.1f))
		{
			if (rpc == 1868777234U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: PickupSound ");
				}
				using (TimeWarning.New("PickupSound", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.PickupSound(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in PickupSound", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 3559537971U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: UpdateItem ");
				}
				using (TimeWarning.New("UpdateItem", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage packet = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdateItem(packet);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in UpdateItem", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0000A296 File Offset: 0x00008496
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00054D6C File Offset: 0x00052F6C
	[BaseEntity.RPC_Client]
	public void UpdateItem(BaseEntity.RPCMessage packet)
	{
		UpdateItem updateItem = ProtoBuf.UpdateItem.Deserialize(packet.read);
		this.item = ItemManager.Load(updateItem.item, this.item, false);
		if (this.item != null)
		{
			this.item.isServer = base.isServer;
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0000A2B9 File Offset: 0x000084B9
	[BaseEntity.RPC_Client]
	public void PickupSound(BaseEntity.RPCMessage msg)
	{
		if (this.item.info.physImpactSoundDef == null)
		{
			return;
		}
		SoundManager.PlayOneshot(this.item.info.physImpactSoundDef, null, false, base.transform.position);
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x00054DC8 File Offset: 0x00052FC8
	public virtual Vector3 IdealMenuPosition(BasePlayer playerUser)
	{
		Collider component = base.GetComponent<Collider>();
		if (!component)
		{
			return base.transform.position;
		}
		RaycastHit raycastHit;
		if (component.Raycast(playerUser.eyes.BodyRay(), ref raycastHit, 10f))
		{
			return raycastHit.point;
		}
		return component.ClosestPointOnBounds(playerUser.eyes.position);
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x00054E24 File Offset: 0x00053024
	public override Info GetMenuInformation(GameObject primaryObject, BasePlayer player)
	{
		if (this.item == null)
		{
			return base.GetMenuInformation(primaryObject, player);
		}
		List<Option> menuItems = this.GetMenuItems(player);
		if (menuItems.Count == 0)
		{
			return default(Info);
		}
		Info info = default(Info);
		info.action = menuItems[0].title;
		info.icon = menuItems[0].icon;
		info.iconSprite = menuItems[0].iconSprite;
		info.hasMoreOptions = false;
		if (menuItems.Count == 1)
		{
			if (info.icon == "pickup")
			{
				info.action = this.item.info.displayName.token;
			}
			return info;
		}
		info.hasMoreOptions = true;
		return info;
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x00054EE8 File Offset: 0x000530E8
	public override List<Option> GetMenuItems(BasePlayer player)
	{
		List<Option> list = base.GetMenuItems(player);
		if (this.item == null)
		{
			return list;
		}
		if (list == null)
		{
			list = new List<Option>();
		}
		if (this.allowPickup)
		{
			list.Add(new Option
			{
				title = "pick_up",
				icon = "pickup",
				show = true,
				function = delegate(BasePlayer ply)
				{
					base.ServerRPC("Pickup");
				}
			});
		}
		return list;
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0000A2F7 File Offset: 0x000084F7
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		this.RemoveItem();
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0000A305 File Offset: 0x00008505
	public override Item GetItem()
	{
		return this.item;
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00054F5C File Offset: 0x0005315C
	public void InitializeItem(Item in_item)
	{
		if (this.item != null)
		{
			this.RemoveItem();
		}
		this.item = in_item;
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty += new Action<Item>(this.OnItemDirty);
		base.name = this.item.info.shortname + " (world)";
		this.item.SetWorldEntity(this);
		this.OnItemDirty(this.item);
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0000A30D File Offset: 0x0000850D
	public void RemoveItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= new Action<Item>(this.OnItemDirty);
		this.item = null;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0000A337 File Offset: 0x00008537
	public void DestroyItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= new Action<Item>(this.OnItemDirty);
		this.item.Remove(0f);
		this.item = null;
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0000A371 File Offset: 0x00008571
	protected virtual void OnItemDirty(Item in_item)
	{
		Assert.IsTrue(this.item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00054FD8 File Offset: 0x000531D8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.worldItem == null)
		{
			return;
		}
		if (info.msg.worldItem.item == null)
		{
			return;
		}
		Item item = ItemManager.Load(info.msg.worldItem.item, this.item, base.isServer);
		if (item != null)
		{
			this.InitializeItem(item);
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x06000A51 RID: 2641 RVA: 0x0000A3A0 File Offset: 0x000085A0
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			if (this.item != null)
			{
				return this.item.Traits;
			}
			return base.Traits;
		}
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0005503C File Offset: 0x0005323C
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				this._name = string.Format("{1}[{0}] {2}", (this.net != null) ? this.net.ID : 0U, base.ShortPrefabName, base.name);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}
}
