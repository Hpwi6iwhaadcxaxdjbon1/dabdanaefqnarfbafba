using System;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class AnimalAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x04000BDE RID: 3038
	public BaseNpc Target;

	// Token: 0x04000BDF RID: 3039
	public Animator Animator;

	// Token: 0x04000BE0 RID: 3040
	public MaterialEffect FootstepEffects;

	// Token: 0x04000BE1 RID: 3041
	public Transform[] Feet;

	// Token: 0x04000BE2 RID: 3042
	[ReadOnly]
	public string BaseFolder;

	// Token: 0x04000BE3 RID: 3043
	private float lastThinkTime;

	// Token: 0x04000BE4 RID: 3044
	private Vector3 previousPosition;

	// Token: 0x04000BE5 RID: 3045
	private float previousRotationYaw;

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0000D5D1 File Offset: 0x0000B7D1
	private void Update()
	{
		if (Time.time - this.lastThinkTime < 0.1f)
		{
			return;
		}
		this.lastThinkTime = Time.time;
		this.Animator.SetBool(AnimalAnimation.Params.Sleeping, this.Target.IsSleeping);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0000D60D File Offset: 0x0000B80D
	private void FrontLeftFootstep()
	{
		this.Footstep(this.Feet[0]);
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0000D61D File Offset: 0x0000B81D
	private void FrontRightFootstep()
	{
		this.Footstep(this.Feet[1]);
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0000D62D File Offset: 0x0000B82D
	private void BackLeftFootstep()
	{
		this.Footstep(this.Feet[2]);
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0000D63D File Offset: 0x0000B83D
	private void BackRightFootstep()
	{
		this.Footstep(this.Feet[3]);
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0000D64D File Offset: 0x0000B84D
	private void Footstep(Transform tx)
	{
		if (!this.FootstepEffects)
		{
			return;
		}
		this.FootstepEffects.SpawnOnRay(new Ray(tx.position, Vector3.down), 10551297, 0.5f, base.transform.forward);
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x00002ECE File Offset: 0x000010CE
	private void DoEffect(string effect)
	{
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x00002ECE File Offset: 0x000010CE
	private void PlayEffect(string effect)
	{
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x00066B14 File Offset: 0x00064D14
	public void PlaySound(string soundName)
	{
		string text = StringFormatCache.Get("{0}/sound/{1}.asset", this.BaseFolder, soundName);
		SoundDefinition soundDefinition = FileSystem.Load<SoundDefinition>(text, true);
		if (soundDefinition != null)
		{
			SoundManager.PlayOneshot(soundDefinition, null, false, base.transform.position);
			return;
		}
		Debug.LogWarningFormat("Couldn't find sound {0}", new object[]
		{
			text
		});
	}

	// Token: 0x020001CB RID: 459
	protected static class Params
	{
		// Token: 0x04000BE6 RID: 3046
		public static int Sleeping = Animator.StringToHash("sleeping");
	}
}
