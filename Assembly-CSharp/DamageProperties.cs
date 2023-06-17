using System;
using UnityEngine;

// Token: 0x020003E6 RID: 998
[CreateAssetMenu(menuName = "Rust/Damage Properties")]
public class DamageProperties : ScriptableObject
{
	// Token: 0x0400155B RID: 5467
	public DamageProperties fallback;

	// Token: 0x0400155C RID: 5468
	[Horizontal(1, 0)]
	public DamageProperties.HitAreaProperty[] bones;

	// Token: 0x060018F9 RID: 6393 RVA: 0x0008EC50 File Offset: 0x0008CE50
	public float GetMultiplier(HitArea area)
	{
		for (int i = 0; i < this.bones.Length; i++)
		{
			DamageProperties.HitAreaProperty hitAreaProperty = this.bones[i];
			if (hitAreaProperty.area == area)
			{
				return hitAreaProperty.damage;
			}
		}
		if (!this.fallback)
		{
			return 1f;
		}
		return this.fallback.GetMultiplier(area);
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x0008ECA8 File Offset: 0x0008CEA8
	public void ScaleDamage(HitInfo info)
	{
		HitArea boneArea = info.boneArea;
		if (boneArea == (HitArea)(-1) || boneArea == (HitArea)0)
		{
			return;
		}
		info.damageTypes.ScaleAll(this.GetMultiplier(boneArea));
	}

	// Token: 0x020003E7 RID: 999
	[Serializable]
	public class HitAreaProperty
	{
		// Token: 0x0400155D RID: 5469
		public HitArea area = HitArea.Head;

		// Token: 0x0400155E RID: 5470
		public float damage = 1f;
	}
}
