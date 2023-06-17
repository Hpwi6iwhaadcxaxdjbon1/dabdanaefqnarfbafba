using System;
using ProtoBuf;

// Token: 0x0200031F RID: 799
public class PlayerCorpse : LootableCorpse
{
	// Token: 0x04001247 RID: 4679
	public Buoyancy buoyancy;

	// Token: 0x04001248 RID: 4680
	public const BaseEntity.Flags Flag_Buoyant = BaseEntity.Flags.Reserved6;

	// Token: 0x04001249 RID: 4681
	[NonSerialized]
	private ItemContainer clientClothing;

	// Token: 0x0400124A RID: 4682
	private uint lastClothes = uint.MaxValue;

	// Token: 0x0400124B RID: 4683
	private bool wasBuoyant_client;

	// Token: 0x0600157B RID: 5499 RVA: 0x00002D2A File Offset: 0x00000F2A
	public bool IsBuoyant()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved6);
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x0001232D File Offset: 0x0001052D
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.wasBuoyant_client = false;
		this.RebuildWorldModel();
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x00084700 File Offset: 0x00082900
	private void RebuildWorldModel()
	{
		if (this.ragdollObject == null)
		{
			return;
		}
		if (this.clientClothing == null)
		{
			return;
		}
		PlayerModel component = this.ragdollObject.GetComponent<PlayerModel>();
		if (!component)
		{
			return;
		}
		component.showSash = base.HasFlag(BaseEntity.Flags.Reserved5);
		component.skinType = ((BasePlayer.GetRandomFloatBasedOnUserID(this.playerSteamID, 4332UL) > 0.5f) ? 1 : 0);
		component.skinColor = BasePlayer.GetRandomFloatBasedOnUserID(this.playerSteamID, 5977UL);
		component.skinNumber = BasePlayer.GetRandomFloatBasedOnUserID(this.playerSteamID, 3975UL);
		component.meshNumber = BasePlayer.GetRandomFloatBasedOnUserID(this.playerSteamID, 2647UL);
		component.hairNumber = BasePlayer.GetRandomFloatBasedOnUserID(this.playerSteamID, 6338UL);
		component.Clear();
		foreach (Item item in this.clientClothing.itemList)
		{
			ItemModWearable component2 = item.info.GetComponent<ItemModWearable>();
			if (!(component2 == null))
			{
				component2.OnDressModel(item, component);
			}
		}
		component.Rebuild(true);
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x00084838 File Offset: 0x00082A38
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isClient && !info.fromDisk && info.msg.storageBox != null)
		{
			this.clientClothing = new ItemContainer();
			this.clientClothing.ClientInitialize(null, 6);
			this.clientClothing.Load(info.msg.storageBox.contents);
			uint num = this.clientClothing.ContentsHash();
			if (this.lastClothes != num && !base.HasFlag(BaseEntity.Flags.Reserved2))
			{
				this.RebuildWorldModel();
			}
			this.lastClothes = num;
		}
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x000848CC File Offset: 0x00082ACC
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (base.isClient && !this.wasBuoyant_client && this.IsBuoyant() && this.ragdollObject != null)
		{
			this.wasBuoyant_client = true;
			Buoyancy[] componentsInChildren = this.ragdollObject.GetComponentsInChildren<Buoyancy>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x00012343 File Offset: 0x00010543
	public override string Categorize()
	{
		return "playercorpse";
	}
}
