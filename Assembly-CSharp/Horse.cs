using System;

// Token: 0x02000127 RID: 295
public class Horse : BaseAnimalNPC
{
	// Token: 0x04000866 RID: 2150
	[ServerVar(Help = "Population active on the server, per square km")]
	public static float Population = 2f;

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x0000B376 File Offset: 0x00009576
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0000B3BB File Offset: 0x000095BB
	public override string Categorize()
	{
		return "Horse";
	}
}
