using System;
using System.Collections.Generic;

// Token: 0x020002BC RID: 700
public class ResourceDispenser : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x04000FD1 RID: 4049
	public ResourceDispenser.GatherType gatherType = ResourceDispenser.GatherType.UNSET;

	// Token: 0x04000FD2 RID: 4050
	public List<ItemAmount> containedItems;

	// Token: 0x04000FD3 RID: 4051
	public float maxDestroyFractionForFinishBonus = 0.2f;

	// Token: 0x04000FD4 RID: 4052
	public List<ItemAmount> finishBonus;

	// Token: 0x04000FD5 RID: 4053
	public float fractionRemaining = 1f;

	// Token: 0x020002BD RID: 701
	public enum GatherType
	{
		// Token: 0x04000FD7 RID: 4055
		Tree,
		// Token: 0x04000FD8 RID: 4056
		Ore,
		// Token: 0x04000FD9 RID: 4057
		Flesh,
		// Token: 0x04000FDA RID: 4058
		UNSET,
		// Token: 0x04000FDB RID: 4059
		LAST
	}

	// Token: 0x020002BE RID: 702
	[Serializable]
	public class GatherPropertyEntry
	{
		// Token: 0x04000FDC RID: 4060
		public float gatherDamage;

		// Token: 0x04000FDD RID: 4061
		public float destroyFraction;

		// Token: 0x04000FDE RID: 4062
		public float conditionLost;
	}

	// Token: 0x020002BF RID: 703
	[Serializable]
	public class GatherProperties
	{
		// Token: 0x04000FDF RID: 4063
		public ResourceDispenser.GatherPropertyEntry Tree;

		// Token: 0x04000FE0 RID: 4064
		public ResourceDispenser.GatherPropertyEntry Ore;

		// Token: 0x04000FE1 RID: 4065
		public ResourceDispenser.GatherPropertyEntry Flesh;

		// Token: 0x0600137A RID: 4986 RVA: 0x0007BDF4 File Offset: 0x00079FF4
		public float GetProficiency()
		{
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				float num2 = fromIndex.gatherDamage * fromIndex.destroyFraction;
				if (num2 > 0f)
				{
					num += fromIndex.gatherDamage / num2;
				}
			}
			return num;
		}

		// Token: 0x0600137B RID: 4987 RVA: 0x0007BE40 File Offset: 0x0007A040
		public bool Any()
		{
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				if (fromIndex.gatherDamage > 0f || fromIndex.conditionLost > 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600137C RID: 4988 RVA: 0x00010832 File Offset: 0x0000EA32
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(int index)
		{
			return this.GetFromIndex((ResourceDispenser.GatherType)index);
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x0001083B File Offset: 0x0000EA3B
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(ResourceDispenser.GatherType index)
		{
			switch (index)
			{
			case ResourceDispenser.GatherType.Tree:
				return this.Tree;
			case ResourceDispenser.GatherType.Ore:
				return this.Ore;
			case ResourceDispenser.GatherType.Flesh:
				return this.Flesh;
			default:
				return null;
			}
		}
	}
}
