using System;
using System.Linq;
using Rust;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000469 RID: 1129
public class ItemModWearable : ItemMod
{
	// Token: 0x0400176F RID: 5999
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x04001770 RID: 6000
	public ProtectionProperties protectionProperties;

	// Token: 0x04001771 RID: 6001
	public ArmorProperties armorProperties;

	// Token: 0x04001772 RID: 6002
	public ClothingMovementProperties movementProperties;

	// Token: 0x04001773 RID: 6003
	public UIBlackoutOverlay.blackoutType occlusionType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x04001774 RID: 6004
	public bool blocksAiming;

	// Token: 0x04001775 RID: 6005
	public bool emissive;

	// Token: 0x04001776 RID: 6006
	public float accuracyBonus;

	// Token: 0x04001777 RID: 6007
	public bool blocksEquipping;

	// Token: 0x04001778 RID: 6008
	public GameObjectRef viewmodelAddition;

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06001A7E RID: 6782 RVA: 0x00015F1E File Offset: 0x0001411E
	public Wearable targetWearable
	{
		get
		{
			if (!this.entityPrefab.isValid)
			{
				return null;
			}
			return this.entityPrefab.Get().GetComponent<Wearable>();
		}
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x00092EC0 File Offset: 0x000910C0
	private void DoPrepare()
	{
		if (!this.entityPrefab.isValid)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab is null! " + base.gameObject, base.gameObject);
		}
		if (this.entityPrefab.isValid && this.targetWearable == null)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab doesn't have a Wearable component! " + base.gameObject, this.entityPrefab.Get());
		}
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x00015F3F File Offset: 0x0001413F
	public override void ModInit()
	{
		if (string.IsNullOrEmpty(this.entityPrefab.resourcePath))
		{
			Debug.LogWarning(this + " - entityPrefab is null or something.. - " + this.entityPrefab.guid);
		}
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x00015F6E File Offset: 0x0001416E
	public bool ProtectsArea(HitArea area)
	{
		return !(this.armorProperties == null) && this.armorProperties.Contains(area);
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x00015F8C File Offset: 0x0001418C
	public bool HasProtections()
	{
		return this.protectionProperties != null;
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x00015F9A File Offset: 0x0001419A
	internal float GetProtection(Item item, DamageType damageType)
	{
		if (this.protectionProperties == null)
		{
			return 0f;
		}
		return this.protectionProperties.Get(damageType) * this.ConditionProtectionScale(item);
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x00015FC4 File Offset: 0x000141C4
	public float ConditionProtectionScale(Item item)
	{
		if (!item.isBroken)
		{
			return 1f;
		}
		return 0.25f;
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x00015FD9 File Offset: 0x000141D9
	public void CollectProtection(Item item, ProtectionProperties protection)
	{
		if (this.protectionProperties == null)
		{
			return;
		}
		protection.Add(this.protectionProperties, this.ConditionProtectionScale(item));
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x00092F30 File Offset: 0x00091130
	private bool IsHeadgear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.HeadTop | Wearable.OccupationSlots.Face | Wearable.OccupationSlots.HeadBack)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x00092F64 File Offset: 0x00091164
	public bool IsFootwear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.LeftFoot | Wearable.OccupationSlots.RightFoot)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x00092F9C File Offset: 0x0009119C
	public void OnDressModel(Item item, PlayerModel model)
	{
		if (!this.entityPrefab.isValid)
		{
			return;
		}
		Wearable targetWearable = this.targetWearable;
		model.AddPart(this.entityPrefab.resourcePath, item, Array.Empty<Type>());
		if ((targetWearable.removeSkin & Wearable.RemoveSkin.Feet) != (Wearable.RemoveSkin)0)
		{
			model.RemovePart("skin_feet");
		}
		if ((targetWearable.removeSkin & Wearable.RemoveSkin.Hands) != (Wearable.RemoveSkin)0)
		{
			model.RemovePart("skin_hands");
		}
		if ((targetWearable.removeSkin & Wearable.RemoveSkin.Torso) != (Wearable.RemoveSkin)0)
		{
			model.RemovePart("skin_torso");
			model.RemovePart("skin_clothsash");
		}
		if ((targetWearable.removeSkin & Wearable.RemoveSkin.Legs) != (Wearable.RemoveSkin)0)
		{
			model.RemovePart("skin_legs");
		}
		if ((targetWearable.removeSkin & Wearable.RemoveSkin.Head) != (Wearable.RemoveSkin)0)
		{
			model.RemovePart("skin_head");
		}
		if ((targetWearable.removeHair & Wearable.RemoveHair.Head) != (Wearable.RemoveHair)0)
		{
			model.RemovePart("hair_head");
		}
		if ((targetWearable.removeHair & Wearable.RemoveHair.Eyebrow) != (Wearable.RemoveHair)0)
		{
			model.RemovePart("hair_eyebrow");
		}
		if ((targetWearable.removeHair & Wearable.RemoveHair.Facial) != (Wearable.RemoveHair)0)
		{
			model.RemovePart("hair_facial");
		}
		if ((targetWearable.removeHair & Wearable.RemoveHair.Armpit) != (Wearable.RemoveHair)0)
		{
			model.RemovePart("hair_armpit");
		}
		if ((targetWearable.removeHair & Wearable.RemoveHair.Pubic) != (Wearable.RemoveHair)0)
		{
			model.RemovePart("hair_pubic");
		}
		ItemFootstepSounds component = base.GetComponent<ItemFootstepSounds>();
		if (component != null)
		{
			FootstepEffects component2 = model.GetComponent<FootstepEffects>();
			if (component2 != null)
			{
				component2.footstepEffectName = "footstep/" + component.effectFolder;
				component2.jumpStartEffectName = "jump-start/" + component.effectFolder;
				component2.jumpLandEffectName = "jump-land/" + component.effectFolder;
			}
		}
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x00093114 File Offset: 0x00091314
	public override void OnObjectSetup(Item item, GameObject obj)
	{
		if (item.skin == 0UL)
		{
			return;
		}
		ItemSkinDirectory.Skin skin = Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(item.info.skins, (ItemSkinDirectory.Skin x) => (long)x.id == (long)item.skin);
		if (skin.id == 0 && item.skin > 0UL)
		{
			WorkshopSkin.Apply(obj, item.skin, null);
			return;
		}
		if ((long)skin.id != (long)item.skin)
		{
			return;
		}
		ItemSkin itemSkin = skin.invItem as ItemSkin;
		if (itemSkin == null)
		{
			return;
		}
		itemSkin.ApplySkin(obj);
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x000931BC File Offset: 0x000913BC
	public bool CanExistWith(ItemModWearable wearable)
	{
		if (wearable == null)
		{
			return true;
		}
		Wearable targetWearable = this.targetWearable;
		Wearable targetWearable2 = wearable.targetWearable;
		return (targetWearable.occupationOver & targetWearable2.occupationOver) == (Wearable.OccupationSlots)0 && (targetWearable.occupationUnder & targetWearable2.occupationUnder) == (Wearable.OccupationSlots)0;
	}
}
