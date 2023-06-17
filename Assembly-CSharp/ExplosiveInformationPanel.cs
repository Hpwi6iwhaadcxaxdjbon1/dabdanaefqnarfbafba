using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000652 RID: 1618
public class ExplosiveInformationPanel : ItemInformationPanel
{
	// Token: 0x04002025 RID: 8229
	public ItemTextValue explosiveDmgDisplay;

	// Token: 0x04002026 RID: 8230
	public ItemTextValue lethalDmgDisplay;

	// Token: 0x04002027 RID: 8231
	public ItemTextValue throwDistanceDisplay;

	// Token: 0x04002028 RID: 8232
	public ItemTextValue projectileDistanceDisplay;

	// Token: 0x04002029 RID: 8233
	public ItemTextValue fuseLengthDisplay;

	// Token: 0x0400202A RID: 8234
	public ItemTextValue blastRadiusDisplay;

	// Token: 0x0400202B RID: 8235
	public Text unreliableText;

	// Token: 0x06002416 RID: 9238 RVA: 0x000BEEAC File Offset: 0x000BD0AC
	public override bool EligableForDisplay(ItemDefinition info)
	{
		ItemModProjectile component = info.GetComponent<ItemModProjectile>();
		ItemModEntity component2 = info.GetComponent<ItemModEntity>();
		if (component2 == null && component == null)
		{
			return false;
		}
		if (component2)
		{
			ThrownWeapon component3 = component2.entityPrefab.Get().GetComponent<ThrownWeapon>();
			if (component3 == null)
			{
				return false;
			}
			if (component3.prefabToThrow.Get().GetComponent<TimedExplosive>() == null)
			{
				return false;
			}
		}
		return !component || !(component.projectileObject.Get().GetComponent<TimedExplosive>() == null);
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000BEF3C File Offset: 0x000BD13C
	public override void SetupForItem(ItemDefinition info, Item item = null)
	{
		ItemModProjectile component = info.GetComponent<ItemModProjectile>();
		ItemModEntity component2 = info.GetComponent<ItemModEntity>();
		if (component2 == null && component == null)
		{
			return;
		}
		ThrownWeapon thrownWeapon = null;
		TimedExplosive timedExplosive = null;
		ServerProjectile serverProjectile = null;
		if (component2)
		{
			thrownWeapon = component2.entityPrefab.Get().GetComponent<ThrownWeapon>();
			if (thrownWeapon == null)
			{
				return;
			}
			timedExplosive = thrownWeapon.prefabToThrow.Get().GetComponent<TimedExplosive>();
			if (timedExplosive == null)
			{
				return;
			}
		}
		if (component)
		{
			serverProjectile = component.projectileObject.Get().GetComponent<ServerProjectile>();
			timedExplosive = component.projectileObject.Get().GetComponent<TimedExplosive>();
			if (timedExplosive == null)
			{
				return;
			}
		}
		float num = 0f;
		float num2 = 0f;
		foreach (DamageTypeEntry damageTypeEntry in timedExplosive.damageTypes)
		{
			num2 += damageTypeEntry.amount;
			if (damageTypeEntry.type == DamageType.Explosion)
			{
				num = damageTypeEntry.amount;
			}
		}
		this.lethalDmgDisplay.SetValue(num2, 0, "");
		this.explosiveDmgDisplay.SetValue(num, 0, "");
		this.fuseLengthDisplay.SetValue(0f, 0, (timedExplosive.timerAmountMin == timedExplosive.timerAmountMax) ? timedExplosive.timerAmountMax.ToString("N0") : (timedExplosive.timerAmountMin + "-" + timedExplosive.timerAmountMax));
		this.blastRadiusDisplay.SetValue(timedExplosive.explosionRadius, 1, "");
		if (thrownWeapon != null)
		{
			this.throwDistanceDisplay.SetValue(thrownWeapon.maxThrowVelocity * 2f, 0, "");
		}
		else if (serverProjectile != null)
		{
			this.projectileDistanceDisplay.SetValue(serverProjectile.speed * 5f, 0, "");
		}
		this.unreliableText.gameObject.SetActive(timedExplosive.GetComponent<DudTimedExplosive>() != null);
		this.lethalDmgDisplay.gameObject.SetActive(num2 > 0f);
		this.explosiveDmgDisplay.gameObject.SetActive(num > 0f);
		this.blastRadiusDisplay.gameObject.SetActive(num2 > 0f || num > 0f);
		this.projectileDistanceDisplay.gameObject.SetActive(serverProjectile != null);
		this.throwDistanceDisplay.gameObject.SetActive(thrownWeapon != null);
		this.explosiveDmgDisplay.gameObject.transform.parent.gameObject.SetActive(false);
		this.explosiveDmgDisplay.gameObject.transform.parent.gameObject.SetActive(true);
		Canvas.ForceUpdateCanvases();
	}
}
