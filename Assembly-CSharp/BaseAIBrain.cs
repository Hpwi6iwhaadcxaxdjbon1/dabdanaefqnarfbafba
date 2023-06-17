using System;
using UnityEngine;

// Token: 0x02000297 RID: 663
public class BaseAIBrain<T> : EntityComponent<T> where T : BaseEntity
{
	// Token: 0x04000F57 RID: 3927
	public BaseAIBrain<T>.BasicAIState[] AIStates;

	// Token: 0x04000F58 RID: 3928
	public const int AIStateIndex_UNSET = 0;

	// Token: 0x04000F59 RID: 3929
	public int _currentState;

	// Token: 0x04000F5A RID: 3930
	public Vector3 mainInterestPoint;

	// Token: 0x060012B4 RID: 4788 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool ShouldThink()
	{
		return true;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void DoThink()
	{
	}

	// Token: 0x02000298 RID: 664
	public class BasicAIState
	{
	}
}
