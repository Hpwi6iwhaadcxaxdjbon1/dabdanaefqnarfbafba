using System;

// Token: 0x02000125 RID: 293
public class Boar : BaseAnimalNPC
{
	// Token: 0x04000864 RID: 2148
	[ServerVar(Help = "Population active on the server, per square km")]
	public static float Population = 5f;

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x0000B376 File Offset: 0x00009576
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x0000B395 File Offset: 0x00009595
	public override string Categorize()
	{
		return "Boar";
	}
}
