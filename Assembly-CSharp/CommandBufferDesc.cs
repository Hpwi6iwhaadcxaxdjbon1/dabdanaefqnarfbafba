using System;
using UnityEngine.Rendering;

// Token: 0x0200057B RID: 1403
public class CommandBufferDesc
{
	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06002011 RID: 8209 RVA: 0x000196CA File Offset: 0x000178CA
	// (set) Token: 0x06002012 RID: 8210 RVA: 0x000196D2 File Offset: 0x000178D2
	public CameraEvent CameraEvent { get; private set; }

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06002013 RID: 8211 RVA: 0x000196DB File Offset: 0x000178DB
	// (set) Token: 0x06002014 RID: 8212 RVA: 0x000196E3 File Offset: 0x000178E3
	public int OrderId { get; private set; }

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06002015 RID: 8213 RVA: 0x000196EC File Offset: 0x000178EC
	// (set) Token: 0x06002016 RID: 8214 RVA: 0x000196F4 File Offset: 0x000178F4
	public Action<CommandBuffer> FillDelegate { get; private set; }

	// Token: 0x06002017 RID: 8215 RVA: 0x000196FD File Offset: 0x000178FD
	public CommandBufferDesc(CameraEvent cameraEvent, int orderId, CommandBufferDesc.FillCommandBuffer fill)
	{
		this.CameraEvent = cameraEvent;
		this.OrderId = orderId;
		this.FillDelegate = new Action<CommandBuffer>(fill.Invoke);
	}

	// Token: 0x0200057C RID: 1404
	// (Invoke) Token: 0x06002019 RID: 8217
	public delegate void FillCommandBuffer(CommandBuffer cb);
}
