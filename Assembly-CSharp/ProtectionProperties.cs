using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020005E6 RID: 1510
[CreateAssetMenu(menuName = "Rust/Protection Properties")]
public class ProtectionProperties : ScriptableObject
{
	// Token: 0x04001E31 RID: 7729
	[TextArea]
	public string comments;

	// Token: 0x04001E32 RID: 7730
	[Range(0f, 100f)]
	public float density = 1f;

	// Token: 0x04001E33 RID: 7731
	[ArrayIndexIsEnumRanged(enumType = typeof(DamageType), min = -4f, max = 1f)]
	public float[] amounts = new float[22];

	// Token: 0x06002217 RID: 8727 RVA: 0x000B79BC File Offset: 0x000B5BBC
	public void OnValidate()
	{
		if (this.amounts.Length < 22)
		{
			float[] array = new float[22];
			for (int i = 0; i < array.Length; i++)
			{
				if (i >= this.amounts.Length)
				{
					if (i == 21)
					{
						array[i] = this.amounts[9];
					}
				}
				else
				{
					array[i] = this.amounts[i];
				}
			}
			this.amounts = array;
		}
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x000B7A1C File Offset: 0x000B5C1C
	public void Clear()
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] = 0f;
		}
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x000B7A4C File Offset: 0x000B5C4C
	public void Add(float amount)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] += amount;
		}
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x0001B0D9 File Offset: 0x000192D9
	public void Add(DamageType index, float amount)
	{
		this.amounts[(int)index] += amount;
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000B7A80 File Offset: 0x000B5C80
	public void Add(ProtectionProperties other, float scale)
	{
		for (int i = 0; i < Mathf.Min(other.amounts.Length, this.amounts.Length); i++)
		{
			this.amounts[i] += other.amounts[i] * scale;
		}
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000B7AC8 File Offset: 0x000B5CC8
	public void Add(List<Item> items, HitArea area = (HitArea)(-1))
	{
		for (int i = 0; i < items.Count; i++)
		{
			Item item = items[i];
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null) && component.ProtectsArea(area))
			{
				component.CollectProtection(item, this);
			}
		}
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x000B7B14 File Offset: 0x000B5D14
	public void Multiply(float multiplier)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] *= multiplier;
		}
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x0001B0EC File Offset: 0x000192EC
	public void Multiply(DamageType index, float multiplier)
	{
		this.amounts[(int)index] *= multiplier;
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x000B7B48 File Offset: 0x000B5D48
	public void Scale(DamageTypeList damageList, float ProtectionAmount = 1f)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			if (this.amounts[i] != 0f)
			{
				damageList.Scale((DamageType)i, 1f - this.amounts[i] * ProtectionAmount);
			}
		}
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x0001B0FF File Offset: 0x000192FF
	public float Get(DamageType damageType)
	{
		return this.amounts[(int)damageType];
	}
}
