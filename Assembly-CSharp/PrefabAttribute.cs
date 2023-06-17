using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200071A RID: 1818
public abstract class PrefabAttribute : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x040023A3 RID: 9123
	[NonSerialized]
	public Vector3 worldPosition;

	// Token: 0x040023A4 RID: 9124
	[NonSerialized]
	public Quaternion worldRotation;

	// Token: 0x040023A5 RID: 9125
	[NonSerialized]
	public Vector3 worldForward;

	// Token: 0x040023A6 RID: 9126
	[NonSerialized]
	public Vector3 localPosition;

	// Token: 0x040023A7 RID: 9127
	[NonSerialized]
	public Vector3 localScale;

	// Token: 0x040023A8 RID: 9128
	[NonSerialized]
	public Quaternion localRotation;

	// Token: 0x040023A9 RID: 9129
	[NonSerialized]
	public string fullName;

	// Token: 0x040023AA RID: 9130
	[NonSerialized]
	public string hierachyName;

	// Token: 0x040023AB RID: 9131
	[NonSerialized]
	public uint prefabID;

	// Token: 0x040023AC RID: 9132
	[NonSerialized]
	public int instanceID;

	// Token: 0x040023AD RID: 9133
	[NonSerialized]
	public PrefabAttribute.Library prefabAttribute;

	// Token: 0x040023AE RID: 9134
	[NonSerialized]
	public GameManager gameManager;

	// Token: 0x040023AF RID: 9135
	[NonSerialized]
	public bool isServer;

	// Token: 0x040023B0 RID: 9136
	public static PrefabAttribute.Library client = new PrefabAttribute.Library(true, false);

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x060027CF RID: 10191 RVA: 0x0001F0DF File Offset: 0x0001D2DF
	public bool isClient
	{
		get
		{
			return !this.isServer;
		}
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x000CDC38 File Offset: 0x000CBE38
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.fullName = name;
		this.hierachyName = base.transform.GetRecursiveName("");
		this.prefabID = StringPool.Get(name);
		this.instanceID = base.GetInstanceID();
		this.worldPosition = base.transform.position;
		this.worldRotation = base.transform.rotation;
		this.worldForward = base.transform.forward;
		this.localPosition = base.transform.localPosition;
		this.localScale = base.transform.localScale;
		this.localRotation = base.transform.localRotation;
		if (clientside)
		{
			this.prefabAttribute = PrefabAttribute.client;
			this.gameManager = GameManager.client;
			this.isServer = false;
		}
		this.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (clientside)
		{
			PrefabAttribute.client.Add(this.prefabID, this);
		}
		preProcess.RemoveComponent(this);
		preProcess.NominateForDeletion(base.gameObject);
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x00002ECE File Offset: 0x000010CE
	protected virtual void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x060027D2 RID: 10194
	protected abstract Type GetIndexedType();

	// Token: 0x060027D3 RID: 10195 RVA: 0x0001F0EA File Offset: 0x0001D2EA
	public static bool operator ==(PrefabAttribute x, PrefabAttribute y)
	{
		return PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x0001F0F3 File Offset: 0x0001D2F3
	public static bool operator !=(PrefabAttribute x, PrefabAttribute y)
	{
		return !PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x0001F0FF File Offset: 0x0001D2FF
	public override bool Equals(object o)
	{
		return PrefabAttribute.ComparePrefabAttribute(this, (PrefabAttribute)o);
	}

	// Token: 0x060027D6 RID: 10198 RVA: 0x0001F10D File Offset: 0x0001D30D
	public override int GetHashCode()
	{
		if (this.hierachyName == null)
		{
			return base.GetHashCode();
		}
		return this.hierachyName.GetHashCode();
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x0001415A File Offset: 0x0001235A
	public static implicit operator bool(PrefabAttribute exists)
	{
		return exists != null;
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000CDD3C File Offset: 0x000CBF3C
	internal static bool ComparePrefabAttribute(PrefabAttribute x, PrefabAttribute y)
	{
		bool flag = x == null;
		bool flag2 = y == null;
		return (flag && flag2) || (!flag && !flag2 && x.instanceID == y.instanceID);
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x0001F129 File Offset: 0x0001D329
	public override string ToString()
	{
		if (this == null)
		{
			return "null";
		}
		return this.hierachyName;
	}

	// Token: 0x0200071B RID: 1819
	public class AttributeCollection
	{
		// Token: 0x040023B1 RID: 9137
		private Dictionary<Type, List<PrefabAttribute>> attributes = new Dictionary<Type, List<PrefabAttribute>>();

		// Token: 0x040023B2 RID: 9138
		private Dictionary<Type, object> cache = new Dictionary<Type, object>();

		// Token: 0x060027DC RID: 10204 RVA: 0x000CDD70 File Offset: 0x000CBF70
		internal List<PrefabAttribute> Find(Type t)
		{
			List<PrefabAttribute> list;
			if (this.attributes.TryGetValue(t, ref list))
			{
				return list;
			}
			list = new List<PrefabAttribute>();
			this.attributes.Add(t, list);
			return list;
		}

		// Token: 0x060027DD RID: 10205 RVA: 0x000CDDA4 File Offset: 0x000CBFA4
		public T[] Find<T>()
		{
			if (this.cache == null)
			{
				this.cache = new Dictionary<Type, object>();
			}
			object obj;
			if (this.cache.TryGetValue(typeof(T), ref obj))
			{
				return (T[])obj;
			}
			obj = Enumerable.ToArray<T>(Enumerable.Cast<T>(this.Find(typeof(T))));
			this.cache.Add(typeof(T), obj);
			return (T[])obj;
		}

		// Token: 0x060027DE RID: 10206 RVA: 0x0001F148 File Offset: 0x0001D348
		public void Add(PrefabAttribute attribute)
		{
			List<PrefabAttribute> list = this.Find(attribute.GetIndexedType());
			Assert.IsTrue(!list.Contains(attribute), "AttributeCollection.Add: Adding twice to list");
			list.Add(attribute);
			this.cache = null;
		}
	}

	// Token: 0x0200071C RID: 1820
	public class Library
	{
		// Token: 0x040023B3 RID: 9139
		public bool clientside;

		// Token: 0x040023B4 RID: 9140
		public bool serverside;

		// Token: 0x040023B5 RID: 9141
		private Dictionary<uint, PrefabAttribute.AttributeCollection> prefabs = new Dictionary<uint, PrefabAttribute.AttributeCollection>();

		// Token: 0x060027E0 RID: 10208 RVA: 0x0001F195 File Offset: 0x0001D395
		public Library(bool clientside, bool serverside)
		{
			this.clientside = clientside;
			this.serverside = serverside;
		}

		// Token: 0x060027E1 RID: 10209 RVA: 0x000CDE1C File Offset: 0x000CC01C
		public PrefabAttribute.AttributeCollection Find(uint prefabID, bool warmup = true)
		{
			PrefabAttribute.AttributeCollection attributeCollection;
			if (this.prefabs.TryGetValue(prefabID, ref attributeCollection))
			{
				return attributeCollection;
			}
			attributeCollection = new PrefabAttribute.AttributeCollection();
			this.prefabs.Add(prefabID, attributeCollection);
			if (warmup)
			{
				if (this.clientside && !this.serverside)
				{
					GameManager.client.FindPrefab(prefabID);
				}
				else if ((this.clientside || !this.serverside) && this.clientside)
				{
					bool flag = this.serverside;
				}
			}
			return attributeCollection;
		}

		// Token: 0x060027E2 RID: 10210 RVA: 0x000CDE90 File Offset: 0x000CC090
		public T Find<T>(uint prefabID) where T : PrefabAttribute
		{
			T[] array = this.Find(prefabID, true).Find<T>();
			if (array.Length == 0)
			{
				return default(T);
			}
			return array[0];
		}

		// Token: 0x060027E3 RID: 10211 RVA: 0x0001F1B6 File Offset: 0x0001D3B6
		public T[] FindAll<T>(uint prefabID) where T : PrefabAttribute
		{
			return this.Find(prefabID, true).Find<T>();
		}

		// Token: 0x060027E4 RID: 10212 RVA: 0x0001F1C5 File Offset: 0x0001D3C5
		public void Add(uint prefabID, PrefabAttribute attribute)
		{
			this.Find(prefabID, false).Add(attribute);
		}
	}
}
