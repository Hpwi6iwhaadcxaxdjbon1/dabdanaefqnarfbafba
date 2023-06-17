using System;
using System.Collections;
using ConVar;
using UnityEngine;

// Token: 0x020002B5 RID: 693
public class EntityItem_RotateWhenOn : EntityComponent<BaseEntity>, IOnPostNetworkUpdate
{
	// Token: 0x04000FB0 RID: 4016
	public EntityItem_RotateWhenOn.State on;

	// Token: 0x04000FB1 RID: 4017
	public EntityItem_RotateWhenOn.State off;

	// Token: 0x04000FB2 RID: 4018
	internal bool currentlyOn;

	// Token: 0x04000FB3 RID: 4019
	internal bool stateInitialized;

	// Token: 0x04000FB4 RID: 4020
	public BaseEntity.Flags targetFlag = BaseEntity.Flags.On;

	// Token: 0x04000FB5 RID: 4021
	private Sound movementLoop;

	// Token: 0x0600135C RID: 4956 RVA: 0x00010725 File Offset: 0x0000E925
	private void OnEnable()
	{
		if (base.baseEntity == null)
		{
			return;
		}
		this.DoInitialize(base.baseEntity);
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x0007B5C4 File Offset: 0x000797C4
	private void DoInitialize(BaseEntity entity)
	{
		if (entity.HasFlag(this.targetFlag))
		{
			base.transform.localRotation = Quaternion.Euler(this.on.rotation);
		}
		else
		{
			base.transform.localRotation = Quaternion.Euler(this.off.rotation);
		}
		this.stateInitialized = true;
		this.currentlyOn = entity.HasFlag(this.targetFlag);
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x0007B630 File Offset: 0x00079830
	public void OnPostNetworkUpdate(BaseEntity entity)
	{
		if (base.baseEntity != entity)
		{
			return;
		}
		if (!this.stateInitialized)
		{
			this.DoInitialize(entity);
			return;
		}
		if (entity.HasFlag(this.targetFlag))
		{
			if (Global.developer > 0)
			{
				UnityEngine.DDraw.Text("DoOpen", base.transform.position, Color.white, 2f);
			}
			this.DoOpen();
			return;
		}
		if (Global.developer > 0)
		{
			UnityEngine.DDraw.Text("DoClose", base.transform.position, Color.white, 2f);
		}
		this.DoClose();
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x0007B6C8 File Offset: 0x000798C8
	public void DoOpen()
	{
		if (this.currentlyOn)
		{
			return;
		}
		this.currentlyOn = true;
		if (this.movementLoop != null)
		{
			this.movementLoop.FadeOutAndRecycle(0.1f);
			this.movementLoop = null;
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.RotateTo(this.on));
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x0007B724 File Offset: 0x00079924
	public void DoClose()
	{
		if (!this.currentlyOn)
		{
			return;
		}
		this.currentlyOn = false;
		if (this.movementLoop != null)
		{
			this.movementLoop.FadeOutAndRecycle(0.1f);
			this.movementLoop = null;
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.RotateTo(this.off));
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x00010742 File Offset: 0x0000E942
	private IEnumerator RotateTo(EntityItem_RotateWhenOn.State state)
	{
		Quaternion start = base.transform.localRotation;
		Quaternion target = Quaternion.Euler(state.rotation);
		bool effectsStarted = false;
		float TimeTaken = state.initialDelay * -1f;
		while (TimeTaken < state.timeToTake)
		{
			TimeTaken += UnityEngine.Time.deltaTime;
			yield return null;
			if (TimeTaken >= 0f)
			{
				if (!effectsStarted)
				{
					if (state.effectOnStart != "")
					{
						Effect.client.Run("assets/bundled/prefabs/fx/" + state.effectOnStart + ".prefab", base.transform.position, base.transform.forward, default(Vector3));
					}
					if (state.startSound != null)
					{
						SoundManager.PlayOneshot(state.startSound, base.gameObject, false, default(Vector3));
					}
					if (this.movementLoop == null && state.movementLoop != null)
					{
						this.movementLoop = SoundManager.RequestSoundInstance(state.movementLoop, base.gameObject, default(Vector3), false);
						this.movementLoop.fade.FadeIn(0.2f);
						this.movementLoop.Play();
					}
					effectsStarted = true;
				}
				float num = Mathf.InverseLerp(0f, state.timeToTake, TimeTaken);
				num = state.animationCurve.Evaluate(num);
				base.transform.localRotation = Quaternion.Slerp(start, target, num);
			}
		}
		base.transform.localRotation = target;
		if (state.effectOnFinish != "")
		{
			Effect.client.Run("assets/bundled/prefabs/fx/" + state.effectOnFinish + ".prefab", base.transform.position, base.transform.forward, default(Vector3));
		}
		if (this.movementLoop != null)
		{
			this.movementLoop.FadeOutAndRecycle(state.movementLoopFadeOutTime);
			this.movementLoop = null;
		}
		if (state.stopSound != null)
		{
			SoundManager.PlayOneshot(state.stopSound, base.gameObject, false, default(Vector3));
		}
		yield break;
	}

	// Token: 0x020002B6 RID: 694
	[Serializable]
	public class State
	{
		// Token: 0x04000FB6 RID: 4022
		public Vector3 rotation;

		// Token: 0x04000FB7 RID: 4023
		public float initialDelay;

		// Token: 0x04000FB8 RID: 4024
		public float timeToTake = 2f;

		// Token: 0x04000FB9 RID: 4025
		public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		// Token: 0x04000FBA RID: 4026
		public string effectOnStart = "";

		// Token: 0x04000FBB RID: 4027
		public string effectOnFinish = "";

		// Token: 0x04000FBC RID: 4028
		public SoundDefinition movementLoop;

		// Token: 0x04000FBD RID: 4029
		public float movementLoopFadeOutTime = 0.1f;

		// Token: 0x04000FBE RID: 4030
		public SoundDefinition startSound;

		// Token: 0x04000FBF RID: 4031
		public SoundDefinition stopSound;
	}
}
