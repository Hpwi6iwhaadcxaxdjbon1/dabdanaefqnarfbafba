using System;

// Token: 0x02000124 RID: 292
public class Bear : BaseAnimalNPC
{
	// Token: 0x04000863 RID: 2147
	[ServerVar(Help = "Population active on the server, per square km")]
	public static float Population = 2f;

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x0000B376 File Offset: 0x00009576
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x0000B37A File Offset: 0x0000957A
	public override string Categorize()
	{
		return "Bear";
	}
}
