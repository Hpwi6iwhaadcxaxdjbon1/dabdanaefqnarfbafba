using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004C3 RID: 1219
public class TerrainCollision : TerrainExtension
{
	// Token: 0x04001908 RID: 6408
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x04001909 RID: 6409
	private TerrainCollider terrainCollider;

	// Token: 0x06001C00 RID: 7168 RVA: 0x00016EAF File Offset: 0x000150AF
	public override void Setup()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>(8);
		this.terrainCollider = this.terrain.GetComponent<TerrainCollider>();
	}

	// Token: 0x06001C01 RID: 7169 RVA: 0x0009A80C File Offset: 0x00098A0C
	public void Clear()
	{
		if (!this.terrainCollider)
		{
			return;
		}
		foreach (Collider collider in this.ignoredColliders.Keys)
		{
			Physics.IgnoreCollision(collider, this.terrainCollider, false);
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x06001C02 RID: 7170 RVA: 0x00016ECE File Offset: 0x000150CE
	public void Reset(Collider collider)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		Physics.IgnoreCollision(collider, this.terrainCollider, false);
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x06001C03 RID: 7171 RVA: 0x00016F00 File Offset: 0x00015100
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<TerrainCollisionTrigger>(pos, radius, 262144, 2);
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x00016F0F File Offset: 0x0001510F
	public bool GetIgnore(RaycastHit hit)
	{
		return hit.collider is TerrainCollider && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x00016F33 File Offset: 0x00015133
	public bool GetIgnore(Collider collider)
	{
		return this.terrainCollider && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x0009A87C File Offset: 0x00098A7C
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (!this.terrainCollider || !collider)
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
				Physics.IgnoreCollision(collider, this.terrainCollider, true);
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

	// Token: 0x06001C07 RID: 7175 RVA: 0x0009A908 File Offset: 0x00098B08
	protected void LateUpdate()
	{
		if (this.ignoredColliders == null)
		{
			return;
		}
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
				Physics.IgnoreCollision(key, this.terrainCollider, false);
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}
}
