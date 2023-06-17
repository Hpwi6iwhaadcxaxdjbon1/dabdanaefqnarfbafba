using System;

// Token: 0x02000738 RID: 1848
[Serializable]
public class FloatConditions
{
	// Token: 0x040023E0 RID: 9184
	public FloatConditions.Condition[] conditions;

	// Token: 0x06002849 RID: 10313 RVA: 0x000CF608 File Offset: 0x000CD808
	public bool AllTrue(float val)
	{
		foreach (FloatConditions.Condition condition in this.conditions)
		{
			if (!condition.Test(val))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x02000739 RID: 1849
	[Serializable]
	public struct Condition
	{
		// Token: 0x040023E1 RID: 9185
		public FloatConditions.Condition.Types type;

		// Token: 0x040023E2 RID: 9186
		public float value;

		// Token: 0x0600284B RID: 10315 RVA: 0x000CF640 File Offset: 0x000CD840
		public bool Test(float val)
		{
			switch (this.type)
			{
			case FloatConditions.Condition.Types.Equal:
				return val == this.value;
			case FloatConditions.Condition.Types.NotEqual:
				return val != this.value;
			case FloatConditions.Condition.Types.Higher:
				return val > this.value;
			case FloatConditions.Condition.Types.Lower:
				return val < this.value;
			default:
				return false;
			}
		}

		// Token: 0x0200073A RID: 1850
		public enum Types
		{
			// Token: 0x040023E4 RID: 9188
			Equal,
			// Token: 0x040023E5 RID: 9189
			NotEqual,
			// Token: 0x040023E6 RID: 9190
			Higher,
			// Token: 0x040023E7 RID: 9191
			Lower
		}
	}
}
