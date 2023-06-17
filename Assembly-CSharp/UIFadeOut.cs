using System;
using UnityEngine;

// Token: 0x0200063C RID: 1596
public class UIFadeOut : MonoBehaviour
{
	// Token: 0x04001FAA RID: 8106
	public float secondsToFadeOut = 3f;

	// Token: 0x04001FAB RID: 8107
	public bool destroyOnFaded = true;

	// Token: 0x04001FAC RID: 8108
	public CanvasGroup targetGroup;

	// Token: 0x04001FAD RID: 8109
	private float timeStarted;

	// Token: 0x0600239B RID: 9115 RVA: 0x0001C2FA File Offset: 0x0001A4FA
	private void Start()
	{
		this.timeStarted = Time.realtimeSinceStartup;
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000BCCD8 File Offset: 0x000BAED8
	private void Update()
	{
		this.targetGroup.alpha = Mathf.InverseLerp(this.timeStarted + this.secondsToFadeOut, this.timeStarted, Time.realtimeSinceStartup);
		if (this.destroyOnFaded && Time.realtimeSinceStartup > this.timeStarted + this.secondsToFadeOut)
		{
			GameManager.Destroy(base.gameObject, 0f);
		}
	}
}
