using System;
using UnityEngine;

// Token: 0x02000550 RID: 1360
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CommandBufferManager))]
[ExecuteInEditMode]
[RequireComponent(typeof(PostOpaqueDepth))]
public class WaterCamera : MonoBehaviour
{
	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001E86 RID: 7814 RVA: 0x000181E2 File Offset: 0x000163E2
	// (set) Token: 0x06001E85 RID: 7813 RVA: 0x000181D9 File Offset: 0x000163D9
	public Camera Camera { get; private set; }

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x06001E88 RID: 7816 RVA: 0x000181F3 File Offset: 0x000163F3
	// (set) Token: 0x06001E87 RID: 7815 RVA: 0x000181EA File Offset: 0x000163EA
	public CommandBufferManager CommandBufferManager { get; private set; }

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06001E8A RID: 7818 RVA: 0x00018204 File Offset: 0x00016404
	// (set) Token: 0x06001E89 RID: 7817 RVA: 0x000181FB File Offset: 0x000163FB
	public PostOpaqueDepth PostOpaqueDepth { get; private set; }

	// Token: 0x06001E8B RID: 7819 RVA: 0x000A7B1C File Offset: 0x000A5D1C
	private void CheckComponents()
	{
		this.Camera = ((this.Camera != null) ? this.Camera : base.GetComponent<Camera>());
		this.CommandBufferManager = ((this.CommandBufferManager != null) ? this.CommandBufferManager : base.GetComponent<CommandBufferManager>());
		this.PostOpaqueDepth = ((this.PostOpaqueDepth != null) ? this.PostOpaqueDepth : base.GetComponent<PostOpaqueDepth>());
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x0001820C File Offset: 0x0001640C
	private void Awake()
	{
		this.CheckComponents();
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x00018214 File Offset: 0x00016414
	private void OnEnable()
	{
		this.CheckComponents();
		WaterSystem.RegisterCamera(this);
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x00018222 File Offset: 0x00016422
	private void OnDisable()
	{
		WaterSystem.UnregisterCamera(this);
	}
}
