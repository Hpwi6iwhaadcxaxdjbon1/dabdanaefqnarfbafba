using System;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public class MeshReplacement : MonoBehaviour
{
	// Token: 0x04001526 RID: 5414
	public SkinnedMeshRenderer Female;

	// Token: 0x060018B6 RID: 6326 RVA: 0x0008DA8C File Offset: 0x0008BC8C
	internal static void Process(GameObject go, bool IsFemale)
	{
		if (!IsFemale)
		{
			return;
		}
		foreach (MeshReplacement meshReplacement in go.GetComponentsInChildren<MeshReplacement>(true))
		{
			SkinnedMeshRenderer component = meshReplacement.GetComponent<SkinnedMeshRenderer>();
			component.sharedMesh = meshReplacement.Female.sharedMesh;
			component.rootBone = meshReplacement.Female.rootBone;
			component.bones = meshReplacement.Female.bones;
		}
	}
}
