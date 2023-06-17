using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Rust.Workshop;
using UnityEngine;

// Token: 0x020007C6 RID: 1990
public class ViewmodelClothing : MonoBehaviour
{
	// Token: 0x04002729 RID: 10025
	public SkeletonSkin[] SkeletonSkins;

	// Token: 0x06002B77 RID: 11127 RVA: 0x000DECC0 File Offset: 0x000DCEC0
	internal void CopyToSkeleton(Skeleton skeleton, GameObject parent, Item item)
	{
		Func<ItemSkinDirectory.Skin, bool> <>9__0;
		foreach (SkeletonSkin skeletonSkin in this.SkeletonSkins)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = parent.transform;
			skeletonSkin.DuplicateAndRetarget(gameObject, skeleton).updateWhenOffscreen = true;
			if (item != null && item.skin > 0UL)
			{
				IEnumerable<ItemSkinDirectory.Skin> skins = item.info.skins;
				Func<ItemSkinDirectory.Skin, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = ((ItemSkinDirectory.Skin x) => (long)x.id == (long)item.skin));
				}
				ItemSkinDirectory.Skin skin = Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(skins, func);
				if (skin.id == 0 && item.skin > 0UL)
				{
					WorkshopSkin.Apply(gameObject, item.skin, null);
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
				itemSkin.ApplySkin(gameObject);
			}
		}
	}
}
