using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200037F RID: 895
public class EffectDictionary
{
	// Token: 0x040013A6 RID: 5030
	private static Dictionary<string, string[]> effectDictionary;

	// Token: 0x060016CB RID: 5835 RVA: 0x0001334A File Offset: 0x0001154A
	public static string GetParticle(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("impacts", impactType, materialName);
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x00088510 File Offset: 0x00086710
	public static string GetParticle(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetParticle("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetParticle("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetParticle("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetParticle("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetParticle("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetParticle("blunt", materialName);
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x00013358 File Offset: 0x00011558
	public static string GetDecal(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("decals", impactType, materialName);
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x00088590 File Offset: 0x00086790
	public static string GetDecal(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetDecal("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetDecal("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetDecal("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetDecal("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetDecal("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetDecal("blunt", materialName);
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x00013366 File Offset: 0x00011566
	public static string GetDisplacement(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("displacement", impactType, materialName);
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x00088610 File Offset: 0x00086810
	private static string LookupEffect(string category, string effect, string material)
	{
		if (EffectDictionary.effectDictionary == null)
		{
			EffectDictionary.effectDictionary = GameManifest.LoadEffectDictionary();
		}
		string format = "assets/bundled/prefabs/fx/{0}/{1}/{2}";
		string[] array;
		if (!EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, material), ref array) && !EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, "generic"), ref array))
		{
			return string.Empty;
		}
		return array[Random.Range(0, array.Length)];
	}
}
