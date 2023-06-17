using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class SpinnerWheel : Signage
{
	// Token: 0x040002D5 RID: 725
	private Option __menuOption_Menu_LockSpin;

	// Token: 0x040002D6 RID: 726
	private Option __menuOption_Menu_Spin;

	// Token: 0x040002D7 RID: 727
	private Option __menuOption_Menu_UnlockSpin;

	// Token: 0x040002D8 RID: 728
	public Transform wheel;

	// Token: 0x040002D9 RID: 729
	public float velocity;

	// Token: 0x040002DA RID: 730
	public Quaternion targetRotation = Quaternion.identity;

	// Token: 0x040002DB RID: 731
	[Header("Sound")]
	public SoundDefinition spinLoopSoundDef;

	// Token: 0x040002DC RID: 732
	public SoundDefinition spinStartSoundDef;

	// Token: 0x040002DD RID: 733
	public SoundDefinition spinAccentSoundDef;

	// Token: 0x040002DE RID: 734
	public SoundDefinition spinStopSoundDef;

	// Token: 0x040002DF RID: 735
	public float minTimeBetweenSpinAccentSounds = 0.3f;

	// Token: 0x040002E0 RID: 736
	public float spinAccentAngleDelta = 180f;

	// Token: 0x040002E1 RID: 737
	private Sound spinSound;

	// Token: 0x040002E2 RID: 738
	private SoundModulation.Modulator spinSoundGain;

	// Token: 0x040002E3 RID: 739
	private float angleRotated;

	// Token: 0x040002E4 RID: 740
	private float lastSpinSound;

	// Token: 0x040002E5 RID: 741
	private float clientSpinSpeed;

	// Token: 0x060005AB RID: 1451 RVA: 0x00040744 File Offset: 0x0003E944
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("SpinnerWheel.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_LockSpin", 0.1f))
			{
				if (this.Menu_LockSpin_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_LockSpin.show = true;
					this.__menuOption_Menu_LockSpin.showDisabled = false;
					this.__menuOption_Menu_LockSpin.longUseOnly = false;
					this.__menuOption_Menu_LockSpin.order = 100;
					this.__menuOption_Menu_LockSpin.icon = "key";
					this.__menuOption_Menu_LockSpin.desc = "lock_spinning_desc";
					this.__menuOption_Menu_LockSpin.title = "lock_spinning";
					if (this.__menuOption_Menu_LockSpin.function == null)
					{
						this.__menuOption_Menu_LockSpin.function = new Action<BasePlayer>(this.Menu_LockSpin);
					}
					list.Add(this.__menuOption_Menu_LockSpin);
				}
			}
			using (TimeWarning.New("Menu_Spin", 0.1f))
			{
				if (this.Menu_Spin_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Spin.show = true;
					this.__menuOption_Menu_Spin.showDisabled = false;
					this.__menuOption_Menu_Spin.longUseOnly = false;
					this.__menuOption_Menu_Spin.order = -100;
					this.__menuOption_Menu_Spin.icon = "rotate";
					this.__menuOption_Menu_Spin.desc = "spin_desc";
					this.__menuOption_Menu_Spin.title = "spin";
					if (this.__menuOption_Menu_Spin.function == null)
					{
						this.__menuOption_Menu_Spin.function = new Action<BasePlayer>(this.Menu_Spin);
					}
					list.Add(this.__menuOption_Menu_Spin);
				}
			}
			using (TimeWarning.New("Menu_UnlockSpin", 0.1f))
			{
				if (this.Menu_UnlockSpin_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_UnlockSpin.show = true;
					this.__menuOption_Menu_UnlockSpin.showDisabled = false;
					this.__menuOption_Menu_UnlockSpin.longUseOnly = false;
					this.__menuOption_Menu_UnlockSpin.order = 100;
					this.__menuOption_Menu_UnlockSpin.icon = "friends_servers";
					this.__menuOption_Menu_UnlockSpin.desc = "unlock_spinning_desc";
					this.__menuOption_Menu_UnlockSpin.title = "unlock_spinning";
					if (this.__menuOption_Menu_UnlockSpin.function == null)
					{
						this.__menuOption_Menu_UnlockSpin.function = new Action<BasePlayer>(this.Menu_UnlockSpin);
					}
					list.Add(this.__menuOption_Menu_UnlockSpin);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060005AC RID: 1452 RVA: 0x0000708C File Offset: 0x0000528C
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_LockSpin_ShowIf(LocalPlayer.Entity) || this.Menu_Spin_ShowIf(LocalPlayer.Entity) || this.Menu_UnlockSpin_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00040A10 File Offset: 0x0003EC10
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpinnerWheel.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool AllowPlayerSpins()
	{
		return true;
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00040A54 File Offset: 0x0003EC54
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spinnerWheel != null)
		{
			Quaternion quaternion = Quaternion.Euler(info.msg.spinnerWheel.spin);
			if (base.isClient)
			{
				this.targetRotation = quaternion;
			}
		}
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x000070C1 File Offset: 0x000052C1
	public virtual float GetMaxSpinSpeed()
	{
		return 720f;
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void Update_Server()
	{
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00040A9C File Offset: 0x0003EC9C
	public void Update_Client()
	{
		this.targetRotation = this.targetRotation.EnsureValid(float.Epsilon);
		if (this.wheel.rotation != this.targetRotation)
		{
			Quaternion rotation = this.wheel.rotation;
			this.wheel.rotation = Quaternion.Lerp(this.wheel.rotation, this.targetRotation, Time.deltaTime * 7f);
			float num = Quaternion.Angle(rotation, this.wheel.rotation);
			this.angleRotated += num;
			this.clientSpinSpeed = Mathf.Lerp(this.clientSpinSpeed, num, Time.deltaTime * 5f);
		}
		else
		{
			this.clientSpinSpeed = Mathf.Lerp(this.clientSpinSpeed, 0f, Time.deltaTime * 5f);
		}
		if (this.clientSpinSpeed < 0.01f)
		{
			this.clientSpinSpeed = 0f;
		}
		this.UpdateSound();
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00040B8C File Offset: 0x0003ED8C
	private void UpdateSound()
	{
		if (this.clientSpinSpeed > 5f && this.spinSound == null)
		{
			SoundManager.PlayOneshot(this.spinStartSoundDef, base.gameObject, false, default(Vector3));
			this.spinSound = SoundManager.RequestSoundInstance(this.spinLoopSoundDef, base.gameObject, default(Vector3), false);
			this.spinSoundGain = this.spinSound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
			this.spinSoundGain.value = 0f;
			this.spinSound.FadeInAndPlay(0.1f);
		}
		else if (this.clientSpinSpeed <= 0.1f && this.spinSound != null)
		{
			this.spinSound.FadeOutAndRecycle(0.1f);
			this.spinSound = null;
			this.spinSoundGain = null;
			if (this.spinStopSoundDef != null)
			{
				SoundManager.PlayOneshot(this.spinStopSoundDef, base.gameObject, false, default(Vector3));
			}
		}
		if (this.spinSound != null && this.spinSoundGain != null)
		{
			this.spinSoundGain.value = Mathf.Lerp(this.spinSoundGain.value, Mathf.Clamp01(this.clientSpinSpeed * 0.05f), Time.deltaTime * 5f);
		}
		if (this.spinAccentSoundDef != null && this.angleRotated > this.spinAccentAngleDelta && Time.time > this.lastSpinSound + this.minTimeBetweenSpinAccentSounds)
		{
			SoundManager.PlayOneshot(this.spinAccentSoundDef, base.gameObject, false, default(Vector3));
			this.angleRotated = 0f;
			this.lastSpinSound = Time.time;
		}
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x000070C8 File Offset: 0x000052C8
	public void Update()
	{
		if (base.isClient)
		{
			this.Update_Client();
		}
		if (base.isServer)
		{
			this.Update_Server();
		}
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x00005EBB File Offset: 0x000040BB
	public bool AnyoneSpin()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x000070E6 File Offset: 0x000052E6
	[BaseEntity.Menu.Description("spin_desc", "Spin the wheel")]
	[BaseEntity.Menu.ShowIf("Menu_Spin_ShowIf")]
	[BaseEntity.Menu("spin", "Spin", Order = -100)]
	[BaseEntity.Menu.Icon("rotate")]
	public void Menu_Spin(BasePlayer player)
	{
		base.ServerRPC("RPC_Spin");
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x000070F3 File Offset: 0x000052F3
	public bool Menu_Spin_ShowIf(BasePlayer player)
	{
		return this.AnyoneSpin() || (player.CanBuild() && this.AllowPlayerSpins());
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x0000710F File Offset: 0x0000530F
	[BaseEntity.Menu.Description("unlock_spinning_desc", "Allow Anyone to spin the wheel")]
	[BaseEntity.Menu.ShowIf("Menu_UnlockSpin_ShowIf")]
	[BaseEntity.Menu.Icon("friends_servers")]
	[BaseEntity.Menu("unlock_spinning", "Anyone Spin", Order = 100)]
	public void Menu_UnlockSpin(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_AnyoneSpin", true);
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x0000711D File Offset: 0x0000531D
	public bool Menu_UnlockSpin_ShowIf(BasePlayer player)
	{
		return player.CanBuild() && !this.AnyoneSpin() && this.AllowPlayerSpins();
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00007137 File Offset: 0x00005337
	[BaseEntity.Menu.Description("lock_spinning_desc", "Only players with Building Privilege can spin the wheel")]
	[BaseEntity.Menu.Icon("key")]
	[BaseEntity.Menu.ShowIf("Menu_LockSpin_ShowIf")]
	[BaseEntity.Menu("lock_spinning", "Owner Spin", Order = 100)]
	public void Menu_LockSpin(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_AnyoneSpin", false);
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00007145 File Offset: 0x00005345
	public bool Menu_LockSpin_ShowIf(BasePlayer player)
	{
		return player.CanBuild() && this.AnyoneSpin() && this.AllowPlayerSpins();
	}
}
