using System;
using UnityEngine;

// Token: 0x02000307 RID: 775
[CreateAssetMenu(menuName = "Rust/Plant Properties")]
public class PlantProperties : ScriptableObject
{
	// Token: 0x04001132 RID: 4402
	[ArrayIndexIsEnum(enumType = typeof(PlantProperties.State))]
	public PlantProperties.Stage[] stages = new PlantProperties.Stage[6];

	// Token: 0x04001133 RID: 4403
	[Header("Metabolism")]
	public AnimationCurve timeOfDayHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(12f, 1f),
		new Keyframe(24f, 0f)
	});

	// Token: 0x04001134 RID: 4404
	public AnimationCurve temperatureHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-10f, -1f),
		new Keyframe(1f, 0f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 0f),
		new Keyframe(80f, -1f)
	});

	// Token: 0x04001135 RID: 4405
	public AnimationCurve fruitCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.75f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04001136 RID: 4406
	public int maxSeasons = 1;

	// Token: 0x04001137 RID: 4407
	public int maxHeldWater = 1000;

	// Token: 0x04001138 RID: 4408
	public int lifetimeWaterConsumption = 5000;

	// Token: 0x04001139 RID: 4409
	public float waterConsumptionLifetime = 60f;

	// Token: 0x0400113A RID: 4410
	public int waterYieldBonus = 1;

	// Token: 0x0400113B RID: 4411
	[Header("Harvesting")]
	public BaseEntity.Menu.Option pickOption;

	// Token: 0x0400113C RID: 4412
	public ItemDefinition pickupItem;

	// Token: 0x0400113D RID: 4413
	public int pickupAmount = 1;

	// Token: 0x0400113E RID: 4414
	public GameObjectRef pickEffect;

	// Token: 0x0400113F RID: 4415
	public int maxHarvests = 1;

	// Token: 0x04001140 RID: 4416
	public bool disappearAfterHarvest;

	// Token: 0x04001141 RID: 4417
	[Header("Cloning")]
	public BaseEntity.Menu.Option cloneOption;

	// Token: 0x04001142 RID: 4418
	public ItemDefinition cloneItem;

	// Token: 0x04001143 RID: 4419
	public int maxClones = 1;

	// Token: 0x02000308 RID: 776
	public enum State
	{
		// Token: 0x04001145 RID: 4421
		Seed,
		// Token: 0x04001146 RID: 4422
		Seedling,
		// Token: 0x04001147 RID: 4423
		Sapling,
		// Token: 0x04001148 RID: 4424
		Mature,
		// Token: 0x04001149 RID: 4425
		Fruiting,
		// Token: 0x0400114A RID: 4426
		Dying
	}

	// Token: 0x02000309 RID: 777
	[Serializable]
	public struct Stage
	{
		// Token: 0x0400114B RID: 4427
		public PlantProperties.State nextState;

		// Token: 0x0400114C RID: 4428
		public float lifeLength;

		// Token: 0x0400114D RID: 4429
		public float health;

		// Token: 0x0400114E RID: 4430
		public float resources;

		// Token: 0x0400114F RID: 4431
		public GameObjectRef skinObject;
	}
}
