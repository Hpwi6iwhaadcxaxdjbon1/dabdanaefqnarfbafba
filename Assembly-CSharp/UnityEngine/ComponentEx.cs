using System;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x0200082F RID: 2095
	public static class ComponentEx
	{
		// Token: 0x06002D78 RID: 11640 RVA: 0x000235D9 File Offset: 0x000217D9
		public static T Instantiate<T>(this T component) where T : Component
		{
			return Facepunch.Instantiate.GameObject(component.gameObject, null).GetComponent<T>();
		}
	}
}
