using System;
using UnityEngine;

// Token: 0x020003DC RID: 988
public class Prefab<T> : Prefab, IComparable<Prefab<T>> where T : Component
{
	// Token: 0x0400153A RID: 5434
	public T Component;

	// Token: 0x060018BE RID: 6334 RVA: 0x00014B1B File Offset: 0x00012D1B
	public Prefab(string name, GameObject prefab, T component, GameManager manager, PrefabAttribute.Library attribute) : base(name, prefab, manager, attribute)
	{
		this.Component = component;
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x00014B30 File Offset: 0x00012D30
	public int CompareTo(Prefab<T> that)
	{
		return base.CompareTo(that);
	}
}
