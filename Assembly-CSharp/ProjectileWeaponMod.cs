using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class ProjectileWeaponMod : BaseEntity
{
	// Token: 0x0400110D RID: 4365
	[Header("Silencer")]
	public GameObjectRef defaultSilencerEffect;

	// Token: 0x0400110E RID: 4366
	public bool isSilencer;

	// Token: 0x0400110F RID: 4367
	[Header("Weapon Basics")]
	public ProjectileWeaponMod.Modifier repeatDelay;

	// Token: 0x04001110 RID: 4368
	public ProjectileWeaponMod.Modifier projectileVelocity;

	// Token: 0x04001111 RID: 4369
	public ProjectileWeaponMod.Modifier projectileDamage;

	// Token: 0x04001112 RID: 4370
	public ProjectileWeaponMod.Modifier projectileDistance;

	// Token: 0x04001113 RID: 4371
	[Header("Recoil")]
	public ProjectileWeaponMod.Modifier aimsway;

	// Token: 0x04001114 RID: 4372
	public ProjectileWeaponMod.Modifier aimswaySpeed;

	// Token: 0x04001115 RID: 4373
	public ProjectileWeaponMod.Modifier recoil;

	// Token: 0x04001116 RID: 4374
	[Header("Aim Cone")]
	public ProjectileWeaponMod.Modifier sightAimCone;

	// Token: 0x04001117 RID: 4375
	public ProjectileWeaponMod.Modifier hipAimCone;

	// Token: 0x04001118 RID: 4376
	[Header("Light Effects")]
	public bool isLight;

	// Token: 0x04001119 RID: 4377
	[Header("MuzzleBrake")]
	public bool isMuzzleBrake;

	// Token: 0x0400111A RID: 4378
	[Header("MuzzleBoost")]
	public bool isMuzzleBoost;

	// Token: 0x0400111B RID: 4379
	[Header("Scope")]
	public bool isScope;

	// Token: 0x0400111C RID: 4380
	public float zoomAmountDisplayOnly;

	// Token: 0x0400111D RID: 4381
	public bool needsOnForEffects;

	// Token: 0x06001432 RID: 5170 RVA: 0x0007DB78 File Offset: 0x0007BD78
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			return;
		}
		ViewmodelAttachment component = base.GetComponent<ViewmodelAttachment>();
		if (component != null)
		{
			component.RootEntFlagsChanged(this);
		}
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x00011357 File Offset: 0x0000F557
	public virtual void SetAiming(bool isAiming)
	{
		if (this.isScope && this.needsOnForEffects)
		{
			base.SetFlag(BaseEntity.Flags.On, isAiming, false, true);
		}
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x0007DBB0 File Offset: 0x0007BDB0
	public static float Sum(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Sum(mods);
		}
		return def;
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x0007DBE0 File Offset: 0x0007BDE0
	public static float Average(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Average(mods);
		}
		return def;
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x0007DC10 File Offset: 0x0007BE10
	public static float Max(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Max(mods);
		}
		return def;
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x0007DC40 File Offset: 0x0007BE40
	public static float Min(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Min(mods);
		}
		return def;
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0007DC70 File Offset: 0x0007BE70
	public static IEnumerable<float> GetMods(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value)
	{
		return Enumerable.Select<ProjectileWeaponMod.Modifier, float>(Enumerable.Where<ProjectileWeaponMod.Modifier>(Enumerable.Select<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>(Enumerable.Where<ProjectileWeaponMod>(Enumerable.Cast<ProjectileWeaponMod>(parentEnt.children), (ProjectileWeaponMod x) => x != null && (!x.needsOnForEffects || x.HasFlag(BaseEntity.Flags.On))), selector_modifier), (ProjectileWeaponMod.Modifier x) => x.enabled), selector_value);
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x00011373 File Offset: 0x0000F573
	public static bool HasBrokenWeaponMod(BaseEntity parentEnt)
	{
		if (parentEnt.children == null)
		{
			return false;
		}
		return Enumerable.Any<ProjectileWeaponMod>(Enumerable.Cast<ProjectileWeaponMod>(parentEnt.children), (ProjectileWeaponMod x) => x != null && x.IsBroken());
	}

	// Token: 0x02000302 RID: 770
	[Serializable]
	public struct Modifier
	{
		// Token: 0x0400111E RID: 4382
		public bool enabled;

		// Token: 0x0400111F RID: 4383
		[Tooltip("1 means no change. 0.5 is half.")]
		public float scalar;

		// Token: 0x04001120 RID: 4384
		[Tooltip("Added after the scalar is applied.")]
		public float offset;
	}
}
