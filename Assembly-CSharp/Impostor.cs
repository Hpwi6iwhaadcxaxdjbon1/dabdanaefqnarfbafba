using System;
using UnityEngine;

// Token: 0x0200059B RID: 1435
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Impostor : MonoBehaviour, IClientComponent
{
	// Token: 0x04001CA7 RID: 7335
	public ImpostorAsset asset;

	// Token: 0x04001CA8 RID: 7336
	[Header("Baking")]
	public GameObject reference;

	// Token: 0x04001CA9 RID: 7337
	public float angle;

	// Token: 0x04001CAA RID: 7338
	public int resolution = 1024;

	// Token: 0x04001CAB RID: 7339
	public int padding = 32;

	// Token: 0x04001CAC RID: 7340
	public bool spriteOutlineAsMesh;

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x060020C8 RID: 8392 RVA: 0x0001A04C File Offset: 0x0001824C
	// (set) Token: 0x060020C9 RID: 8393 RVA: 0x0001A054 File Offset: 0x00018254
	public ImpostorInstanceData InstanceData { get; private set; }

	// Token: 0x060020CA RID: 8394 RVA: 0x0001A05D File Offset: 0x0001825D
	private void Awake()
	{
		this.InitializeInstanceData();
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x0001A065 File Offset: 0x00018265
	private void OnEnable()
	{
		if (this.InstanceData != null && Application.isPlaying)
		{
			ImpostorRenderer.Register(this);
		}
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x0001A07C File Offset: 0x0001827C
	private void OnDisable()
	{
		if (this.InstanceData != null && Application.isPlaying)
		{
			ImpostorRenderer.Unregister(this);
		}
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000B1B60 File Offset: 0x000AFD60
	private void InitializeInstanceData()
	{
		Renderer component = base.GetComponent<Renderer>();
		Material material = (component != null) ? component.sharedMaterial : null;
		if (this.asset.mesh != null && material != null && material.FindPass("DEFERRED") >= 0)
		{
			this.InstanceData = new ImpostorInstanceData(component, this.asset.mesh, material);
			return;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"[Impostor] InitializeInstanceData failed on ",
			base.name,
			", mesh: ",
			this.asset.mesh,
			", mat: ",
			material
		}));
		base.gameObject.layer = LayerMask.NameToLayer("Default");
		this.InstanceData = null;
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x0001A093 File Offset: 0x00018293
	public void UpdateInstance()
	{
		if (this.InstanceData != null)
		{
			this.InstanceData.Update();
		}
	}
}
