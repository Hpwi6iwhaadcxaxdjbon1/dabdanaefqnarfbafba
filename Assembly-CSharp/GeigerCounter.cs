using System;
using UnityEngine;

// Token: 0x020002A0 RID: 672
public class GeigerCounter : AttackEntity
{
	// Token: 0x04000F8A RID: 3978
	public Transform geigerRod;

	// Token: 0x04000F8B RID: 3979
	private BasePlayer playerCache;

	// Token: 0x04000F8C RID: 3980
	private Transform attachmentBoneCache;

	// Token: 0x060012E8 RID: 4840 RVA: 0x0007A6CC File Offset: 0x000788CC
	private void LateUpdate()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (this.geigerRod != null && ownerPlayer.model != null)
		{
			if (this.attachmentBoneCache == null || this.playerCache != ownerPlayer)
			{
				this.playerCache = ownerPlayer;
				this.attachmentBoneCache = ownerPlayer.model.FindBone("l_prop");
			}
			if (this.attachmentBoneCache != null)
			{
				this.geigerRod.position = this.attachmentBoneCache.position;
				this.geigerRod.rotation = this.attachmentBoneCache.rotation;
			}
		}
	}
}
