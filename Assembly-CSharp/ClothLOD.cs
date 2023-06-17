using System;
using UnityEngine;

// Token: 0x020003BB RID: 955
public class ClothLOD : FacepunchBehaviour
{
	// Token: 0x040014AB RID: 5291
	[ServerVar(Help = "distance cloth will simulate until")]
	public static float clothLODDist = 20f;

	// Token: 0x040014AC RID: 5292
	public Cloth cloth;

	// Token: 0x040014AD RID: 5293
	private bool lastWantsEnabled = true;

	// Token: 0x060017FC RID: 6140 RVA: 0x00014022 File Offset: 0x00012222
	public void Awake()
	{
		base.InvokeRandomized(new Action(this.LODCheck), 1f, 1f, 0.25f);
	}

	// Token: 0x060017FD RID: 6141 RVA: 0x0008C218 File Offset: 0x0008A418
	public void LODCheck()
	{
		bool flag = MainCamera.Distance(base.transform.position) < ClothLOD.clothLODDist;
		if (flag != this.lastWantsEnabled)
		{
			base.CancelInvoke(new Action(this.DisableCloth));
			this.cloth.SetEnabledFading(flag, 0.5f);
			if (flag)
			{
				this.EnableCloth();
			}
			else
			{
				base.Invoke(new Action(this.DisableCloth), 0.5f);
			}
			this.lastWantsEnabled = flag;
		}
	}

	// Token: 0x060017FE RID: 6142 RVA: 0x00014045 File Offset: 0x00012245
	public void EnableCloth()
	{
		this.cloth.enabled = true;
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x00014053 File Offset: 0x00012253
	public void DisableCloth()
	{
		this.cloth.enabled = false;
	}
}
