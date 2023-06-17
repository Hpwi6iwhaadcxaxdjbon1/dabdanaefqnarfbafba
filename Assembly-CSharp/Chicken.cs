using System;

// Token: 0x02000126 RID: 294
public class Chicken : BaseAnimalNPC
{
	// Token: 0x04000865 RID: 2149
	[ServerVar(Help = "Population active on the server, per square km")]
	public static float Population = 3f;

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x06000BDD RID: 3037 RVA: 0x0000B376 File Offset: 0x00009576
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x0000B3A8 File Offset: 0x000095A8
	public override string Categorize()
	{
		return "Chicken";
	}
}
