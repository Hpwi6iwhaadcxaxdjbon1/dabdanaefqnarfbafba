using System;
using ConVar;
using UnityEngine;

// Token: 0x02000202 RID: 514
public static class FirstPersonSpectatorMode
{
	// Token: 0x06000FDA RID: 4058 RVA: 0x0006BAB0 File Offset: 0x00069CB0
	public static void Apply(Camera cam, BaseEntity target)
	{
		if (target == null)
		{
			return;
		}
		if (!(target is BasePlayer))
		{
			Transform eyeTransform = target.GetEyeTransform();
			cam.transform.position = eyeTransform.position;
			cam.transform.rotation = eyeTransform.rotation;
			return;
		}
		cam.fieldOfView = ConVar.Graphics.fov;
		cam.transform.position = target.ToPlayer().eyes.position;
		if (target.ToPlayer().playerModel)
		{
			cam.transform.rotation = target.ToPlayer().playerModel.LookAngles;
			return;
		}
		cam.transform.rotation = target.ToPlayer().eyes.rotation;
	}
}
