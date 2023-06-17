using System;
using UnityEngine;

// Token: 0x020003AB RID: 939
public interface IPrefabProcessor
{
	// Token: 0x06001797 RID: 6039
	void RemoveComponent(Component component);

	// Token: 0x06001798 RID: 6040
	void NominateForDeletion(GameObject obj);
}
