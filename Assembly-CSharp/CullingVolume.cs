using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000376 RID: 886
public class CullingVolume : MonoBehaviour, IClientComponent
{
	// Token: 0x04001389 RID: 5001
	[Tooltip("Override occludee root from children of this object (default) to children of any other object.")]
	public GameObject OccludeeRoot;

	// Token: 0x0400138A RID: 5002
	[Tooltip("Invert visibility. False will show occludes. True will hide them.")]
	public bool Invert;

	// Token: 0x0400138B RID: 5003
	[Tooltip("A portal in the culling volume chain does not toggle objects visible, it merely signals the non-portal volumes to hide their occludees.")]
	public bool Portal;

	// Token: 0x0400138C RID: 5004
	[Tooltip("Secondary culling volumes, connected to this one, that will get signaled when this trigger is activated.")]
	public List<CullingVolume> Connections = new List<CullingVolume>();

	// Token: 0x0400138D RID: 5005
	private Transform root;

	// Token: 0x0400138E RID: 5006
	private Bounds bounds;

	// Token: 0x0400138F RID: 5007
	private List<LODComponent> components;

	// Token: 0x04001390 RID: 5008
	private List<LightOccludee> lights;

	// Token: 0x04001391 RID: 5009
	private float lastSeen;

	// Token: 0x060016A0 RID: 5792 RVA: 0x00087AD8 File Offset: 0x00085CD8
	protected void Awake()
	{
		base.gameObject.layer = 18;
		this.root = ((this.OccludeeRoot != null) ? this.OccludeeRoot.transform : base.transform);
		this.UpdateBounds();
		this.UpdateTrigger();
		this.UpdateComponents();
		this.UpdateLights();
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x000131E9 File Offset: 0x000113E9
	protected void Start()
	{
		this.SetVisible(false);
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x000131F2 File Offset: 0x000113F2
	private void UpdateBounds()
	{
		this.bounds = this.root.GetBounds(true, false, true);
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x00087B34 File Offset: 0x00085D34
	private void UpdateTrigger()
	{
		if (base.gameObject.GetComponent<BoxCollider>() == null)
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;
			boxCollider.center = this.bounds.center;
			boxCollider.size = this.bounds.size;
		}
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x00087B88 File Offset: 0x00085D88
	private void UpdateComponents()
	{
		if (this.components == null)
		{
			this.components = new List<LODComponent>();
		}
		else
		{
			this.components.Clear();
		}
		List<LODComponent> list = Pool.GetList<LODComponent>();
		this.root.GetComponentsInChildren<LODComponent>(true, list);
		for (int i = 0; i < list.Count; i++)
		{
			LODComponent lodcomponent = list[i];
			if (!lodcomponent.GetComponentInParent<BaseNetworkable>())
			{
				this.components.Add(lodcomponent);
			}
		}
		Pool.FreeList<LODComponent>(ref list);
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x00087C04 File Offset: 0x00085E04
	private void UpdateLights()
	{
		if (this.lights == null)
		{
			this.lights = new List<LightOccludee>();
		}
		else
		{
			this.lights.Clear();
		}
		List<LightOccludee> list = Pool.GetList<LightOccludee>();
		this.root.GetComponentsInChildren<LightOccludee>(true, list);
		for (int i = 0; i < list.Count; i++)
		{
			LightOccludee lightOccludee = list[i];
			this.lights.Add(lightOccludee);
		}
		Pool.FreeList<LightOccludee>(ref list);
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x00087C70 File Offset: 0x00085E70
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.1f, 0.5f, 0.1f, 0.5f);
		GizmosUtil.DrawMeshes(base.transform);
		Gizmos.color = new Color(0.1f, 0.1f, 0.5f, 0.5f);
		for (int i = 0; i < this.Connections.Count; i++)
		{
			GizmosUtil.DrawMeshes(this.Connections[i].transform);
		}
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x00087CF0 File Offset: 0x00085EF0
	public void MarkSeen(bool recursive = true)
	{
		if (!this.IsVisible())
		{
			this.SetVisible(true);
		}
		this.lastSeen = Time.time;
		if (recursive)
		{
			for (int i = 0; i < this.Connections.Count; i++)
			{
				CullingVolume cullingVolume = this.Connections[i];
				if (cullingVolume == null)
				{
					Debug.LogError("Missing culling volume connection.", this);
				}
				else
				{
					cullingVolume.MarkSeen(false);
				}
			}
		}
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x00087D5C File Offset: 0x00085F5C
	public bool UpdateVisible(bool recursive = true)
	{
		if (!this.IsVisible())
		{
			this.SetVisible(false);
			if (recursive)
			{
				for (int i = 0; i < this.Connections.Count; i++)
				{
					CullingVolume cullingVolume = this.Connections[i];
					if (cullingVolume == null)
					{
						Debug.LogError("Missing culling volume connection.", this);
					}
					else
					{
						cullingVolume.UpdateVisible(false);
					}
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x00013208 File Offset: 0x00011408
	private bool IsVisible()
	{
		return Time.time - this.lastSeen < 1f;
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x00087DC0 File Offset: 0x00085FC0
	private void SetVisible(bool state)
	{
		if (!this.Portal)
		{
			state = (this.Invert ? (!state) : state);
			for (int i = 0; i < this.components.Count; i++)
			{
				LODComponent lodcomponent = this.components[i];
				if (lodcomponent == null)
				{
					Debug.LogError("Missing level of detail component.", this);
				}
				else
				{
					lodcomponent.SetVisible(state);
				}
			}
			for (int j = 0; j < this.lights.Count; j++)
			{
				LightOccludee lightOccludee = this.lights[j];
				if (lightOccludee == null)
				{
					Debug.LogError("Missing light component.", this);
				}
				else
				{
					lightOccludee.SetVolumeVisible(state);
				}
			}
		}
	}
}
