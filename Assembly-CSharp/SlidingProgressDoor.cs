using System;
using UnityEngine;

// Token: 0x02000357 RID: 855
public class SlidingProgressDoor : ProgressDoor
{
	// Token: 0x04001312 RID: 4882
	public Vector3 openPosition;

	// Token: 0x04001313 RID: 4883
	public Vector3 closedPosition;

	// Token: 0x04001314 RID: 4884
	public GameObject doorObject;

	// Token: 0x04001315 RID: 4885
	[NonSerialized]
	private float client_targetStoredEnergy;

	// Token: 0x06001618 RID: 5656 RVA: 0x00086198 File Offset: 0x00084398
	public override void UpdateProgress()
	{
		base.UpdateProgress();
		Vector3 localPosition = this.doorObject.transform.localPosition;
		float t = this.storedEnergy / this.energyForOpen;
		Vector3 localPosition2 = Vector3.Lerp(this.closedPosition, this.openPosition, t);
		this.doorObject.transform.localPosition = localPosition2;
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x00012A86 File Offset: 0x00010C86
	public void Update()
	{
		if (base.isClient)
		{
			this.storedEnergy = Mathf.Lerp(this.storedEnergy, this.client_targetStoredEnergy, Time.deltaTime * 10f);
			this.UpdateProgress();
		}
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x00012AB8 File Offset: 0x00010CB8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity == null)
		{
			return;
		}
		this.client_targetStoredEnergy = info.msg.sphereEntity.radius;
	}
}
