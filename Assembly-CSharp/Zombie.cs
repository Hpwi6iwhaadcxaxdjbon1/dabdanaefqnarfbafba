using System;

// Token: 0x02000137 RID: 311
public class Zombie : BaseAnimalNPC
{
	// Token: 0x040008C3 RID: 2243
	[ServerVar(Help = "Population active on the server, per square km")]
	public static float Population;

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000BFD RID: 3069 RVA: 0x0000B376 File Offset: 0x00009576
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x0000B486 File Offset: 0x00009686
	public override string Categorize()
	{
		return "Zombie";
	}
}
