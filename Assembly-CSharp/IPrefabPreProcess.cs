using System;
using UnityEngine;

// Token: 0x020003A9 RID: 937
public interface IPrefabPreProcess
{
	// Token: 0x06001795 RID: 6037
	void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling);
}
