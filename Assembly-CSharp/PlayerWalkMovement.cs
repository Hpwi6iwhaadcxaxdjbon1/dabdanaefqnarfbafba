using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200031E RID: 798
public class PlayerWalkMovement : BaseMovement
{
	// Token: 0x0400121A RID: 4634
	public const float WaterLevelHead = 0.75f;

	// Token: 0x0400121B RID: 4635
	public const float WaterLevelNeck = 0.65f;

	// Token: 0x0400121C RID: 4636
	public PhysicMaterial zeroFrictionMaterial;

	// Token: 0x0400121D RID: 4637
	public PhysicMaterial highFrictionMaterial;

	// Token: 0x0400121E RID: 4638
	public float capsuleHeight = 1f;

	// Token: 0x0400121F RID: 4639
	public float capsuleCenter = 1f;

	// Token: 0x04001220 RID: 4640
	public float capsuleHeightDucked = 1f;

	// Token: 0x04001221 RID: 4641
	public float capsuleCenterDucked = 1f;

	// Token: 0x04001222 RID: 4642
	public float gravityTestRadius = 0.2f;

	// Token: 0x04001223 RID: 4643
	public float gravityMultiplier = 2.5f;

	// Token: 0x04001224 RID: 4644
	public float gravityMultiplierSwimming = 0.1f;

	// Token: 0x04001225 RID: 4645
	public float maxAngleWalking = 50f;

	// Token: 0x04001226 RID: 4646
	public float maxAngleClimbing = 60f;

	// Token: 0x04001227 RID: 4647
	public float maxAngleSliding = 90f;

	// Token: 0x04001228 RID: 4648
	private Rigidbody body;

	// Token: 0x04001229 RID: 4649
	private CapsuleCollider capsule;

	// Token: 0x0400122A RID: 4650
	private TriggerLadder ladder;

	// Token: 0x0400122B RID: 4651
	private float maxVelocity = 50f;

	// Token: 0x0400122C RID: 4652
	private float groundAngle;

	// Token: 0x0400122D RID: 4653
	private float groundAngleNew;

	// Token: 0x0400122E RID: 4654
	private float groundTime;

	// Token: 0x0400122F RID: 4655
	private float jumpTime;

	// Token: 0x04001230 RID: 4656
	private float landTime;

	// Token: 0x04001231 RID: 4657
	private Vector3 previousPosition = Vector3.zero;

	// Token: 0x04001232 RID: 4658
	private Vector3 previousVelocity = Vector3.zero;

	// Token: 0x04001233 RID: 4659
	private Vector3 groundNormal = Vector3.up;

	// Token: 0x04001234 RID: 4660
	private Vector3 groundNormalNew = Vector3.up;

	// Token: 0x04001235 RID: 4661
	private float nextSprintTime = float.NegativeInfinity;

	// Token: 0x04001236 RID: 4662
	private float lastSprintTime = float.NegativeInfinity;

	// Token: 0x04001237 RID: 4663
	private bool sprintForced;

	// Token: 0x04001238 RID: 4664
	private BaseEntity.MovementModify modify;

	// Token: 0x04001239 RID: 4665
	private bool grounded;

	// Token: 0x0400123A RID: 4666
	private bool wasGrounded;

	// Token: 0x0400123B RID: 4667
	private bool climbing;

	// Token: 0x0400123C RID: 4668
	private bool wasClimbing;

	// Token: 0x0400123D RID: 4669
	private bool sliding;

	// Token: 0x0400123E RID: 4670
	private bool wasSliding;

	// Token: 0x0400123F RID: 4671
	private bool swimming;

	// Token: 0x04001240 RID: 4672
	private bool wasSwimming;

	// Token: 0x04001241 RID: 4673
	private bool jumping;

	// Token: 0x04001242 RID: 4674
	private bool wasJumping;

	// Token: 0x04001243 RID: 4675
	private bool falling;

	// Token: 0x04001244 RID: 4676
	private bool wasFalling;

	// Token: 0x04001245 RID: 4677
	private bool flying;

	// Token: 0x04001246 RID: 4678
	private bool wasFlying;

