using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200066B RID: 1643
public class ProjectileWeaponInformationPanel : ItemInformationPanel
{
	// Token: 0x0400208E RID: 8334
	public ItemStatValue damageDisplay;

	// Token: 0x0400208F RID: 8335
	public ItemStatValue recoilDisplay;

	// Token: 0x04002090 RID: 8336
	public ItemStatValue rofDisplay;

	// Token: 0x04002091 RID: 8337
	public ItemStatValue accuracyDisplay;

	// Token: 0x04002092 RID: 8338
	public ItemStatValue rangeDisplay;

	// Token: 0x06002494 RID: 9364 RVA: 0x000C1400 File Offset: 0x000BF600
	public override bool EligableForDisplay(ItemDefinition info)
	{
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		return !(component == null) && !(component.entityPrefab.Get().GetComponent<BaseProjectile>() == null);
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000C143C File Offset: 0x000BF63C
	public override void SetupForItem(ItemDefinition info, Item item = null)
	{
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		if (component == null)
		{
			return;
		}
		BaseProjectile component2 = component.entityPrefab.Get().GetComponent<BaseProjectile>();
		if (component2 == null)
		{
			return;
		}
		ItemModProjectile component3 = component2.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		float num = 0f;
		Projectile component4 = component3.projectileObject.Get().GetComponent<Projectile>();
		if (component4)
		{
			using (List<DamageTypeEntry>.Enumerator enumerator = component4.damageTypes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DamageTypeEntry damageTypeEntry = enumerator.Current;
					num += damageTypeEntry.amount;
				}
				goto IL_F2;
			}
		}
		TimedExplosive component5 = component3.projectileObject.Get().GetComponent<TimedExplosive>();
		if (component5 != null)
		{
			foreach (DamageTypeEntry damageTypeEntry2 in component5.damageTypes)
			{
				num += damageTypeEntry2.amount;
			}
		}
		IL_F2:
		float val = num * component2.GetDamageScale(true) * (float)component3.numProjectiles;
		this.damageDisplay.SetValue(val, 0, "");
		float val2;
		if (component2.GetRecoil().useCurves)
		{
			float num2 = 0f;
			RecoilProperties recoil = component2.GetRecoil();
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			for (int i = 0; i < recoil.yawCurve.length; i++)
			{
				int num6 = i - 1;
				float num7 = (num6 >= 0) ? recoil.yawCurve.keys[num6].value : 0f;
				float value = recoil.yawCurve.keys[i].value;
				if (Mathf.Abs(value - num7) > num4)
				{
					num4 = Mathf.Abs(value - num7);
				}
				if (Mathf.Abs(value) > num3)
				{
					num3 = Mathf.Abs(value);
				}
				num5 += Mathf.Abs(value);
				num2 += Mathf.Abs(value - num7);
			}
			val2 = num2 * Mathf.Abs(component2.GetRecoil().recoilYawMax);
		}
		else
		{
			val2 = Mathf.Abs(component2.GetRecoil().recoilPitchMax) + Mathf.Abs(component2.GetRecoil().recoilYawMax);
		}
		this.recoilDisplay.SetValue(val2, 1, "");
		float val3 = 60f / component2.repeatDelay;
		if (component2.primaryMagazine.definition.builtInSize <= 1)
		{
			val3 = 60f / component2.reloadTime;
		}
		this.rofDisplay.SetValue(val3, 0, "");
		float val4 = component2.aimCone + component2.hipAimCone + component2.aimConePenaltyMax + component3.projectileSpread;
		this.accuracyDisplay.SetValue(val4, 0, "");
		float val5 = 1f;
		if (component4 != null)
		{
			val5 = component3.projectileVelocity * component2.GetProjectileVelocityScale(true) * component2.GetDistanceScale(true) * (component4.damageDistances.y / 100f) * 0.5f;
		}
		this.rangeDisplay.SetValue(val5, 0, "");
	}
}
