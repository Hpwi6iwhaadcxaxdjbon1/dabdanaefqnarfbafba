using System;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public class ItemModDeployable : MonoBehaviour
{
	// Token: 0x0400172D RID: 5933
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x0400172E RID: 5934
	[Header("Tooltips")]
	public bool showCrosshair;

	// Token: 0x0400172F RID: 5935
	public string UnlockAchievement;

	// Token: 0x06001A5A RID: 6746 RVA: 0x00015CB0 File Offset: 0x00013EB0
	public Deployable GetDeployable(BaseEntity entity)
	{
		if (entity.gameManager.FindPrefab(this.entityPrefab.resourcePath) == null)
		{
			return null;
		}
		return entity.prefabAttribute.Find<Deployable>(this.entityPrefab.resourceID);
	}
}
