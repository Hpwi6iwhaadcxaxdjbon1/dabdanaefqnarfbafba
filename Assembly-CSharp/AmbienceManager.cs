using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class AmbienceManager : SingletonComponent<AmbienceManager>, IClientComponent
{
	// Token: 0x0400094A RID: 2378
	public List<AmbienceManager.EmitterTypeLimit> localEmitterLimits = new List<AmbienceManager.EmitterTypeLimit>();

	// Token: 0x0400094B RID: 2379
	public AmbienceManager.EmitterTypeLimit catchallEmitterLimit = new AmbienceManager.EmitterTypeLimit();

	// Token: 0x0400094C RID: 2380
	public int maxActiveLocalEmitters = 5;

	// Token: 0x0400094D RID: 2381
	public int activeLocalEmitters;

	// Token: 0x0400094E RID: 2382
	public List<AmbienceEmitter> cameraEmitters = new List<AmbienceEmitter>();

	// Token: 0x0400094F RID: 2383
	public List<AmbienceEmitter> emittersInRange = new List<AmbienceEmitter>();

	// Token: 0x04000950 RID: 2384
	public List<AmbienceEmitter> activeEmitters = new List<AmbienceEmitter>();

	// Token: 0x04000951 RID: 2385
	public float localEmitterRange = 30f;

	// Token: 0x04000952 RID: 2386
	public List<AmbienceZone> currentAmbienceZones = new List<AmbienceZone>();

	// Token: 0x04000953 RID: 2387
	private Dictionary<AmbienceDefinitionList, AmbienceManager.EmitterTypeLimit> emitterLimitByAmbience = new Dictionary<AmbienceDefinitionList, AmbienceManager.EmitterTypeLimit>();

	// Token: 0x04000954 RID: 2388
	private float tickInterval = 0.25f;

	// Token: 0x04000955 RID: 2389
	private float lastTick;

	// Token: 0x04000956 RID: 2390
	private List<AmbienceManager.AmbienceGroup> ambienceGroups = new List<AmbienceManager.AmbienceGroup>();

	// Token: 0x04000957 RID: 2391
	private Dictionary<AmbienceDefinitionList, AmbienceManager.AmbienceGroup> ambienceGroupsByDef = new Dictionary<AmbienceDefinitionList, AmbienceManager.AmbienceGroup>();

	// Token: 0x06000C9D RID: 3229 RVA: 0x0005C558 File Offset: 0x0005A758
	private void Start()
	{
		for (int i = 0; i < this.localEmitterLimits.Count; i++)
		{
			for (int j = 0; j < this.localEmitterLimits[i].ambience.Count; j++)
			{
				this.emitterLimitByAmbience.Add(this.localEmitterLimits[i].ambience[j], this.localEmitterLimits[i]);
			}
		}
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0005C5CC File Offset: 0x0005A7CC
	private void OnApplicationQuit()
	{
		for (int i = 0; i < this.ambienceGroups.Count; i++)
		{
			if (this.ambienceGroups[i].cullingGroup != null)
			{
				this.ambienceGroups[i].cullingGroup.Dispose();
				this.ambienceGroups[i].cullingGroup = null;
			}
		}
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0000BC8B File Offset: 0x00009E8B
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.DisableAdvancedAmbience();
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0005C62C File Offset: 0x0005A82C
	private void Update()
	{
		if (Audio.ambience)
		{
			this.EnableAdvancedAmbience();
		}
		else
		{
			this.DisableAdvancedAmbience();
		}
		if (UnityEngine.Time.time > this.lastTick + this.tickInterval)
		{
			this.Tick();
		}
		for (int i = 0; i < this.ambienceGroups.Count; i++)
		{
			if (MainCamera.isValid && this.ambienceGroups[i].cullingGroup != null && this.ambienceGroups[i].cullingGroup.targetCamera != MainCamera.mainCamera)
			{
				this.ambienceGroups[i].cullingGroup.targetCamera = MainCamera.mainCamera;
				this.ambienceGroups[i].cullingGroup.SetDistanceReferencePoint(SingletonComponent<MainCamera>.Instance.transform);
			}
		}
		for (int j = 0; j < this.activeEmitters.Count; j++)
		{
			this.activeEmitters[j].DoUpdate();
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0000BC9B File Offset: 0x00009E9B
	private void Tick()
	{
		this.UpdateCullingGroups();
		this.SortLocalEmitters();
		this.TickLocalEmitters();
		this.TickCameraEmitters();
		this.lastTick = UnityEngine.Time.time;
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0005C720 File Offset: 0x0005A920
	private void TickLocalEmitters()
	{
		for (int i = 0; i < this.localEmitterLimits.Count; i++)
		{
			this.localEmitterLimits[i].active = 0;
		}
		this.catchallEmitterLimit.active = 0;
		this.activeLocalEmitters = 0;
		for (int j = 0; j < this.emittersInRange.Count; j++)
		{
			AmbienceEmitter ambienceEmitter = this.emittersInRange[j];
			if (ambienceEmitter == null)
			{
				this.RemoveEmitter(ambienceEmitter);
			}
			else if (ambienceEmitter.cameraDistance > this.localEmitterRange)
			{
				this.EmitterLeaveRange(ambienceEmitter);
			}
			else
			{
				AmbienceManager.EmitterTypeLimit emitterTypeLimit = this.EmitterLimit(ambienceEmitter);
				if (emitterTypeLimit == null)
				{
					Debug.LogWarning("Couldn't find emitter limit settings for AmbienceEmitter on " + ambienceEmitter.gameObject);
				}
				else if (emitterTypeLimit.active < emitterTypeLimit.limit && this.activeLocalEmitters < this.maxActiveLocalEmitters && ambienceEmitter.gameObject.activeInHierarchy && ambienceEmitter.isActiveAndEnabled)
				{
					if (!ambienceEmitter.active)
					{
						this.ActivateEmitter(ambienceEmitter);
					}
					ambienceEmitter.Tick();
					emitterTypeLimit.active++;
					this.activeLocalEmitters++;
				}
				else if (ambienceEmitter.active && !ambienceEmitter.IsFadingOut())
				{
					ambienceEmitter.FadeOut(true);
				}
			}
		}
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0000BCC0 File Offset: 0x00009EC0
	public void DeactivateEmitter(AmbienceEmitter emitter)
	{
		emitter.active = false;
		this.activeEmitters.Remove(emitter);
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0000BCD6 File Offset: 0x00009ED6
	public void ActivateEmitter(AmbienceEmitter emitter)
	{
		if (emitter.active && !emitter.IsFadingOut())
		{
			return;
		}
		emitter.active = true;
		emitter.Reset();
		this.activeEmitters.Add(emitter);
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0005C860 File Offset: 0x0005AA60
	private void TickCameraEmitters()
	{
		for (int i = 0; i < this.cameraEmitters.Count; i++)
		{
			this.cameraEmitters[i].Tick();
		}
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0005C894 File Offset: 0x0005AA94
	private void SortLocalEmitters()
	{
		for (int i = 0; i < this.emittersInRange.Count; i++)
		{
			if (this.emittersInRange[i] == null)
			{
				this.RemoveEmitter(this.emittersInRange[i]);
			}
			else
			{
				this.emittersInRange[i].UpdateCameraDistance();
			}
		}
		if (this.emittersInRange.Count > 1)
		{
			this.SortEmitters(this.emittersInRange);
		}
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0000BD02 File Offset: 0x00009F02
	private AmbienceManager.EmitterTypeLimit EmitterLimit(AmbienceEmitter emitter)
	{
		if (this.emitterLimitByAmbience.ContainsKey(emitter.baseAmbience))
		{
			return this.emitterLimitByAmbience[emitter.baseAmbience];
		}
		return this.catchallEmitterLimit;
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x0005C90C File Offset: 0x0005AB0C
	public void OnCullingGroupChange(CullingGroupEvent evt, AmbienceManager.AmbienceGroup group)
	{
		if (evt.currentDistance != evt.previousDistance && evt.currentDistance == 0)
		{
			AmbienceEmitter ambienceEmitter = group.emittersBySphereIdx[evt.index];
			if (!this.emittersInRange.Contains(ambienceEmitter))
			{
				this.emittersInRange.Add(ambienceEmitter);
			}
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0000BD2F File Offset: 0x00009F2F
	public void EmitterLeaveRange(AmbienceEmitter emitter)
	{
		this.emittersInRange.Remove(emitter);
		emitter.FadeOut(true);
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0005C960 File Offset: 0x0005AB60
	public void AddEmitter(AmbienceEmitter emitter)
	{
		AmbienceManager.AmbienceGroup ambienceGroup = this.AmbienceGroupForEmitter(emitter);
		ambienceGroup.emitters.Add(emitter);
		if (ambienceGroup.cullingGroup != null)
		{
			ambienceGroup.cullingGroupDirty = true;
		}
		if (MainCamera.Distance(emitter.transform.position) < this.localEmitterRange)
		{
			if (!this.emittersInRange.Contains(emitter))
			{
				this.emittersInRange.Add(emitter);
			}
			AmbienceManager.EmitterTypeLimit emitterTypeLimit = this.EmitterLimit(emitter);
			if (emitterTypeLimit == null)
			{
				Debug.LogWarning("Couldn't find emitter limit settings for AmbienceEmitter on " + emitter.gameObject, emitter.gameObject);
				return;
			}
			if (emitterTypeLimit.active < emitterTypeLimit.limit && this.activeLocalEmitters < this.maxActiveLocalEmitters && emitter.gameObject.activeInHierarchy && emitter.isActiveAndEnabled)
			{
				if (!emitter.active)
				{
					this.ActivateEmitter(emitter);
				}
				emitter.Tick();
				emitterTypeLimit.active++;
				this.activeLocalEmitters++;
			}
		}
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0005CA50 File Offset: 0x0005AC50
	public void RemoveEmitter(AmbienceEmitter emitter)
	{
		AmbienceManager.AmbienceGroup ambienceGroup = this.AmbienceGroupForEmitter(emitter);
		ambienceGroup.emitters.Remove(emitter);
		this.emittersInRange.Remove(emitter);
		this.activeEmitters.Remove(emitter);
		if (ambienceGroup.cullingGroup != null)
		{
			ambienceGroup.cullingGroupDirty = true;
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0000BD45 File Offset: 0x00009F45
	public void AddCameraEmitter(AmbienceEmitter emitter)
	{
		this.cameraEmitters.Add(emitter);
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0000BD53 File Offset: 0x00009F53
	public void RemoveCameraEmitter(AmbienceEmitter emitter)
	{
		this.cameraEmitters.Remove(emitter);
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0005CA9C File Offset: 0x0005AC9C
	private void UpdateCullingGroups()
	{
		for (int i = 0; i < this.ambienceGroups.Count; i++)
		{
			if (this.ambienceGroups[i].cullingGroupDirty)
			{
				this.ambienceGroups[i].emittersBySphereIdx.Clear();
				if (this.ambienceGroups[i].cullGroupSpheres.Length < this.ambienceGroups[i].emitters.Count)
				{
					Array.Resize<BoundingSphere>(ref this.ambienceGroups[i].cullGroupSpheres, this.ambienceGroups[i].emitters.Count + 100);
				}
				int num = 0;
				foreach (AmbienceEmitter ambienceEmitter in this.ambienceGroups[i].emitters)
				{
					this.ambienceGroups[i].cullGroupSpheres[num++] = ambienceEmitter.boundingSphere;
					this.ambienceGroups[i].emittersBySphereIdx.Add(ambienceEmitter);
				}
				this.ambienceGroups[i].cullingGroup.SetBoundingSpheres(this.ambienceGroups[i].cullGroupSpheres);
				this.ambienceGroups[i].cullingGroup.SetBoundingSphereCount(this.ambienceGroups[i].emitters.Count);
				this.ambienceGroups[i].cullingGroupDirty = false;
			}
		}
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0005CC34 File Offset: 0x0005AE34
	private AmbienceManager.AmbienceGroup AmbienceGroupForEmitter(AmbienceEmitter emitter)
	{
		if (this.ambienceGroupsByDef.ContainsKey(emitter.baseAmbience))
		{
			return this.ambienceGroupsByDef[emitter.baseAmbience];
		}
		AmbienceManager.AmbienceGroup ambienceGroup = new AmbienceManager.AmbienceGroup();
		ambienceGroup.ambienceDefinition = emitter.baseAmbience;
		this.ambienceGroups.Add(ambienceGroup);
		this.ambienceGroupsByDef.Add(emitter.baseAmbience, ambienceGroup);
		return ambienceGroup;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0005CC98 File Offset: 0x0005AE98
	public void EnableAdvancedAmbience()
	{
		for (int i = 0; i < this.ambienceGroups.Count; i++)
		{
			if (this.ambienceGroups[i].cullingGroup == null)
			{
				this.ambienceGroups[i].cullingGroup = new CullingGroup();
				float[] boundingDistances = new float[]
				{
					this.localEmitterRange,
					float.PositiveInfinity
				};
				this.ambienceGroups[i].cullingGroup.SetBoundingDistances(boundingDistances);
				this.ambienceGroups[i].cullingGroup.onStateChanged = new CullingGroup.StateChanged(this.ambienceGroups[i].OnCullingGroupChange);
				this.ambienceGroups[i].cullingGroupDirty = true;
			}
		}
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0005CD5C File Offset: 0x0005AF5C
	public void DisableAdvancedAmbience()
	{
		for (int i = 0; i < this.ambienceGroups.Count; i++)
		{
			if (this.ambienceGroups[i].cullingGroup == null)
			{
				return;
			}
			for (int j = 0; j < this.emittersInRange.Count; j++)
			{
				this.emittersInRange[j].FadeOut(true);
			}
			this.emittersInRange.Clear();
			this.ambienceGroups[i].cullingGroup.Dispose();
			this.ambienceGroups[i].cullingGroup = null;
			this.ambienceGroups[i].cullingGroupDirty = false;
		}
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0000BD62 File Offset: 0x00009F62
	private void SortEmitters(List<AmbienceEmitter> emitterList)
	{
		this.SortEmitters(emitterList, 0, emitterList.Count - 1);
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0005CE08 File Offset: 0x0005B008
	private void SortEmitters(List<AmbienceEmitter> emitterList, int left, int right)
	{
		int i = left;
		int num = right;
		AmbienceEmitter other = emitterList[left + (right - left) / 2];
		while (i <= num)
		{
			while (emitterList[i].CompareTo(other) < 0)
			{
				i++;
			}
			while (emitterList[num].CompareTo(other) > 0)
			{
				num--;
			}
			if (i <= num)
			{
				AmbienceEmitter ambienceEmitter = emitterList[i];
				emitterList[i] = emitterList[num];
				emitterList[num] = ambienceEmitter;
				i++;
				num--;
			}
		}
		if (left < num)
		{
			this.SortEmitters(emitterList, left, num);
		}
		if (i < right)
		{
			this.SortEmitters(emitterList, i, right);
		}
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0000BD74 File Offset: 0x00009F74
	public void AmbienceZoneEntered(AmbienceZone zone)
	{
		this.currentAmbienceZones.Add(zone);
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0000BD82 File Offset: 0x00009F82
	public void AmbienceZoneExited(AmbienceZone zone)
	{
		this.currentAmbienceZones.Remove(zone);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0005CE9C File Offset: 0x0005B09C
	public AmbienceZone CurrentAmbienceZone()
	{
		AmbienceZone result = null;
		float num = float.NegativeInfinity;
		for (int i = 0; i < this.currentAmbienceZones.Count; i++)
		{
			if (this.currentAmbienceZones[i].priority > num)
			{
				result = this.currentAmbienceZones[i];
				num = this.currentAmbienceZones[i].priority;
			}
		}
		return result;
	}

	// Token: 0x02000159 RID: 345
	[Serializable]
	public class EmitterTypeLimit
	{
		// Token: 0x04000958 RID: 2392
		public List<AmbienceDefinitionList> ambience;

		// Token: 0x04000959 RID: 2393
		public int limit = 1;

		// Token: 0x0400095A RID: 2394
		public int active;
	}

	// Token: 0x0200015A RID: 346
	public class AmbienceGroup
	{
		// Token: 0x0400095B RID: 2395
		public AmbienceDefinitionList ambienceDefinition;

		// Token: 0x0400095C RID: 2396
		public HashSet<AmbienceEmitter> emitters = new HashSet<AmbienceEmitter>();

		// Token: 0x0400095D RID: 2397
		public CullingGroup cullingGroup;

		// Token: 0x0400095E RID: 2398
		public BoundingSphere[] cullGroupSpheres = new BoundingSphere[100];

		// Token: 0x0400095F RID: 2399
		public List<AmbienceEmitter> emittersBySphereIdx = new List<AmbienceEmitter>();

		// Token: 0x04000960 RID: 2400
		public bool cullingGroupDirty = true;

		// Token: 0x06000CB9 RID: 3257 RVA: 0x0000BDA0 File Offset: 0x00009FA0
		public void OnCullingGroupChange(CullingGroupEvent evt)
		{
			SingletonComponent<AmbienceManager>.Instance.OnCullingGroupChange(evt, this);
		}
	}
}
