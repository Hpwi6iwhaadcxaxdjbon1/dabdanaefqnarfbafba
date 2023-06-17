using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000219 RID: 537
public interface ILerpTarget
{
	// Token: 0x06001068 RID: 4200
	float GetExtrapolationTime();

	// Token: 0x06001069 RID: 4201
	float GetInterpolationDelay();

	// Token: 0x0600106A RID: 4202
	float GetInterpolationSmoothing();

	// Token: 0x0600106B RID: 4203
	Vector3 GetNetworkPosition();

	// Token: 0x0600106C RID: 4204
	Quaternion GetNetworkRotation();

	// Token: 0x0600106D RID: 4205
	void SetNetworkPosition(Vector3 pos);

	// Token: 0x0600106E RID: 4206
	void SetNetworkRotation(Quaternion rot);

	// Token: 0x0600106F RID: 4207
	void DrawInterpolationState(TransformInterpolator.Segment segment, List<TransformInterpolator.Entry> entries);
}
