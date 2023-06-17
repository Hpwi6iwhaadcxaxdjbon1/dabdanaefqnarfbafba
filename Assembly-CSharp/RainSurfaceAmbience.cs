using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class RainSurfaceAmbience : MonoBehaviour
{
	// Token: 0x04000969 RID: 2409
	public float tickRate = 1f;

	// Token: 0x0400096A RID: 2410
	public float gridSize = 20f;

	// Token: 0x0400096B RID: 2411
	public float gridSamples = 10f;

	// Token: 0x0400096C RID: 2412
	public float startHeight = 100f;

	// Token: 0x0400096D RID: 2413
	public float rayLength = 250f;

	// Token: 0x0400096E RID: 2414
	public LayerMask layerMask;

	// Token: 0x0400096F RID: 2415
	public float spreadScale = 8f;

	// Token: 0x04000970 RID: 2416
	public float maxDistance = 10f;

	// Token: 0x04000971 RID: 2417
	public float lerpSpeed = 5f;

	// Token: 0x04000972 RID: 2418
	public List<RainSurfaceAmbience.SurfaceSound> surfaces = new List<RainSurfaceAmbience.SurfaceSound>();

	// Token: 0x04000973 RID: 2419
	private float lastTick;

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0005D120 File Offset: 0x0005B320
	protected void Start()
	{
		foreach (RainSurfaceAmbience.SurfaceSound surfaceSound in this.surfaces)
		{
			surfaceSound.sound = SoundManager.RequestSoundInstance(surfaceSound.soundDef, null, default(Vector3), false);
			surfaceSound.gainMod = surfaceSound.sound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
			surfaceSound.gainMod.value = 0f;
			surfaceSound.spreadMod = surfaceSound.sound.modulation.CreateModulator(SoundModulation.Parameter.Spread);
			surfaceSound.spreadMod.value = 0f;
		}
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0005D1D4 File Offset: 0x0005B3D4
	protected void Update()
	{
		if (Time.time > this.lastTick + 1f / this.tickRate)
		{
			this.Tick();
		}
		for (int i = 0; i < this.surfaces.Count; i++)
		{
			RainSurfaceAmbience.SurfaceSound surfaceSound = this.surfaces[i];
			float num = 10000f;
			Vector3 position = base.transform.position;
			for (int j = 0; j < surfaceSound.points.Count; j++)
			{
				Vector3 vector = surfaceSound.points[j];
				float num2 = Vector3Ex.Distance2D(position, vector);
				if (num2 < num)
				{
					num = num2;
				}
			}
			float num3 = 1f - Mathf.Clamp(num / this.maxDistance, 0f, 1f);
			if (surfaceSound.position != Vector3.zero)
			{
				surfaceSound.sound.transform.position = Vector3.Lerp(surfaceSound.sound.transform.position, surfaceSound.position, Time.deltaTime * this.lerpSpeed);
			}
			surfaceSound.gainMod.value = Mathf.Lerp(surfaceSound.gainMod.value, (surfaceSound.amount * 0.4f + 0.6f) * num3 * Climate.GetRain(base.transform.position), Time.deltaTime * this.lerpSpeed);
			surfaceSound.spreadMod.value = Mathf.Lerp(surfaceSound.spreadMod.value, Mathf.Clamp(surfaceSound.bounds.size.magnitude * this.spreadScale, 0f, 180f) / 180f, Time.deltaTime * this.lerpSpeed);
			if (surfaceSound.sound != null && surfaceSound.sound.playing && surfaceSound.sound.audioSourceVolue < 0.1f)
			{
				surfaceSound.sound.Stop();
			}
			if (surfaceSound.sound != null && !surfaceSound.sound.playing && surfaceSound.gainMod.value > 0f)
			{
				surfaceSound.sound.Play();
			}
		}
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0005D3F4 File Offset: 0x0005B5F4
	private void Tick()
	{
		if (Climate.GetRain(base.transform.position) > 0f)
		{
			this.ResetSurfaces();
			this.UpdateSurfaces();
		}
		else
		{
			for (int i = 0; i < this.surfaces.Count; i++)
			{
				this.surfaces[i].gainMod.value = 0f;
			}
		}
		this.lastTick = Time.time;
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0005D464 File Offset: 0x0005B664
	private void ResetSurfaces()
	{
		foreach (RainSurfaceAmbience.SurfaceSound surfaceSound in this.surfaces)
		{
			surfaceSound.points.Clear();
			surfaceSound.amount = 0f;
			surfaceSound.position = Vector3.zero;
			surfaceSound.bounds.center = Vector3.zero;
			surfaceSound.bounds.size = Vector3.zero;
		}
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0005D4F0 File Offset: 0x0005B6F0
	private void UpdateSurfaces()
	{
		Vector3 vector = new Vector3(0f, base.transform.position.y + this.startHeight, 0f);
		float num = this.gridSize / this.gridSamples;
		int num2 = 0;
		while ((float)num2 < this.gridSamples)
		{
			int num3 = 0;
			while ((float)num3 < this.gridSamples)
			{
				vector.x = (float)num2 * num - this.gridSize / 2f + base.transform.position.x;
				vector.z = (float)num3 * num - this.gridSize / 2f + base.transform.position.z;
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, Vector3.down, ref raycastHit, this.rayLength, this.layerMask))
				{
					PhysicMaterial materialAt = raycastHit.collider.GetMaterialAt(raycastHit.point);
					if (!(materialAt == null))
					{
						for (int i = 0; i < this.surfaces.Count; i++)
						{
							bool flag = false;
							for (int j = 0; j < this.surfaces[i].materials.Count; j++)
							{
								if (this.surfaces[i].materials[j] == materialAt)
								{
									flag = true;
								}
							}
							if (flag)
							{
								if (this.surfaces[i].bounds.center == Vector3.zero)
								{
									this.surfaces[i].bounds.center = raycastHit.point;
								}
								this.surfaces[i].bounds.Encapsulate(raycastHit.point);
								this.surfaces[i].points.Add(raycastHit.point);
								this.surfaces[i].position += raycastHit.point;
							}
						}
					}
				}
				num3++;
			}
			num2++;
		}
		float num4 = this.gridSamples * this.gridSamples;
		foreach (RainSurfaceAmbience.SurfaceSound surfaceSound in this.surfaces)
		{
			surfaceSound.position = ((surfaceSound.points.Count == 0) ? Vector3.zero : new Vector3(surfaceSound.position.x / (float)surfaceSound.points.Count, surfaceSound.position.y / (float)surfaceSound.points.Count, surfaceSound.position.z / (float)surfaceSound.points.Count));
			surfaceSound.amount = Mathf.Clamp((float)surfaceSound.points.Count / num4, 0f, 1f);
		}
	}

	// Token: 0x0200015E RID: 350
	[Serializable]
	public class SurfaceSound
	{
		// Token: 0x04000974 RID: 2420
		public SoundDefinition soundDef;

		// Token: 0x04000975 RID: 2421
		public List<PhysicMaterial> materials = new List<PhysicMaterial>();

		// Token: 0x04000976 RID: 2422
		[HideInInspector]
		public Sound sound;

		// Token: 0x04000977 RID: 2423
		[HideInInspector]
		public float amount;

		// Token: 0x04000978 RID: 2424
		[HideInInspector]
		public Vector3 position = Vector3.zero;

		// Token: 0x04000979 RID: 2425
		[HideInInspector]
		public Bounds bounds;

		// Token: 0x0400097A RID: 2426
		[HideInInspector]
		public List<Vector3> points = new List<Vector3>();

		// Token: 0x0400097B RID: 2427
		[HideInInspector]
		public SoundModulation.Modulator gainMod;

		// Token: 0x0400097C RID: 2428
		[HideInInspector]
		public SoundModulation.Modulator spreadMod;
	}
}
