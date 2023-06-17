using System;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class FootstepEffects : BaseFootstepEffect
{
	// Token: 0x04000EEA RID: 3818
	public Transform leftFoot;

	// Token: 0x04000EEB RID: 3819
	public Transform rightFoot;

	// Token: 0x04000EEC RID: 3820
	public string footstepEffectName = "footstep/barefoot";

	// Token: 0x04000EED RID: 3821
	public string jumpStartEffectName = "jump-start/barefoot";

	// Token: 0x04000EEE RID: 3822
	public string jumpLandEffectName = "jump-land/barefoot";

	// Token: 0x04000EEF RID: 3823
	private bool lastWasLeft;

	// Token: 0x04000EF0 RID: 3824
	private bool lastWasStopped;

	// Token: 0x04000EF1 RID: 3825
	private BasePlayer player;

	// Token: 0x04000EF2 RID: 3826
	private bool isOnGround;

	// Token: 0x0600124E RID: 4686 RVA: 0x0000FC83 File Offset: 0x0000DE83
	private void OnEnable()
	{
		this.player = (base.gameObject.ToBaseEntity() as BasePlayer);
		if (this.player != null)
		{
			this.isOnGround = this.player.IsOnGround();
		}
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x000782C4 File Offset: 0x000764C4
	private void Update()
	{
		if (this.player != null)
		{
			if (!this.isOnGround && this.player.IsOnGround())
			{
				this.DoLandingEffect();
			}
			this.isOnGround = this.player.IsOnGround();
			if (this.player.IsLocalPlayer() && this.player.movement && !this.lastWasStopped)
			{
				this.lastWasStopped = (this.player.movement.TargetMovement.magnitude < 0.1f);
			}
		}
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x00078358 File Offset: 0x00076558
	private void LeftFoot(float fVolume)
	{
		if (this.lastWasLeft && !this.lastWasStopped)
		{
			return;
		}
		this.lastWasLeft = true;
		this.lastWasStopped = false;
		if (fVolume <= -1f)
		{
			this.Footstep(this.leftFoot.position, true, true);
			return;
		}
		this.Footstep(this.leftFoot.position, true, false);
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x000783B4 File Offset: 0x000765B4
	private void RightFoot(float fVolume)
	{
		if (!this.lastWasLeft && !this.lastWasStopped)
		{
			return;
		}
		this.lastWasLeft = false;
		this.lastWasStopped = false;
		if (fVolume <= -1f)
		{
			this.Footstep(this.rightFoot.position, false, true);
			return;
		}
		this.Footstep(this.rightFoot.position, false, false);
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x00078410 File Offset: 0x00076610
	private void Footstep(Vector3 vFootPos, bool left, bool bIgnoreDistanceCheck = false)
	{
		if (this.player == null)
		{
			return;
		}
		if (this.player.IsLocalPlayer() && this.player.movement && this.player.movement.TargetMovement.magnitude < 1.25f)
		{
			return;
		}
		if (this.player.isMounted)
		{
			return;
		}
		BaseFootstepEffect.GroundInfo groundInfo = base.GetGroundInfo(vFootPos, base.transform.forward, bIgnoreDistanceCheck);
		string effectType = this.footstepEffectName;
		if (this.player.IsDucked())
		{
			effectType = "footstep/crouch";
		}
		GameObject gameObject = groundInfo.SpawnEffect(effectType);
		if (gameObject != null)
		{
			this.SetupPlayerFootstep(this.player, gameObject, left);
		}
		if (this.player.playerModel != null)
		{
			string effectType2 = this.player.playerModel.HasPart("skin_feet") ? "footstep/bare" : "footstep/boot";
			groundInfo.SpawnDecal(effectType2);
		}
		groundInfo.SpawnDisplacement("player");
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x00078518 File Offset: 0x00076718
	private void DoLandingEffect()
	{
		if (this.player == null)
		{
			return;
		}
		GameObject gameObject = base.GetGroundInfo(base.transform.position, base.transform.forward, true).SpawnEffect(this.jumpLandEffectName);
		if (gameObject != null && this.player.IsLocalPlayer())
		{
			SoundPlayer component = gameObject.GetComponent<SoundPlayer>();
			if (component)
			{
				component.MakeFirstPerson();
			}
		}
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x0007858C File Offset: 0x0007678C
	private void DoJumpStartEffect()
	{
		if (this.player == null)
		{
			return;
		}
		GameObject gameObject = base.GetGroundInfo(base.transform.position, base.transform.forward, true).SpawnEffect(this.jumpStartEffectName);
		if (gameObject != null && this.player.IsLocalPlayer())
		{
			SoundPlayer component = gameObject.GetComponent<SoundPlayer>();
			if (component)
			{
				component.MakeFirstPerson();
			}
		}
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x00078600 File Offset: 0x00076800
	private void SetupPlayerFootstep(BasePlayer player, GameObject effect, bool left)
	{
		FootstepSound.Hardness hardness = FootstepSound.Hardness.Medium;
		FootstepSound component = effect.GetComponent<FootstepSound>();
		float sqrMagnitude = (base.transform.position - this.lastFootstepPos).sqrMagnitude;
		if (player.IsDucked())
		{
			hardness = FootstepSound.Hardness.Light;
		}
		else if (sqrMagnitude > 0.19f)
		{
			hardness = FootstepSound.Hardness.Hard;
		}
		if (component)
		{
			component.PlayFootstep(hardness, player.IsLocalPlayer(), player.gameObject, left);
		}
	}
}
