using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class AimConeUtil
{
	// Token: 0x06000C75 RID: 3189 RVA: 0x0005B964 File Offset: 0x00059B64
	public static Vector3 GetModifiedAimConeDirection(float aimCone, Vector3 inputVec, bool anywhereInside = true)
	{
		Quaternion lhs = Quaternion.LookRotation(inputVec);
		Vector2 vector = anywhereInside ? Random.insideUnitCircle : Random.insideUnitCircle.normalized;
		return lhs * Quaternion.Euler(vector.x * aimCone * 0.5f, vector.y * aimCone * 0.5f, 0f) * Vector3.forward;
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x0005B9C4 File Offset: 0x00059BC4
	public static Quaternion GetAimConeQuat(float aimCone)
	{
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		return Quaternion.Euler(insideUnitSphere.x * aimCone * 0.5f, insideUnitSphere.y * aimCone * 0.5f, 0f);
	}
}
