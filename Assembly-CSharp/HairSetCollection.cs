using System;
using UnityEngine;

// Token: 0x020005D8 RID: 1496
[CreateAssetMenu(menuName = "Rust/Hair Set Collection")]
public class HairSetCollection : ScriptableObject
{
	// Token: 0x04001E10 RID: 7696
	public HairSetCollection.HairSetEntry[] Head;

	// Token: 0x04001E11 RID: 7697
	public HairSetCollection.HairSetEntry[] Eyebrow;

	// Token: 0x04001E12 RID: 7698
	public HairSetCollection.HairSetEntry[] Facial;

	// Token: 0x04001E13 RID: 7699
	public HairSetCollection.HairSetEntry[] Armpit;

	// Token: 0x04001E14 RID: 7700
	public HairSetCollection.HairSetEntry[] Pubic;

	// Token: 0x06002201 RID: 8705 RVA: 0x000B78B0 File Offset: 0x000B5AB0
	public HairSetCollection.HairSetEntry[] GetListByType(HairType hairType)
	{
		switch (hairType)
		{
		case HairType.Head:
			return this.Head;
		case HairType.Eyebrow:
			return this.Eyebrow;
		case HairType.Facial:
			return this.Facial;
		case HairType.Armpit:
			return this.Armpit;
		case HairType.Pubic:
			return this.Pubic;
		default:
			return null;
		}
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x0001AFF5 File Offset: 0x000191F5
	public int GetIndex(HairSetCollection.HairSetEntry[] list, float typeNum)
	{
		return Mathf.Clamp(Mathf.FloorToInt(typeNum * (float)list.Length), 0, list.Length - 1);
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x000B7900 File Offset: 0x000B5B00
	public int GetIndex(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return this.GetIndex(listByType, typeNum);
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x000B7920 File Offset: 0x000B5B20
	public HairSetCollection.HairSetEntry Get(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return listByType[this.GetIndex(listByType, typeNum)];
	}

	// Token: 0x020005D9 RID: 1497
	[Serializable]
	public struct HairSetEntry
	{
		// Token: 0x04001E15 RID: 7701
		public HairSet HairSet;

		// Token: 0x04001E16 RID: 7702
		public HairDyeCollection HairDyeCollection;
	}
}
