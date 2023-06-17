using System;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class ConstructionSocket : Socket_Base
{
	// Token: 0x04000B6D RID: 2925
	public ConstructionSocket.Type socketType;

	// Token: 0x04000B6E RID: 2926
	public int rotationDegrees;

	// Token: 0x04000B6F RID: 2927
	public int rotationOffset;

	// Token: 0x04000B70 RID: 2928
	public bool restrictPlacementAngle;

	// Token: 0x04000B71 RID: 2929
	public float faceAngle;

	// Token: 0x04000B72 RID: 2930
	public float angleAllowed = 150f;

	// Token: 0x04000B73 RID: 2931
	[Range(0f, 1f)]
	public float support = 1f;

	// Token: 0x06000E4F RID: 3663 RVA: 0x00064920 File Offset: 0x00062B20
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.6f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x0000D20A File Offset: 0x0000B40A
	private void OnDrawGizmosSelected()
	{
		if (this.female)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
		}
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x0000D235 File Offset: 0x0000B435
	public override bool TestTarget(Construction.Target target)
	{
		return base.TestTarget(target) && this.IsCompatible(target.socket);
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x000649BC File Offset: 0x00062BBC
	public override bool IsCompatible(Socket_Base socket)
	{
		if (!base.IsCompatible(socket))
		{
			return false;
		}
		ConstructionSocket constructionSocket = socket as ConstructionSocket;
		return !(constructionSocket == null) && constructionSocket.socketType != ConstructionSocket.Type.None && this.socketType != ConstructionSocket.Type.None && constructionSocket.socketType == this.socketType;
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x00064A0C File Offset: 0x00062C0C
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.one);
		Vector3 a = matrix4x.MultiplyPoint3x4(this.worldPosition);
		Vector3 b = matrix4x2.MultiplyPoint3x4(socket.worldPosition);
		if (Vector3.Distance(a, b) > 0.01f)
		{
			return false;
		}
		Vector3 vector = matrix4x.MultiplyVector(this.worldRotation * Vector3.forward);
		Vector3 vector2 = matrix4x2.MultiplyVector(socket.worldRotation * Vector3.forward);
		float num = Vector3.Angle(vector, vector2);
		if (this.male && this.female)
		{
			num = Mathf.Min(num, Vector3.Angle(-vector, vector2));
		}
		if (socket.male && socket.female)
		{
			num = Mathf.Min(num, Vector3.Angle(vector, -vector2));
		}
		return num <= 1f;
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x00064B04 File Offset: 0x00062D04
	public bool TestRestrictedAngles(Vector3 suggestedPos, Quaternion suggestedAng, Construction.Target target)
	{
		if (this.restrictPlacementAngle)
		{
			Quaternion rotation = Quaternion.Euler(0f, this.faceAngle, 0f) * suggestedAng;
			float num = Vector3Ex.DotDegrees(Vector3Ex.XZ3D(target.ray.direction), rotation * Vector3.forward);
			if (num > this.angleAllowed * 0.5f)
			{
				return false;
			}
			if (num < this.angleAllowed * -0.5f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00064B7C File Offset: 0x00062D7C
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		if (!target.entity || !target.entity.transform)
		{
			return null;
		}
		Vector3 worldPosition = target.GetWorldPosition();
		Quaternion worldRotation = target.GetWorldRotation(true);
		if (this.rotationDegrees > 0)
		{
			Construction.Placement placement = new Construction.Placement();
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < 360; i += this.rotationDegrees)
			{
				Quaternion lhs = Quaternion.Euler(0f, (float)(this.rotationOffset + i), 0f);
				Vector3 direction = target.ray.direction;
				Vector3 to = lhs * worldRotation * Vector3.up;
				float num3 = Vector3.Angle(direction, to);
				if (num3 < num)
				{
					num = num3;
					num2 = (float)i;
				}
			}
			for (int j = 0; j < 360; j += this.rotationDegrees)
			{
				Quaternion rhs = worldRotation * Quaternion.Inverse(this.rotation);
				Quaternion lhs2 = Quaternion.Euler(target.rotation);
				Quaternion rhs2 = Quaternion.Euler(0f, (float)(this.rotationOffset + j) + num2, 0f);
				Quaternion rotation = lhs2 * rhs2 * rhs;
				Vector3 b = rotation * this.position;
				placement.position = worldPosition - b;
				placement.rotation = rotation;
				if (this.CheckSocketMods(placement))
				{
					return placement;
				}
			}
		}
		Construction.Placement placement2 = new Construction.Placement();
		Quaternion rotation2 = worldRotation * Quaternion.Inverse(this.rotation);
		Vector3 b2 = rotation2 * this.position;
		placement2.position = worldPosition - b2;
		placement2.rotation = rotation2;
		if (!this.TestRestrictedAngles(worldPosition, worldRotation, target))
		{
			return null;
		}
		return placement2;
	}

	// Token: 0x020001A6 RID: 422
	public enum Type
	{
		// Token: 0x04000B75 RID: 2933
		None,
		// Token: 0x04000B76 RID: 2934
		Foundation,
		// Token: 0x04000B77 RID: 2935
		Floor,
		// Token: 0x04000B78 RID: 2936
		Doorway = 4,
		// Token: 0x04000B79 RID: 2937
		Wall,
		// Token: 0x04000B7A RID: 2938
		Block,
		// Token: 0x04000B7B RID: 2939
		Window = 11,
		// Token: 0x04000B7C RID: 2940
		Shutters,
		// Token: 0x04000B7D RID: 2941
		WallFrame,
		// Token: 0x04000B7E RID: 2942
		FloorFrame,
		// Token: 0x04000B7F RID: 2943
		WindowDressing,
		// Token: 0x04000B80 RID: 2944
		DoorDressing
	}
}
