using System;
using Rust;

// Token: 0x020002D4 RID: 724
public abstract class BaseMetabolism<T> : EntityComponent<T> where T : BaseCombatEntity
{
	// Token: 0x04001010 RID: 4112
	protected T owner;

	// Token: 0x04001011 RID: 4113
	public MetabolismAttribute calories = new MetabolismAttribute();

	// Token: 0x04001012 RID: 4114
	public MetabolismAttribute hydration = new MetabolismAttribute();

	// Token: 0x04001013 RID: 4115
	public MetabolismAttribute heartrate = new MetabolismAttribute();

	// Token: 0x060013B5 RID: 5045 RVA: 0x00010B60 File Offset: 0x0000ED60
	public virtual void Reset()
	{
		this.calories.Reset();
		this.hydration.Reset();
		this.heartrate.Reset();
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x00010B83 File Offset: 0x0000ED83
	protected virtual void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x00010B99 File Offset: 0x0000ED99
	public bool ShouldDie()
	{
		return this.owner && this.owner.Health() <= 0f;
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x00010BC9 File Offset: 0x0000EDC9
	public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
		case MetabolismAttribute.Type.Calories:
			return this.calories;
		case MetabolismAttribute.Type.Hydration:
			return this.hydration;
		case MetabolismAttribute.Type.Heartrate:
			return this.heartrate;
		default:
			return null;
		}
	}
}
