using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020005A1 RID: 1441
[RequireComponent(typeof(Light))]
public class ImpostorShadows : MonoBehaviour
{
	// Token: 0x04001CCD RID: 7373
	private Light light;

	// Token: 0x04001CCE RID: 7374
	private const string commandBufferName = "ImpostorShadows";

	// Token: 0x04001CCF RID: 7375
	private LightEvent commandBufferEvent = LightEvent.AfterShadowMapPass;

	// Token: 0x04001CD0 RID: 7376
	public CommandBuffer commandBuffer;

	// Token: 0x0600210A RID: 8458 RVA: 0x000B2370 File Offset: 0x000B0570
	public static bool TryToggle(bool state)
	{
		bool result = false;
		if (TOD_Sky.Instance != null)
		{
			ImpostorShadows component = TOD_Sky.Instance.Components.Light.gameObject.GetComponent<ImpostorShadows>();
			if (component != null)
			{
				component.enabled = state;
				result = state;
			}
		}
		return result;
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x0001A3DF File Offset: 0x000185DF
	private void Awake()
	{
		this.light = base.GetComponent<Light>();
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x000B23BC File Offset: 0x000B05BC
	private void OnEnable()
	{
		if (ImpostorRenderer.IsSupported && this.light.type == LightType.Directional)
		{
			ImpostorRenderer.Register(this);
			this.CreateCommandBuffer();
			return;
		}
		if (this.light.type != LightType.Directional)
		{
			Debug.LogWarning("[ImpostorShadows] Only directional lights are supported at the moment.");
		}
		base.enabled = false;
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x0001A3ED File Offset: 0x000185ED
	private void OnDisable()
	{
		this.DestroyCommandBuffer();
		ImpostorRenderer.Unregister(this);
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x0001A3FB File Offset: 0x000185FB
	private void Update()
	{
		if (ImpostorRenderer.Instance != null && ImpostorRenderer.Instance.enabled)
		{
			this.CreateCommandBuffer();
			this.commandBuffer.Clear();
			return;
		}
		this.DestroyCommandBuffer();
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x0001A42E File Offset: 0x0001862E
	private void CreateCommandBuffer()
	{
		if (this.commandBuffer == null)
		{
			this.commandBuffer = new CommandBuffer();
			this.commandBuffer.name = "ImpostorShadows";
			this.light.AddCommandBuffer(this.commandBufferEvent, this.commandBuffer);
		}
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000B240C File Offset: 0x000B060C
	private void DestroyCommandBuffer()
	{
		if (this.commandBuffer != null)
		{
			CommandBuffer[] commandBuffers = this.light.GetCommandBuffers(this.commandBufferEvent);
			for (int i = 0; i < commandBuffers.Length; i++)
			{
				if (commandBuffers[i].name == "ImpostorShadows")
				{
					this.light.RemoveCommandBuffer(this.commandBufferEvent, commandBuffers[i]);
				}
			}
			this.commandBuffer.Dispose();
			this.commandBuffer = null;
		}
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x0001A46A File Offset: 0x0001866A
	public void IssueDrawBatch(ImpostorBatch batch)
	{
		if (this.commandBuffer != null)
		{
			this.commandBuffer.DrawMeshInstancedIndirect(batch.Mesh, 0, batch.Material, batch.ShadowPass, batch.ArgsBuffer, 0);
		}
	}
}
