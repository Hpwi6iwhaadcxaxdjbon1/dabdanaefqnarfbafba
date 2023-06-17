using System;
using UnityEngine;

// Token: 0x020007A7 RID: 1959
public class ExplosionsShaderQueue : MonoBehaviour
{
	// Token: 0x0400261E RID: 9758
	public int AddQueue = 1;

	// Token: 0x0400261F RID: 9759
	private Renderer rend;

	// Token: 0x06002A84 RID: 10884 RVA: 0x000D8E88 File Offset: 0x000D7088
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue += this.AddQueue;
			return;
		}
		base.Invoke("SetProjectorQueue", 0.1f);
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x000210EC File Offset: 0x0001F2EC
	private void SetProjectorQueue()
	{
		base.GetComponent<Projector>().material.renderQueue += this.AddQueue;
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x0002110B File Offset: 0x0001F30B
	private void OnDisable()
	{
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue = -1;
		}
	}
}
