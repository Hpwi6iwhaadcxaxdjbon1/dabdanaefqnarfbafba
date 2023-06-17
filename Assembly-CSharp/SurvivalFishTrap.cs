using System;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class SurvivalFishTrap : WildlifeTrap
{
	// Token: 0x06001596 RID: 5526 RVA: 0x00084AAC File Offset: 0x00082CAC
	public override void TrappedEffect()
	{
		if (base.HasCatch() && Time.realtimeSinceStartup >= this.nextEffectTime)
		{
			this.nextEffectTime = Time.realtimeSinceStartup + this.trappedEffectRepeatRate * Random.Range(0.8f, 1.2f);
			Vector3 position = base.transform.position;
			position = new Vector3(position.x, TerrainMeta.WaterMap.GetHeight(position), position.z);
			Effect.client.Run(this.trappedEffect.resourcePath, position, Vector3.up, default(Vector3));
		}
	}
}
