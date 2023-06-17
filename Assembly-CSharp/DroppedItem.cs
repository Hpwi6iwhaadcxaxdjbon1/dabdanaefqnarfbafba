using System;
using System.Linq;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class DroppedItem : WorldItem
{
	// Token: 0x040012FA RID: 4858
	[Header("DroppedItem")]
	public GameObject itemModel;

	// Token: 0x06001602 RID: 5634 RVA: 0x00085FFC File Offset: 0x000841FC
	public override void PostInitShared()
	{
		base.PostInitShared();
		GameObject gameObject;
		if (this.item != null && this.item.info.worldModelPrefab.isValid)
		{
			gameObject = this.item.info.worldModelPrefab.Instantiate(null);
		}
		else
		{
			gameObject = Object.Instantiate<GameObject>(this.itemModel);
		}
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetLayerRecursive(base.gameObject.layer);
		Collider component = gameObject.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
		if (this.item != null)
		{
			PhysicsEffects component2 = base.gameObject.GetComponent<PhysicsEffects>();
			if (component2 != null)
			{
				component2.entity = this;
				if (this.item.info.physImpactSoundDef != null)
				{
					component2.physImpactSoundDef = this.item.info.physImpactSoundDef;
				}
			}
			if (this.item.skin > 0UL)
			{
				ItemSkinDirectory.Skin skin = Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(this.item.info.skins, (ItemSkinDirectory.Skin x) => (long)x.id == (long)this.item.skin);
				if ((long)skin.id == (long)this.item.skin)
				{
					ItemSkin itemSkin = skin.invItem as ItemSkin;
					if (itemSkin != null)
					{
						itemSkin.ApplySkin(gameObject);
					}
				}
				else if (this.item.skin != 0UL)
				{
					WorkshopSkin.Apply(gameObject, this.item.skin, null);
				}
			}
		}
		gameObject.SetActive(true);
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x0003D614 File Offset: 0x0003B814
	public override void OnSignal(BaseEntity.Signal signal, string arg)
	{
		base.OnSignal(signal, arg);
		float hardness;
		if (signal == BaseEntity.Signal.PhysImpact && float.TryParse(arg, ref hardness))
		{
			PhysicsEffects component = base.gameObject.GetComponent<PhysicsEffects>();
			if (component != null)
			{
				component.PlayImpactSound(hardness);
			}
		}
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool PhysicsDriven()
	{
		return true;
	}
}
