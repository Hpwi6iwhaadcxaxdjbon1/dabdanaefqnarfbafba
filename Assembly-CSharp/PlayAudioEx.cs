using System;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class PlayAudioEx : MonoBehaviour
{
	// Token: 0x04000D2D RID: 3373
	public float delay;

	// Token: 0x0600105A RID: 4186 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x0006E554 File Offset: 0x0006C754
	private void OnEnable()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		if (component)
		{
			component.PlayDelayed(this.delay);
		}
	}
}
