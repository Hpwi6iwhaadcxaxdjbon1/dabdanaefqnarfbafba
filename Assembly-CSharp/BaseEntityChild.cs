using System;
using Rust;
using Rust.Registry;
using UnityEngine;

// Token: 0x020002A4 RID: 676
public class BaseEntityChild : MonoBehaviour
{
	// Token: 0x060012FA RID: 4858 RVA: 0x0007ABB0 File Offset: 0x00078DB0
	public static void Setup(GameObject obj, BaseEntity parent)
	{
		using (TimeWarning.New("Registry.Entity.Register", 0.1f))
		{
			Entity.Register(obj, parent);
		}
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x0007ABF0 File Offset: 0x00078DF0
	public void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		using (TimeWarning.New("Registry.Entity.Unregister", 0.1f))
		{
			Entity.Unregister(base.gameObject);
		}
	}
}
