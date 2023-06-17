using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class SoundManager : SingletonComponent<SoundManager>, IClientComponent
{
	// Token: 0x04000AE6 RID: 2790
	public SoundClass defaultSoundClass;

	// Token: 0x04000AE7 RID: 2791
	public Dictionary<SoundDefinition, List<Sound>> activeSoundsByDef = new Dictionary<SoundDefinition, List<Sound>>();

	// Token: 0x04000AE8 RID: 2792
	public List<ISoundBudgetedUpdate> budgetUpdatables = new List<ISoundBudgetedUpdate>();

	// Token: 0x04000AE9 RID: 2793
	public List<SoundPlayer> pendingSoundPlayers = new List<SoundPlayer>();

	// Token: 0x04000AEA RID: 2794
	private List<SoundManager.ScheduledSound> scheduledSounds = new List<SoundManager.ScheduledSound>();

	// Token: 0x04000AEB RID: 2795
	private int updatableIndex;

	// Token: 0x04000AEC RID: 2796
	private Stopwatch watch = new Stopwatch();

	// Token: 0x04000AED RID: 2797
	private List<Sound> otherLocalSounds = new List<Sound>();

	// Token: 0x06000DC4 RID: 3524 RVA: 0x00062144 File Offset: 0x00060344
	public void Update()
	{
		this.StartPendingSoundPlayers();
		this.StartScheduledSounds();
		this.watch.Reset();
		this.watch.Start();
		while (this.updatableIndex < this.budgetUpdatables.Count)
		{
			ISoundBudgetedUpdate soundBudgetedUpdate = this.budgetUpdatables[this.updatableIndex];
			if (soundBudgetedUpdate != null && !soundBudgetedUpdate.Equals(null))
			{
				soundBudgetedUpdate.DoUpdate();
			}
			this.updatableIndex++;
			if (this.watch.Elapsed.TotalMilliseconds > (double)Audio.framebudget)
			{
				return;
			}
		}
		this.updatableIndex = 0;
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x000621E0 File Offset: 0x000603E0
	private void StartPendingSoundPlayers()
	{
		for (int i = this.pendingSoundPlayers.Count - 1; i >= 0; i--)
		{
			SoundPlayer soundPlayer = this.pendingSoundPlayers[i];
			if (soundPlayer == null || !soundPlayer.pending)
			{
				this.pendingSoundPlayers.RemoveAt(i);
			}
			else
			{
				soundPlayer.DoPendingUpdate();
			}
		}
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00062238 File Offset: 0x00060438
	private void StartScheduledSounds()
	{
		for (int i = this.scheduledSounds.Count - 1; i >= 0; i--)
		{
			SoundManager.ScheduledSound scheduledSound = this.scheduledSounds[i];
			if (UnityEngine.Time.time > scheduledSound.startTime)
			{
				Sound sound = SoundManager.RequestSoundInstance(scheduledSound.def, null, default(Vector3), false);
				if (!(sound == null))
				{
					sound.transform.position = scheduledSound.position;
					if (scheduledSound.volumeMod != 1f)
					{
						sound.modulation.CreateModulator(SoundModulation.Parameter.Gain).value = scheduledSound.volumeMod;
					}
					sound.Play();
					sound.RecycleAfterPlaying();
					this.scheduledSounds.Remove(scheduledSound);
				}
			}
		}
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0000CA72 File Offset: 0x0000AC72
	public static void AddPendingSoundPlayer(SoundPlayer player)
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return;
		}
		SingletonComponent<SoundManager>.Instance.pendingSoundPlayers.Add(player);
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x0000CA91 File Offset: 0x0000AC91
	public static void AddBudgetedUpdatable(ISoundBudgetedUpdate updatable)
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return;
		}
		SingletonComponent<SoundManager>.Instance.budgetUpdatables.Add(updatable);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0000CAB0 File Offset: 0x0000ACB0
	public static void RemoveBudgetedUpdatable(ISoundBudgetedUpdate updatable)
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return;
		}
		SingletonComponent<SoundManager>.Instance.budgetUpdatables.Remove(updatable);
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x000622EC File Offset: 0x000604EC
	public static SoundManager.ScheduledSound ScheduleOneshot(SoundDefinition def, float startTime, Vector3 position, float volumeMod = 1f)
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return null;
		}
		SoundManager.ScheduledSound scheduledSound = new SoundManager.ScheduledSound(def, startTime, position, volumeMod);
		SingletonComponent<SoundManager>.Instance.scheduledSounds.Add(scheduledSound);
		return scheduledSound;
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x0000CAD0 File Offset: 0x0000ACD0
	public static void CancelScheduledSound(SoundManager.ScheduledSound scheduledSound)
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return;
		}
		SingletonComponent<SoundManager>.Instance.scheduledSounds.Remove(scheduledSound);
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00062324 File Offset: 0x00060524
	public static Sound PlayOneshot(SoundDefinition def, GameObject targetParent = null, bool firstPerson = false, Vector3 position = default(Vector3))
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return null;
		}
		Sound result;
		using (TimeWarning.New("SoundManager.PlayOneshot", 0.1f))
		{
			Sound sound = SoundManager.RequestSoundInstance(def, targetParent, position, firstPerson);
			if (sound == null)
			{
				result = null;
			}
			else
			{
				if (position != Vector3.zero && targetParent == null)
				{
					sound.transform.position = position;
				}
				sound.Play();
				sound.RecycleAfterPlaying();
				result = sound;
			}
		}
		return result;
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x000623B4 File Offset: 0x000605B4
	public static Sound RequestSoundInstance(SoundDefinition def, GameObject targetParent = null, Vector3 position = default(Vector3), bool firstPerson = false)
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return null;
		}
		Sound result;
		using (TimeWarning.New("SoundManager.RequestSoundInstance", 0.1f))
		{
			if (def == null)
			{
				Debug.LogWarning("Attempted to create Sound from null SoundDefinition. targetParent: " + targetParent);
				result = null;
			}
			else if (def.template.Get() == null)
			{
				Debug.LogWarning("SoundDefinition " + def + " has null sound template!");
				result = null;
			}
			else if (!def.loop && targetParent != null && def.template.Get().GetComponent<AudioSource>().maxDistance < MainCamera.Distance(targetParent.transform.position))
			{
				result = null;
			}
			else
			{
				if (!SingletonComponent<SoundManager>.Instance.activeSoundsByDef.ContainsKey(def))
				{
					SingletonComponent<SoundManager>.Instance.activeSoundsByDef.Add(def, new List<Sound>());
				}
				int num = -100;
				if (targetParent != null)
				{
					position = targetParent.transform.position;
				}
				if (!def.loop && SingletonComponent<SoundManager>.Instance.activeSoundsByDef[def].Count >= def.globalVoiceMaxCount && SingletonComponent<SoundManager>.Instance.activeSoundsByDef[def][SingletonComponent<SoundManager>.Instance.activeSoundsByDef[def].Count - 1].startTime + def.localVoiceDebounceTime > UnityEngine.Time.time)
				{
					result = null;
				}
				else
				{
					if (SingletonComponent<SoundManager>.Instance.activeSoundsByDef[def].Count >= def.localVoiceMaxCount)
					{
						SingletonComponent<SoundManager>.Instance.otherLocalSounds.Clear();
						if ((targetParent != null || position != Vector3.zero) && !def.dontVoiceLimit)
						{
							SingletonComponent<SoundManager>.Instance.otherLocalSounds = SingletonComponent<SoundManager>.Instance.GetOtherLocalSoundInstances(def, position, SingletonComponent<SoundManager>.Instance.otherLocalSounds);
							num = SingletonComponent<SoundManager>.Instance.otherLocalSounds.Count - def.localVoiceMaxCount;
							if (num >= 0 && SingletonComponent<SoundManager>.Instance.otherLocalSounds[SingletonComponent<SoundManager>.Instance.otherLocalSounds.Count - 1].startTime + def.localVoiceDebounceTime > UnityEngine.Time.time)
							{
								return null;
							}
						}
					}
					Sound sound = SingletonComponent<SoundManager>.Instance.CreateSoundInstance(def, targetParent, firstPerson);
					num++;
					sound.transform.position = position;
					for (int i = 0; i < num; i++)
					{
						SingletonComponent<SoundManager>.Instance.otherLocalSounds[i].FadeOutAndRecycle(def.voiceLimitFadeOutTime);
					}
					int num2 = SingletonComponent<SoundManager>.Instance.activeSoundsByDef[def].Count - def.globalVoiceMaxCount;
					for (int j = 0; j < num2; j++)
					{
						Sound sound2 = SingletonComponent<SoundManager>.Instance.activeSoundsByDef[def][j];
						if (!(sound2 == null) && sound2.isAudioSourcePlaying && !sound2.fade.isFadingOut)
						{
							SingletonComponent<SoundManager>.Instance.activeSoundsByDef[def][j].FadeOutAndRecycle(def.voiceLimitFadeOutTime);
						}
					}
					result = sound;
				}
			}
		}
		return result;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x000626E0 File Offset: 0x000608E0
	private Sound CreateSoundInstance(SoundDefinition def, GameObject targetParent, bool firstPerson)
	{
		Sound result;
		using (TimeWarning.New("CreateSoundInstance", 0.1f))
		{
			GameObject soundObject = this.GetSoundObject(def);
			SoundSource soundSource = null;
			if (targetParent != null)
			{
				soundObject.transform.position = targetParent.transform.position;
				soundObject.transform.rotation = targetParent.transform.rotation;
				soundObject.transform.parent = targetParent.transform;
				soundSource = targetParent.GetComponent<SoundSource>();
				if (soundSource == null)
				{
					soundSource = targetParent.GetComponentInParent<SoundSource>();
				}
			}
			Sound component = soundObject.GetComponent<Sound>();
			if ((component.definition != null && component.definition.useCustomFalloffCurve) || def.useCustomFalloffCurve)
			{
				component.SetCustomFalloffCurve(def.falloffCurve);
			}
			if ((component.definition != null && component.definition.useCustomSpatialBlendCurve) || def.useCustomSpatialBlendCurve)
			{
				component.SetCustomSpatialBlendCurve(def.spatialBlendCurve);
			}
			if ((component.definition != null && component.definition.useCustomSpreadCurve) || def.useCustomSpreadCurve)
			{
				component.SetCustomSpreadCurve(def.spreadCurve);
			}
			component.definition = def;
			SoundRandomizer.RandomizeClipIndex(component);
			component.Init(soundSource);
			SoundRandomizer.Randomize(component, false);
			component.ApplyModulations();
			component.DoDistantCrossfade();
			if (!def.useCustomSpatialBlendCurve)
			{
				if (firstPerson || def.defaultToFirstPerson)
				{
					component.MakeFirstPerson();
				}
				else
				{
					component.MakeThirdPerson();
				}
			}
			this.budgetUpdatables.Add(component);
			this.activeSoundsByDef[component.definition].Add(component);
			result = component;
		}
		return result;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00062890 File Offset: 0x00060A90
	private List<Sound> GetOtherLocalSoundInstances(SoundDefinition def, Vector3 pos, List<Sound> otherSounds)
	{
		for (int i = 0; i < this.activeSoundsByDef[def].Count; i++)
		{
			Sound sound = this.activeSoundsByDef[def][i];
			if (!(sound == null) && sound.isAudioSourcePlaying && !sound.fade.isFadingOut && Vector3.Distance(sound.transform.position, pos) < def.localVoiceRange)
			{
				otherSounds.Add(sound);
			}
		}
		return otherSounds;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x0000CAF0 File Offset: 0x0000ACF0
	private GameObject GetSoundObject(SoundDefinition soundDef)
	{
		return GameManager.client.CreatePrefab(soundDef.template.resourcePath, Vector3.zero, Quaternion.identity, true);
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x0006290C File Offset: 0x00060B0C
	public static void RecycleSound(Sound sound)
	{
		if (!SingletonComponent<SoundManager>.Instance)
		{
			return;
		}
		using (TimeWarning.New("SoundManager.RecycleSound", 0.1f))
		{
			SingletonComponent<SoundManager>.Instance.activeSoundsByDef[sound.definition].Remove(sound);
			SingletonComponent<SoundManager>.Instance.budgetUpdatables.Remove(sound);
			if (sound.playing || sound.isAudioSourcePlaying)
			{
				sound.Stop();
			}
			GameManager.client.Retire(sound.gameObject);
		}
	}

	// Token: 0x02000189 RID: 393
	public class ScheduledSound
	{
		// Token: 0x04000AEE RID: 2798
		public SoundDefinition def;

		// Token: 0x04000AEF RID: 2799
		public float startTime;

		// Token: 0x04000AF0 RID: 2800
		public Vector3 position;

		// Token: 0x04000AF1 RID: 2801
		public float volumeMod = 1f;

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0000CB12 File Offset: 0x0000AD12
		public ScheduledSound(SoundDefinition def, float startTime, Vector3 position, float volumeMod = 1f)
		{
			this.def = def;
			this.startTime = startTime;
			this.position = position;
			this.volumeMod = volumeMod;
		}
	}
}
