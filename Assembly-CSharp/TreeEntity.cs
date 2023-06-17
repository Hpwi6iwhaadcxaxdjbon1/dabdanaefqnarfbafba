using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class TreeEntity : ResourceEntity
{
	// Token: 0x0400067F RID: 1663
	public GameObjectRef prefab;

	// Token: 0x04000680 RID: 1664
	public bool hasBonusGame = true;

	// Token: 0x04000681 RID: 1665
	public GameObjectRef bonusHitEffect;

	// Token: 0x04000682 RID: 1666
	public GameObjectRef bonusHitSound;

	// Token: 0x04000683 RID: 1667
	public Collider serverCollider;

	// Token: 0x04000684 RID: 1668
	public Collider clientCollider;

	// Token: 0x04000685 RID: 1669
	public SoundDefinition smallCrackSoundDef;

	// Token: 0x04000686 RID: 1670
	public SoundDefinition medCrackSoundDef;

	// Token: 0x04000687 RID: 1671
	private float lastAttackDamage;

	// Token: 0x04000688 RID: 1672
	private GameObject instance;

	// Token: 0x04000689 RID: 1673
	[Header("Falling")]
	public bool fallOnKilled = true;

	// Token: 0x0400068A RID: 1674
	public float fallDuration = 1.5f;

	// Token: 0x0400068B RID: 1675
	public GameObjectRef fallStartSound;

	// Token: 0x0400068C RID: 1676
	public GameObjectRef fallImpactSound;

	// Token: 0x0400068D RID: 1677
	public GameObjectRef fallImpactParticles;

	// Token: 0x0400068E RID: 1678
	public SoundDefinition fallLeavesLoopDef;

	// Token: 0x0400068F RID: 1679
	[NonSerialized]
	public bool[] usedHeights = new bool[20];

	// Token: 0x04000690 RID: 1680
	public bool impactSoundPlayed;

	// Token: 0x04000691 RID: 1681
	[NonSerialized]
	public float treeDistanceUponFalling;

	// Token: 0x04000692 RID: 1682
	protected Transform treeBaseRef;

	// Token: 0x04000693 RID: 1683
	protected Vector3 oldInstancePos = Vector3.zero;

	// Token: 0x04000694 RID: 1684
	protected Quaternion oldInstanceRot = Quaternion.identity;

	// Token: 0x04000695 RID: 1685
	protected float lastTreeFallTickTime;

	// Token: 0x04000696 RID: 1686
	protected float fallStartTime = -1f;

	// Token: 0x04000697 RID: 1687
	private Sound fallLeavesLoop;

	// Token: 0x04000698 RID: 1688
	private SoundModulation.Modulator fallLeavesGainMod;

	// Token: 0x04000699 RID: 1689
	[NonSerialized]
	private Vector3 hitDirection;

	// Token: 0x0400069A RID: 1690
	[NonSerialized]
	private Vector3 rotateDirection;

	// Token: 0x0400069B RID: 1691
	private float impactSoundCheckHeight;

	// Token: 0x06000A13 RID: 2579 RVA: 0x00053860 File Offset: 0x00051A60
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 1414662401U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CrackSound ");
				}
				using (TimeWarning.New("CrackSound", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CrackSound(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CrackSound", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 2817179413U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: TreeFall ");
				}
				using (TimeWarning.New("TreeFall", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TreeFall(msg3);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in TreeFall", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00009F9D File Offset: 0x0000819D
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isClient)
		{
			this.DestroySkin();
		}
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool ShouldLerp()
	{
		return false;
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00009FB3 File Offset: 0x000081B3
	public override void InitShared()
	{
		base.InitShared();
		if (!base.isClient)
		{
			return;
		}
		DeferredAction.Invoke(this, new Action(this.SpawnSkin), ActionPriority.Medium);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00009FD7 File Offset: 0x000081D7
	private void SpawnSkin()
	{
		this.instance = GameManager.client.CreatePrefab(this.prefab.resourcePath, base.transform, true);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00009FFB File Offset: 0x000081FB
	private void DestroySkin()
	{
		this.ResetTreeData();
		GameManager.client.Retire(this.instance);
		this.instance = null;
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00053AA8 File Offset: 0x00051CA8
	[BaseEntity.RPC_Client]
	public void CrackSound(BaseEntity.RPCMessage msg)
	{
		if (MainCamera.Distance(base.transform.position) > 30f)
		{
			return;
		}
		if (msg.read.Int32() == 1)
		{
			SoundManager.PlayOneshot(this.medCrackSoundDef, null, false, base.transform.position);
			return;
		}
		SoundManager.PlayOneshot(this.smallCrackSoundDef, null, false, base.transform.position);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00005B85 File Offset: 0x00003D85
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00053B10 File Offset: 0x00051D10
	[BaseEntity.RPC_Client]
	public void TreeFall(BaseEntity.RPCMessage msg)
	{
		if (this.instance == null)
		{
			return;
		}
		this.treeDistanceUponFalling = MainCamera.Distance(base.transform.position);
		if (this.treeDistanceUponFalling >= 100f)
		{
			return;
		}
		this.hitDirection = msg.read.Vector3();
		this.rotateDirection = Quaternion.LookRotation(this.hitDirection, Vector3.up) * Vector3.right;
		this.oldInstancePos = this.instance.transform.position;
		this.oldInstanceRot = this.instance.transform.rotation;
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
		this.treeBaseRef = this.instance.transform.Find("treebase");
		if (this.treeBaseRef)
		{
			this.treeBaseRef.gameObject.SetActive(false);
		}
		Effect.client.Run(this.fallStartSound.isValid ? this.fallStartSound.resourcePath : "assets/content/nature/treesprefabs/trees/effects/tree_fall.prefab", base.transform.position, default(Vector3), default(Vector3));
		this.impactSoundCheckHeight = this.bounds.size.y - 8f;
		this.lastTreeFallTickTime = UnityEngine.Time.time + 0f;
		base.InvokeRepeating(new Action(this.TreeFallTick), 0f, 0f);
		this.fallLeavesLoop = SoundManager.RequestSoundInstance(this.fallLeavesLoopDef, this.instance.gameObject, default(Vector3), false);
		if (this.fallLeavesLoop != null)
		{
			this.fallLeavesLoop.transform.localPosition += this.instance.transform.up * (this.bounds.size.y - 3f);
			this.fallLeavesGainMod = this.fallLeavesLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
			this.fallLeavesGainMod.value = 0f;
			this.fallLeavesLoop.FadeInAndPlay(0.1f);
		}
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x00053D38 File Offset: 0x00051F38
	public void TreeFallTick()
	{
		if (this.fallStartTime == -1f)
		{
			this.fallStartTime = UnityEngine.Time.time;
		}
		float num = UnityEngine.Time.time - this.lastTreeFallTickTime;
		this.instance.transform.position += Vector3.down * 0.15f * num;
		float num2 = UnityEngine.Time.time - this.fallStartTime;
		float num3 = Mathf.InverseLerp(0f, this.fallDuration, num2);
		float num4 = Mathf.Lerp(15f, 90f, num3);
		if (this.fallLeavesGainMod != null)
		{
			this.fallLeavesGainMod.value = Mathf.Clamp01(num3);
		}
		if (num2 < 2f)
		{
			this.instance.transform.Rotate(Vector3.up, num4 * 0.25f * num, Space.Self);
			this.instance.transform.Rotate(this.rotateDirection, num4 * num, Space.World);
		}
		if (this.treeDistanceUponFalling <= 200f)
		{
			for (int i = 0; i < 20; i++)
			{
				float num5 = (float)(3 + i * 3);
				if (!this.usedHeights[i])
				{
					if (num5 > this.bounds.size.y && i != 0)
					{
						break;
					}
					Vector3 down = Vector3.down;
					RaycastHit raycastHit;
					if (Physics.Raycast(this.instance.transform.position + this.instance.transform.up * num5, down, ref raycastHit, 2f, 8454144))
					{
						this.usedHeights[i] = true;
						if (num5 > this.impactSoundCheckHeight && !this.impactSoundPlayed)
						{
							Effect.client.Run(this.fallImpactSound.isValid ? this.fallImpactSound.resourcePath : "assets/content/nature/treesprefabs/trees/effects/tree_impact.prefab", raycastHit.point, default(Vector3), default(Vector3));
							if (this.fallLeavesLoop != null)
							{
								this.fallLeavesLoop.FadeOutAndRecycle(2f);
							}
							this.impactSoundPlayed = true;
						}
						Effect.client.Run(this.fallImpactParticles.isValid ? this.fallImpactParticles.resourcePath : "assets/content/nature/treesprefabs/trees/effects/tree_impact_mask.prefab", raycastHit.point, Vector3.up, default(Vector3));
					}
				}
			}
		}
		this.lastTreeFallTickTime = UnityEngine.Time.time;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00053F90 File Offset: 0x00052190
	public void ResetTreeData()
	{
		this.fallStartTime = -1f;
		if (this.instance)
		{
			this.instance.transform.position = this.oldInstancePos;
			this.instance.transform.rotation = this.oldInstanceRot;
		}
		if (this.treeBaseRef)
		{
			this.treeBaseRef.gameObject.SetActive(true);
		}
	}
}
