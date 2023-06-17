using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class SkinnedMeshCollision : BaseCollision
{
	// Token: 0x0600113F RID: 4415 RVA: 0x00072DFC File Offset: 0x00070FFC
	public override void TraceTest(HitTest test, List<TraceInfo> hits)
	{
		if (test.type == HitTest.Type.ProjectileEffect)
		{
			return;
		}
		if (this.model == null)
		{
			Debug.LogWarning("SkinnedMeshCollision - Model is null " + base.transform.root.gameObject.name, base.transform.root.gameObject);
			return;
		}
		List<SkinnedMeshCollider> list = Pool.GetList<SkinnedMeshCollider>();
		this.model.gameObject.GetComponentsInChildren<SkinnedMeshCollider>(true, list);
		if (list.Count == 0)
		{
			Debug.LogWarning("SkinnedMeshCollision without any SkinnedMeshCollider: " + this.model.transform.root.name);
		}
		int count = hits.Count;
		for (int i = 0; i < list.Count; i++)
		{
			SkinnedMeshCollider skinnedMeshCollider = list[i];
			if (!(skinnedMeshCollider == null))
			{
				skinnedMeshCollider.TraceAll(this, test, hits);
			}
		}
		Pool.FreeList<SkinnedMeshCollider>(ref list);
		int num = hits.Count - count;
		DamageProperties damageProperties = test.damageProperties;
		if (test.BestHit)
		{
			bool flag = false;
			if (test.type != HitTest.Type.MeleeAttack)
			{
				for (int j = 0; j < num; j++)
				{
					int num2 = count + j;
					if (hits[num2].distance <= test.MaxDistance)
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				for (int k = 0; k < num; k++)
				{
					int num3 = count + k;
					TraceInfo traceInfo = hits[num3];
					traceInfo.valid = false;
					hits[num3] = traceInfo;
				}
				return;
			}
			if (damageProperties && num > 1)
			{
				BaseCombatEntity baseCombatEntity = this.Owner as BaseCombatEntity;
				if (baseCombatEntity)
				{
					int num4 = count;
					TraceInfo traceInfo2 = hits[num4];
					float num5 = damageProperties.GetMultiplier(baseCombatEntity.SkeletonLookup(traceInfo2.BoneID()));
					for (int l = 1; l < num; l++)
					{
						int num6 = count + l;
						TraceInfo traceInfo3 = hits[num6];
						float multiplier = damageProperties.GetMultiplier(baseCombatEntity.SkeletonLookup(traceInfo3.BoneID()));
						if (num5 < multiplier)
						{
							traceInfo2.valid = false;
							hits[num4] = traceInfo2;
							traceInfo2 = traceInfo3;
							num4 = num6;
							num5 = multiplier;
						}
						else
						{
							traceInfo3.valid = false;
							hits[num6] = traceInfo3;
						}
					}
				}
			}
		}
	}
}
