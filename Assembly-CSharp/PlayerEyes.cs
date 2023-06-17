using System;
using ConVar;
using UnityEngine;

// Token: 0x0200030F RID: 783
public class PlayerEyes : EntityComponent<BasePlayer>
{
	// Token: 0x0400115C RID: 4444
	public static readonly Vector3 EyeOffset = new Vector3(0f, 1.5f, 0f);

	// Token: 0x0400115D RID: 4445
	public static readonly Vector3 DuckOffset = new Vector3(0f, -0.6f, 0f);

	// Token: 0x0400115E RID: 4446
	public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);

	// Token: 0x0400115F RID: 4447
	public LazyAimProperties defaultLazyAim;

	// Token: 0x04001160 RID: 4448
	private Vector3 viewOffset = Vector3.zero;

	// Token: 0x04001164 RID: 4452
	private Quaternion lazyRotation;

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x0600146E RID: 5230 RVA: 0x0007E7B8 File Offset: 0x0007C9B8
	public Vector3 worldMountedPosition
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity);
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return this.worldStandingPosition;
		}
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x0600146F RID: 5231 RVA: 0x00011659 File Offset: 0x0000F859
	public Vector3 worldStandingPosition
	{
		get
		{
			return base.transform.position + PlayerEyes.EyeOffset;
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06001470 RID: 5232 RVA: 0x00011670 File Offset: 0x0000F870
	public Vector3 worldCrouchedPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.DuckOffset;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06001471 RID: 5233 RVA: 0x0007E80C File Offset: 0x0007CA0C
	public Vector3 position
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity);
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y);
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06001472 RID: 5234 RVA: 0x0007E890 File Offset: 0x0007CA90
	public Vector3 center
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity);
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y);
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06001473 RID: 5235 RVA: 0x00011682 File Offset: 0x0000F882
	public Vector3 offset
	{
		get
		{
			return base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y);
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06001474 RID: 5236 RVA: 0x000116AA File Offset: 0x0000F8AA
	// (set) Token: 0x06001475 RID: 5237 RVA: 0x000116BD File Offset: 0x0000F8BD
	public Quaternion rotation
	{
		get
		{
			return this.parentRotation * this.bodyRotation;
		}
		set
		{
			this.bodyRotation = Quaternion.Inverse(this.parentRotation) * value;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06001476 RID: 5238 RVA: 0x000116D6 File Offset: 0x0000F8D6
	// (set) Token: 0x06001477 RID: 5239 RVA: 0x000116DE File Offset: 0x0000F8DE
	public Quaternion bodyRotation { get; set; }

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06001478 RID: 5240 RVA: 0x000116E7 File Offset: 0x0000F8E7
	// (set) Token: 0x06001479 RID: 5241 RVA: 0x000116EF File Offset: 0x0000F8EF
	public Quaternion headRotation { get; set; }

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x0600147A RID: 5242 RVA: 0x000116F8 File Offset: 0x0000F8F8
	// (set) Token: 0x0600147B RID: 5243 RVA: 0x00011700 File Offset: 0x0000F900
	public Quaternion rotationLook { get; set; }

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x0600147C RID: 5244 RVA: 0x0007E914 File Offset: 0x0007CB14
	public Quaternion parentRotation
	{
		get
		{
			if (!(base.transform.parent != null))
			{
				return Quaternion.identity;
			}
			return Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x0600147D RID: 5245 RVA: 0x00011709 File Offset: 0x0000F909
	public bool canUpdateViewAngles
	{
		get
		{
			return base.baseEntity.IsSpectating() || (!base.baseEntity.IsDead() && !base.baseEntity.IsSleeping());
		}
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x0007E968 File Offset: 0x0007CB68
	public void FrameUpdate(Camera cam)
	{
		if (this.canUpdateViewAngles)
		{
			this.bodyRotation = base.baseEntity.input.ClientAngles();
			this.headRotation = base.baseEntity.input.HeadAngles();
		}
		Vector3 vector = Vector3.zero;
		if (base.baseEntity.movement)
		{
			vector = Vector3.Lerp(vector, PlayerEyes.DuckOffset, base.baseEntity.movement.Ducking);
		}
		this.viewOffset = Vector3.Lerp(this.viewOffset, vector, UnityEngine.Time.deltaTime * 10f);
		this.UpdateCamera(cam);
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0007EA04 File Offset: 0x0007CC04
	private void UpdateCamera(Camera cam)
	{
		if (cam == null)
		{
			return;
		}
		cam.fieldOfView = ConVar.Graphics.fov;
		switch (base.baseEntity.currentViewMode)
		{
		case BasePlayer.CameraMode.FirstPerson:
			this.DoFirstPersonCamera(cam);
			return;
		case BasePlayer.CameraMode.ThirdPerson:
			this.DoThirdPersonCamera(cam);
			return;
		case BasePlayer.CameraMode.Eyes:
			this.DoInEyeCamera(cam);
			return;
		default:
			return;
		}
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0007EA5C File Offset: 0x0007CC5C
	private void DoFirstPersonCamera(Camera cam)
	{
		if (SingletonComponent<CameraMan>.Instance != null && SingletonComponent<CameraMan>.Instance.enabled)
		{
			return;
		}
		this.lazyRotation = this.rotation;
		cam.transform.position = this.position;
		cam.transform.rotation = (this.rotationLook = this.lazyRotation * this.headRotation);
		if (base.baseEntity.IsSpectating())
		{
			FirstPersonSpectatorMode.Apply(cam, base.baseEntity.GetParentEntity());
		}
		this.ApplyCameraMoves(cam);
		if (SingletonComponent<MainCamera>.Instance != null)
		{
			LocalPlayer.ModifyCamera();
		}
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x0007EAFC File Offset: 0x0007CCFC
	private void DoInEyeCamera(Camera cam)
	{
		if (SingletonComponent<CameraMan>.Instance != null && SingletonComponent<CameraMan>.Instance.enabled)
		{
			return;
		}
		Transform eyeTransform = base.baseEntity.GetEyeTransform();
		if (base.baseEntity.IsSpectating())
		{
			BaseEntity parentEntity = base.baseEntity.GetParentEntity();
			if (parentEntity)
			{
				eyeTransform = parentEntity.GetEyeTransform();
			}
		}
		if (eyeTransform)
		{
			cam.transform.position = eyeTransform.position;
			cam.transform.rotation = eyeTransform.rotation;
			this.rotationLook = eyeTransform.rotation;
			return;
		}
		this.DoFirstPersonCamera(cam);
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x0007EB98 File Offset: 0x0007CD98
	private void DoThirdPersonCamera(Camera cam)
	{
		if (SingletonComponent<CameraMan>.Instance != null && SingletonComponent<CameraMan>.Instance.enabled)
		{
			return;
		}
		BaseEntity baseEntity = base.baseEntity;
		Vector3 vector = this.position;
		if (base.baseEntity.IsSpectating())
		{
			baseEntity = base.baseEntity.GetParentEntity();
			if (baseEntity == null)
			{
				baseEntity = base.baseEntity;
			}
			BasePlayer basePlayer = base.baseEntity.GetParentEntity() as BasePlayer;
			if (basePlayer)
			{
				vector = basePlayer.eyes.position;
			}
		}
		if (base.baseEntity.IsSleeping())
		{
			cam.transform.position = this.position - this.rotation * this.thirdPersonSleepingOffset;
			cam.transform.rotation = this.rotation;
			this.rotationLook = base.baseEntity.transform.rotation;
			return;
		}
		Vector3 vector2 = Vector3.zero;
		if (base.baseEntity.IsSpectating())
		{
			if (UnityEngine.Input.GetKey(KeyCode.Mouse1))
			{
				ConVar.Client.camfov += UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 1f * Mathf.InverseLerp(-2f, 90f, ConVar.Client.camfov);
			}
			else
			{
				ConVar.Client.camdist += UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 0.03f * Mathf.InverseLerp(-2f, 10f, ConVar.Client.camdist);
			}
			ConVar.Client.camfov = Mathf.Clamp(ConVar.Client.camfov, 1f, 120f);
			ConVar.Client.camdist = Mathf.Clamp(ConVar.Client.camdist, 0.01f, 20f);
			Transform transform = null;
			if (ConVar.Client.cambone != string.Empty)
			{
				transform = baseEntity.FindBone(ConVar.Client.cambone);
			}
			else
			{
				vector2 = ConVar.Client.camoffset;
			}
			if (transform != null)
			{
				this.rotationLook = transform.rotation * this.rotation;
				vector = transform.position;
			}
			else
			{
				Model model = baseEntity.GetModel();
				if (model)
				{
					vector = model.transform.position;
					this.rotationLook = model.transform.rotation * this.rotation;
				}
				else
				{
					vector = baseEntity.transform.position;
					this.rotationLook = baseEntity.transform.rotation * this.rotation;
				}
			}
			if (baseEntity == base.baseEntity)
			{
				this.rotationLook = this.rotation;
			}
		}
		else
		{
			vector2 = ConVar.Client.camoffset;
			this.rotationLook = this.rotation;
			Model model2 = baseEntity.GetModel();
			if (model2)
			{
				vector = model2.transform.position;
			}
			else
			{
				vector = baseEntity.transform.position;
			}
		}
		if (ConVar.Client.camoffset_relative)
		{
			vector += vector2.x * (this.rotationLook * Vector3.right);
			vector += vector2.z * (this.rotationLook * Vector3.forward);
			vector.y += vector2.y;
		}
		else
		{
			vector += vector2;
		}
		Vector3 b = this.rotationLook * Vector3.back * ConVar.Client.camdist;
		Vector3 position = vector + b;
		cam.transform.rotation = this.rotationLook;
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, b.normalized, ref raycastHit, b.magnitude, 1235288065))
		{
			cam.transform.position = raycastHit.point;
		}
		else
		{
			cam.transform.position = position;
		}
		cam.fieldOfView = ConVar.Client.camfov;
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x00011739 File Offset: 0x0000F939
	private void ApplyCameraMoves(Camera cam)
	{
		BaseScreenShake.Apply(cam, BaseViewModel.ActiveModel);
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x0007EF28 File Offset: 0x0007D128
	public Vector3 MovementForward()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.forward;
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x0007EF68 File Offset: 0x0007D168
	public Vector3 MovementRight()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.right;
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x00011746 File Offset: 0x0000F946
	public Ray BodyRay()
	{
		return new Ray(this.position, this.BodyForward());
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x00011759 File Offset: 0x0000F959
	public Vector3 BodyForward()
	{
		return this.rotation * Vector3.forward;
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x0001176B File Offset: 0x0000F96B
	public Vector3 BodyRight()
	{
		return this.rotation * Vector3.right;
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x0001177D File Offset: 0x0000F97D
	public Vector3 BodyUp()
	{
		return this.rotation * Vector3.up;
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x0001178F File Offset: 0x0000F98F
	public Ray HeadRay()
	{
		return new Ray(this.position, this.HeadForward());
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x000117A2 File Offset: 0x0000F9A2
	public Vector3 HeadForward()
	{
		return this.rotation * this.headRotation * Vector3.forward;
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x000117BF File Offset: 0x0000F9BF
	public Vector3 HeadRight()
	{
		return this.rotation * this.headRotation * Vector3.right;
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x000117DC File Offset: 0x0000F9DC
	public Vector3 HeadUp()
	{
		return this.rotation * this.headRotation * Vector3.up;
	}
}
