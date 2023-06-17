using System;
using Rust;

// Token: 0x02000384 RID: 900
public class EffectRecycleLite : BasePrefab, IClientComponent, IEffectRecycle, IOnParentDestroying
{
	// Token: 0x040013B6 RID: 5046
	private const float lifeTime = 60f;

	// Token: 0x040013B7 RID: 5047
	private Action recycleAction;

	// Token: 0x060016E3 RID: 5859 RVA: 0x000134A2 File Offset: 0x000116A2
	protected void Awake()
	{
		this.recycleAction = new Action(this.Recycle);
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x000134B7 File Offset: 0x000116B7
	protected void OnEnable()
	{
		if (Application.isLoadingPrefabs)
		{
			return;
		}
		base.Invoke(this.recycleAction, 60f);
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x0000DF0E File Offset: 0x0000C10E
	public void Recycle()
	{
		GameManager.client.Retire(base.gameObject);
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x000134D2 File Offset: 0x000116D2
	public void OnParentDestroying()
	{
		base.CancelInvoke(this.recycleAction);
		this.Recycle();
	}
}
