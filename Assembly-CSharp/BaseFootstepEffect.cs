using System;
using ConVar;
using UnityEngine;

// Token: 0x0200026A RID: 618
public abstract class BaseFootstepEffect : MonoBehaviour, IClientComponent
{
	// Token: 0x04000E96 RID: 3734
	public LayerMask validImpactLayers = -1;

	// Token: 0x04000E97 RID: 3735
	private const float minTimeBetweenSteps = 0.05f;

	// Token: 0x04000E98 RID: 3736
	private float lastFootstepTime;

	// Token: 0x04000E99 RID: 3737
	private const float minDistanceBetweenSteps = 0.5f;

	// Token: 0x04000E9A RID: 3738
	private const float minDistanceBetweenStepsSqr = 0.25f;

	// Token: 0x04000E9B RID: 3739
	protected Vector3 lastFootstepPos = Vector3.zero;

	// Token: 0x04000E9C RID: 3740
	private const float maxDistanceToCamera = 75f;

	// Token: 0x060011F6 RID: 4598 RVA: 0x0007635C File Offset: 0x0007455C
	protected BaseFootstepEffect.GroundInfo GetGroundInfo(Vector3 position, Vector3 forward, bool bIgnoreDistanceCheck = false)
	{
		BaseFootstepEffect.GroundInfo result = default(BaseFootstepEffect.GroundInfo);
		result.position = position;
		result.rotation = Quaternion.identity;
		if (!Effects.footsteps)
		{
			return result;
		}
		if (UnityEngine.Time.time < this.lastFootstepTime + 0.05f)
		{
			return result;
		}
		if (!bIgnoreDistanceCheck && (position - this.lastFootstepPos).sqrMagnitude < 0.25f)
		{
			return result;
		}
		if (MainCamera.Distance(position) > 75f)
		{
			return result;
		}
		RaycastHit raycastHit;
		if (!TransformUtil.GetGroundInfo(position + new Vector3(0f, 0.75f, 0f), out raycastHit, 2f, this.validImpactLayers, null))
		{
			return result;
		}
		PhysicMaterial materialAt = raycastHit.collider.GetMaterialAt(raycastHit.point);
		result.position = raycastHit.point;
		result.rotation = QuaternionEx.LookRotationForcedUp(forward, raycastHit.normal);
		result.surface = (materialAt ? materialAt.GetNameLower() : "concrete");
		this.lastFootstepTime = UnityEngine.Time.time;
		this.lastFootstepPos = position;
		return result;
	}

	// Token: 0x0200026B RID: 619
	protected struct GroundInfo
	{
		// Token: 0x04000E9D RID: 3741
		public string surface;

		// Token: 0x04000E9E RID: 3742
		public Vector3 position;

		// Token: 0x04000E9F RID: 3743
		public Quaternion rotation;

		// Token: 0x060011F8 RID: 4600 RVA: 0x00076468 File Offset: 0x00074668
		public GameObject SpawnDecal(string effectType)
		{
			if (string.IsNullOrEmpty(this.surface))
			{
				return null;
			}
			string decal = EffectDictionary.GetDecal(effectType, this.surface);
			if (string.IsNullOrEmpty(decal))
			{
				return null;
			}
			return EffectLibrary.CreateEffect(decal, this.position, this.rotation);
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x000764B0 File Offset: 0x000746B0
		public GameObject SpawnEffect(string effectType)
		{
			if (string.IsNullOrEmpty(this.surface))
			{
				return null;
			}
			string particle = EffectDictionary.GetParticle(effectType, this.surface);
			if (string.IsNullOrEmpty(particle))
			{
				return null;
			}
			return EffectLibrary.CreateEffect(particle, this.position, this.rotation);
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x000764F8 File Offset: 0x000746F8
		public GameObject SpawnDisplacement(string effectType)
		{
			if (string.IsNullOrEmpty(this.surface))
			{
				return null;
			}
			string displacement = EffectDictionary.GetDisplacement(effectType, this.surface);
			if (string.IsNullOrEmpty(displacement))
			{
				return null;
			}
			return EffectLibrary.CreateEffect(displacement, this.position, this.rotation);
		}
	}
}
