using System;
using UnityEngine;

// Token: 0x020005E7 RID: 1511
[CreateAssetMenu(menuName = "Rust/Recoil Properties")]
public class RecoilProperties : ScriptableObject
{
	// Token: 0x04001E34 RID: 7732
	public float recoilYawMin;

	// Token: 0x04001E35 RID: 7733
	public float recoilYawMax;

	// Token: 0x04001E36 RID: 7734
	public float recoilPitchMin;

	// Token: 0x04001E37 RID: 7735
	public float recoilPitchMax;

	// Token: 0x04001E38 RID: 7736
	public float timeToTakeMin;

	// Token: 0x04001E39 RID: 7737
	public float timeToTakeMax = 0.1f;

	// Token: 0x04001E3A RID: 7738
	public float ADSScale = 0.5f;

	// Token: 0x04001E3B RID: 7739
	public float movementPenalty;

	// Token: 0x04001E3C RID: 7740
	public float clampPitch = float.NegativeInfinity;

	// Token: 0x04001E3D RID: 7741
	public AnimationCurve pitchCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04001E3E RID: 7742
	public AnimationCurve yawCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04001E3F RID: 7743
	public bool useCurves;

	// Token: 0x04001E40 RID: 7744
	public int shotsUntilMax = 30;
}
