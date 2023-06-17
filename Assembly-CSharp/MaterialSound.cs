using System;
using UnityEngine;

// Token: 0x02000475 RID: 1141
[CreateAssetMenu(menuName = "Rust/MaterialSound")]
public class MaterialSound : ScriptableObject
{
	// Token: 0x04001796 RID: 6038
	public SoundDefinition DefaultSound;

	// Token: 0x04001797 RID: 6039
	public MaterialSound.Entry[] Entries;

	// Token: 0x02000476 RID: 1142
	[Serializable]
	public class Entry
	{
		// Token: 0x04001798 RID: 6040
		public PhysicMaterial Material;

		// Token: 0x04001799 RID: 6041
		public SoundDefinition Sound;
	}
}
