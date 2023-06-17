using System;
using ConVar;
using UnityEngine;

// Token: 0x0200029B RID: 667
public class AttackEntity : HeldEntity
{
	// Token: 0x04000F5F RID: 3935
	[Header("Attack Entity")]
	public float deployDelay = 1f;

	// Token: 0x04000F60 RID: 3936
	public float repeatDelay = 0.5f;

	// Token: 0x04000F61 RID: 3937
	public float animationDelay;

	// Token: 0x04000F62 RID: 3938
	[Header("NPCUsage")]
	public float effectiveRange = 1f;

	// Token: 0x04000F63 RID: 3939
	public float npcDamageScale = 1f;

	// Token: 0x04000F64 RID: 3940
	public float attackLengthMin = -1f;

	// Token: 0x04000F65 RID: 3941
	public float attackLengthMax = -1f;

	// Token: 0x04000F66 RID: 3942
	public float attackSpacing;

	// Token: 0x04000F67 RID: 3943
	public float aiAimSwayOffset;

	// Token: 0x04000F68 RID: 3944
	public float aiAimCone;

	// Token: 0x04000F69 RID: 3945
	public bool aiOnlyInRange;

	// Token: 0x04000F6A RID: 3946
	public NPCPlayerApex.WeaponTypeEnum effectiveRangeType = NPCPlayerApex.WeaponTypeEnum.MediumRange;

	// Token: 0x04000F6B RID: 3947
	public float CloseRangeAddition;

	// Token: 0x04000F6C RID: 3948
	public float MediumRangeAddition;

	// Token: 0x04000F6D RID: 3949
	public float LongRangeAddition;

	// Token: 0x04000F6E RID: 3950
	public bool CanUseAtMediumRange = true;

	// Token: 0x04000F6F RID: 3951
	public bool CanUseAtLongRange = true;

	// Token: 0x04000F70 RID: 3952
	public SoundDefinition[] reloadSounds;

	// Token: 0x04000F71 RID: 3953
	public SoundDefinition thirdPersonMeleeSound;

	// Token: 0x04000F72 RID: 3954
	private float nextAttackTime = float.NegativeInfinity;

	// Token: 0x04000F73 RID: 3955
	private float lastTickTime;

	// Token: 0x04000F74 RID: 3956
	private float nextTickTime;

	// Token: 0x04000F75 RID: 3957
	private float timeSinceDeploy = 60f;

	// Token: 0x04000F76 RID: 3958
	public static Vector3 reductionSpeedScalars = new Vector3(0.3f, 0.3f, 1f);

	// Token: 0x04000F77 RID: 3959
	[Header("Recoil Compensation")]
	public float recoilCompDelayOverride;

	// Token: 0x04000F78 RID: 3960
	public bool wantsRecoilComp;

	// Token: 0x04000F79 RID: 3961
	private float lastRecoilCompTime;

	// Token: 0x04000F7A RID: 3962
	private Vector3 startAimingDirection;

	// Token: 0x04000F7B RID: 3963
	private bool wasDoingRecoilComp;

