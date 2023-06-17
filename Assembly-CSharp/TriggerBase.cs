using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public class TriggerBase : BaseMonoBehaviour
{
	// Token: 0x040015AD RID: 5549
	public LayerMask interestLayers;

	// Token: 0x040015AE RID: 5550
	[NonSerialized]
	public HashSet<GameObject> contents;

	// Token: 0x040015AF RID: 5551
	[NonSerialized]
	public HashSet<BaseEntity> entityContents;

	// Token: 0x0600192D RID: 6445 RVA: 0x0008F0B8 File Offset: 0x0008D2B8
	internal virtual GameObject InterestedInObject(GameObject obj)
	{
		int num = 1 << obj.layer;
		if ((this.interestLayers.value & num) != num)
		{
			return null;
		}
		return obj;
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x0008F0E4 File Offset: 0x0008D2E4
	protected virtual void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.contents == null)
		{
			return;
		}
		foreach (GameObject targetObj in Enumerable.ToArray<GameObject>(this.contents))
		{
			this.OnTriggerExit(targetObj);
		}
		this.contents = null;
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x00014EA9 File Offset: 0x000130A9
	internal virtual void OnEntityEnter(BaseEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			this.entityContents = new HashSet<BaseEntity>();
		}
		this.entityContents.Add(ent);
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x00014ED5 File Offset: 0x000130D5
	internal virtual void OnEntityLeave(BaseEntity ent)
	{
		if (this.entityContents == null)
		{
			return;
		}
		this.entityContents.Remove(ent);
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x0008F130 File Offset: 0x0008D330
	internal virtual void OnObjectAdded(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			this.OnEntityEnter(baseEntity);
			baseEntity.EnterTrigger(this);
		}
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x0008F168 File Offset: 0x0008D368
	internal virtual void OnObjectRemoved(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			this.OnEntityLeave(baseEntity);
			baseEntity.LeaveTrigger(this);
		}
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x0008F19C File Offset: 0x0008D39C
	internal void RemoveInvalidEntities()
	{
		if (this.entityContents == null)
		{
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		foreach (BaseEntity baseEntity in Enumerable.ToArray<BaseEntity>(this.entityContents))
		{
			if (baseEntity == null)
			{
				Debug.LogWarning("Trigger " + this.ToString() + " contains destroyed entity.");
			}
			else if (!bounds.Contains(baseEntity.ClosestPoint(base.transform.position)))
			{
				Debug.LogWarning("Trigger " + this.ToString() + " contains entity that is too far away: " + baseEntity.ToString());
				this.RemoveEntity(baseEntity);
			}
		}
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x0008F260 File Offset: 0x0008D460
	internal bool CheckEntity(BaseEntity ent)
	{
		if (ent == null)
		{
			return true;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return true;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		return bounds.Contains(ent.ClosestPoint(base.transform.position));
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x00002ECE File Offset: 0x000010CE
	internal virtual void OnObjects()
	{
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x00014EED File Offset: 0x000130ED
	internal virtual void OnEmpty()
	{
		this.contents = null;
		this.entityContents = null;
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x0008F2B8 File Offset: 0x0008D4B8
	public void RemoveObject(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		this.OnTriggerExit(component);
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x00014EFD File Offset: 0x000130FD
	public void RemoveEntity(BaseEntity obj)
	{
		this.OnTriggerExit(obj.gameObject);
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x0008F2E8 File Offset: 0x0008D4E8
	public void OnTriggerEnter(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (ConVar.Vis.triggers)
		{
			UnityEngine.DDraw.Box(collider.transform.position, 0.2f, Color.green, 5f, true);
			UnityEngine.DDraw.Box(base.transform.position, 0.2f, Color.blue, 5f, true);
			UnityEngine.DDraw.Line(collider.transform.position, base.transform.position, Color.blue, 5f, true, true);
		}
		using (TimeWarning.New("TriggerBase.OnTriggerEnter", 0.1f))
		{
			GameObject gameObject = this.InterestedInObject(collider.gameObject);
			if (gameObject == null)
			{
				return;
			}
			if (this.contents == null)
			{
				this.contents = new HashSet<GameObject>();
			}
			if (this.contents.Contains(gameObject))
			{
				return;
			}
			bool count = this.contents.Count != 0;
			this.contents.Add(gameObject);
			this.OnObjectAdded(gameObject);
			if (!count && this.contents.Count == 1)
			{
				this.OnObjects();
			}
		}
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x0008F418 File Offset: 0x0008D618
	public void OnTriggerExit(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (collider == null)
		{
			return;
		}
		if (ConVar.Vis.triggers)
		{
			UnityEngine.DDraw.Box(collider.transform.position, 0.2f, Color.red, 5f, true);
			UnityEngine.DDraw.Box(base.transform.position, 0.2f, Color.blue, 5f, true);
			UnityEngine.DDraw.Line(collider.transform.position, base.transform.position, Color.blue, 5f, true, true);
		}
		GameObject gameObject = this.InterestedInObject(collider.gameObject);
		if (gameObject == null)
		{
			return;
		}
		this.OnTriggerExit(gameObject);
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x0008F4D4 File Offset: 0x0008D6D4
	private void OnTriggerExit(GameObject targetObj)
	{
		if (this.contents == null)
		{
			return;
		}
		if (!this.contents.Contains(targetObj))
		{
			return;
		}
		this.contents.Remove(targetObj);
		this.OnObjectRemoved(targetObj);
		if (this.contents == null || this.contents.Count == 0)
		{
			this.OnEmpty();
		}
	}
}
