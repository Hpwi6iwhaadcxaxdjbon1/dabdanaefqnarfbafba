using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000348 RID: 840
public class DeployGuide : BaseMonoBehaviour
{
	// Token: 0x040012F5 RID: 4853
	public static DeployGuide current;

	// Token: 0x040012F6 RID: 4854
	private bool hidden;

	// Token: 0x040012F7 RID: 4855
	[NonSerialized]
	public bool isGoodPlacement;

	// Token: 0x040012F8 RID: 4856
	[NonSerialized]
	private MeshRenderer meshRenderer;

	// Token: 0x040012F9 RID: 4857
	[NonSerialized]
	private MeshFilter meshFilter;

	// Token: 0x060015F6 RID: 5622 RVA: 0x000128D8 File Offset: 0x00010AD8
	public static void HideFor(float seconds)
	{
		if (DeployGuide.current == null)
		{
			return;
		}
		DeployGuide.current.hidden = true;
		DeployGuide.current.Invoke(new Action(DeployGuide.current.UnHide), seconds);
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x0001290E File Offset: 0x00010B0E
	private void UnHide()
	{
		this.hidden = false;
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x00012917 File Offset: 0x00010B17
	public static void Start(Deployable deployable)
	{
		DeployGuide.End();
		if (deployable == null)
		{
			return;
		}
		DeployGuide.current = DeployGuide.Create(deployable);
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x00012933 File Offset: 0x00010B33
	public static void End()
	{
		if (!DeployGuide.current)
		{
			return;
		}
		GameManager.Destroy(DeployGuide.current.gameObject, 0f);
		DeployGuide.current = null;
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x0001295C File Offset: 0x00010B5C
	public static void Place(Vector3 pos, Quaternion rot = default(Quaternion))
	{
		if (!DeployGuide.current)
		{
			return;
		}
		DeployGuide.current.transform.position = pos;
		DeployGuide.current.transform.rotation = rot;
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x0001298B File Offset: 0x00010B8B
	public static DeployGuide Create(Deployable source)
	{
		DeployGuide deployGuide = new GameObject("Deploy Guide").AddComponent<DeployGuide>();
		deployGuide.Setup(source);
		return deployGuide;
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x00085E74 File Offset: 0x00084074
	public void Setup(Deployable source)
	{
		if (source)
		{
			this.meshFilter = base.gameObject.AddComponent<MeshFilter>();
			this.meshFilter.sharedMesh = source.guideMesh;
			this.meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.meshRenderer.receiveShadows = false;
		}
		this.isGoodPlacement = false;
		this.SetGood();
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x00085EE4 File Offset: 0x000840E4
	public void SetGood()
	{
		if (this.isGoodPlacement)
		{
			return;
		}
		this.isGoodPlacement = true;
		this.meshRenderer.sharedMaterial = FileSystem.Load<Material>("Assets/Content/materials/guide_good.mat", true);
		this.meshRenderer.sharedMaterials = new Material[]
		{
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial
		};
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x00085F70 File Offset: 0x00084170
	public void SetBad()
	{
		if (!this.isGoodPlacement)
		{
			return;
		}
		this.isGoodPlacement = false;
		this.meshRenderer.sharedMaterial = FileSystem.Load<Material>("Assets/Content/materials/guide_bad.mat", true);
		this.meshRenderer.sharedMaterials = new Material[]
		{
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial,
			this.meshRenderer.sharedMaterial
		};
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x000129A3 File Offset: 0x00010BA3
	public static bool Update()
	{
		if (!DeployGuide.current)
		{
			return true;
		}
		if (DeployGuide.current.hidden)
		{
			DeployGuide.current.meshRenderer.enabled = false;
			return true;
		}
		return false;
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x000129D2 File Offset: 0x00010BD2
	public static void SetValid(bool valid)
	{
		if (!DeployGuide.current)
		{
			return;
		}
		if (valid)
		{
			DeployGuide.current.SetGood();
			return;
		}
		DeployGuide.current.SetBad();
	}
}
