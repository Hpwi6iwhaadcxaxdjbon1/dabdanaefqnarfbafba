using System;
using ConVar;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class NpcWalkAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x04000BEC RID: 3052
	public Vector3 HipFudge = new Vector3(-90f, 0f, 90f);

	// Token: 0x04000BED RID: 3053
	public BaseNpc Npc;

	// Token: 0x04000BEE RID: 3054
	public Animator Animator;

	// Token: 0x04000BEF RID: 3055
	public Transform HipBone;

	// Token: 0x04000BF0 RID: 3056
	public Transform LookBone;

	// Token: 0x04000BF1 RID: 3057
	public bool UpdateWalkSpeed = true;

	// Token: 0x04000BF2 RID: 3058
	public bool UpdateFacingDirection = true;

	// Token: 0x04000BF3 RID: 3059
	public bool UpdateGroundNormal = true;

	// Token: 0x04000BF4 RID: 3060
	public Transform alignmentRoot;

	// Token: 0x04000BF5 RID: 3061
	public bool LaggyAss = true;

	// Token: 0x04000BF6 RID: 3062
	public bool LookAtTarget;

	// Token: 0x04000BF7 RID: 3063
	public float MaxLaggyAssRotation = 70f;

	// Token: 0x04000BF8 RID: 3064
	public float MaxWalkAnimSpeed = 25f;

	// Token: 0x04000BF9 RID: 3065
	public bool UseDirectionBlending;

	// Token: 0x04000BFA RID: 3066
	private static int ParamWalkSpeed = Animator.StringToHash("walkSpeed");

	// Token: 0x04000BFB RID: 3067
	private static int ParamWalkSpeedAverage = Animator.StringToHash("walkSpeedAverage");

	// Token: 0x04000BFC RID: 3068
	private static int ParamWalkDirX = 0;

	// Token: 0x04000BFD RID: 3069
	private static int ParamWalkDirZ = 0;

	// Token: 0x04000BFE RID: 3070
	private AverageVelocity AverageVelocity = new AverageVelocity();

	// Token: 0x04000BFF RID: 3071
	private Vector3 oldPosition;

	// Token: 0x04000C00 RID: 3072
	private Quaternion hipForward;

	// Token: 0x04000C01 RID: 3073
	private Quaternion baseHipRotation;

	// Token: 0x04000C02 RID: 3074
	private Quaternion baseLookRotation;

	// Token: 0x04000C03 RID: 3075
	private Vector3 targetUp;

	// Token: 0x04000C04 RID: 3076
	private Vector3 targetOffset;

	// Token: 0x06000EE0 RID: 3808 RVA: 0x00066BE8 File Offset: 0x00064DE8
	public void Start()
	{
		if (this.HipBone)
		{
			this.hipForward = this.HipBone.rotation;
			this.baseHipRotation = this.HipBone.localRotation;
		}
		if (this.LookBone)
		{
			this.baseLookRotation = this.LookBone.localRotation;
		}
		if (this.UseDirectionBlending && NpcWalkAnimation.ParamWalkDirX == 0)
		{
			NpcWalkAnimation.ParamWalkDirX = Animator.StringToHash("walkDirX");
			NpcWalkAnimation.ParamWalkDirZ = Animator.StringToHash("walkDirZ");
		}
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x00066C70 File Offset: 0x00064E70
	public void Update()
	{
		if (this.Npc.isServer)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = localPosition - this.oldPosition;
		this.oldPosition = localPosition;
		this.AverageVelocity.Record(localPosition);
		float magnitude = vector.magnitude;
		Quaternion b = this.Npc.NetworkRotation;
		if (this.UpdateGroundNormal && AI.groundAlign && MainCamera.Distance(base.transform.position) < AI.maxGroundAlignDist)
		{
			Vector3 vector2 = Vector3.up;
			Vector3 zero = Vector3.zero;
			RaycastHit raycastHit;
			if (this.GroundSample(base.transform.position, out raycastHit))
			{
				this.targetUp = raycastHit.normal;
				this.targetOffset = base.transform.InverseTransformPoint(raycastHit.point);
			}
			else
			{
				this.targetUp = Vector3.up;
				this.targetOffset = Vector3.zero;
			}
			if (this.alignmentRoot)
			{
				this.alignmentRoot.transform.localPosition = Vector3.MoveTowards(this.alignmentRoot.transform.localPosition, this.targetOffset, UnityEngine.Time.deltaTime * 2f);
			}
			vector2 = Vector3.MoveTowards(base.transform.up, this.targetUp, UnityEngine.Time.deltaTime * 180f);
			b = Quaternion.LookRotation(Vector3.Cross(this.Npc.NetworkRotation * Vector3.right, vector2), vector2);
		}
		if (this.UpdateWalkSpeed && this.Animator.gameObject.activeSelf)
		{
			this.Animator.SetFloat(NpcWalkAnimation.ParamWalkSpeed, Mathf.Min(magnitude / UnityEngine.Time.deltaTime, this.MaxWalkAnimSpeed));
			this.Animator.SetFloat(NpcWalkAnimation.ParamWalkSpeedAverage, Mathf.Min(this.AverageVelocity.Speed, this.MaxWalkAnimSpeed), 0.1f, UnityEngine.Time.smoothDeltaTime);
		}
		if (this.UpdateFacingDirection)
		{
			if (vector.magnitude > 0.5f)
			{
				b = Quaternion.LookRotation(vector.normalized);
			}
			float turnSpeed = this.Npc.Stats.TurnSpeed;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, UnityEngine.Time.smoothDeltaTime * turnSpeed);
			if (this.LaggyAss)
			{
				this.hipForward = Quaternion.Slerp(this.hipForward, b, UnityEngine.Time.smoothDeltaTime * 15f);
				if (Quaternion.Angle(this.hipForward, base.transform.rotation) > this.MaxLaggyAssRotation)
				{
					this.hipForward = Quaternion.RotateTowards(base.transform.rotation, this.hipForward, this.MaxLaggyAssRotation);
				}
			}
		}
		bool lookAtTarget = this.LookAtTarget;
		if (this.UseDirectionBlending && magnitude > 0f)
		{
			Vector3 normalized = vector.normalized;
			this.Animator.SetFloat(NpcWalkAnimation.ParamWalkDirX, normalized.x);
			this.Animator.SetFloat(NpcWalkAnimation.ParamWalkDirZ, normalized.z);
		}
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x00009303 File Offset: 0x00007503
	public bool GroundSample(Vector3 origin, out RaycastHit hit)
	{
		return Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, ref hit, 1f, 8454144);
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x00066F60 File Offset: 0x00065160
	public void LateUpdate()
	{
		if (this.Npc.IsSleeping)
		{
			return;
		}
		if (this.LaggyAss)
		{
			Quaternion rhs = Quaternion.Inverse(this.baseHipRotation) * this.HipBone.localRotation;
			this.HipBone.rotation = this.hipForward * Quaternion.Euler(this.HipFudge) * rhs;
		}
		if (this.LookAtTarget)
		{
			Quaternion rotation = Quaternion.Inverse(this.baseLookRotation) * this.LookBone.localRotation;
			this.LookBone.rotation = rotation;
		}
	}
}
