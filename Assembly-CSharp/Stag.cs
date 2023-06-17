using System;

// Token: 0x02000135 RID: 309
public class Stag : BaseAnimalNPC
{
	// Token: 0x040008C1 RID: 2241
	[ServerVar(Help = "Population active on the server, per square km")]
	public static float Population = 3f;

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000BF5 RID: 3061 RVA: 0x0000B376 File Offset: 0x00009576
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0000B460 File Offset: 0x00009660
	public override string Categorize()
	{
		return "Stag";
	}
}
