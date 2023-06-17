using System;
using UnityEngine;

// Token: 0x02000700 RID: 1792
public struct CachedTransform<T> where T : Component
{
	// Token: 0x04002350 RID: 9040
	public T component;

	// Token: 0x04002351 RID: 9041
	public Vector3 position;

	// Token: 0x04002352 RID: 9042
	public Quaternion rotation;

	// Token: 0x04002353 RID: 9043
	public Vector3 localScale;

	// Token: 0x06002767 RID: 10087 RVA: 0x000CC9AC File Offset: 0x000CABAC
	public CachedTransform(T instance)
	{
		this.component = instance;
		if (this.component)
		{
			this.position = this.component.transform.position;
			this.rotation = this.component.transform.rotation;
			this.localScale = this.component.transform.localScale;
			return;
		}
		this.position = Vector3.zero;
		this.rotation = Quaternion.identity;
		this.localScale = Vector3.one;
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000CCA48 File Offset: 0x000CAC48
	public void Apply()
	{
		if (this.component)
		{
			this.component.transform.SetPositionAndRotation(this.position, this.rotation);
			this.component.transform.localScale = this.localScale;
		}
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000CCAA4 File Offset: 0x000CACA4
	public void RotateAround(Vector3 center, Vector3 axis, float angle)
	{
		Quaternion rhs = Quaternion.AngleAxis(angle, axis);
		Vector3 b = rhs * (this.position - center);
		this.position = center + b;
		this.rotation *= Quaternion.Inverse(this.rotation) * rhs * this.rotation;
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x0600276A RID: 10090 RVA: 0x0001EBEF File Offset: 0x0001CDEF
	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.position, this.rotation, this.localScale);
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x0600276B RID: 10091 RVA: 0x000CCB08 File Offset: 0x000CAD08
	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			return this.localToWorldMatrix.inverse;
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x0600276C RID: 10092 RVA: 0x0001EC08 File Offset: 0x0001CE08
	public Vector3 forward
	{
		get
		{
			return this.rotation * Vector3.forward;
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x0600276D RID: 10093 RVA: 0x0001EC1A File Offset: 0x0001CE1A
	public Vector3 up
	{
		get
		{
			return this.rotation * Vector3.up;
		}
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x0600276E RID: 10094 RVA: 0x0001EC2C File Offset: 0x0001CE2C
	public Vector3 right
	{
		get
		{
			return this.rotation * Vector3.right;
		}
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x0001EC3E File Offset: 0x0001CE3E
	public static implicit operator bool(CachedTransform<T> instance)
	{
		return instance.component != null;
	}
}
