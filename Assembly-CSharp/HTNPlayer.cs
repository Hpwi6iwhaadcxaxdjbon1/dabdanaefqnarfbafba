using System;
using Rust.Ai.HTN;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class HTNPlayer : BasePlayer
{
	// Token: 0x040008DB RID: 2267
	[Header("Hierarchical Task Network")]
	public HTNDomain _aiDomain;

	// Token: 0x040008DC RID: 2268
	[Header("Ai Definition")]
	public BaseNpcDefinition _aiDefinition;

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000C37 RID: 3127 RVA: 0x0000B730 File Offset: 0x00009930
	public BaseNpcDefinition AiDefinition
	{
		get
		{
			return this._aiDefinition;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000C38 RID: 3128 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x0000B738 File Offset: 0x00009938
	public override float StartHealth()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Vitals.HP;
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0000B738 File Offset: 0x00009938
	public override float StartMaxHealth()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Vitals.HP;
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0000B738 File Offset: 0x00009938
	public override float MaxHealth()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Vitals.HP;
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x0000B754 File Offset: 0x00009954
	public override float MaxVelocity()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Movement.RunSpeed;
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000C3D RID: 3133 RVA: 0x00004B3B File Offset: 0x00002D3B
	public BaseEntity Body
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000C3E RID: 3134 RVA: 0x000079E3 File Offset: 0x00005BE3
	public Vector3 BodyPosition
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000C3F RID: 3135 RVA: 0x0005B7C0 File Offset: 0x000599C0
	public Vector3 EyePosition
	{
		get
		{
			BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				return this.BodyPosition + parentEntity.transform.up * PlayerEyes.EyeOffset.y;
			}
			return this.eyes.position;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000C40 RID: 3136 RVA: 0x0000B770 File Offset: 0x00009970
	public Quaternion EyeRotation
	{
		get
		{
			return this.eyes.rotation;
		}
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00004B81 File Offset: 0x00002D81
	public override Quaternion GetNetworkRotation()
	{
		if (base.isClient)
		{
			return this.eyes.bodyRotation;
		}
		return Quaternion.identity;
	}
}
