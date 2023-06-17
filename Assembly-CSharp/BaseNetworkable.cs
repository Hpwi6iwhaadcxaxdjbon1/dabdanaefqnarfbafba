using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Facepunch;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Registry;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020002A5 RID: 677
public abstract class BaseNetworkable : BaseMonoBehaviour, IEntity, NetworkHandler
{
	// Token: 0x04000F8F RID: 3983
	private DeferredAction entityDestroy;

	// Token: 0x04000F90 RID: 3984
	[ReadOnly]
	[Header("BaseNetworkable")]
	public uint prefabID;

	// Token: 0x04000F91 RID: 3985
	[Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
	public bool globalBroadcast;

	// Token: 0x04000F92 RID: 3986
	[NonSerialized]
	public Networkable net;

	// Token: 0x04000F94 RID: 3988
	private string _prefabName;

	// Token: 0x04000F95 RID: 3989
	private string _prefabNameWithoutExtension;

	// Token: 0x04000F96 RID: 3990
	public static BaseNetworkable.EntityRealm clientEntities = new BaseNetworkable.EntityRealmClient();

	// Token: 0x04000F97 RID: 3991
	private const bool isServersideEntity = false;

	// Token: 0x04000F98 RID: 3992
	[NonSerialized]
	public EntityRef parentEntity;

	// Token: 0x04000F99 RID: 3993
	[NonSerialized]
	public List<BaseEntity> children = new List<BaseEntity>();

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x060012FD RID: 4861 RVA: 0x0001030E File Offset: 0x0000E50E
	// (set) Token: 0x060012FE RID: 4862 RVA: 0x00010316 File Offset: 0x0000E516
	protected bool JustCreated { get; set; }

	// Token: 0x060012FF RID: 4863 RVA: 0x0007AC3C File Offset: 0x00078E3C
	public void ClientSpawn(Entity info)
	{
		this.JustCreated = true;
		using (TimeWarning.New("SpawnShared", 0.1f))
		{
			this.SpawnShared();
		}
		using (TimeWarning.New("PreInitShared", 0.1f))
		{
			this.PreInitShared();
		}
		using (TimeWarning.New("InitShared", 0.1f))
		{
			this.InitShared();
		}
		using (TimeWarning.New("ClientInit", 0.1f))
		{
			this.ClientInit(info);
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "ClientSpawn");
		using (TimeWarning.New("PostInitShared", 0.1f))
		{
			this.PostInitShared();
		}
		this.JustCreated = false;
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x0001031F File Offset: 0x0000E51F
	public virtual void ClientOnEnable()
	{
		base.gameObject.BroadcastOnPostNetworkUpdate(this as BaseEntity);
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x0007AD50 File Offset: 0x00078F50
	protected virtual void ClientInit(Entity info)
	{
		this.PreNetworkUpdate();
		BaseNetworkable.LoadInfo info2 = new BaseNetworkable.LoadInfo
		{
			msg = info
		};
		this.Load(info2);
		Assert.IsTrue(this.net != null, ".net is null!");
		BaseNetworkable.clientEntities.RegisterID(this);
		Net.cl.SetupNetworkable(this.net);
		this.PostNetworkUpdate();
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x0007ADB0 File Offset: 0x00078FB0
	public void OnNetworkUpdate(Entity entity)
	{
		if (base.gameObject == null)
		{
			return;
		}
		if (base.transform == null)
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkUpdate");
		this.PreNetworkUpdate();
		BaseNetworkable.LoadInfo info = new BaseNetworkable.LoadInfo
		{
			msg = entity
		};
		this.Load(info);
		this.PostNetworkUpdate();
		base.gameObject.BroadcastOnPostNetworkUpdate(this as BaseEntity);
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void PreNetworkUpdate()
	{
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void PostNetworkUpdate()
	{
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool ShouldDestroyWithGroup()
	{
		return true;
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool ShouldDestroyImmediately()
	{
		return false;
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x0007AE20 File Offset: 0x00079020
	public void NetworkDestroy(bool immediately)
	{
		if (this.IsDestroyed)
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "Client NetworkDestroy");
		base.gameObject.BroadcastOnParentDestroying();
		this.DoNetworkDestroy();
		BaseNetworkable.clientEntities.UnregisterID(this);
		if (this.net != null)
		{
			Net.cl.DestroyNetworkable(ref this.net);
		}
		base.StopAllCoroutines();
		this.DoEntityDestroy();
		if (immediately)
		{
			this.EntityDestroy();
			return;
		}
		if (this.entityDestroy == null)
		{
			this.entityDestroy = new DeferredAction(this, new Action(this.EntityDestroy), (this is StabilityEntity) ? ActionPriority.Lowest : ActionPriority.Low);
		}
		this.entityDestroy.Invoke();
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00010332 File Offset: 0x0000E532
	protected virtual void DoClientDestroy()
	{
		BaseNetworkable.clientEntities.UnregisterID(this);
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00002ECE File Offset: 0x000010CE
	internal virtual void DoNetworkDestroy()
	{
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x0600130A RID: 4874 RVA: 0x0001033F File Offset: 0x0000E53F
	// (set) Token: 0x0600130B RID: 4875 RVA: 0x00010347 File Offset: 0x0000E547
	public bool IsDestroyed { get; private set; }

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x0600130C RID: 4876 RVA: 0x00010350 File Offset: 0x0000E550
	public string PrefabName
	{
		get
		{
			if (this._prefabName == null)
			{
				this._prefabName = StringPool.Get(this.prefabID);
			}
			return this._prefabName;
		}
	}

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x0600130D RID: 4877 RVA: 0x00010371 File Offset: 0x0000E571
	public string ShortPrefabName
	{
		get
		{
			if (this._prefabNameWithoutExtension == null)
			{
				this._prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(this.PrefabName);
			}
			return this._prefabNameWithoutExtension;
		}
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00010392 File Offset: 0x0000E592
	public virtual Vector3 GetNetworkPosition()
	{
		return base.transform.localPosition;
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x0001039F File Offset: 0x0000E59F
	public virtual Quaternion GetNetworkRotation()
	{
		return base.transform.localRotation;
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool PhysicsDriven()
	{
		return false;
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x000103AC File Offset: 0x0000E5AC
	public float GetNetworkTime()
	{
		if (this.PhysicsDriven())
		{
			return Time.fixedTime;
		}
		return Time.time;
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x0007AEC4 File Offset: 0x000790C4
	public string InvokeString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<InvokeAction> list = Pool.GetList<InvokeAction>();
		InvokeHandler.FindInvokes(this, list);
		foreach (InvokeAction invokeAction in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(invokeAction.action.Method.Name);
		}
		Pool.FreeList<InvokeAction>(ref list);
		return stringBuilder.ToString();
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x000103C1 File Offset: 0x0000E5C1
	public BaseEntity LookupPrefab()
	{
		return this.gameManager.FindPrefab(this.PrefabName).ToBaseEntity();
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x000103D9 File Offset: 0x0000E5D9
	public bool EqualNetID(BaseNetworkable other)
	{
		return other != null && other.net != null && this.net != null && other.net.ID == this.net.ID;
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x0001040E File Offset: 0x0000E60E
	public virtual void ResetState()
	{
		if (this.children.Count > 0)
		{
			this.children.Clear();
		}
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void InitShared()
	{
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void PreInitShared()
	{
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void PostInitShared()
	{
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x0007AF58 File Offset: 0x00079158
	public virtual void DestroyShared()
	{
		using (TimeWarning.New("Registry.Entity.Unregister", 0.1f))
		{
			Entity.Unregister(base.gameObject);
		}
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnNetworkGroupEnter(Group group)
	{
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnNetworkGroupLeave(Group group)
	{
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x0007AF9C File Offset: 0x0007919C
	public void OnNetworkGroupChange()
	{
		if (this.children != null)
		{
			foreach (BaseEntity baseEntity in this.children)
			{
				if (baseEntity.ShouldInheritNetworkGroup())
				{
					baseEntity.net.SwitchGroup(this.net.group);
				}
			}
		}
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnNetworkSubscribersEnter(List<Connection> connections)
	{
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnNetworkSubscribersLeave(List<Connection> connections)
	{
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x00010429 File Offset: 0x0000E629
	private void EntityDestroy()
	{
		if (base.gameObject)
		{
			this.ResetState();
			this.gameManager.Retire(base.gameObject);
		}
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x0001044F File Offset: 0x0000E64F
	private void DoEntityDestroy()
	{
		if (this.IsDestroyed)
		{
			return;
		}
		this.IsDestroyed = true;
		if (Application.isQuitting)
		{
			return;
		}
		this.DestroyShared();
		if (this.isClient)
		{
			this.DoClientDestroy();
		}
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x0007B010 File Offset: 0x00079210
	private void SpawnShared()
	{
		this.IsDestroyed = false;
		using (TimeWarning.New("Registry.Entity.Register", 0.1f))
		{
			Entity.Register(base.gameObject, this);
		}
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x0007B05C File Offset: 0x0007925C
	public virtual void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.baseNetworkable == null)
		{
			return;
		}
		BaseNetworkable baseNetworkable = info.msg.baseNetworkable;
		if (this.prefabID != baseNetworkable.prefabID)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Prefab IDs don't match! ",
				this.prefabID,
				"/",
				baseNetworkable.prefabID,
				" -> ",
				base.gameObject
			}), base.gameObject);
		}
		if (this.isClient && this.net == null)
		{
			this.net = Net.cl.CreateNetworkable(baseNetworkable.uid, baseNetworkable.group);
			this.net.handler = this;
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06001323 RID: 4899 RVA: 0x0000508F File Offset: 0x0000328F
	public bool isServer
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06001324 RID: 4900 RVA: 0x00002D44 File Offset: 0x00000F44
	public bool isClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x0007B11C File Offset: 0x0007931C
	public T ToClient<T>() where T : BaseNetworkable
	{
		if (this.isClient)
		{
			return this as T;
		}
		return default(T);
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}

	// Token: 0x06001327 RID: 4903 RVA: 0x0001047D File Offset: 0x0000E67D
	public BaseEntity GetParentEntity()
	{
		return this.parentEntity.Get(this.isServer);
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x00010490 File Offset: 0x0000E690
	public bool HasParent()
	{
		return this.parentEntity.IsValid(this.isServer);
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x000104A3 File Offset: 0x0000E6A3
	public void AddChild(BaseEntity child)
	{
		if (this.children.Contains(child))
		{
			return;
		}
		this.children.Add(child);
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x000104C0 File Offset: 0x0000E6C0
	public void RemoveChild(BaseEntity child)
	{
		this.children.Remove(child);
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x0600132B RID: 4907 RVA: 0x000104CF File Offset: 0x0000E6CF
	public GameManager gameManager
	{
		get
		{
			if (this.isClient)
			{
				return GameManager.client;
			}
			throw new NotImplementedException("Missing gameManager path");
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x0600132C RID: 4908 RVA: 0x000104E9 File Offset: 0x0000E6E9
	public PrefabAttribute.Library prefabAttribute
	{
		get
		{
			if (this.isClient)
			{
				return PrefabAttribute.client;
			}
			throw new NotImplementedException("Missing prefabAttribute path");
		}
	}

	// Token: 0x020002A6 RID: 678
	public struct SaveInfo
	{
		// Token: 0x04000F9A RID: 3994
		public Entity msg;

		// Token: 0x04000F9B RID: 3995
		public bool forDisk;

		// Token: 0x04000F9C RID: 3996
		public Connection forConnection;

		// Token: 0x0600132F RID: 4911 RVA: 0x00010522 File Offset: 0x0000E722
		internal bool SendingTo(Connection ownerConnection)
		{
			return ownerConnection != null && this.forConnection != null && this.forConnection == ownerConnection;
		}
	}

	// Token: 0x020002A7 RID: 679
	public struct LoadInfo
	{
		// Token: 0x04000F9D RID: 3997
		public Entity msg;

		// Token: 0x04000F9E RID: 3998
		public bool fromDisk;
	}

	// Token: 0x020002A8 RID: 680
	public class EntityRealmClient : BaseNetworkable.EntityRealm
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06001330 RID: 4912 RVA: 0x0001053C File Offset: 0x0000E73C
		protected override Manager visibilityManager
		{
			get
			{
				if (Net.cl == null)
				{
					return null;
				}
				return Net.cl.visibility;
			}
		}
	}

	// Token: 0x020002A9 RID: 681
	public abstract class EntityRealm : IEnumerable<BaseNetworkable>, IEnumerable
	{
		// Token: 0x04000F9F RID: 3999
		private ListDictionary<uint, BaseNetworkable> entityList = new ListDictionary<uint, BaseNetworkable>(8);

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06001332 RID: 4914 RVA: 0x00010559 File Offset: 0x0000E759
		public int Count
		{
			get
			{
				return this.entityList.Count;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06001333 RID: 4915
		protected abstract Manager visibilityManager { get; }

		// Token: 0x06001334 RID: 4916 RVA: 0x0007B148 File Offset: 0x00079348
		public BaseNetworkable Find(uint uid)
		{
			BaseNetworkable result = null;
			if (!this.entityList.TryGetValue(uid, ref result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x0007B16C File Offset: 0x0007936C
		public void RegisterID(BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				if (this.entityList.Contains(ent.net.ID))
				{
					this.entityList[ent.net.ID] = ent;
					return;
				}
				this.entityList.Add(ent.net.ID, ent);
			}
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x00010566 File Offset: 0x0000E766
		public void UnregisterID(BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				this.entityList.Remove(ent.net.ID);
			}
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x0007B1C8 File Offset: 0x000793C8
		public Group FindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.Get(uid);
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x0007B1E8 File Offset: 0x000793E8
		public Group TryFindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.TryGet(uid);
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x0007B208 File Offset: 0x00079408
		public void FindInGroup(uint uid, List<BaseNetworkable> list)
		{
			Group group = this.TryFindGroup(uid);
			if (group == null)
			{
				return;
			}
			int count = group.networkables.Values.Count;
			Networkable[] buffer = group.networkables.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				Networkable networkable = buffer[i];
				BaseNetworkable baseNetworkable = this.Find(networkable.ID);
				if (!(baseNetworkable == null) && baseNetworkable.net != null && baseNetworkable.net.group != null)
				{
					if (baseNetworkable.net.group.ID != uid)
					{
						Debug.LogWarning("Group ID mismatch: " + baseNetworkable.ToString());
					}
					else
					{
						list.Add(baseNetworkable);
					}
				}
			}
		}

		// Token: 0x0600133A RID: 4922 RVA: 0x00010587 File Offset: 0x0000E787
		public IEnumerator<BaseNetworkable> GetEnumerator()
		{
			return this.entityList.Values.GetEnumerator();
		}

		// Token: 0x0600133B RID: 4923 RVA: 0x00010599 File Offset: 0x0000E799
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}

	// Token: 0x020002AA RID: 682
	public enum DestroyMode : byte
	{
		// Token: 0x04000FA1 RID: 4001
		None,
		// Token: 0x04000FA2 RID: 4002
		Gib
	}
}
