using System;
using Rust;

// Token: 0x020001FA RID: 506
public class DecalRecycle : BasePrefab, IClientComponent, IEffectRecycle, IOnParentDestroying
{
	// Token: 0x04000CA2 RID: 3234
	public float LifeTime = 60f;

	// Token: 0x04000CA3 RID: 3235
	private Action recycleAction;

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0000DEF9 File Offset: 0x0000C0F9
	protected void Awake()
	{
		this.recycleAction = new Action(this.Recycle);
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0006B578 File Offset: 0x00069778
	protected void OnEnable()
	{
		if (Application.isLoadingPrefabs)
		{
			return;
		}
		DecalComponent[] components = PrefabAttribute.client.FindAll<DecalComponent>(this.prefabID);
		base.transform.ApplyDecalComponents(components);
		base.Invoke(this.recycleAction, this.LifeTime);
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0000DF0E File Offset: 0x0000C10E
	public void Recycle()
	{
		GameManager.client.Retire(base.gameObject);
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0000DF20 File Offset: 0x0000C120
	public void OnParentDestroying()
	{
		base.CancelInvoke(this.recycleAction);
		this.Recycle();
	}
}
