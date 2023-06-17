using System;
using Rust;
using UnityEngine;

// Token: 0x020005B5 RID: 1461
[RequireComponent(typeof(WindZone))]
[ExecuteInEditMode]
public class WindZoneEx : MonoBehaviour
{
	// Token: 0x04001D7E RID: 7550
	private WindZone windZone;

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x060021A5 RID: 8613 RVA: 0x0001AC57 File Offset: 0x00018E57
	public WindZoneMode Mode
	{
		get
		{
			return this.windZone.mode;
		}
	}

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x060021A6 RID: 8614 RVA: 0x0001AC64 File Offset: 0x00018E64
	public float Radius
	{
		get
		{
			return this.windZone.radius;
		}
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x0001AC71 File Offset: 0x00018E71
	private void Awake()
	{
		this.windZone = base.GetComponent<WindZone>();
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x0001AC7F File Offset: 0x00018E7F
	private void OnEnable()
	{
		if (this.windZone != null)
		{
			WindZoneExManager.Register(this);
		}
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x0001AC95 File Offset: 0x00018E95
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		WindZoneExManager.Unregister(this);
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000B6538 File Offset: 0x000B4738
	public Vector4 PackInfo()
	{
		if (this.Mode == null)
		{
			Vector3 forward = base.transform.forward;
			return new Vector4(forward.x, forward.y, forward.z, 0f);
		}
		Vector3 position = base.transform.position;
		return new Vector4(position.x, position.y, position.z, this.Radius);
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x0001ACA5 File Offset: 0x00018EA5
	public Vector4 PackParam()
	{
		return new Vector4(this.windZone.windMain, this.windZone.windTurbulence, this.windZone.windPulseMagnitude, this.windZone.windPulseFrequency);
	}
}
