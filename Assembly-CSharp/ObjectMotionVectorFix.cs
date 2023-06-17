using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005A9 RID: 1449
public class ObjectMotionVectorFix : MonoBehaviour
{
	// Token: 0x04001D07 RID: 7431
	private Renderer renderer;

	// Token: 0x04001D08 RID: 7432
	private static Queue<Renderer> restoreQueue = new Queue<Renderer>(1024);

	// Token: 0x06002140 RID: 8512 RVA: 0x000B347C File Offset: 0x000B167C
	private void OnBecameVisible()
	{
		this.renderer = ((this.renderer == null) ? base.GetComponent<Renderer>() : this.renderer);
		if (this.renderer != null)
		{
			this.renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			ObjectMotionVectorFix.restoreQueue.Enqueue(this.renderer);
		}
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x000B34D8 File Offset: 0x000B16D8
	public static void DisableObjectMotionVectors(LODGroup lodGroup)
	{
		LOD[] lods = lodGroup.GetLODs();
		for (int i = 0; i < lods.Length; i++)
		{
			foreach (Renderer renderer in lods[i].renderers)
			{
				if (renderer != null && renderer.motionVectorGenerationMode == MotionVectorGenerationMode.Object)
				{
					renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
					if (renderer.gameObject.GetComponent<ObjectMotionVectorFix>() == null)
					{
						renderer.gameObject.AddComponent<ObjectMotionVectorFix>();
					}
				}
			}
		}
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x000B3558 File Offset: 0x000B1758
	public static void RestoreObjectMotionVectors()
	{
		while (ObjectMotionVectorFix.restoreQueue.Count > 0)
		{
			Renderer renderer = ObjectMotionVectorFix.restoreQueue.Dequeue();
			if (renderer != null)
			{
				renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
			}
		}
	}
}
