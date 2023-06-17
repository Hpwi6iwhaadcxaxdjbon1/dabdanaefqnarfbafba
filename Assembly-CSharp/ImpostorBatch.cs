using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class ImpostorBatch
{
	// Token: 0x04001CBF RID: 7359
	public SimpleList<Vector4> Positions;

	// Token: 0x04001CC1 RID: 7361
	private uint[] args = new uint[5];

	// Token: 0x04001CC2 RID: 7362
	private Queue<int> recycle = new Queue<int>(32);

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x060020E0 RID: 8416 RVA: 0x0001A194 File Offset: 0x00018394
	// (set) Token: 0x060020DF RID: 8415 RVA: 0x0001A18B File Offset: 0x0001838B
	public Mesh Mesh { get; private set; }

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x060020E2 RID: 8418 RVA: 0x0001A1A5 File Offset: 0x000183A5
	// (set) Token: 0x060020E1 RID: 8417 RVA: 0x0001A19C File Offset: 0x0001839C
	public Material Material { get; private set; }

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x060020E4 RID: 8420 RVA: 0x0001A1B6 File Offset: 0x000183B6
	// (set) Token: 0x060020E3 RID: 8419 RVA: 0x0001A1AD File Offset: 0x000183AD
	public int DeferredPass { get; private set; } = -1;

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x060020E6 RID: 8422 RVA: 0x0001A1C7 File Offset: 0x000183C7
	// (set) Token: 0x060020E5 RID: 8421 RVA: 0x0001A1BE File Offset: 0x000183BE
	public int ShadowPass { get; private set; } = -1;

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x060020E8 RID: 8424 RVA: 0x0001A1D8 File Offset: 0x000183D8
	// (set) Token: 0x060020E7 RID: 8423 RVA: 0x0001A1CF File Offset: 0x000183CF
	public ComputeBuffer PositionBuffer { get; private set; }

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x060020EA RID: 8426 RVA: 0x0001A1E9 File Offset: 0x000183E9
	// (set) Token: 0x060020E9 RID: 8425 RVA: 0x0001A1E0 File Offset: 0x000183E0
	public ComputeBuffer ArgsBuffer { get; private set; }

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x060020EB RID: 8427 RVA: 0x0001A1F1 File Offset: 0x000183F1
	public int Count
	{
		get
		{
			return this.Positions.Count;
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x060020EC RID: 8428 RVA: 0x0001A1FE File Offset: 0x000183FE
	public bool Visible
	{
		get
		{
			return this.Positions.Count - this.recycle.Count > 0;
		}
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x00018739 File Offset: 0x00016939
	private ComputeBuffer SafeRelease(ComputeBuffer buffer)
	{
		if (buffer != null)
		{
			buffer.Release();
		}
		return null;
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x000B1D00 File Offset: 0x000AFF00
	public void Initialize(Mesh mesh, Material material)
	{
		this.Mesh = mesh;
		this.Material = material;
		this.DeferredPass = material.FindPass("DEFERRED");
		this.ShadowPass = material.FindPass("SHADOWCASTER");
		this.Positions = Pool.Get<SimpleList<Vector4>>();
		this.Positions.Clear();
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
		this.ArgsBuffer = new ComputeBuffer(1, this.args.Length * 4, ComputeBufferType.DrawIndirect);
		this.args[0] = this.Mesh.GetIndexCount(0);
		this.args[2] = this.Mesh.GetIndexStart(0);
		this.args[3] = this.Mesh.GetBaseVertex(0);
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x0001A21A File Offset: 0x0001841A
	public void Release()
	{
		this.recycle.Clear();
		Pool.Free<SimpleList<Vector4>>(ref this.Positions);
		this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x000B1DBC File Offset: 0x000AFFBC
	public void AddInstance(ImpostorInstanceData data)
	{
		data.Batch = this;
		if (this.recycle.Count > 0)
		{
			data.BatchIndex = this.recycle.Dequeue();
			this.Positions[data.BatchIndex] = data.PositionAndScale();
			return;
		}
		data.BatchIndex = this.Positions.Count;
		this.Positions.Add(data.PositionAndScale());
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x000B1E2C File Offset: 0x000B002C
	public void RemoveInstance(ImpostorInstanceData data)
	{
		this.Positions[data.BatchIndex] = new Vector4(0f, 0f, 0f, -1f);
		this.recycle.Enqueue(data.BatchIndex);
		data.BatchIndex = 0;
		data.Batch = null;
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x000B1E84 File Offset: 0x000B0084
	public void UpdateBuffers()
	{
		bool flag = false;
		if (this.PositionBuffer == null || this.PositionBuffer.count != this.Positions.Count)
		{
			this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
			this.PositionBuffer = new ComputeBuffer(this.Positions.Count, 16);
			flag = true;
		}
		if (this.PositionBuffer != null)
		{
			this.PositionBuffer.SetData(this.Positions.Array, 0, 0, this.Positions.Count);
		}
		if (this.ArgsBuffer != null && flag)
		{
			this.args[1] = (uint)this.Positions.Count;
			this.ArgsBuffer.SetData(this.args);
		}
	}
}
