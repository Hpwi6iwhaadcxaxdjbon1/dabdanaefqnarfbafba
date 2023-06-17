using System;
using Rust;
using UnityEngine;

// Token: 0x02000775 RID: 1909
public class IronSightOverride : MonoBehaviour
{
	// Token: 0x0400249D RID: 9373
	public IronsightAimPoint aimPoint;

	// Token: 0x0400249E RID: 9374
	public float fieldOfViewOffset = -20f;

	// Token: 0x0400249F RID: 9375
	[Tooltip("If set to 1, the FOV is set to what this override is set to. If set to 0.5 it's half way between the weapon iconsights default and this scope.")]
	public float fovBias = 0.5f;

	// Token: 0x0600299B RID: 10651 RVA: 0x000D39C8 File Offset: 0x000D1BC8
	public void Update()
	{
		IronSights componentInParent = base.GetComponentInParent<IronSights>();
		if (componentInParent)
		{
			componentInParent.ironsightsOverride = this;
		}
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x000D39EC File Offset: 0x000D1BEC
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		IronSights componentInParent = base.GetComponentInParent<IronSights>();
		if (componentInParent && componentInParent.ironsightsOverride == this)
		{
			componentInParent.ironsightsOverride = null;
		}
	}
}
