using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class FlashlightBeam : MonoBehaviour, IClientComponent
{
	// Token: 0x04000823 RID: 2083
	public Vector2 scrollDir;

	// Token: 0x04000824 RID: 2084
	public Vector3 localEndPoint = new Vector3(0f, 0f, 2f);

	// Token: 0x04000825 RID: 2085
	public LineRenderer beamRenderer;

	// Token: 0x04000826 RID: 2086
	private MaterialPropertyBlock block;

	// Token: 0x04000827 RID: 2087
	private Vector4 BeamST;

	// Token: 0x06000BA4 RID: 2980 RVA: 0x00059CF8 File Offset: 0x00057EF8
	public void OnEnable()
	{
		if (!this.beamRenderer)
		{
			Debug.LogErrorFormat("BeamRenderer on {0} was null!", new object[]
			{
				base.gameObject.name
			});
			return;
		}
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.BeamST = this.beamRenderer.sharedMaterial.GetVector("_ShadowTex_ST");
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x00059D60 File Offset: 0x00057F60
	private void LateUpdate()
	{
		if (!this.beamRenderer)
		{
			return;
		}
		if (this.beamRenderer)
		{
			float time = Time.time;
			if (this.block == null)
			{
				return;
			}
			Vector3 position = base.transform.position;
			Vector3 a = base.transform.localToWorldMatrix.MultiplyPoint(this.localEndPoint);
			Vector2 vector = this.scrollDir * time;
			this.block.Clear();
			this.block.SetVector("_PlanarFadeWorldVector", a - position);
			this.block.SetVector("_ShadowTex_ST", new Vector4(this.BeamST.x, this.BeamST.y, vector.x, vector.y));
			this.beamRenderer.SetPropertyBlock(this.block);
		}
	}
}
