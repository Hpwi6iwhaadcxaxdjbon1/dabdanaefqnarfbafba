using System;
using System.Collections.Generic;

namespace Rust
{
	// Token: 0x020008AD RID: 2221
	public class DamageTypeList
	{
		// Token: 0x04002AE1 RID: 10977
		public float[] types = new float[22];

		// Token: 0x0600300B RID: 12299 RVA: 0x00024F8B File Offset: 0x0002318B
		public void Set(DamageType index, float amount)
		{
			this.types[(int)index] = amount;
		}

		// Token: 0x0600300C RID: 12300 RVA: 0x00024F96 File Offset: 0x00023196
		public float Get(DamageType index)
		{
			return this.types[(int)index];
		}

		// Token: 0x0600300D RID: 12301 RVA: 0x00024FA0 File Offset: 0x000231A0
		public void Add(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) + amount);
		}

		// Token: 0x0600300E RID: 12302 RVA: 0x00024FB2 File Offset: 0x000231B2
		public void Scale(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) * amount);
		}

		// Token: 0x0600300F RID: 12303 RVA: 0x00024FC4 File Offset: 0x000231C4
		public bool Has(DamageType index)
		{
			return this.Get(index) > 0f;
		}

		// Token: 0x06003010 RID: 12304 RVA: 0x000EBE84 File Offset: 0x000EA084
		public float Total()
		{
			float num = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num2 = this.types[i];
				if (!float.IsNaN(num2) && !float.IsInfinity(num2))
				{
					num += num2;
				}
			}
			return num;
		}

		// Token: 0x06003011 RID: 12305 RVA: 0x000EBEC8 File Offset: 0x000EA0C8
		public void Add(List<DamageTypeEntry> entries)
		{
			foreach (DamageTypeEntry damageTypeEntry in entries)
			{
				this.Add(damageTypeEntry.type, damageTypeEntry.amount);
			}
		}

		// Token: 0x06003012 RID: 12306 RVA: 0x000EBF24 File Offset: 0x000EA124
		public void ScaleAll(float amount)
		{
			for (int i = 0; i < this.types.Length; i++)
			{
				this.Scale((DamageType)i, amount);
			}
		}

		// Token: 0x06003013 RID: 12307 RVA: 0x000EBF4C File Offset: 0x000EA14C
		public DamageType GetMajorityDamageType()
		{
			int result = 0;
			float num = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num2 = this.types[i];
				if (!float.IsNaN(num2) && !float.IsInfinity(num2) && num2 >= num)
				{
					result = i;
					num = num2;
				}
			}
			return (DamageType)result;
		}

		// Token: 0x06003014 RID: 12308 RVA: 0x000EBF98 File Offset: 0x000EA198
		public bool IsMeleeType()
		{
			DamageType majorityDamageType = this.GetMajorityDamageType();
			return majorityDamageType == DamageType.Blunt || majorityDamageType == DamageType.Slash || majorityDamageType == DamageType.Stab;
		}

		// Token: 0x06003015 RID: 12309 RVA: 0x000EBFC0 File Offset: 0x000EA1C0
		public bool IsBleedCausing()
		{
			DamageType majorityDamageType = this.GetMajorityDamageType();
			return majorityDamageType == DamageType.Bite || majorityDamageType == DamageType.Slash || majorityDamageType == DamageType.Stab || majorityDamageType == DamageType.Bullet || majorityDamageType == DamageType.Arrow;
		}
	}
}
