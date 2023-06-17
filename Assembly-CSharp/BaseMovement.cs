using System;
using UnityEngine;

// Token: 0x0200031D RID: 797
public class BaseMovement : MonoBehaviour
{
	// Token: 0x04001213 RID: 4627
	[NonSerialized]
	public bool adminCheat;

	// Token: 0x04001214 RID: 4628
	[NonSerialized]
	public float adminSpeed = 1f;

	// Token: 0x04001215 RID: 4629
	protected BasePlayer Owner;

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x0600153E RID: 5438 RVA: 0x000120A4 File Offset: 0x000102A4
	// (set) Token: 0x0600153F RID: 5439 RVA: 0x000120AC File Offset: 0x000102AC
	public Vector3 TargetMovement { get; protected set; }

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06001540 RID: 5440 RVA: 0x000120B5 File Offset: 0x000102B5
	// (set) Token: 0x06001541 RID: 5441 RVA: 0x000120BD File Offset: 0x000102BD
	public float Running { get; protected set; }

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06001542 RID: 5442 RVA: 0x000120C6 File Offset: 0x000102C6
	// (set) Token: 0x06001543 RID: 5443 RVA: 0x000120CE File Offset: 0x000102CE
	public float Ducking { get; protected set; }

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06001544 RID: 5444 RVA: 0x000120D7 File Offset: 0x000102D7
	// (set) Token: 0x06001545 RID: 5445 RVA: 0x000120DF File Offset: 0x000102DF
	public float Grounded { get; protected set; }

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06001546 RID: 5446 RVA: 0x000120E8 File Offset: 0x000102E8
	public bool IsRunning
	{
		get
		{
			return this.Running > 0.5f;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06001547 RID: 5447 RVA: 0x000120F7 File Offset: 0x000102F7
	public bool IsDucked
	{
		get
		{
			return this.Ducking > 0.5f;
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06001548 RID: 5448 RVA: 0x00012106 File Offset: 0x00010306
	public bool IsGrounded
	{
		get
		{
			return this.Grounded > 0.5f;
		}
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x0000989B File Offset: 0x00007A9B
	public virtual Vector3 CurrentVelocity()
	{
		return Vector3.zero;
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x0000481F File Offset: 0x00002A1F
	public virtual float CurrentMoveSpeed()
	{
		return 0f;
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x0000792A File Offset: 0x00005B2A
	public virtual Collider GetCollider()
	{
		return null;
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x00012115 File Offset: 0x00010315
	public virtual void Init(BasePlayer player)
	{
		this.Owner = player;
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void BlockJump(float duration)
	{
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void BlockSprint(float duration)
	{
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void ClientInput(InputState state, ModelState modelState)
	{
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void DoFixedUpdate(ModelState modelState)
	{
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x0001211E File Offset: 0x0001031E
	public virtual void FrameUpdate(BasePlayer player, ModelState modelState)
	{
		if (player.HasLocalControls())
		{
			player.transform.position = base.transform.position;
			return;
		}
		base.transform.position = player.transform.position;
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x00012155 File Offset: 0x00010355
	public virtual void TeleportTo(Vector3 position, BasePlayer player)
	{
		base.transform.position = position;
		player.transform.position = position;
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void Push(Vector3 velocity)
	{
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void SetParent(Transform parent)
	{
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x0001216F File Offset: 0x0001036F
	public void FixedUpdate()
	{
		if (this.Owner == null)
		{
			return;
		}
		this.Owner.DoMovement();
	}
}
