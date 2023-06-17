using System;
using Rust;
using UnityEngine;

// Token: 0x02000576 RID: 1398
[ExecuteInEditMode]
public class AtmosphereVolume : MonoBehaviour
{
	// Token: 0x04001BFD RID: 7165
	public bool StickyGizmos;

	// Token: 0x04001BFE RID: 7166
	public float MaxVisibleDistance = 750f;

	// Token: 0x04001BFF RID: 7167
	public float BoundsAttenuationDecay = 5f;

	// Token: 0x04001C00 RID: 7168
	public FogSettings FogSettings = FogSettings.Default;

	// Token: 0x06001FF1 RID: 8177 RVA: 0x0001949E File Offset: 0x0001769E
	private void OnEnable()
	{
		AtmosphereVolumeRenderer.Register(this);
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000194A6 File Offset: 0x000176A6
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		AtmosphereVolumeRenderer.Unregister(this);
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000194B6 File Offset: 0x000176B6
	protected void OnDrawGizmos()
	{
		if (this.StickyGizmos)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x000194C6 File Offset: 0x000176C6
	protected void OnDrawGizmosSelected()
	{
		if (!this.StickyGizmos)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x000AE9D4 File Offset: 0x000ACBD4
	private void DrawGizmos()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 lossyScale = base.transform.lossyScale;
		Gizmos.color = new Color(0.5f, 0.5f, 0.75f, 0.5f);
		GizmosUtil.DrawCube(position, lossyScale, rotation);
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		GizmosUtil.DrawWireCube(position, lossyScale, rotation);
	}
}
