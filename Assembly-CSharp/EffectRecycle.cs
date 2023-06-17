using System;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Token: 0x02000380 RID: 896
public class EffectRecycle : BaseMonoBehaviour, IClientComponent, IRagdollInhert, IEffectRecycle, IOnParentDestroying
{
	// Token: 0x040013A7 RID: 5031
	[ReadOnly]
	[FormerlySerializedAs("lifeTime")]
	public float detachTime;

	// Token: 0x040013A8 RID: 5032
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float recycleTime;

	// Token: 0x040013A9 RID: 5033
	public EffectRecycle.PlayMode playMode;

	// Token: 0x040013AA RID: 5034
	public EffectRecycle.ParentDestroyBehaviour onParentDestroyed;

	// Token: 0x040013AB RID: 5035
	private Action recycleAction;

	// Token: 0x040013AC RID: 5036
	private Action detachWaitRecycleAction;

	// Token: 0x060016D3 RID: 5843 RVA: 0x00013374 File Offset: 0x00011574
	protected void Awake()
	{
		this.recycleAction = new Action(this.Recycle);
		this.detachWaitRecycleAction = new Action(this.DetachWaitRecycle);
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x00088678 File Offset: 0x00086878
	private float GetParticleSystemLength()
	{
		float num = 0f;
		foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>(true))
		{
			num = Mathf.Max(num, particleSystem.duration + particleSystem.main.startLifetime.constantMax);
		}
		return num;
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x000886CC File Offset: 0x000868CC
	private float GetSoundLength()
	{
		float num = 0f;
		foreach (AudioSource audioSource in base.GetComponentsInChildren<AudioSource>(true))
		{
			if (audioSource.clip)
			{
				num = Mathf.Max(num, audioSource.clip.length);
			}
		}
		foreach (SoundPlayer soundPlayer in base.GetComponentsInChildren<SoundPlayer>(true))
		{
			num = Mathf.Max(num, soundPlayer.GetLength());
		}
		return num;
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x00088748 File Offset: 0x00086948
	private float GetScreenShakeLength()
	{
		float num = 0f;
		foreach (BaseScreenShake baseScreenShake in base.GetComponentsInChildren<BaseScreenShake>(true))
		{
			num = Mathf.Max(num, baseScreenShake.length);
		}
		return num;
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x0001339B File Offset: 0x0001159B
	public virtual Transform RagdollInhertTransform()
	{
		return base.transform;
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x00088784 File Offset: 0x00086984
	protected void OnEnable()
	{
		if (Application.isLoadingPrefabs)
		{
			return;
		}
		if (this.playMode == EffectRecycle.PlayMode.Once)
		{
			if (this.detachTime > 0f)
			{
				base.Invoke(this.detachWaitRecycleAction, this.detachTime);
				return;
			}
			base.Invoke(this.recycleAction, this.recycleTime);
		}
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x000133A3 File Offset: 0x000115A3
	public void Recycle()
	{
		base.gameObject.BroadcastOnParentDestroying();
		GameManager.client.Retire(base.gameObject);
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x000133C0 File Offset: 0x000115C0
	private void DetachFromParent()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
		SceneManager.MoveGameObjectToScene(base.gameObject, Rust.Client.EffectScene);
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x000133F1 File Offset: 0x000115F1
	private void DetachWaitRecycle()
	{
		if (this.recycleTime > this.detachTime)
		{
			this.DetachFromParent();
			base.Invoke(this.recycleAction, this.recycleTime - this.detachTime);
			return;
		}
		this.Recycle();
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x00013427 File Offset: 0x00011627
	public void OnParentDestroying()
	{
		if (this.onParentDestroyed == EffectRecycle.ParentDestroyBehaviour.Detach)
		{
			this.DetachFromParent();
		}
		if (this.onParentDestroyed == EffectRecycle.ParentDestroyBehaviour.DetachWaitDestroy)
		{
			this.DetachFromParent();
			if (this.playMode != EffectRecycle.PlayMode.Once)
			{
				base.Invoke(this.recycleAction, this.recycleTime);
			}
		}
	}

	// Token: 0x02000381 RID: 897
	public enum PlayMode
	{
		// Token: 0x040013AE RID: 5038
		Once,
		// Token: 0x040013AF RID: 5039
		Looped
	}

	// Token: 0x02000382 RID: 898
	public enum ParentDestroyBehaviour
	{
		// Token: 0x040013B1 RID: 5041
		Detach,
		// Token: 0x040013B2 RID: 5042
		Destroy,
		// Token: 0x040013B3 RID: 5043
		DetachWaitDestroy
	}
}
