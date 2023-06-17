using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class BaseHelicopter : BaseCombatEntity
{
	// Token: 0x040003AA RID: 938
	public GameObject rotorPivot;

	// Token: 0x040003AB RID: 939
	public GameObject mainRotor;

	// Token: 0x040003AC RID: 940
	public GameObject mainRotor_blades;

	// Token: 0x040003AD RID: 941
	public GameObject mainRotor_blur;

	// Token: 0x040003AE RID: 942
	public GameObject tailRotor;

	// Token: 0x040003AF RID: 943
	public GameObject tailRotor_blades;

	// Token: 0x040003B0 RID: 944
	public GameObject tailRotor_blur;

	// Token: 0x040003B1 RID: 945
	public GameObject rocket_tube_left;

	// Token: 0x040003B2 RID: 946
	public GameObject rocket_tube_right;

	// Token: 0x040003B3 RID: 947
	public GameObject left_gun_yaw;

	// Token: 0x040003B4 RID: 948
	public GameObject left_gun_pitch;

	// Token: 0x040003B5 RID: 949
	public GameObject left_gun_muzzle;

	// Token: 0x040003B6 RID: 950
	public GameObject right_gun_yaw;

	// Token: 0x040003B7 RID: 951
	public GameObject right_gun_pitch;

	// Token: 0x040003B8 RID: 952
	public GameObject right_gun_muzzle;

	// Token: 0x040003B9 RID: 953
	public GameObject spotlight_rotation;

	// Token: 0x040003BA RID: 954
	public GameObjectRef rocket_fire_effect;

	// Token: 0x040003BB RID: 955
	public GameObjectRef gun_fire_effect;

	// Token: 0x040003BC RID: 956
	public GameObjectRef bulletEffect;

	// Token: 0x040003BD RID: 957
	public GameObjectRef explosionEffect;

	// Token: 0x040003BE RID: 958
	public GameObjectRef fireBall;

	// Token: 0x040003BF RID: 959
	public GameObjectRef crateToDrop;

	// Token: 0x040003C0 RID: 960
	public int maxCratesToSpawn = 4;

	// Token: 0x040003C1 RID: 961
	public float bulletSpeed = 250f;

	// Token: 0x040003C2 RID: 962
	public float bulletDamage = 20f;

	// Token: 0x040003C3 RID: 963
	public GameObjectRef servergibs;

	// Token: 0x040003C4 RID: 964
	public GameObjectRef debrisFieldMarker;

	// Token: 0x040003C5 RID: 965
	public SoundDefinition rotorWashSoundDef;

	// Token: 0x040003C6 RID: 966
	public SoundDefinition engineSoundDef;

	// Token: 0x040003C7 RID: 967
	public SoundDefinition rotorSoundDef;

	// Token: 0x040003C8 RID: 968
	private Sound _engineSound;

	// Token: 0x040003C9 RID: 969
	private Sound _rotorSound;

	// Token: 0x040003CA RID: 970
	private Sound _rotorWashSound;

	// Token: 0x040003CB RID: 971
	public float spotlightJitterAmount = 5f;

	// Token: 0x040003CC RID: 972
	public float spotlightJitterSpeed = 5f;

	// Token: 0x040003CD RID: 973
	public GameObject[] nightLights;

	// Token: 0x040003CE RID: 974
	public Vector3 spotlightTarget;

	// Token: 0x040003CF RID: 975
	public float engineSpeed = 1f;

	// Token: 0x040003D0 RID: 976
	public float targetEngineSpeed = 1f;

	// Token: 0x040003D1 RID: 977
	public float blur_rotationScale = 0.05f;

	// Token: 0x040003D2 RID: 978
	public ParticleSystem[] _rotorWashParticles;

	// Token: 0x040003D3 RID: 979
	private PatrolHelicopterAI myAI;

	// Token: 0x040003D4 RID: 980
	private Quaternion client_rotorPivotIdeal = Quaternion.identity;

	// Token: 0x040003D5 RID: 981
	private bool nightLightsOn;

	// Token: 0x040003D6 RID: 982
	public BaseHelicopter.weakspot[] weakspots;

	// Token: 0x06000706 RID: 1798 RVA: 0x0004563C File Offset: 0x0004383C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseHelicopter.OnRpcMessage", 0.1f))
		{
			if (rpc == 3857190246U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: FireGun ");
				}
				using (TimeWarning.New("FireGun", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.FireGun(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in FireGun", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00004EC7 File Offset: 0x000030C7
	public override float MaxVelocity()
	{
		return 100f;
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x00007F54 File Offset: 0x00006154
	public override void InitShared()
	{
		base.InitShared();
		this.InitalizeWeakspots();
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00045758 File Offset: 0x00043958
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.helicopter != null)
		{
			this.client_rotorPivotIdeal = Quaternion.Euler(info.msg.helicopter.tiltRot);
			this.spotlightTarget = info.msg.helicopter.spotlightVec;
			for (int i = 0; i < this.weakspots.Length; i++)
			{
				this.weakspots[i].ClientHealthUpdate(info.msg.helicopter.weakspothealths[i]);
			}
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00007F62 File Offset: 0x00006162
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		if (base.isServer)
		{
			return;
		}
		this.InitializeClientsideEffects();
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x000457E0 File Offset: 0x000439E0
	public void InitializeClientsideEffects()
	{
		if (this.engineSoundDef != null)
		{
			this._engineSound = SoundManager.RequestSoundInstance(this.engineSoundDef, base.gameObject, default(Vector3), false);
			this._engineSound.Play();
		}
		if (this.rotorSoundDef != null)
		{
			this._rotorSound = SoundManager.RequestSoundInstance(this.rotorSoundDef, base.gameObject, default(Vector3), false);
			this._rotorSound.Play();
		}
		if (this.rotorWashSoundDef != null)
		{
			this._rotorWashSound = SoundManager.RequestSoundInstance(this.rotorWashSoundDef, base.gameObject, default(Vector3), false);
			this._rotorWashSound.Play();
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0004589C File Offset: 0x00043A9C
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		if (this._rotorSound)
		{
			this._rotorSound.StopAndRecycle(0f);
		}
		if (this._engineSound)
		{
			this._engineSound.StopAndRecycle(0f);
		}
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x000458EC File Offset: 0x00043AEC
	public void SetLights(bool areOn)
	{
		if (areOn == this.nightLightsOn)
		{
			return;
		}
		GameObject[] array = this.nightLights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(areOn);
		}
		this.nightLightsOn = areOn;
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00045928 File Offset: 0x00043B28
	public void Update()
	{
		if (base.isClient)
		{
			this.engineSpeed = Mathf.Lerp(this.engineSpeed, this.targetEngineSpeed, UnityEngine.Time.deltaTime * 0.25f);
			this.SetLights(TOD_Sky.Instance.IsNight);
			if (this.rotorPivot.transform.localRotation != this.client_rotorPivotIdeal)
			{
				this.rotorPivot.transform.localRotation = Quaternion.Lerp(this.rotorPivot.transform.localRotation, this.client_rotorPivotIdeal, UnityEngine.Time.deltaTime * 5f);
			}
			Vector3 a = (this.spotlightTarget == Vector3.zero) ? (base.transform.position + base.transform.forward * 10f + base.transform.up * -10f) : this.spotlightTarget;
			this.spotlight_rotation.transform.rotation = Quaternion.Lerp(this.spotlight_rotation.transform.rotation, Quaternion.LookRotation((a - this.spotlight_rotation.transform.position).normalized), UnityEngine.Time.deltaTime * 3f);
			float num = UnityEngine.Time.time * this.spotlightJitterSpeed;
			float num2 = this.spotlightJitterAmount;
			Vector3 forward = this.spotlight_rotation.transform.forward;
			forward.y += (Mathf.PerlinNoise(num, num) - 0.5f) * num2 * UnityEngine.Time.deltaTime;
			forward.x += (Mathf.PerlinNoise(num + 0.1f, num + 0.2f) - 0.5f) * num2 * UnityEngine.Time.deltaTime;
			this.spotlight_rotation.transform.rotation = Quaternion.LookRotation(forward);
			this.UpdateEffects();
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00045B00 File Offset: 0x00043D00
	public void UpdateEffects()
	{
		if (base.isServer)
		{
			return;
		}
		float num = 1800f * this.engineSpeed * UnityEngine.Time.deltaTime;
		float angle = num * this.blur_rotationScale;
		this.mainRotor.transform.Rotate(Vector3.up, num);
		this.mainRotor_blur.transform.Rotate(Vector3.up, angle);
		this.tailRotor.transform.Rotate(Vector3.right, num);
		this.tailRotor_blur.transform.Rotate(Vector3.right, angle);
		this.UpdateRotorVisibility();
		int num2 = 1;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, -this.rotorPivot.transform.up, ref raycastHit, 20f, num2))
		{
			this.UpdateRotorWashPosition(raycastHit.point + new Vector3(0f, 0.25f, 0f), raycastHit.normal);
			this.ToggleRotorWash(true);
			return;
		}
		this.ToggleRotorWash(false);
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00045C00 File Offset: 0x00043E00
	public void UpdateRotorWashPosition(Vector3 pos, Vector3 normal)
	{
		foreach (ParticleSystem particleSystem in this._rotorWashParticles)
		{
			particleSystem.transform.position = pos;
			particleSystem.transform.rotation = Quaternion.LookRotation(normal);
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00045C44 File Offset: 0x00043E44
	public void ToggleRotorWash(bool enabled)
	{
		ParticleSystem[] rotorWashParticles = this._rotorWashParticles;
		for (int i = 0; i < rotorWashParticles.Length; i++)
		{
			rotorWashParticles[i].enableEmission = enabled;
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00045C70 File Offset: 0x00043E70
	public void UpdateRotorVisibility()
	{
		bool flag = this.engineSpeed < 0.75f;
		this.mainRotor_blades.SetActive(flag);
		this.tailRotor_blades.SetActive(flag);
		this.mainRotor_blur.SetActive(!flag);
		this.tailRotor_blur.SetActive(!flag);
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00045CC4 File Offset: 0x00043EC4
	[BaseEntity.RPC_Client]
	public void FireGun(BaseEntity.RPCMessage rpc)
	{
		bool flag = rpc.read.Bit();
		Vector3 a = rpc.read.Vector3();
		Transform transform = flag ? this.left_gun_muzzle.transform : this.right_gun_muzzle.transform;
		Vector3 position = transform.transform.position;
		Vector3 normalized = (a - position).normalized;
		Effect.client.Run(this.gun_fire_effect.resourcePath, this, StringPool.Get(transform.gameObject.name), Vector3.zero, transform.InverseTransformDirection(normalized));
		GameObject gameObject = GameManager.client.CreatePrefab(this.bulletEffect.resourcePath, position + normalized * 2f, Quaternion.LookRotation(normalized), false);
		if (gameObject == null)
		{
			return;
		}
		Projectile component = gameObject.GetComponent<Projectile>();
		if (component)
		{
			component.clientsideEffect = true;
			component.owner = null;
			component.seed = 0;
			component.InitializeVelocity(normalized * this.bulletSpeed);
		}
		gameObject.SetActive(true);
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00045DCC File Offset: 0x00043FCC
	public void InitalizeWeakspots()
	{
		BaseHelicopter.weakspot[] array = this.weakspots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].body = this;
		}
	}

	// Token: 0x0200006C RID: 108
	[Serializable]
	public class weakspot
	{
		// Token: 0x040003D7 RID: 983
		[NonSerialized]
		public BaseHelicopter body;

		// Token: 0x040003D8 RID: 984
		public string[] bonenames;

		// Token: 0x040003D9 RID: 985
		public float maxHealth;

		// Token: 0x040003DA RID: 986
		public float health;

		// Token: 0x040003DB RID: 987
		public float healthFractionOnDestroyed = 0.5f;

		// Token: 0x040003DC RID: 988
		public GameObjectRef destroyedParticles;

		// Token: 0x040003DD RID: 989
		public GameObjectRef damagedParticles;

		// Token: 0x040003DE RID: 990
		public GameObject damagedEffect;

		// Token: 0x040003DF RID: 991
		public GameObject destroyedEffect;

		// Token: 0x040003E0 RID: 992
		public List<BasePlayer> attackers;

		// Token: 0x06000716 RID: 1814 RVA: 0x00007F7A File Offset: 0x0000617A
		public float HealthFraction()
		{
			return this.health / this.maxHealth;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x00045E6C File Offset: 0x0004406C
		public void ClientHealthUpdate(float newHealth)
		{
			if (newHealth == this.health)
			{
				return;
			}
			this.health = newHealth;
			float num = this.HealthFraction();
			if (this.destroyedEffect != null)
			{
				this.destroyedEffect.SetActive(num <= 0f);
			}
			if (this.damagedEffect != null)
			{
				this.damagedEffect.SetActive(num <= 0.5f && num > 0f);
			}
		}
	}
}
