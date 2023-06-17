using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020002CD RID: 717
public class EntityLink : Pool.IPooled
{
	// Token: 0x04000FF6 RID: 4086
	public BaseEntity owner;

	// Token: 0x04000FF7 RID: 4087
	public Socket_Base socket;

	// Token: 0x04000FF8 RID: 4088
	public List<EntityLink> connections = new List<EntityLink>(8);

	// Token: 0x04000FF9 RID: 4089
	public int capacity = int.MaxValue;

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06001398 RID: 5016 RVA: 0x000109D9 File Offset: 0x0000EBD9
	public string name
	{
		get
		{
			return this.socket.socketName;
		}
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x000109E6 File Offset: 0x0000EBE6
	public void Setup(BaseEntity owner, Socket_Base socket)
	{
		this.owner = owner;
		this.socket = socket;
		if (socket.monogamous)
		{
			this.capacity = 1;
		}
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x00010A05 File Offset: 0x0000EC05
	public void EnterPool()
	{
		this.owner = null;
		this.socket = null;
		this.capacity = int.MaxValue;
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x00002ECE File Offset: 0x000010CE
	public void LeavePool()
	{
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x00010A20 File Offset: 0x0000EC20
	public bool Contains(EntityLink entity)
	{
		return this.connections.Contains(entity);
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x00010A2E File Offset: 0x0000EC2E
	public void Add(EntityLink entity)
	{
		this.connections.Add(entity);
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x00010A3C File Offset: 0x0000EC3C
	public void Remove(EntityLink entity)
	{
		this.connections.Remove(entity);
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x0007C008 File Offset: 0x0007A208
	public void Clear()
	{
		for (int i = 0; i < this.connections.Count; i++)
		{
			this.connections[i].Remove(this);
		}
		this.connections.Clear();
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x00010A4B File Offset: 0x0000EC4B
	public bool IsEmpty()
	{
		return this.connections.Count == 0;
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x00010A5B File Offset: 0x0000EC5B
	public bool IsOccupied()
	{
		return this.connections.Count >= this.capacity;
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x00010A73 File Offset: 0x0000EC73
	public bool IsMale()
	{
		return this.socket.male;
	}

	// Token: 0x060013A3 RID: 5027 RVA: 0x00010A80 File Offset: 0x0000EC80
	public bool IsFemale()
	{
		return this.socket.female;
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x0007C048 File Offset: 0x0007A248
	public bool CanConnect(EntityLink link)
	{
		return !this.IsOccupied() && link != null && !link.IsOccupied() && this.socket.CanConnect(this.owner.transform.position, this.owner.transform.rotation, link.socket, link.owner.transform.position, link.owner.transform.rotation);
	}
}
