using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020005A0 RID: 1440
[RequireComponent(typeof(CommandBufferManager))]
[RequireComponent(typeof(Camera))]
public class ImpostorRenderer : MonoBehaviour
{
	// Token: 0x04001CC3 RID: 7363
	private Camera camera;

	// Token: 0x04001CC4 RID: 7364
	private CommandBufferManager commandBufferManager;

	// Token: 0x04001CC5 RID: 7365
	private CommandBufferDesc commandBufferDesc;

	// Token: 0x04001CC6 RID: 7366
	private static HashSet<ImpostorShadows> Shadows = new HashSet<ImpostorShadows>();

	// Token: 0x04001CC7 RID: 7367
	private static HashSet<Impostor> Impostors = new HashSet<Impostor>();

	// Token: 0x04001CC8 RID: 7368
	private static Dictionary<ImpostorInstanceData, ImpostorBatch> Batches = new Dictionary<ImpostorInstanceData, ImpostorBatch>();

	// Token: 0x04001CC9 RID: 7369
	private static MaterialPropertyBlock block = null;

	// Token: 0x04001CCA RID: 7370
	private int treeLayerMask;

	// Token: 0x04001CCB RID: 7371
	private int impostorLayerMask;

	// Token: 0x04001CCC RID: 7372
	private static ImpostorRenderer instance;

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x060020F4 RID: 8436 RVA: 0x000B1F3C File Offset: 0x000B013C
	private Camera Camera
	{
		get
		{
			return this.camera = ((this.camera != null) ? this.camera : base.GetComponent<Camera>());
		}
	}

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x060020F5 RID: 8437 RVA: 0x000B1F70 File Offset: 0x000B0170
	private CommandBufferManager CommandBufferManager
	{
		get
		{
			return this.commandBufferManager = ((this.commandBufferManager != null) ? this.commandBufferManager : base.GetComponent<CommandBufferManager>());
		}
	}

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x060020F6 RID: 8438 RVA: 0x0001A285 File Offset: 0x00018485
	public static MaterialPropertyBlock Block
	{
		get
		{
			return ImpostorRenderer.block = ((ImpostorRenderer.block != null) ? ImpostorRenderer.block : new MaterialPropertyBlock());
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x060020F7 RID: 8439 RVA: 0x0001A2A0 File Offset: 0x000184A0
	public static bool IsSupported
	{
		get
		{
			return SystemInfo.supportsInstancing;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x060020F8 RID: 8440 RVA: 0x0001A2A7 File Offset: 0x000184A7
	public static ImpostorRenderer Instance
	{
		get
		{
			return ImpostorRenderer.instance;
		}
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x0001A2AE File Offset: 0x000184AE
	private void CheckInstance()
	{
		ImpostorRenderer.instance = ((ImpostorRenderer.instance != null) ? ImpostorRenderer.instance : this);
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000B1FA4 File Offset: 0x000B01A4
	public static void Clear()
	{
		if (ImpostorRenderer.Impostors != null)
		{
			foreach (Impostor impostor in ImpostorRenderer.Impostors)
			{
				ImpostorRenderer.UnregisterBatching(impostor);
			}
			ImpostorRenderer.Impostors.Clear();
		}
		if (ImpostorRenderer.Batches != null)
		{
			foreach (KeyValuePair<ImpostorInstanceData, ImpostorBatch> keyValuePair in ImpostorRenderer.Batches)
			{
				ImpostorBatch value = keyValuePair.Value;
				value.Release();
				Pool.Free<ImpostorBatch>(ref value);
			}
			ImpostorRenderer.Batches.Clear();
		}
		if (ImpostorRenderer.Shadows != null)
		{
			ImpostorRenderer.Shadows.Clear();
		}
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x0001A2CA File Offset: 0x000184CA
	public static void Register(ImpostorShadows shadows)
	{
		if (ImpostorRenderer.Shadows != null)
		{
			ImpostorRenderer.Shadows.Add(shadows);
		}
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x0001A2DF File Offset: 0x000184DF
	public static void Unregister(ImpostorShadows shadows)
	{
		if (ImpostorRenderer.Shadows != null)
		{
			ImpostorRenderer.Shadows.Remove(shadows);
		}
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x0001A2F4 File Offset: 0x000184F4
	public static void Register(Impostor impostor)
	{
		if (ImpostorRenderer.Impostors != null)
		{
			ImpostorRenderer.Impostors.Add(impostor);
			ImpostorRenderer.RegisterBatching(impostor);
		}
	}

	// Token: 0x060020FE RID: 8446 RVA: 0x0001A30F File Offset: 0x0001850F
	public static void Unregister(Impostor impostor)
	{
		if (ImpostorRenderer.Impostors != null)
		{
			ImpostorRenderer.UnregisterBatching(impostor);
			ImpostorRenderer.Impostors.Remove(impostor);
		}
	}

	// Token: 0x060020FF RID: 8447 RVA: 0x000B2078 File Offset: 0x000B0278
	private static void RegisterBatching(Impostor impostor)
	{
		ImpostorInstanceData instanceData = impostor.InstanceData;
		ImpostorBatch impostorBatch = null;
		if (!ImpostorRenderer.Batches.TryGetValue(instanceData, ref impostorBatch))
		{
			impostorBatch = Pool.Get<ImpostorBatch>();
			impostorBatch.Initialize(instanceData.Mesh, instanceData.Material);
			ImpostorRenderer.Batches.Add(instanceData, impostorBatch);
		}
		impostorBatch.AddInstance(instanceData);
	}

	// Token: 0x06002100 RID: 8448 RVA: 0x000B20C8 File Offset: 0x000B02C8
	private static void UnregisterBatching(Impostor impostor)
	{
		ImpostorInstanceData instanceData = impostor.InstanceData;
		ImpostorBatch impostorBatch = null;
		if (ImpostorRenderer.Batches.TryGetValue(instanceData, ref impostorBatch))
		{
			impostorBatch.RemoveInstance(instanceData);
			return;
		}
		instanceData.Batch = null;
		instanceData.BatchIndex = 0;
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x0001A32A File Offset: 0x0001852A
	private void Awake()
	{
		this.CheckInstance();
		this.treeLayerMask = 1 << LayerMask.NameToLayer("Tree");
		this.impostorLayerMask = 1 << LayerMask.NameToLayer("Impostor");
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000B2104 File Offset: 0x000B0304
	private void OnEnable()
	{
		if (ImpostorRenderer.IsSupported)
		{
			this.CheckInstance();
			this.CheckCommandBuffer();
			this.Camera.cullingMask &= ~this.impostorLayerMask;
			return;
		}
		Debug.LogWarning("[ImpostorRenderer] Disabled due to unsupported Instancing on device " + SystemInfo.graphicsDeviceType + ". Falling back to standard impostor rendering.");
		ImpostorRenderer.Shadows = null;
		ImpostorRenderer.Impostors = null;
		ImpostorRenderer.Batches = null;
		base.enabled = false;
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x0001A35C File Offset: 0x0001855C
	private void OnDisable()
	{
		this.CleanupCommandBuffer();
		this.Camera.cullingMask |= this.impostorLayerMask;
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x0001A37C File Offset: 0x0001857C
	public void OnPreRender()
	{
		this.CheckCommandBuffer();
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x000B2178 File Offset: 0x000B0378
	private void CheckCommandBuffer()
	{
		if (this.commandBufferManager == null)
		{
			this.commandBufferManager = base.GetComponent<CommandBufferManager>();
		}
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferDesc = ((this.commandBufferDesc == null) ? new CommandBufferDesc(CameraEvent.AfterGBuffer, 100, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer)) : this.commandBufferDesc);
			this.commandBufferManager.AddCommands(this.commandBufferDesc);
		}
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x0001A384 File Offset: 0x00018584
	private void CleanupCommandBuffer()
	{
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferManager.RemoveCommands(this.commandBufferDesc);
			this.commandBufferManager = null;
		}
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000B21F8 File Offset: 0x000B03F8
	private void FillCommandBuffer(CommandBuffer cb)
	{
		if ((this.camera.cullingMask & this.treeLayerMask) != 0)
		{
			ImpostorRenderer.Block.Clear();
			ImpostorRenderer.Block.CopySHCoefficientArraysFrom(MainCamera.LightProbe);
			cb.SetGlobalFloat("_NaN", float.NaN);
			foreach (KeyValuePair<ImpostorInstanceData, ImpostorBatch> keyValuePair in ImpostorRenderer.Batches)
			{
				keyValuePair.Value.UpdateBuffers();
			}
			foreach (KeyValuePair<ImpostorInstanceData, ImpostorBatch> keyValuePair2 in ImpostorRenderer.Batches)
			{
				ImpostorBatch value = keyValuePair2.Value;
				if (value.Visible)
				{
					ImpostorInstanceData key = keyValuePair2.Key;
					cb.SetGlobalBuffer("_PositionBuffer", value.PositionBuffer);
					cb.DrawMeshInstancedIndirect(value.Mesh, 0, value.Material, value.DeferredPass, value.ArgsBuffer, 0, ImpostorRenderer.Block);
					foreach (ImpostorShadows impostorShadows in ImpostorRenderer.Shadows)
					{
						if (impostorShadows != null)
						{
							impostorShadows.IssueDrawBatch(value);
						}
					}
				}
			}
		}
	}
}
