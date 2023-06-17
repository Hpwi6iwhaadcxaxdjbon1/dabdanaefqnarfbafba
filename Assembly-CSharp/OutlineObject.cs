using System;
using UnityEngine;

// Token: 0x02000422 RID: 1058
public class OutlineObject : MonoBehaviour, IClientComponent
{
	// Token: 0x04001610 RID: 5648
	public Mesh[] meshes;

	// Token: 0x04001611 RID: 5649
	public Transform[] meshTransforms;

	// Token: 0x04001612 RID: 5650
	private Collider _col;

	// Token: 0x06001986 RID: 6534 RVA: 0x0009068C File Offset: 0x0008E88C
	public virtual float SampleVisibility()
	{
		Camera mainCamera = MainCamera.mainCamera;
		Vector3 normalized = (base.transform.position - mainCamera.transform.position).normalized;
		float num = Vector3.Distance(mainCamera.transform.position, base.transform.position);
		RaycastHit raycastHit;
		if (!Physics.Raycast(new Ray(mainCamera.transform.position, normalized), ref raycastHit, num * 0.95f, 1218652417))
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x00015167 File Offset: 0x00013367
	public Collider GetCollider()
	{
		if (this._col == null)
		{
			this._col = base.GetComponent<Collider>();
			if (this._col == null)
			{
				this._col = base.GetComponentInChildren<Collider>();
			}
		}
		return this._col;
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void BecomeVisible()
	{
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void BecomeInvisible()
	{
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x000151A3 File Offset: 0x000133A3
	public virtual Color GetColor()
	{
		return Color.white;
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void Registered()
	{
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x000151AA File Offset: 0x000133AA
	public virtual bool ShouldDisplay()
	{
		return this.meshTransforms != null && this.meshTransforms.Length != 0 && !(this.meshTransforms[0] == null);
	}
}
