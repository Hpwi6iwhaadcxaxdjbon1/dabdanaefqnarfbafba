using System;
using UnityEngine;

// Token: 0x02000624 RID: 1572
public class EntityHUDRender : MonoBehaviour
{
	// Token: 0x04001F3C RID: 7996
	internal BaseEntity cachedEntity;

	// Token: 0x06002327 RID: 8999 RVA: 0x000BAD90 File Offset: 0x000B8F90
	private void OnWillRenderObject()
	{
		if (Camera.current != MainCamera.mainCamera)
		{
			return;
		}
		if (this.cachedEntity == null)
		{
			this.cachedEntity = base.gameObject.ToBaseEntity();
			if (this.cachedEntity == null)
			{
				return;
			}
		}
		this.cachedEntity.OnRendered();
	}
}
