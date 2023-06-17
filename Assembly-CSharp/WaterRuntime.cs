using System;
using UnityEngine;

// Token: 0x02000565 RID: 1381
public class WaterRuntime
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x06001F55 RID: 8021 RVA: 0x00018BDB File Offset: 0x00016DDB
	// (set) Token: 0x06001F54 RID: 8020 RVA: 0x00018BD2 File Offset: 0x00016DD2
	public WaterCamera WaterCamera { get; private set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x06001F57 RID: 8023 RVA: 0x00018BEC File Offset: 0x00016DEC
	// (set) Token: 0x06001F56 RID: 8022 RVA: 0x00018BE3 File Offset: 0x00016DE3
	public Camera Camera { get; private set; }

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06001F59 RID: 8025 RVA: 0x00018BFD File Offset: 0x00016DFD
	// (set) Token: 0x06001F58 RID: 8024 RVA: 0x00018BF4 File Offset: 0x00016DF4
	public PostOpaqueDepth PostOpaqueDepth { get; private set; }

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x06001F5B RID: 8027 RVA: 0x00018C0E File Offset: 0x00016E0E
	// (set) Token: 0x06001F5A RID: 8026 RVA: 0x00018C05 File Offset: 0x00016E05
	public CommandBufferManager CommandBufferManager { get; private set; }

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06001F5D RID: 8029 RVA: 0x00018C1F File Offset: 0x00016E1F
	// (set) Token: 0x06001F5C RID: 8028 RVA: 0x00018C16 File Offset: 0x00016E16
	public bool SimulateNextFrame { get; private set; }

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001F5F RID: 8031 RVA: 0x00018C30 File Offset: 0x00016E30
	// (set) Token: 0x06001F5E RID: 8030 RVA: 0x00018C27 File Offset: 0x00016E27
	public WaterSimulation Simulation { get; private set; }

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001F61 RID: 8033 RVA: 0x00018C41 File Offset: 0x00016E41
	// (set) Token: 0x06001F60 RID: 8032 RVA: 0x00018C38 File Offset: 0x00016E38
	public WaterRendering Rendering { get; private set; }

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001F63 RID: 8035 RVA: 0x00018C52 File Offset: 0x00016E52
	// (set) Token: 0x06001F62 RID: 8034 RVA: 0x00018C49 File Offset: 0x00016E49
	public int VisibilityMask { get; private set; } = 255;

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001F65 RID: 8037 RVA: 0x00018C63 File Offset: 0x00016E63
	// (set) Token: 0x06001F64 RID: 8036 RVA: 0x00018C5A File Offset: 0x00016E5A
	public bool IsInitialized { get; private set; }

	// Token: 0x06001F66 RID: 8038 RVA: 0x00018C6B File Offset: 0x00016E6B
	public WaterRuntime(WaterCamera camera)
	{
		this.WaterCamera = camera;
		this.Simulation = new WaterSimulation();
		this.Rendering = new WaterRendering();
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x000ABB70 File Offset: 0x000A9D70
	public void Initialize(WaterSystem water)
	{
		this.Camera = this.WaterCamera.Camera;
		this.PostOpaqueDepth = this.WaterCamera.PostOpaqueDepth;
		this.CommandBufferManager = this.WaterCamera.CommandBufferManager;
		if (water.Quality >= WaterQuality.Medium)
		{
			this.Simulation.Initialize(water, this);
		}
		this.Rendering.Initialize(water, this, WaterSystem.QualityToMaxVertices[(int)water.Quality]);
		CommandBufferManager commandBufferManager = this.CommandBufferManager;
		commandBufferManager.OnPreCullCall = (Action)Delegate.Remove(commandBufferManager.OnPreCullCall, new Action(this.IssueRender));
		CommandBufferManager commandBufferManager2 = this.CommandBufferManager;
		commandBufferManager2.OnPreCullCall = (Action)Delegate.Combine(commandBufferManager2.OnPreCullCall, new Action(this.IssueRender));
		this.SimulateNextFrame = true;
		this.IsInitialized = true;
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x000ABC3C File Offset: 0x000A9E3C
	public void Shutdown()
	{
		if (this.Rendering != null && this.Rendering.IsInitialized)
		{
			this.Rendering.Destroy();
		}
		if (this.Simulation != null && this.Simulation.IsInitialized)
		{
			this.Simulation.Destroy();
		}
		this.Camera = null;
		this.PostOpaqueDepth = null;
		if (this.CommandBufferManager != null)
		{
			CommandBufferManager commandBufferManager = this.CommandBufferManager;
			commandBufferManager.OnPreCullCall = (Action)Delegate.Remove(commandBufferManager.OnPreCullCall, new Action(this.IssueRender));
			this.CommandBufferManager = null;
		}
		this.IsInitialized = false;
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x00018C9B File Offset: 0x00016E9B
	public void SetVisibilityMask(int mask)
	{
		this.VisibilityMask = mask;
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x000ABCDC File Offset: 0x000A9EDC
	private void Simulate()
	{
		Debug.Assert(this.IsInitialized);
		if (this.Simulation.IsInitialized)
		{
			this.Simulation.Update();
			if (this.VisibilityMask != 0 || this.SimulateNextFrame)
			{
				if (!this.Simulation.IsPlaying)
				{
					this.Simulation.Play();
					return;
				}
			}
			else if (this.Simulation.IsPlaying)
			{
				this.Simulation.Stop();
			}
		}
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x00018CA4 File Offset: 0x00016EA4
	private void PrepareRender()
	{
		Debug.Assert(this.IsInitialized);
		if (this.Rendering.IsInitialized)
		{
			this.Rendering.PrepareRender(this.VisibilityMask);
		}
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x00018CCF File Offset: 0x00016ECF
	public void Update()
	{
		if (this.IsInitialized)
		{
			this.PostOpaqueDepth.Update();
			this.Simulate();
			this.PrepareRender();
		}
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x00018CF0 File Offset: 0x00016EF0
	public void IssueRender()
	{
		if (this.IsInitialized && this.Rendering.IsInitialized)
		{
			this.SimulateNextFrame = this.Rendering.IssueRender(false);
		}
	}
}
