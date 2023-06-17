using System;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x04000D29 RID: 3369
	public float destroyAfterSeconds;

	// Token: 0x06001056 RID: 4182 RVA: 0x0000E702 File Offset: 0x0000C902
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		base.GetComponent<ParticleSystem>().enableEmission = false;
		if (this.destroyAfterSeconds > 0f)
		{
			GameManager.Destroy(base.gameObject, this.destroyAfterSeconds);
		}
	}
}
