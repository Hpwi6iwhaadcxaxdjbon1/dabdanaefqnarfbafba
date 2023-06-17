using System;
using UnityEngine;

// Token: 0x02000372 RID: 882
public class ColliderInfo : MonoBehaviour
{
	// Token: 0x0400137D RID: 4989
	public const ColliderInfo.Flags FlagsNone = (ColliderInfo.Flags)0;

	// Token: 0x0400137E RID: 4990
	public const ColliderInfo.Flags FlagsEverything = (ColliderInfo.Flags)(-1);

	// Token: 0x0400137F RID: 4991
	public const ColliderInfo.Flags FlagsDefault = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x04001380 RID: 4992
	[InspectorFlags]
	public ColliderInfo.Flags flags = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x06001696 RID: 5782 RVA: 0x00013149 File Offset: 0x00011349
	public bool HasFlag(ColliderInfo.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x00013156 File Offset: 0x00011356
	public void SetFlag(ColliderInfo.Flags f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x000879C0 File Offset: 0x00085BC0
	public bool Filter(HitTest info)
	{
		switch (info.type)
		{
		case HitTest.Type.ProjectileEffect:
		case HitTest.Type.Projectile:
			if ((this.flags & ColliderInfo.Flags.Shootable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.MeleeAttack:
			if ((this.flags & ColliderInfo.Flags.Melee) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.Use:
			if ((this.flags & ColliderInfo.Flags.Usable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x02000373 RID: 883
	[Flags]
	public enum Flags
	{
		// Token: 0x04001382 RID: 4994
		Usable = 1,
		// Token: 0x04001383 RID: 4995
		Shootable = 2,
		// Token: 0x04001384 RID: 4996
		Melee = 4,
		// Token: 0x04001385 RID: 4997
		Opaque = 8,
		// Token: 0x04001386 RID: 4998
		Airflow = 16
	}
}
