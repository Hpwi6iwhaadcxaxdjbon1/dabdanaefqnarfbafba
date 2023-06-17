using System;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class SkeletonScale : MonoBehaviour
{
	// Token: 0x04000DA6 RID: 3494
	protected BoneInfoComponent[] bones;

	// Token: 0x04000DA7 RID: 3495
	public int seed;

	// Token: 0x04000DA8 RID: 3496
	public GameObject leftShoulder;

	// Token: 0x04000DA9 RID: 3497
	public GameObject rightShoulder;

	// Token: 0x04000DAA RID: 3498
	public GameObject spine;

	// Token: 0x060010B9 RID: 4281 RVA: 0x0000EB28 File Offset: 0x0000CD28
	protected void Awake()
	{
		this.bones = base.GetComponentsInChildren<BoneInfoComponent>(true);
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x00070C30 File Offset: 0x0006EE30
	public void UpdateBones(int seedNumber)
	{
		this.seed = seedNumber;
		foreach (BoneInfoComponent boneInfoComponent in this.bones)
		{
			if (!(boneInfoComponent.sizeVariation == Vector3.zero))
			{
				Random.State state = Random.state;
				Random.InitState(boneInfoComponent.sizeVariationSeed + this.seed);
				boneInfoComponent.transform.localScale = Vector3.one + boneInfoComponent.sizeVariation * Random.Range(-1f, 1f);
				Random.state = state;
			}
		}
		if (this.spine != null)
		{
			Transform transform = this.rightShoulder.transform;
			Transform transform2 = this.leftShoulder.transform;
			Vector3 localScale = new Vector3(1f / this.spine.transform.localScale.x, 1f / this.spine.transform.localScale.y, 1f / this.spine.transform.localScale.z);
			transform2.localScale = localScale;
			transform.localScale = localScale;
		}
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x00070D44 File Offset: 0x0006EF44
	public void Reset()
	{
		foreach (BoneInfoComponent boneInfoComponent in this.bones)
		{
			if (!(boneInfoComponent.sizeVariation == Vector3.zero))
			{
				boneInfoComponent.transform.localScale = Vector3.one;
			}
		}
	}
}
