using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020003F8 RID: 1016
[CreateAssetMenu(menuName = "Rust/Spawn Population")]
public class SpawnPopulation : BaseScriptableObject
{
	// Token: 0x0400157C RID: 5500
	[Header("Spawnables")]
	public string ResourceFolder = string.Empty;

	// Token: 0x0400157D RID: 5501
	public GameObjectRef[] ResourceList;

	// Token: 0x0400157E RID: 5502
	[Tooltip("Usually per square km")]
	[Header("Spawn Info")]
	[FormerlySerializedAs("TargetDensity")]
	[SerializeField]
	private float _targetDensity = 1f;

	// Token: 0x0400157F RID: 5503
	public float SpawnRate = 1f;

	// Token: 0x04001580 RID: 5504
	public int ClusterSizeMin = 1;

	// Token: 0x04001581 RID: 5505
	public int ClusterSizeMax = 1;

	// Token: 0x04001582 RID: 5506
	public int ClusterDithering;

	// Token: 0x04001583 RID: 5507
	public int SpawnAttemptsInitial = 20;

	// Token: 0x04001584 RID: 5508
	public int SpawnAttemptsRepeating = 10;

	// Token: 0x04001585 RID: 5509
	public bool EnforcePopulationLimits = true;

	// Token: 0x04001586 RID: 5510
	public bool ScaleWithSpawnFilter = true;

	// Token: 0x04001587 RID: 5511
	public bool ScaleWithServerPopulation;

	// Token: 0x04001588 RID: 5512
	public bool AlignToNormal;

	// Token: 0x04001589 RID: 5513
	public SpawnFilter Filter = new SpawnFilter();

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x0600191A RID: 6426 RVA: 0x00014E20 File Offset: 0x00013020
	public virtual float TargetDensity
	{
		get
		{
			return this._targetDensity;
		}
	}
}
