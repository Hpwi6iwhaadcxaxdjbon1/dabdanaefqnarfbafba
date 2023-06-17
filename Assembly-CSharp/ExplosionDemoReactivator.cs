using System;
using UnityEngine;

// Token: 0x0200079D RID: 1949
public class ExplosionDemoReactivator : MonoBehaviour
{
	// Token: 0x040025E6 RID: 9702
	public float TimeDelayToReactivate = 3f;

	// Token: 0x06002A5D RID: 10845 RVA: 0x00020E53 File Offset: 0x0001F053
	private void Start()
	{
		base.InvokeRepeating("Reactivate", 0f, this.TimeDelayToReactivate);
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x000D8838 File Offset: 0x000D6A38
	private void Reactivate()
	{
		foreach (Transform transform in base.GetComponentsInChildren<Transform>())
		{
			transform.gameObject.SetActive(false);
			transform.gameObject.SetActive(true);
		}
	}
}
