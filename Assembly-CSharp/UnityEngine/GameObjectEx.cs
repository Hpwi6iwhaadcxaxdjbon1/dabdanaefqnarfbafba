using System;
using Rust;
using Rust.Registry;

namespace UnityEngine
{
	// Token: 0x02000831 RID: 2097
	public static class GameObjectEx
	{
		// Token: 0x06002D7C RID: 11644 RVA: 0x000E49E4 File Offset: 0x000E2BE4
		public static BaseEntity ToBaseEntity(this GameObject go)
		{
			IEntity entity = GameObjectEx.GetEntityFromRegistry(go);
			if (entity == null && !go.transform.gameObject.activeSelf)
			{
				entity = GameObjectEx.GetEntityFromComponent(go);
			}
			return entity as BaseEntity;
		}

		// Token: 0x06002D7D RID: 11645 RVA: 0x000E4A1C File Offset: 0x000E2C1C
		private static IEntity GetEntityFromRegistry(GameObject go)
		{
			Transform transform = go.transform;
			IEntity entity = Entity.Get(transform.gameObject);
			while (entity == null && transform.parent != null)
			{
				transform = transform.parent;
				entity = Entity.Get(transform.gameObject);
			}
			return entity;
		}

		// Token: 0x06002D7E RID: 11646 RVA: 0x000E4A64 File Offset: 0x000E2C64
		private static IEntity GetEntityFromComponent(GameObject go)
		{
			Transform transform = go.transform;
			IEntity component = transform.GetComponent<IEntity>();
			while (component == null && transform.parent != null)
			{
				transform = transform.parent;
				component = transform.GetComponent<IEntity>();
			}
			return component;
		}

		// Token: 0x06002D7F RID: 11647 RVA: 0x00023619 File Offset: 0x00021819
		public static void SetHierarchyGroup(this GameObject obj, string strRoot, bool groupActive = true, bool persistant = false)
		{
			obj.transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}
	}
}