	// Token: 0x04000F7C RID: 3964
	private Vector3 reductionSpeed = Vector3.one;

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x060012C5 RID: 4805 RVA: 0x0001012B File Offset: 0x0000E32B
	public float NextAttackTime
	{
		get
		{
			return this.nextAttackTime;
		}
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void GetAttackStats(HitInfo info)
	{
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x00010133 File Offset: 0x0000E333
	protected void StartAttackCooldown(float cooldown)
	{
		this.nextAttackTime = this.CalculateCooldownTime(this.nextAttackTime, cooldown, true);
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x00010149 File Offset: 0x0000E349
	protected void ResetAttackCooldown()
	{
		this.nextAttackTime = float.NegativeInfinity;
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x00010156 File Offset: 0x0000E356
	public bool HasAttackCooldown()
	{
		return UnityEngine.Time.time < this.nextAttackTime;
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x00010165 File Offset: 0x0000E365
	protected float GetAttackCooldown()
	{
		return Mathf.Max(this.nextAttackTime - UnityEngine.Time.time, 0f);
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x0001017D File Offset: 0x0000E37D
	protected float GetAttackIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextAttackTime, 0f);
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x00079BD4 File Offset: 0x00077DD4
	protected float CalculateCooldownTime(float nextTime, float cooldown, bool catchup)
	{
		float num = UnityEngine.Time.time;
		float num2 = 0f;
		if (base.isClient && catchup && this.lastTickTime < nextTime)
		{
			num = nextTime;
		}
		if (nextTime < 0f)
		{
			nextTime = Mathf.Max(0f, num + cooldown - num2);
		}
		else if (num - nextTime <= num2)
		{
			nextTime = Mathf.Min(nextTime + cooldown, num + cooldown);
		}
		else
		{
			nextTime = Mathf.Max(nextTime + cooldown, num + cooldown - num2);
		}
		return nextTime;
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x00010195 File Offset: 0x0000E395
	public virtual bool IsFullyDeployed()
	{
		return this.timeSinceDeploy > this.deployDelay;
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x000101A5 File Offset: 0x0000E3A5
	protected void ProcessInputTime()
	{
		this.lastTickTime = this.nextTickTime;
		this.nextTickTime = UnityEngine.Time.time;
		this.timeSinceDeploy += UnityEngine.Time.deltaTime;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x000101D0 File Offset: 0x0000E3D0
	public override void OnInput()
	{
		this.ProcessInputTime();
		base.OnInput();
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x000101DE File Offset: 0x0000E3DE
	public override void OnDeploy()
	{
		this.timeSinceDeploy = 0f;
		base.OnDeploy();
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x000101F1 File Offset: 0x0000E3F1
	public bool RecoilCompReady()
	{
		return this._punches.Count == 0 && UnityEngine.Time.time >= this.lastPunchTime + ((this.recoilCompDelayOverride != 0f) ? this.recoilCompDelayOverride : this.repeatDelay);
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x00079C44 File Offset: 0x00077E44
	public override void AddPunch(Vector3 amount, float duration)
	{
		if (this.RecoilCompReady())
		{
			this.punchAdded = Vector3.zero;
		}
		base.AddPunch(amount, duration);
		if (this.punchAdded == Vector3.zero || this._punches.Count == 0)
		{
			if (!base.GetOwnerPlayer())
			{
				return;
			}
			this.startAimingDirection = base.GetOwnerPlayer().input.ClientLookVars();
			this.wasDoingRecoilComp = false;
		}
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x00079CB8 File Offset: 0x00077EB8
	public override void DoRecoilCompensation()
	{
		if (!this.wantsRecoilComp || !Player.recoilcomp)
		{
			return;
		}
		base.DoRecoilCompensation();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		float num = UnityEngine.Time.time - this.lastRecoilCompTime;
		this.lastRecoilCompTime = UnityEngine.Time.time;
		if (num <= 0f)
		{
			return;
		}
		if (this.RecoilCompReady())
		{
			if (this.punchAdded != Vector3.zero)
			{
				Vector3 vector = ownerPlayer.input.ClientLookVars();
				if (!this.wasDoingRecoilComp)
				{
					Vector3 vector2 = vector - this.startAimingDirection;
					if (Global.developer > 0)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Recoil Compensation Starting, startDir : ",
							this.startAimingDirection,
							": curlook :",
							vector,
							" : punchx : ",
							this.punchAdded.x,
							" : delta :",
							vector2
						}));
					}
					if (vector2.x > this.punchAdded.x && vector2.x < 0f)
					{
						this.punchAdded.x = vector2.x;
					}
					else if (vector2.x >= 0f)
					{
						this.punchAdded.x = 0f;
					}
					for (int i = 0; i < 3; i++)
					{
						this.reductionSpeed[i] = this.punchAdded[i] * (1f / AttackEntity.reductionSpeedScalars[i]);
					}
				}
				this.wasDoingRecoilComp = true;
				for (int j = 0; j < 3; j++)
				{
					float num2 = this.punchAdded[j];
					float num3 = Mathf.Abs(this.reductionSpeed[j]) * num;
					if (Mathf.Abs(num3) < 0.2f)
					{
						num3 = 0.2f * Mathf.Sign(num3);
					}
					float num4 = (Mathf.Abs(num2) < num3) ? num2 : (num3 * Mathf.Sign(num2));
					ref Vector3 ptr = ref vector;
					int index = j;
					ptr[index] -= num4;
					ptr = ref this.punchAdded;
					index = j;
					ptr[index] -= num4;
				}
				ownerPlayer.input.SetViewVars(vector);
				return;
			}
			this.startAimingDirection = Vector3.zero;
			this.wasDoingRecoilComp = false;
		}
	}
}
