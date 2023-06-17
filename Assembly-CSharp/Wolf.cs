using System;

// Token: 0x02000136 RID: 310
public class Wolf : BaseAnimalNPC
{
	// Token: 0x040008C2 RID: 2242
	[ServerVar(Help = "Population active on the server, per square km")]
	public static float Population = 2f;

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000BF9 RID: 3065 RVA: 0x0000B376 File Offset: 0x00009576
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x0000B473 File Offset: 0x00009673
	public override string Categorize()
	{
		return "Wolf";
	}
}
