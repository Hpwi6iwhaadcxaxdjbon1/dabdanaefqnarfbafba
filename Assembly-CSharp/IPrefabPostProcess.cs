using System;
using UnityEngine;

// Token: 0x020003AA RID: 938
public interface IPrefabPostProcess
{
	// Token: 0x06001796 RID: 6038
	void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling);
}
