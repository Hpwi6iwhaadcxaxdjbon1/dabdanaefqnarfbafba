using System;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class SoundModifier : MonoBehaviour
{
	// Token: 0x04000AF2 RID: 2802
	[HideInInspector]
	public Sound sound;

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0000CB42 File Offset: 0x0000AD42
	public virtual void Init(Sound targetSound)
	{
		this.sound = targetSound;
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void ApplyModification()
	{
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnSoundPlay()
	{
	}
}
