using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class CargoShipInteriorSoundTrigger : MonoBehaviour, IClientComponent
{
	// Token: 0x0400003F RID: 63
	public CargoShipSounds cargoShipSounds;

	// Token: 0x06000045 RID: 69 RVA: 0x00027980 File Offset: 0x00025B80
	private void OnTriggerEnter(Collider other)
	{
		if (other == null || LocalPlayer.Entity == null || LocalPlayer.Entity.gameObject == null)
		{
			return;
		}
		if (other.gameObject != LocalPlayer.Entity.gameObject)
		{
			return;
		}
		this.cargoShipSounds.InteriorTriggerEntered(this);
	}

	// Token: 0x06000046 RID: 70 RVA: 0x000279DC File Offset: 0x00025BDC
	private void OnTriggerExit(Collider other)
	{
		if (other == null || LocalPlayer.Entity == null || LocalPlayer.Entity.gameObject == null)
		{
			return;
		}
		if (other.gameObject != LocalPlayer.Entity.gameObject)
		{
			return;
		}
		this.cargoShipSounds.InteriorTriggerExited(this);
	}
}
