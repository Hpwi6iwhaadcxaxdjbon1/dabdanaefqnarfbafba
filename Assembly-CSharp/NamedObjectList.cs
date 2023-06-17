using System;
using UnityEngine;

// Token: 0x020005E1 RID: 1505
[CreateAssetMenu(menuName = "Rust/Named Object List")]
public class NamedObjectList : ScriptableObject
{
	// Token: 0x04001E26 RID: 7718
	public NamedObjectList.NamedObject[] objects;

	// Token: 0x020005E2 RID: 1506
	[Serializable]
	public struct NamedObject
	{
		// Token: 0x04001E27 RID: 7719
		public string name;

		// Token: 0x04001E28 RID: 7720
		public Object obj;
	}
}
