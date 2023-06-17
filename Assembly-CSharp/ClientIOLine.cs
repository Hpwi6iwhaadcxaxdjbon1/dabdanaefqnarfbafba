using System;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class ClientIOLine : MonoBehaviour
{
	// Token: 0x0400081E RID: 2078
	public LODComponent _lod;

	// Token: 0x0400081F RID: 2079
	public LineRenderer _line;

	// Token: 0x06000B99 RID: 2969 RVA: 0x0000B0F7 File Offset: 0x000092F7
	public void Clear()
	{
		this._line.positionCount = 0;
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x0000B105 File Offset: 0x00009305
	public void SetVisible(bool isVisible)
	{
		if (this._lod)
		{
			this._lod.SetVisible(isVisible);
			return;
		}
		this._line.enabled = isVisible;
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x0000B12D File Offset: 0x0000932D
	public void SetPositions(Vector3[] positions)
	{
		this._line.positionCount = positions.Length;
		this._line.SetPositions(positions);
		this.UpdateBoundsAndPosition();
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x00059B70 File Offset: 0x00057D70
	public void AddPosition(Vector3 pos)
	{
		LineRenderer line = this._line;
		int positionCount = line.positionCount;
		line.positionCount = positionCount + 1;
		this._line.SetPosition(this._line.positionCount - 1, pos);
		this.UpdateBoundsAndPosition();
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0000B14F File Offset: 0x0000934F
	public void SetLastNodePosition(Vector3 pos)
	{
		this._line.SetPosition(this._line.positionCount - 1, pos);
		this.UpdateBoundsAndPosition();
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x00059BB4 File Offset: 0x00057DB4
	public float GetLength()
	{
		float num = 0f;
		if (this._line.positionCount > 1)
		{
			Vector3 a = this._line.GetPosition(0);
			for (int i = 1; i < this._line.positionCount; i++)
			{
				Vector3 position = this._line.GetPosition(i);
				num += Vector3.Distance(a, position);
				a = position;
			}
		}
		return num;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00059C14 File Offset: 0x00057E14
	public void UpdateBoundsAndPosition()
	{
		base.transform.position = this._line.bounds.center;
		this._lod.ResetCulling();
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0000B170 File Offset: 0x00009370
	public Vector3 GetLastPlacedNodePosition()
	{
		if (this._line.positionCount - 1 >= 0)
		{
			return this._line.GetPosition(this._line.positionCount - 1);
		}
		return Vector3.zero;
	}
}
