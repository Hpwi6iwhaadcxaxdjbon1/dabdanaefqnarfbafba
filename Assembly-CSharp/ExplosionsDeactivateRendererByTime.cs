using System;
using UnityEngine;

// Token: 0x020007A1 RID: 1953
public class ExplosionsDeactivateRendererByTime : MonoBehaviour
{
	// Token: 0x040025F9 RID: 9721
	public float TimeDelay = 1f;

	// Token: 0x040025FA RID: 9722
	private Renderer rend;

	// Token: 0x06002A6D RID: 10861 RVA: 0x00020F84 File Offset: 0x0001F184
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x00020F92 File Offset: 0x0001F192
	private void DeactivateRenderer()
	{
		this.rend.enabled = false;
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x00020FA0 File Offset: 0x0001F1A0
	private void OnEnable()
	{
		this.rend.enabled = true;
		base.Invoke("DeactivateRenderer", this.TimeDelay);
	}
}
