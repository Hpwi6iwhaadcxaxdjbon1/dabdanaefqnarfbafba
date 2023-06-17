using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000551 RID: 1361
public class WaterCollision : MonoBehaviour
{
	// Token: 0x04001AF4 RID: 6900
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x04001AF5 RID: 6901
	private HashSet<Collider> waterColliders;

	// Token: 0x06001E90 RID: 7824 RVA: 0x0001822A File Offset: 0x0001642A
	private void Awake()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>(8);
		this.waterColliders = new HashSet<Collider>();
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x000A7B90 File Offset: 0x000A5D90
	public void Clear()
	{
		if (this.waterColliders.Count == 0)
		{
			return;
		}
		HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (Collider collider in this.ignoredColliders.Keys)
			{
				Physics.IgnoreCollision(collider, enumerator.Current, false);
			}
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x000A7C18 File Offset: 0x000A5E18
	public void Reset(Collider collider)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return;
		}
		foreach (Collider collider2 in this.waterColliders)
		{
			Physics.IgnoreCollision(collider, collider2, false);
		}
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x00018243 File Offset: 0x00016443
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, 2);
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x00018252 File Offset: 0x00016452
	public bool GetIgnore(Bounds bounds)
	{
		return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, 2);
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x00018260 File Offset: 0x00016460
	public bool GetIgnore(RaycastHit hit)
	{
		return this.waterColliders.Contains(hit.collider) && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x0001828A File Offset: 0x0001648A
	public bool GetIgnore(Collider collider)
	{
		return this.waterColliders.Count != 0 && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000A7C70 File Offset: 0x000A5E70
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return;
		}
		if (!this.GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> list = new List<Collider>();
				list.Add(trigger);
				List<Collider> list2 = list;
				foreach (Collider collider2 in this.waterColliders)
				{
					Physics.IgnoreCollision(collider, collider2, true);
				}
				this.ignoredColliders.Add(collider, list2);
				return;
			}
		}
		else
		{
			List<Collider> list3 = this.ignoredColliders[collider];
			if (ignore)
			{
				if (!list3.Contains(trigger))
				{
					list3.Add(trigger);
					return;
				}
			}
			else if (list3.Contains(trigger))
			{
				list3.Remove(trigger);
			}
		}
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x000A7D14 File Offset: 0x000A5F14
	protected void LateUpdate()
	{
		for (int i = 0; i < this.ignoredColliders.Count; i++)
		{
			KeyValuePair<Collider, List<Collider>> byIndex = this.ignoredColliders.GetByIndex(i);
			Collider key = byIndex.Key;
			List<Collider> value = byIndex.Value;
			if (key == null)
			{
				this.ignoredColliders.RemoveAt(i--);
			}
			else if (value.Count == 0)
			{
				foreach (Collider collider in this.waterColliders)
				{
					Physics.IgnoreCollision(key, collider, false);
				}
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}
}
