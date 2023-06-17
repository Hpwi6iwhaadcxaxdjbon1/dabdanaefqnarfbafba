using System;
using UnityEngine;

// Token: 0x0200079E RID: 1950
public class ExplosionPlatformActivator : MonoBehaviour
{
	// Token: 0x040025E7 RID: 9703
	public GameObject Effect;

	// Token: 0x040025E8 RID: 9704
	public float TimeDelay;

	// Token: 0x040025E9 RID: 9705
	public float DefaultRepeatTime = 5f;

	// Token: 0x040025EA RID: 9706
	public float NearRepeatTime = 3f;

	// Token: 0x040025EB RID: 9707
	private float currentTime;

	// Token: 0x040025EC RID: 9708
	private float currentRepeatTime;

	// Token: 0x040025ED RID: 9709
	private bool canUpdate;

	// Token: 0x06002A60 RID: 10848 RVA: 0x00020E7E File Offset: 0x0001F07E
	private void Start()
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
		base.Invoke("Init", this.TimeDelay);
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x00020E9D File Offset: 0x0001F09D
	private void Init()
	{
		this.canUpdate = true;
		this.Effect.SetActive(true);
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000D8874 File Offset: 0x000D6A74
	private void Update()
	{
		if (!this.canUpdate || this.Effect == null)
		{
			return;
		}
		this.currentTime += Time.deltaTime;
		if (this.currentTime > this.currentRepeatTime)
		{
			this.currentTime = 0f;
			this.Effect.SetActive(false);
			this.Effect.SetActive(true);
		}
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x00020EB2 File Offset: 0x0001F0B2
	private void OnTriggerEnter(Collider coll)
	{
		this.currentRepeatTime = this.NearRepeatTime;
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x00020EC0 File Offset: 0x0001F0C0
	private void OnTriggerExit(Collider other)
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
	}
}