	// Token: 0x06001557 RID: 5463 RVA: 0x0001219E File Offset: 0x0001039E
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody>();
		this.capsule = base.GetComponent<CapsuleCollider>();
	}

	// Token: 0x06001558 RID: 5464 RVA: 0x000121B8 File Offset: 0x000103B8
	public override Collider GetCollider()
	{
		return this.capsule;
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x00082E80 File Offset: 0x00081080
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (TerrainMeta.Collision != null)
		{
			TerrainMeta.Collision.Reset(this.capsule);
		}
		if (WaterSystem.Collision != null)
		{
			WaterSystem.Collision.Reset(this.capsule);
		}
		if (WaterSystem.Instance != null)
		{
			WaterSystem.Instance.ResetVisibility();
		}
		this.previousVelocity = (this.body.velocity = Vector3.zero);
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x00082F00 File Offset: 0x00081100
	public override void Init(BasePlayer player)
	{
		base.Init(player);
		base.transform.rotation = Quaternion.identity;
		this.previousPosition = (base.transform.localPosition = player.transform.localPosition);
		this.previousVelocity = (this.body.velocity = Vector3.zero);
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x00082F5C File Offset: 0x0008115C
	protected void OnCollisionEnter(Collision collision)
	{
		using (TimeWarning.New("PlayerWalkMovement.OnCollisionEnter", 0.1f))
		{
			this.GroundCheck(collision);
		}
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x00082F9C File Offset: 0x0008119C
	protected void OnCollisionStay(Collision collision)
	{
		using (TimeWarning.New("PlayerWalkMovement.OnCollisionStay", 0.1f))
		{
			this.GroundCheck(collision);
		}
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x00082FDC File Offset: 0x000811DC
	private void GroundCheck(Collision collision)
	{
		float num = this.capsule.bounds.min.y + this.capsule.radius;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			if (contactPoint.point.y <= num)
			{
				Vector3 normal = contactPoint.normal;
				float num2 = Vector3.Angle(normal, Vector3.up);
				if (num2 < this.groundAngleNew)
				{
					this.groundAngleNew = num2;
					this.groundNormalNew = normal;
				}
			}
		}
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000121C0 File Offset: 0x000103C0
	public override void TeleportTo(Vector3 position, BasePlayer player)
	{
		base.TeleportTo(position, player);
		player.ForceUpdateTriggers(true, true, true);
		this.UpdateCurrentLadder(player.input.state, player.modelState, true);
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x0008306C File Offset: 0x0008126C
	public void UpdateCurrentLadder(InputState input, ModelState modelState, bool force = false)
	{
		TriggerLadder x = this.Owner.FindTrigger<TriggerLadder>();
		if (x == null)
		{
			this.ladder = null;
			return;
		}
		if (!(this.ladder == null))
		{
			x = this.ladder;
			return;
		}
		if (input.IsDown(BUTTON.JUMP))
		{
			return;
		}
		if (force || input.IsDown(BUTTON.FORWARD) || input.IsDown(BUTTON.BACKWARD) || input.IsDown(BUTTON.LEFT) || input.IsDown(BUTTON.RIGHT))
		{
			this.ladder = x;
			return;
		}
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x000830E8 File Offset: 0x000812E8
	public override void ClientInput(InputState input, ModelState modelState)
	{
		this.wasFlying = this.flying;
		this.flying = false;
		if (this.Owner.IsAdmin || this.Owner.IsDeveloper)
		{
			this.flying = this.adminCheat;
		}
		this.UpdateCurrentLadder(input, modelState, false);
		this.modify = this.Owner.GetMovementModify();
		modelState.jumped = false;
		modelState.onground = false;
		modelState.onLadder = false;
		modelState.flying = false;
		modelState.mounted = false;
		if (this.Owner.isMounted)
		{
			this.HandleDucking(modelState, false);
			this.Movement_Mounted(input, modelState);
			return;
		}
		if (this.flying)
		{
			this.Movement_Noclip(input, modelState);
			return;
		}
		if (this.ladder)
		{
			this.Movement_Ladder(input, modelState);
		}
		else if (this.swimming)
		{
			this.Movement_Water(input, modelState);
		}
		else
		{
			this.Movement_Walking(input, modelState);
		}
		modelState.sprinting = base.IsRunning;
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000831D8 File Offset: 0x000813D8
	private void Movement_Water(InputState input, ModelState modelState)
	{
		base.TargetMovement = Vector3.zero;
		if (!ProgressBarUI.IsOpen())
		{
			if (input.IsDown(BUTTON.FORWARD))
			{
				base.TargetMovement += this.Owner.eyes.BodyForward();
			}
			if (input.IsDown(BUTTON.BACKWARD))
			{
				base.TargetMovement -= this.Owner.eyes.BodyForward();
			}
			if (input.IsDown(BUTTON.LEFT))
			{
				base.TargetMovement -= this.Owner.eyes.BodyRight();
			}
			if (input.IsDown(BUTTON.RIGHT))
			{
				base.TargetMovement += this.Owner.eyes.BodyRight();
			}
			if (input.IsDown(BUTTON.JUMP))
			{
				base.TargetMovement += Vector3.up;
			}
		}
		this.HandleGrounded(modelState, false);
		bool wantsRun = input.IsDown(BUTTON.SPRINT) && this.CanSprint();
		bool wantsDuck = false;
		this.HandleRunning(modelState, wantsRun);
		this.HandleDucking(modelState, wantsDuck);
		float num = this.Owner.GetMinSpeed();
		if (!base.IsRunning)
		{
			num *= 0.5f;
		}
		if (this.flying)
		{
			num = 100f;
		}
		float a = Mathf.InverseLerp(0.65f, 1f, modelState.waterLevel);
		base.TargetMovement = new Vector3(base.TargetMovement.x, Mathf.Min(a, base.TargetMovement.y), base.TargetMovement.z);
		if (base.TargetMovement != Vector3.zero)
		{
			base.TargetMovement = base.TargetMovement.normalized * num;
		}
		if (input.IsDown(BUTTON.JUMP) && UnityEngine.Time.time - this.jumpTime >= 0.25f)
		{
			Vector3 vector = Quaternion.Euler(0f, this.Owner.eyes.rotation.eulerAngles.y, 0f) * Vector3.forward;
			Vector3 vector2 = this.Owner.eyes.position + new Vector3(0f, 0.25f, 0f);
			RaycastHit raycastHit;
			RaycastHit raycastHit2;
			if (!WaterLevel.Test(vector2) && !Physics.Raycast(vector2, vector, ref raycastHit, this.capsule.radius * 1.1f, 1537286401) && Physics.Raycast(this.Owner.eyes.position + new Vector3(0f, 0.25f, 0f) + vector * this.capsule.radius * 1.1f, Vector3.down, ref raycastHit2, 1f, 1537286401))
			{
				this.body.velocity += Vector3.Lerp(Vector3.up * 11f, Vector3.zero, this.modify.drag);
				this.jumpTime = UnityEngine.Time.time;
			}
		}
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000834F4 File Offset: 0x000816F4
	private void Movement_Mounted(InputState input, ModelState modelState)
	{
		BaseMountable mounted = this.Owner.GetMounted();
		this.HandleGrounded(modelState, true);
		if (mounted)
		{
			mounted.DoPlayerMovement(this.Owner, input, modelState);
		}
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x0008352C File Offset: 0x0008172C
	private void Movement_Ladder(InputState input, ModelState modelState)
	{
		base.TargetMovement = Vector3.zero;
		Vector3 b = Vector3.zero;
		if (Vector3.Dot(this.Owner.eyes.BodyForward(), Vector3.up) > -0.5f)
		{
			b = Vector3.up;
		}
		if (!ProgressBarUI.IsOpen())
		{
			if (input.IsDown(BUTTON.FORWARD))
			{
				base.TargetMovement += this.Owner.eyes.BodyForward() + b;
			}
			if (input.IsDown(BUTTON.BACKWARD))
			{
				base.TargetMovement -= this.Owner.eyes.BodyForward();
			}
			if (input.IsDown(BUTTON.LEFT))
			{
				base.TargetMovement -= this.Owner.eyes.BodyRight();
			}
			if (input.IsDown(BUTTON.RIGHT))
			{
				base.TargetMovement += this.Owner.eyes.BodyRight();
			}
		}
		this.HandleGrounded(modelState, true);
		modelState.onLadder = true;
		bool wantsRun = input.IsDown(BUTTON.SPRINT) && this.CanSprint();
		bool wantsDuck = false;
		bool wantsJump = input.WasJustPressed(BUTTON.JUMP);
		this.HandleRunning(modelState, wantsRun);
		this.HandleDucking(modelState, wantsDuck);
		float num = this.Owner.GetMinSpeed();
		if (!base.IsRunning)
		{
			num *= 0.5f;
		}
		if (base.TargetMovement != Vector3.zero)
		{
			base.TargetMovement = base.TargetMovement.normalized * num;
		}
		this.HandleJumping(modelState, wantsJump, true);
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x000836C4 File Offset: 0x000818C4
	private void Movement_Noclip(InputState input, ModelState modelState)
	{
		base.TargetMovement = Vector3.zero;
		if (!ProgressBarUI.IsOpen())
		{
			if (input.IsDown(BUTTON.FORWARD))
			{
				base.TargetMovement += this.Owner.eyes.BodyForward();
			}
			if (input.IsDown(BUTTON.BACKWARD))
			{
				base.TargetMovement -= this.Owner.eyes.BodyForward();
			}
			if (input.IsDown(BUTTON.LEFT))
			{
				base.TargetMovement -= this.Owner.eyes.BodyRight();
			}
			if (input.IsDown(BUTTON.RIGHT))
			{
				base.TargetMovement += this.Owner.eyes.BodyRight();
			}
			if (input.IsDown(BUTTON.JUMP))
			{
				base.TargetMovement += Vector3.up;
			}
		}
		float d = 10f;
		if (input.IsDown(BUTTON.DUCK))
		{
			d = 2f;
		}
		if (input.IsDown(BUTTON.SPRINT))
		{
			d = 50f;
		}
		if (base.TargetMovement != Vector3.zero)
		{
			base.TargetMovement = base.TargetMovement.normalized * d;
		}
		modelState.flying = true;
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x0008380C File Offset: 0x00081A0C
	private void Movement_Walking(InputState input, ModelState modelState)
	{
		bool flag = input.IsDown(BUTTON.FORWARD) && (!input.IsDown(BUTTON.LEFT) && !input.IsDown(BUTTON.BACKWARD)) && !input.IsDown(BUTTON.RIGHT);
		if (flag)
		{
			if (input.IsDown(BUTTON.SPRINT))
			{
				if (!input.WasDown(BUTTON.SPRINT))
				{
					this.lastSprintTime = UnityEngine.Time.time;
					if (this.sprintForced)
					{
						this.sprintForced = false;
					}
				}
				if (UnityEngine.Time.time - this.lastSprintTime > 2f)
				{
					this.sprintForced = true;
				}
			}
		}
		else
		{
			this.sprintForced = false;
			this.lastSprintTime = UnityEngine.Time.time;
		}
		base.TargetMovement = Vector3.zero;
		if (!ProgressBarUI.IsOpen())
		{
			if (input.IsDown(BUTTON.FORWARD))
			{
				base.TargetMovement += this.Owner.eyes.MovementForward();
			}
			if (input.IsDown(BUTTON.BACKWARD))
			{
				base.TargetMovement -= this.Owner.eyes.MovementForward();
			}
			if (input.IsDown(BUTTON.LEFT))
			{
				base.TargetMovement -= this.Owner.eyes.MovementRight();
			}
			if (input.IsDown(BUTTON.RIGHT))
			{
				base.TargetMovement += this.Owner.eyes.MovementRight();
			}
		}
		if (this.swimming || this.jumping || (this.falling && UnityEngine.Time.time - this.groundTime > 0.3f))
		{
			this.HandleGrounded(modelState, false);
		}
		else
		{
			this.HandleGrounded(modelState, true);
		}
		bool wantsRun = (input.IsDown(BUTTON.SPRINT) || this.sprintForced) && flag && this.CanSprint();
		bool wantsDuck = input.IsDown(BUTTON.DUCK);
		bool wantsJump = input.WasJustPressed(BUTTON.JUMP);
		this.HandleRunning(modelState, wantsRun);
		this.HandleDucking(modelState, wantsDuck);
		if (base.TargetMovement != Vector3.zero)
		{
			base.TargetMovement = base.TargetMovement.normalized * this.Owner.GetSpeed(base.Running, base.Ducking);
		}
		float t = Mathf.Max(this.modify.drag, this.Owner.clothingMoveSpeedReduction);
		base.TargetMovement = Vector3.Lerp(base.TargetMovement, Vector3.zero, t);
		if (base.TargetMovement.magnitude < 0.1f)
		{
			base.Running = 0f;
		}
		this.HandleJumping(modelState, wantsJump, false);
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x000121EB File Offset: 0x000103EB
	private void HandleGrounded(ModelState state, bool wantsGrounded)
	{
		base.Grounded = (wantsGrounded ? 1f : 0f);
		state.onground = base.IsGrounded;
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x0001220E File Offset: 0x0001040E
	private void HandleRunning(ModelState state, bool wantsRun)
	{
		base.Running = (wantsRun ? Mathf.Lerp(1f, 0.6f, Mathf.Clamp01(this.groundAngle / this.maxAngleWalking)) : 0f);
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x00083A90 File Offset: 0x00081C90
	private void HandleDucking(ModelState state, bool wantsDuck)
	{
		base.Ducking = (wantsDuck ? 1f : this.GetForcedDuck());
		if (this.Owner.isMounted)
		{
			base.Ducking = 0f;
		}
		this.capsule.height = Mathf.Lerp(this.capsuleHeight, this.capsuleHeightDucked, base.Ducking);
		this.capsule.center = new Vector3(0f, Mathf.Lerp(this.capsuleCenter, this.capsuleCenterDucked, base.Ducking), 0f);
		state.ducked = base.IsDucked;
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x00083B2C File Offset: 0x00081D2C
	private float GetForcedDuck()
	{
		Vector3 vector = this.body.transform.position + new Vector3(0f, this.capsule.radius, 0f);
		Sphere sphere;
		sphere..ctor(vector, this.capsule.radius - 0.1f);
		float num = this.capsuleHeight - this.capsule.radius - sphere.radius + 0.2f;
		sphere.Move(base.transform.up, num, 1537286401);
		float value = sphere.position.y + sphere.radius - this.body.transform.position.y - 0.2f;
		float num2 = Mathf.InverseLerp(this.capsuleHeight, this.capsuleHeightDucked, value);
		if (!ConVar.Input.autocrouch && num2 < 0.5f)
		{
			num2 = 0f;
		}
		return num2;
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00012241 File Offset: 0x00010441
	private void HandleJumping(ModelState state, bool wantsJump, bool jumpInDirection = false)
	{
		if (!wantsJump || !this.CanJump())
		{
			return;
		}
		this.Jump(state, jumpInDirection);
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x00083C18 File Offset: 0x00081E18
	private bool CanJump()
	{
		return UnityEngine.Time.time - this.jumpTime >= 0.5f && (this.ladder != null || (UnityEngine.Time.time - this.groundTime <= 0.1f && UnityEngine.Time.time - this.landTime >= 0.1f && (UnityEngine.Time.time - this.landTime >= 0.2f || !this.sliding)));
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x00012257 File Offset: 0x00010457
	public override void BlockJump(float duration)
	{
		if (duration > 0f)
		{
			this.jumpTime = UnityEngine.Time.time + duration - 0.5f;
		}
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00012274 File Offset: 0x00010474
	public override void BlockSprint(float duration)
	{
		if (duration > 0f)
		{
			this.nextSprintTime = UnityEngine.Time.time + duration;
			base.Running = 0f;
			this.sprintForced = false;
		}
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x0001229D File Offset: 0x0001049D
	public void SetKinematic(bool kinematic)
	{
		this.body.isKinematic = kinematic;
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x000122AB File Offset: 0x000104AB
	private bool CanSprint()
	{
		return UnityEngine.Time.time - this.landTime >= 0.2f && !this.Owner.HasPlayerFlag(BasePlayer.PlayerFlags.NoSprint) && UnityEngine.Time.time > this.nextSprintTime;
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x00083C94 File Offset: 0x00081E94
	private void Jump(ModelState state, bool inDirection)
	{
		this.grounded = (this.climbing = (this.sliding = false));
		state.ducked = false;
		this.jumping = (state.jumped = true);
		this.jumpTime = UnityEngine.Time.time;
		this.ladder = null;
		if (inDirection)
		{
			this.body.velocity += Vector3.Lerp(this.Owner.eyes.BodyForward() * 9f, Vector3.zero, this.modify.drag);
		}
		else
		{
			this.body.velocity += Vector3.Lerp(Vector3.up * 9f, Vector3.zero, this.modify.drag);
		}
		Effect.client.Run("assets/bundled/prefabs/fx/screen_jump.prefab", default(Vector3), default(Vector3), default(Vector3));
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x00083D8C File Offset: 0x00081F8C
	public override void DoFixedUpdate(ModelState modelState)
	{
		base.transform.rotation = Quaternion.identity;
		this.groundAngle = this.groundAngleNew;
		this.groundNormal = this.groundNormalNew;
		this.swimming = (modelState.waterLevel > 0.65f);
		this.grounded = (this.groundAngle <= this.maxAngleWalking && !this.jumping && !this.swimming);
		this.climbing = (this.groundAngle <= this.maxAngleClimbing && !this.jumping && !this.swimming && !this.grounded);
		this.sliding = (this.groundAngle <= this.maxAngleSliding && !this.jumping && !this.swimming && !this.grounded && !this.climbing);
		this.jumping = (this.body.velocity.y > 0f && !this.swimming && !this.grounded && !this.climbing && !this.sliding);
		this.falling = (!this.swimming && !this.grounded && !this.climbing && !this.sliding && !this.jumping);
		bool isMounted = this.Owner.isMounted;
		this.body.isKinematic = isMounted;
		if (this.body.isKinematic)
		{
			this.body.velocity = Vector3.zero;
		}
		if (!this.flying && (this.wasJumping || this.wasFalling) && !this.jumping && !this.falling && !this.swimming && UnityEngine.Time.time - this.groundTime > 0.3f)
		{
			this.Owner.OnLand(this.previousVelocity.y);
			this.landTime = UnityEngine.Time.time;
		}
		if (!this.wasSwimming && this.swimming)
		{
			this.body.velocity *= 0.1f;
		}
		if (this.grounded || this.climbing || this.sliding)
		{
			this.groundTime = UnityEngine.Time.time;
		}
		this.UpdateVelocity();
		this.UpdateGravity(modelState);
		this.wasGrounded = this.grounded;
		this.wasClimbing = this.climbing;
		this.wasSliding = this.sliding;
		this.wasSwimming = this.swimming;
		this.wasJumping = this.jumping;
		this.wasFalling = this.falling;
		this.previousPosition = base.transform.localPosition;
		this.previousVelocity = this.body.velocity;
		this.groundAngleNew = float.MaxValue;
		this.groundNormalNew = Vector3.up;
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x0008404C File Offset: 0x0008224C
	private void UpdateVelocity()
	{
		Vector3 vector = this.body.velocity;
		if (this.wasFlying && !this.flying)
		{
			vector = Vector3.zero;
		}
		if (this.flying)
		{
			vector += (base.TargetMovement - vector) * Mathf.Clamp01(10f * UnityEngine.Time.fixedDeltaTime);
		}
		else if (this.ladder)
		{
			vector += (base.TargetMovement - vector) * Mathf.Clamp01(20f * UnityEngine.Time.fixedDeltaTime);
		}
		else if (this.grounded || this.climbing)
		{
			Vector3 normalized = Vector3Ex.XZ3D(this.groundNormal).normalized;
			Vector3 b = base.TargetMovement + normalized * Mathf.Max(0f, -Vector3.Dot(normalized, base.TargetMovement));
			float t = this.groundAngle - this.maxAngleWalking + 0.5f;
			vector = Vector3.Lerp(base.TargetMovement, b, t) + this.FallVelocity();
		}
		else if (this.sliding)
		{
			vector += (base.TargetMovement - Vector3Ex.XZ3D(vector)) * Mathf.Clamp01(3f * UnityEngine.Time.fixedDeltaTime);
			vector = Vector3.Lerp(vector, (vector - Vector3Ex.XZ3D(vector)) * 0.2f, this.modify.drag);
		}
		else if (this.swimming)
		{
			vector += (base.TargetMovement - vector) * Mathf.Clamp01(5f * UnityEngine.Time.fixedDeltaTime);
			vector = Vector3.Lerp(vector, (vector - Vector3Ex.XZ3D(vector)) * 0.2f, this.modify.drag);
		}
		else
		{
			vector += (base.TargetMovement - Vector3Ex.XZ3D(vector)) * Mathf.Clamp01(3f * UnityEngine.Time.fixedDeltaTime);
			vector = Vector3.Lerp(vector, (vector - Vector3Ex.XZ3D(vector)) * 0.2f, this.modify.drag);
		}
		if (!this.flying)
		{
			float magnitude = vector.magnitude;
			float num = Mathf.Max(this.maxVelocity, base.TargetMovement.magnitude);
			if (magnitude > num)
			{
				vector *= num / magnitude;
			}
		}
		this.body.velocity = vector;
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x000842C8 File Offset: 0x000824C8
	private void UpdateGravity(ModelState modelState)
	{
		this.capsule.material = ((base.TargetMovement.magnitude > 0f) ? this.zeroFrictionMaterial : this.highFrictionMaterial);
		if (this.flying)
		{
			return;
		}
		if (this.ladder)
		{
			return;
		}
		if (this.swimming)
		{
			this.body.AddForce(Physics.gravity * this.gravityMultiplierSwimming * this.body.mass);
			this.capsule.material = this.zeroFrictionMaterial;
			return;
		}
		Ray ray = new Ray(this.capsule.bounds.center, Vector3.down);
		float y = this.capsule.bounds.extents.y;
		if ((!this.grounded && !this.climbing) || !Physics.SphereCast(ray, this.gravityTestRadius, y, 1537286401))
		{
			this.body.AddForce(Physics.gravity * this.gravityMultiplier * this.body.mass);
			this.capsule.material = this.zeroFrictionMaterial;
		}
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x00084400 File Offset: 0x00082600
	public override void FrameUpdate(BasePlayer player, ModelState state)
	{
		base.transform.rotation = Quaternion.identity;
		this.capsule.isTrigger = this.flying;
		if (!player.HasLocalControls())
		{
			this.previousPosition = (base.transform.localPosition = player.transform.localPosition);
			this.previousVelocity = (this.body.velocity = Vector3.zero);
			return;
		}
		if (player.isMounted)
		{
			this.previousPosition = (base.transform.localPosition = player.transform.localPosition);
			return;
		}
		float t = (UnityEngine.Time.time - UnityEngine.Time.fixedTime) / UnityEngine.Time.fixedDeltaTime;
		Vector3 vector = Vector3.Lerp(this.previousPosition, base.transform.localPosition, t);
		vector.y = Mathf.Lerp(player.transform.localPosition.y, vector.y, UnityEngine.Time.smoothDeltaTime * 15f);
		if (vector != player.transform.localPosition)
		{
			bool hasChanged = player.transform.hasChanged;
			player.transform.localPosition = vector;
			player.transform.hasChanged = hasChanged;
		}
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00084528 File Offset: 0x00082728
	private void TransformState(Matrix4x4 matrix)
	{
		this.previousPosition = matrix.MultiplyPoint3x4(this.previousPosition);
		Vector3 a = new Vector3(0f, matrix.rotation.eulerAngles.y, 0f);
		this.Owner.input.SetViewVars(a + this.Owner.input.ClientLookVars());
		this.Owner.input.ApplyViewAngles();
		this.Owner.eyes.bodyRotation = this.Owner.input.ClientAngles();
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x000845C4 File Offset: 0x000827C4
	public override void SetParent(Transform parent)
	{
		if (base.transform.parent != parent)
		{
			if (base.transform.parent != null)
			{
				this.TransformState(base.transform.parent.localToWorldMatrix);
			}
			if (parent != null)
			{
				this.TransformState(parent.worldToLocalMatrix);
			}
			base.transform.parent = parent;
		}
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x000122E3 File Offset: 0x000104E3
	private Vector3 FallVelocity()
	{
		return new Vector3(0f, Mathf.Min(0f, this.body.velocity.y), 0f);
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x0001230E File Offset: 0x0001050E
	public override Vector3 CurrentVelocity()
	{
		return this.body.velocity;
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x0001231B File Offset: 0x0001051B
	public override float CurrentMoveSpeed()
	{
		return Vector3Ex.Magnitude2D(this.body.velocity);
	}
}
