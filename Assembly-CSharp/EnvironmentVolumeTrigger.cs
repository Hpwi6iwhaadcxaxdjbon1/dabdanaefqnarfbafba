using System;
using UnityEngine;

// Token: 0x02000389 RID: 905
public class EnvironmentVolumeTrigger : MonoBehaviour
{
	// Token: 0x040013C4 RID: 5060
	[HideInInspector]
	public Vector3 Center = Vector3.zero;

	// Token: 0x040013C5 RID: 5061
	[HideInInspector]
	public Vector3 Size = Vector3.one;

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x060016F8 RID: 5880 RVA: 0x00013581 File Offset: 0x00011781
	// (set) Token: 0x060016F9 RID: 5881 RVA: 0x00013589 File Offset: 0x00011789
	public EnvironmentVolume volume { get; private set; }

	// Token: 0x060016FA RID: 5882 RVA: 0x00088A00 File Offset: 0x00086C00
	protected void Awake()
	{
		this.volume = base.gameObject.GetComponent<EnvironmentVolume>();
		if (this.volume == null)
		{
			this.volume = base.gameObject.AddComponent<EnvironmentVolume>();
			this.volume.Center = this.Center;
			this.volume.Size = this.Size;
			this.volume.UpdateTrigger();
		}
	}
}
