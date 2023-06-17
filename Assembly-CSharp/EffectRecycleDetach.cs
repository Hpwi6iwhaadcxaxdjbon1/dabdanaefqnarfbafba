using System;
using Rust;
using UnityEngine.SceneManagement;

// Token: 0x02000383 RID: 899
public class EffectRecycleDetach : BaseMonoBehaviour, IClientComponent, IEffectRecycle, IOnParentDestroying
{
	// Token: 0x040013B4 RID: 5044
	public float recycleTime = 1f;

	// Token: 0x040013B5 RID: 5045
	private Action recycleAction;

	// Token: 0x060016DE RID: 5854 RVA: 0x00013460 File Offset: 0x00011660
	protected void Awake()
	{
		this.recycleAction = new Action(this.Recycle);
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x0000DF0E File Offset: 0x0000C10E
	public void Recycle()
	{
		GameManager.client.Retire(base.gameObject);
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x000133C0 File Offset: 0x000115C0
	private void DetachFromParent()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
		SceneManager.MoveGameObjectToScene(base.gameObject, Rust.Client.EffectScene);
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x00013475 File Offset: 0x00011675
	public void OnParentDestroying()
	{
		this.DetachFromParent();
		base.Invoke(this.recycleAction, this.recycleTime);
	}
}
