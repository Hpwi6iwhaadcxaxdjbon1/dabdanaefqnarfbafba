using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008C9 RID: 2249
	public class NavmeshPrefabInstantiator : MonoBehaviour
	{
		// Token: 0x04002B31 RID: 11057
		public GameObjectRef NavmeshPrefab;

		// Token: 0x0600305B RID: 12379 RVA: 0x00025354 File Offset: 0x00023554
		private void Start()
		{
			if (this.NavmeshPrefab != null)
			{
				this.NavmeshPrefab.Instantiate(base.transform).SetActive(true);
				Object.Destroy(this);
			}
		}
	}
}
