using System;
using UnityEngine;

// Token: 0x02000784 RID: 1924
public class ViewmodelLower : MonoBehaviour
{
	// Token: 0x040024E8 RID: 9448
	private bool shouldLower = true;

	// Token: 0x040024E9 RID: 9449
	internal float rotateAngle;

	// Token: 0x060029D5 RID: 10709 RVA: 0x000207CD File Offset: 0x0001E9CD
	public void SetShouldLower(bool shouldLower)
	{
		this.shouldLower = shouldLower;
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x000D4F3C File Offset: 0x000D313C
	private void Update()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (entity == null)
		{
			return;
		}
		bool flag = this.ShouldLower(entity);
		this.rotateAngle = Mathf.Lerp(this.rotateAngle, flag ? 25f : 0f, Time.deltaTime * 20f);
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x000207D6 File Offset: 0x0001E9D6
	private bool ShouldLower(BasePlayer player)
	{
		return this.shouldLower && (!player.CanAttack() || player.IsRunning());
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x000207F2 File Offset: 0x0001E9F2
	public void Apply(ref CachedTransform<BaseViewModel> vm)
	{
		vm.rotation *= Quaternion.Euler(this.rotateAngle, 0f, 0f);
	}
}
