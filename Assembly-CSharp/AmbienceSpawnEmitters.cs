using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class AmbienceSpawnEmitters : MonoBehaviour, IClientComponent
{
	// Token: 0x04000961 RID: 2401
	public int baseEmitterCount = 5;

	// Token: 0x04000962 RID: 2402
	public int baseEmitterDistance = 10;

	// Token: 0x04000963 RID: 2403
	public GameObjectRef emitterPrefab;

	// Token: 0x06000CBB RID: 3259 RVA: 0x0000BDE0 File Offset: 0x00009FE0
	private void Start()
	{
		this.CreateBaseEmitters();
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0005CF90 File Offset: 0x0005B190
	private void CreateBaseEmitters()
	{
		for (int i = 0; i < this.baseEmitterCount; i++)
		{
			GameObject gameObject = this.emitterPrefab.Instantiate(null);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			if (this.baseEmitterCount > 1)
			{
				gameObject.transform.Translate((float)this.baseEmitterDistance, 0f, 0f);
				gameObject.transform.RotateAround(base.gameObject.transform.position, Vector3.up, (float)(360 / this.baseEmitterCount * i));
			}
			AmbienceEmitter component = gameObject.GetComponent<AmbienceEmitter>();
			gameObject.SetActive(true);
			if (component != null)
			{
				SingletonComponent<AmbienceManager>.Instance.ActivateEmitter(component);
				SingletonComponent<AmbienceManager>.Instance.AddCameraEmitter(component);
			}
		}
	}
}
