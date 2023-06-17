using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000834 RID: 2100
	public static class ObjectEffectsEx
	{
		// Token: 0x06002D81 RID: 11649 RVA: 0x000E4AA4 File Offset: 0x000E2CA4
		public static void SingleGib(this GameObject obj, GameObject model, GameObject physics)
		{
			if (Gib.gibCount > Effects.maxgibs)
			{
				return;
			}
			GameObject gameObject = Instantiate.GameObject(model, null);
			GameObject gameObject2 = Instantiate.GameObject(physics, null);
			gameObject2.transform.position = obj.transform.position;
			gameObject2.transform.rotation = obj.transform.rotation;
			gameObject2.layer = 26;
			gameObject.transform.SetParent(gameObject2.transform, false);
			MeshCollider component = gameObject2.GetComponent<MeshCollider>();
			if (component != null)
			{
				component.convex = true;
			}
			Rigidbody rigidbody = gameObject2.GetComponent<Rigidbody>();
			if (rigidbody == null)
			{
				rigidbody = gameObject2.AddComponent<Rigidbody>();
			}
			rigidbody.useGravity = true;
			rigidbody.mass = 50f;
			rigidbody.interpolation = 1;
			rigidbody.WakeUp();
			rigidbody.angularVelocity = Vector3Ex.Range(-10f, 10f).normalized * 1f;
			gameObject2.AddComponent<Gib>();
		}

		// Token: 0x06002D82 RID: 11650 RVA: 0x000E4B90 File Offset: 0x000E2D90
		public static void CustomGib(this GameObject obj)
		{
			List<Gibbable> list = Pool.GetList<Gibbable>();
			obj.GetComponentsInChildren<Gibbable>(true, list);
			if (list.Count > 0)
			{
				using (List<Gibbable>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Gibbable gibbable = enumerator.Current;
						gibbable.CreateGibs(obj.transform.parent);
					}
					goto IL_6D;
				}
			}
			if (Global.developer > 0)
			{
				Debug.LogWarning("No Gibs for: " + obj.name);
			}
			IL_6D:
			Pool.FreeList<Gibbable>(ref list);
		}
	}
}
