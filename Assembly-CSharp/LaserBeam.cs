using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class LaserBeam : MonoBehaviour
{
	// Token: 0x04000829 RID: 2089
	public float scrollSpeed = 0.5f;

	// Token: 0x0400082A RID: 2090
	public LineRenderer beamRenderer;

	// Token: 0x0400082B RID: 2091
	public GameObject dotObject;

	// Token: 0x0400082C RID: 2092
	public Renderer dotRenderer;

	// Token: 0x0400082D RID: 2093
	public GameObject dotSpotlight;

	// Token: 0x0400082E RID: 2094
	public Vector2 scrollDir;

	// Token: 0x0400082F RID: 2095
	public float maxDistance = 100f;

	// Token: 0x04000830 RID: 2096
	public float stillBlendFactor = 0.1f;

	// Token: 0x04000831 RID: 2097
	public float movementBlendFactor = 0.5f;

	// Token: 0x04000832 RID: 2098
	public float movementThreshhold = 0.15f;

	// Token: 0x04000833 RID: 2099
	public bool isFirstPerson;

	// Token: 0x04000834 RID: 2100
	public Transform emissionOverride;

	// Token: 0x04000835 RID: 2101
	private MaterialPropertyBlock block;

	// Token: 0x04000836 RID: 2102
	private float aimToBarrelBlendFrac;

	// Token: 0x04000837 RID: 2103
	private Vector4 laserST;

	// Token: 0x06000BAD RID: 2989 RVA: 0x00059EF0 File Offset: 0x000580F0
	public BaseProjectile GetLocalPlayerWeapon()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (entity == null)
		{
			return null;
		}
		HeldEntity heldEntity = entity.GetHeldEntity();
		if (heldEntity == null)
		{
			return null;
		}
		return heldEntity.GetComponent<BaseProjectile>();
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x00059F28 File Offset: 0x00058128
	public Vector3 DotCast(Ray castRay)
	{
		if (!this.isFirstPerson)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(castRay, ref raycastHit, this.maxDistance, 1219701521))
			{
				return raycastHit.point;
			}
			return Vector3.zero;
		}
		else
		{
			HitTest hitTest = new HitTest();
			hitTest.AttackRay = castRay;
			hitTest.MaxDistance = this.maxDistance;
			hitTest.Radius = 0f;
			hitTest.Forgiveness = 0f;
			hitTest.type = HitTest.Type.Projectile;
			if (!GameTrace.Trace(hitTest, 1269916433))
			{
				return Vector3.zero;
			}
			return hitTest.HitPointWorld();
		}
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x00059FB0 File Offset: 0x000581B0
	private void UpdateDot()
	{
		if (!this.dotObject)
		{
			return;
		}
		Vector3 vector = (this.emissionOverride != null) ? this.emissionOverride.position : base.transform.position;
		Vector3 vector2 = Vector3.zero;
		Vector3 vector3 = this.DotCast(new Ray(vector, base.transform.forward));
		bool active = true;
		if (vector3 == Vector3.zero)
		{
			vector3 = vector + base.transform.forward * this.maxDistance;
			active = false;
		}
		if (this.isFirstPerson)
		{
			BasePlayer entity = LocalPlayer.Entity;
			Vector3 vector4 = this.DotCast(entity.eyes.BodyRay());
			if (vector4 == Vector3.zero)
			{
				vector4 = entity.eyes.BodyRay().origin + entity.eyes.BodyRay().direction * this.maxDistance * 2f;
			}
			BaseProjectile localPlayerWeapon = this.GetLocalPlayerWeapon();
			if (localPlayerWeapon)
			{
				float b = (localPlayerWeapon.ReadyToFire() && entity.CanAttack() && !entity.IsRunning()) ? ((entity.movement.CurrentMoveSpeed() < this.movementThreshhold) ? this.stillBlendFactor : this.movementBlendFactor) : 1f;
				this.aimToBarrelBlendFrac = Mathf.Lerp(this.aimToBarrelBlendFrac, b, Time.deltaTime * 8f);
				Vector3 normalized = (vector3 - vector4).normalized;
				float d = Vector3.Distance(vector3, vector4);
				vector2 = vector4 + normalized * d * this.aimToBarrelBlendFrac;
				float num = Vector3.Distance(vector2, base.transform.position);
				this.dotObject.transform.localScale = Vector3.one + Vector3.one * (num / 12.5f);
				this.dotRenderer.enabled = (num <= this.maxDistance);
				if (this.dotSpotlight)
				{
					this.dotSpotlight.transform.position = vector2 + (entity.eyes.BodyRay().origin - vector2).normalized * 3f;
					this.dotSpotlight.SetActive(this.dotRenderer.enabled);
				}
			}
		}
		else
		{
			vector2 = vector3;
		}
		this.dotObject.SetActive(active);
		this.dotObject.transform.position = vector2;
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0000B1F9 File Offset: 0x000093F9
	public void OnEnable()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.laserST = this.beamRenderer.sharedMaterial.GetVector("_ShadowTex_ST");
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0005A248 File Offset: 0x00058448
	private void UpdateBeam()
	{
		if (!this.beamRenderer)
		{
			return;
		}
		if (!this.dotObject)
		{
			return;
		}
		if (this.block == null)
		{
			return;
		}
		if (this.beamRenderer)
		{
			float time = Time.time;
			this.block.Clear();
			this.block.SetVector("_PlanarFadeWorldVector", this.dotObject.transform.position - base.transform.position);
			this.block.SetVector("_ShadowTex_ST", new Vector4(this.laserST.x, this.laserST.y, (this.scrollDir * time).x, (this.scrollDir * time).y));
			this.beamRenderer.SetPropertyBlock(this.block);
			this.beamRenderer.SetPosition(0, base.transform.position);
			this.beamRenderer.SetPosition(1, this.dotObject.transform.position);
		}
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0000B229 File Offset: 0x00009429
	private void UpdateLaserPositions()
	{
		this.UpdateDot();
		this.UpdateBeam();
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0000B237 File Offset: 0x00009437
	public void LateUpdate()
	{
		this.UpdateLaserPositions();
	}
}
