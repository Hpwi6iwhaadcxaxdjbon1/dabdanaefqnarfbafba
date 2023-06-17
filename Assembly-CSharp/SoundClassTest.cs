using System;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class SoundClassTest : MonoBehaviour
{
	// Token: 0x04000AC0 RID: 2752
	public SoundClass soundClass;

	// Token: 0x04000AC1 RID: 2753
	public float soundInterval = 0.5f;

	// Token: 0x04000AC2 RID: 2754
	private float lastPlayed;

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00061E0C File Offset: 0x0006000C
	private void Update()
	{
		if (Time.time > this.soundInterval + this.lastPlayed)
		{
			SoundDefinition soundDefinition = this.soundClass.definitions[Random.Range(0, this.soundClass.definitions.Count)];
			Debug.Log("Playing sound " + soundDefinition, soundDefinition);
			SoundManager.PlayOneshot(soundDefinition, base.gameObject, false, default(Vector3));
			this.lastPlayed = Time.time;
		}
	}
}
