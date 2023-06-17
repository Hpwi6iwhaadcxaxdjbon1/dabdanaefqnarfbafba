using System;
using System.Linq;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class RealmedRemove : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x0400156A RID: 5482
	public GameObject[] removedFromClient;

	// Token: 0x0400156B RID: 5483
	public Component[] removedComponentFromClient;

	// Token: 0x0400156C RID: 5484
	public GameObject[] removedFromServer;

	// Token: 0x0400156D RID: 5485
	public Component[] removedComponentFromServer;

	// Token: 0x0400156E RID: 5486
	public Component[] doNotRemoveFromServer;

	// Token: 0x0400156F RID: 5487
	public Component[] doNotRemoveFromClient;

	// Token: 0x06001908 RID: 6408 RVA: 0x0008EF58 File Offset: 0x0008D158
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside)
		{
			GameObject[] array = this.removedFromClient;
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromClient;
			for (int i = 0; i < array2.Length; i++)
			{
				Object.DestroyImmediate(array2[i], true);
			}
		}
		if (serverside)
		{
			GameObject[] array = this.removedFromServer;
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromServer;
			for (int i = 0; i < array2.Length; i++)
			{
				Object.DestroyImmediate(array2[i], true);
			}
		}
		if (!bundling)
		{
			process.RemoveComponent(this);
		}
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x00014D5D File Offset: 0x00012F5D
	public bool ShouldDelete(Component comp, bool client, bool server)
	{
		return (!client || this.doNotRemoveFromClient == null || !Enumerable.Contains<Component>(this.doNotRemoveFromClient, comp)) && (!server || this.doNotRemoveFromServer == null || !Enumerable.Contains<Component>(this.doNotRemoveFromServer, comp));
	}
}
