using System;
using Network;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class Detonator : HeldEntity, IRFObject
{
	// Token: 0x04000570 RID: 1392
	public int frequency = 55;

	// Token: 0x04000571 RID: 1393
	private float timeSinceDeploy;

	// Token: 0x04000572 RID: 1394
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x04000573 RID: 1395
	public GameObjectRef attackEffect;

	// Token: 0x04000574 RID: 1396
	public GameObjectRef unAttackEffect;

	// Token: 0x04000575 RID: 1397
	private float deployDelay = 0.5f;

	// Token: 0x04000576 RID: 1398
	private bool wasAttacking;

	// Token: 0x04000577 RID: 1399
	private float configProgress;

	// Token: 0x04000578 RID: 1400
	private float attackHeldTime;

	// Token: 0x04000579 RID: 1401
	private float attackReleasedTime;

	// Token: 0x060008A5 RID: 2213 RVA: 0x0004D690 File Offset: 0x0004B890
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Detonator.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x00008FF7 File Offset: 0x000071F7
	public virtual bool IsFullyDeployed()
	{
		return this.timeSinceDeploy > this.deployDelay;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x00009007 File Offset: 0x00007207
	protected void ProcessInputTime()
	{
		this.timeSinceDeploy += Time.deltaTime;
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0000901B File Offset: 0x0000721B
	public override void OnDeploy()
	{
		this.wasAttacking = false;
		this.timeSinceDeploy = 0f;
		base.OnDeploy();
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x00009035 File Offset: 0x00007235
	public override void OnHolstered()
	{
		base.OnHolstered();
		PowerBar.Instance.SetVisible(false);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x00009048 File Offset: 0x00007248
	private void OpenConfigPanel()
	{
		FrequencyConfig component = GameManager.client.CreatePrefab(this.frequencyPanelPrefab.resourcePath, true).GetComponent<FrequencyConfig>();
		component.SetRFObj(this);
		component.OpenDialog();
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0004D6D4 File Offset: 0x0004B8D4
	public override void OnInput()
	{
		this.ProcessInputTime();
		base.OnInput();
		if (!this.IsFullyDeployed())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY))
		{
			this.configProgress += Time.deltaTime;
			PowerBar.Instance.SetProgress(this.configProgress * 2f);
			PowerBar.Instance.SetVisible(this.configProgress > 0f);
			if (this.configProgress >= 0.5f)
			{
				this.OpenConfigPanel();
				this.configProgress = 0f;
			}
			return;
		}
		this.configProgress = 0f;
		PowerBar.Instance.SetVisible(false);
		ownerPlayer.modelState.aiming = false;
		bool flag = ownerPlayer.CanAttack();
		bool flag2 = ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY);
		if (this.wasAttacking)
		{
			this.attackHeldTime += Time.deltaTime;
			this.attackReleasedTime = 0f;
		}
		else
		{
			this.attackReleasedTime += Time.deltaTime;
			this.attackHeldTime = 0f;
		}
		bool flag3 = this.wasAttacking && !flag2 && this.attackHeldTime < 1f;
		bool flag4 = flag && (flag2 || flag3);
		BaseViewModel instance = this.viewModel.instance;
		if (this.wasAttacking != flag4 && flag4)
		{
			this.BeginSounds();
		}
		if (instance)
		{
			instance.SetBool("attack", flag4);
		}
		this.wasAttacking = flag4;
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x00002ECE File Offset: 0x000010CE
	public void BeginSounds()
	{
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0004D868 File Offset: 0x0004BA68
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (this.viewModel)
		{
			if (name == "press")
			{
				base.ServerRPC<bool>("SetPressed", true);
				return;
			}
			if (name == "unpress")
			{
				base.ServerRPC<bool>("SetPressed", false);
			}
		}
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x00009071 File Offset: 0x00007271
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0000909D File Offset: 0x0000729D
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x00006C27 File Offset: 0x00004E27
	public void ClientSetFrequency(int newFreq)
	{
		base.ServerRPC<int>("ServerSetFrequency", newFreq);
	}
}
