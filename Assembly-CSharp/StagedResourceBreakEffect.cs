using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000322 RID: 802
public class StagedResourceBreakEffect : MonoBehaviour, IEffect
{
	// Token: 0x06001588 RID: 5512 RVA: 0x00084968 File Offset: 0x00082B68
	public virtual void SetupEffect(Effect effect)
	{
		StagedResourceEntity stagedResourceEntity = BaseNetworkable.clientEntities.Find(effect.entity) as StagedResourceEntity;
		if (stagedResourceEntity == null)
		{
			return;
		}
		MeshRenderer stageComponent = stagedResourceEntity.GetStageComponent<MeshRenderer>();
		if (stageComponent)
		{
			List<ParticleSystem> list = Pool.GetList<ParticleSystem>();
			base.GetComponentsInChildren<ParticleSystem>(list);
			foreach (ParticleSystem particleSystem in list)
			{
				particleSystem.shape.meshRenderer = stageComponent;
			}
			Pool.FreeList<ParticleSystem>(ref list);
			return;
		}
		Debug.LogWarning("No Mesh Renderer for StagedResouceBreakEffect");
	}
}
