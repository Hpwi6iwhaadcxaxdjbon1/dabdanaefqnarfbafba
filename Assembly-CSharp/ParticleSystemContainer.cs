using System;
using UnityEngine;

// Token: 0x02000718 RID: 1816
public class ParticleSystemContainer : MonoBehaviour
{
	// Token: 0x060027C7 RID: 10183 RVA: 0x000CDB20 File Offset: 0x000CBD20
	public void Play()
	{
		if (base.gameObject.activeInHierarchy)
		{
			foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
			{
				LODComponentParticleSystem component = particleSystem.GetComponent<LODComponentParticleSystem>();
				if (component)
				{
					component.SetVisible(true);
				}
				else
				{
					particleSystem.Play();
				}
			}
		}
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x000CDB74 File Offset: 0x000CBD74
	public void Pause()
	{
		if (base.gameObject.activeInHierarchy)
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Pause();
			}
		}
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000CDBAC File Offset: 0x000CBDAC
	public void Stop()
	{
		if (base.gameObject.activeInHierarchy)
		{
			foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
			{
				LODComponentParticleSystem component = particleSystem.GetComponent<LODComponentParticleSystem>();
				if (component)
				{
					component.SetVisible(false);
				}
				else
				{
					particleSystem.Stop();
				}
			}
		}
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000CDC00 File Offset: 0x000CBE00
	public void Clear()
	{
		if (base.gameObject.activeInHierarchy)
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Clear();
			}
		}
	}
}
