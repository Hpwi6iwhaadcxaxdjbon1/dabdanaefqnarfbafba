using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000336 RID: 822
public class BaseWheeledVehicle : BaseVehicle
{
	// Token: 0x040012A2 RID: 4770
	[Header("Wheels")]
	public BaseWheeledVehicle.VehicleWheel[] wheels;

	// Token: 0x060015BF RID: 5567 RVA: 0x0008516C File Offset: 0x0008336C
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		BaseWheeledVehicle.VehicleWheel[] array = this.wheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].wheelCollider.enabled = false;
		}
	}

	// Token: 0x02000337 RID: 823
	[Serializable]
	public class VehicleWheel
	{
		// Token: 0x040012A3 RID: 4771
		public Transform shock;

		// Token: 0x040012A4 RID: 4772
		public WheelCollider wheelCollider;

		// Token: 0x040012A5 RID: 4773
		public Transform wheel;

		// Token: 0x040012A6 RID: 4774
		public Transform axle;

		// Token: 0x040012A7 RID: 4775
		public bool steerWheel;

		// Token: 0x040012A8 RID: 4776
		public bool brakeWheel = true;

		// Token: 0x040012A9 RID: 4777
		public bool powerWheel = true;
	}
}
