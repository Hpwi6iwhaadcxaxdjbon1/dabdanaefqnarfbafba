using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class MovementSoundTrigger : TriggerBase, IClientComponentEx, ILOD
{
	// Token: 0x040009EF RID: 2543
	public SoundDefinition softSound;

	// Token: 0x040009F0 RID: 2544
	public SoundDefinition medSound;

	// Token: 0x040009F1 RID: 2545
	public SoundDefinition hardSound;

	// Token: 0x040009F2 RID: 2546
	public Collider collider;

	// Token: 0x040009F3 RID: 2547
	private const float maxDistance = 100f;

	// Token: 0x040009F4 RID: 2548
	private const float tickRate = 2f;

	// Token: 0x040009F5 RID: 2549
	private LODCell cell;

	// Token: 0x040009F6 RID: 2550
	private Dictionary<GameObject, Vector3> lastPositionByObject;

	// Token: 0x06000CF6 RID: 3318 RVA: 0x0005EACC File Offset: 0x0005CCCC
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x0000BF69 File Offset: 0x0000A169
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), 0f, 0.5f);
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x0000BF87 File Offset: 0x0000A187
	internal override void OnEmpty()
	{
		base.OnEmpty();
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0000BFA1 File Offset: 0x0000A1A1
	internal override void OnObjectRemoved(GameObject obj)
	{
		base.OnObjectRemoved(obj);
		if (this.lastPositionByObject == null)
		{
			return;
		}
		this.lastPositionByObject.Remove(obj);
		if (this.lastPositionByObject.Count == 0)
		{
			this.lastPositionByObject = null;
		}
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x0000BFD4 File Offset: 0x0000A1D4
	internal override void OnObjectAdded(GameObject obj)
	{
		base.OnObjectAdded(obj);
		if (this.lastPositionByObject == null)
		{
			this.lastPositionByObject = new Dictionary<GameObject, Vector3>();
		}
		this.lastPositionByObject.Add(obj, obj.transform.position);
		this.PlaySound(obj, 0f);
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x0005EB08 File Offset: 0x0005CD08
	private void OnTick()
	{
		if (this.lastPositionByObject == null || this.contents == null)
		{
			return;
		}
		foreach (GameObject gameObject in this.contents)
		{
			if (!(gameObject == null))
			{
				float num = Vector3.Magnitude(this.lastPositionByObject[gameObject] - gameObject.transform.position);
				if (num > 0.1f)
				{
					this.PlaySound(gameObject, num);
					this.lastPositionByObject[gameObject] = gameObject.transform.position;
				}
			}
		}
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x0000C013 File Offset: 0x0000A213
	protected void OnEnable()
	{
		LODGrid.Add(this, base.transform, ref this.cell);
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x0000C027 File Offset: 0x0000A227
	protected override void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		base.OnDisable();
		LODGrid.Remove(this, base.transform, ref this.cell);
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x0000C049 File Offset: 0x0000A249
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x0000C05D File Offset: 0x0000A25D
	public void ChangeLOD()
	{
		this.collider.enabled = (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(100f));
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x0005EBB8 File Offset: 0x0005CDB8
	private void PlaySound(GameObject obj, float speed = 0f)
	{
		if (!this.collider.bounds.Contains(obj.transform.position))
		{
			return;
		}
		if (speed > 2f)
		{
			SoundManager.PlayOneshot(this.hardSound, obj, false, default(Vector3));
			return;
		}
		if (speed > 1.2f)
		{
			SoundManager.PlayOneshot(this.medSound, obj, false, default(Vector3));
			return;
		}
		SoundManager.PlayOneshot(this.softSound, obj, false, default(Vector3));
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x0000C082 File Offset: 0x0000A282
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.collider);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}
}
