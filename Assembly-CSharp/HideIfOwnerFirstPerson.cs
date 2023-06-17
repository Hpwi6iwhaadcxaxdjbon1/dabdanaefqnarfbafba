using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002B9 RID: 697
public class HideIfOwnerFirstPerson : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged
{
	// Token: 0x04000FC9 RID: 4041
	public GameObject[] disableGameObjects;

	// Token: 0x04000FCA RID: 4042
	public bool worldModelEffect;

	// Token: 0x04000FCB RID: 4043
	protected bool lastHidden;

	// Token: 0x04000FCC RID: 4044
	private Dictionary<Renderer, ShadowCastingMode> previousValues = new Dictionary<Renderer, ShadowCastingMode>();

	// Token: 0x0600136B RID: 4971 RVA: 0x00010791 File Offset: 0x0000E991
	public void OnEnable()
	{
		GlobalMessages.onViewModeChanged.Add(this);
		this.OnViewModeChanged();
		base.Invoke(new Action(this.OnViewModeChanged), 0f);
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x000107BC File Offset: 0x0000E9BC
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onViewModeChanged.Remove(this);
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x0007BAD4 File Offset: 0x00079CD4
	public void OnViewModeChanged()
	{
		if (base.baseEntity == null)
		{
			return;
		}
		bool flag = this.ShouldHide();
		if (this.worldModelEffect)
		{
			base.gameObject.SetActive(!flag);
		}
		if (this.lastHidden == flag)
		{
			return;
		}
		this.lastHidden = flag;
		if (flag)
		{
			this.SetShadowsOnly(base.gameObject);
		}
		else
		{
			this.RevertShadowsOnly(base.gameObject);
		}
		if (this.disableGameObjects != null)
		{
			GameObject[] array = this.disableGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(!flag);
			}
		}
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x0007BB68 File Offset: 0x00079D68
	public bool ShouldHide()
	{
		BaseEntity parentEntity = base.baseEntity.GetParentEntity();
		while (!(parentEntity is BasePlayer))
		{
			if (parentEntity == null)
			{
				return false;
			}
			parentEntity = parentEntity.GetParentEntity();
		}
		BasePlayer basePlayer = parentEntity as BasePlayer;
		return !(basePlayer == null) && basePlayer.IsLocalPlayer() && basePlayer.currentViewMode == BasePlayer.CameraMode.FirstPerson;
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x0007BBC4 File Offset: 0x00079DC4
	private void SetShadowsOnly(GameObject root)
	{
		this.RevertShadowsOnly(root);
		List<Renderer> list = Pool.GetList<Renderer>();
		root.GetComponentsInChildren<Renderer>(true, list);
		foreach (Renderer renderer in list)
		{
			if (!(renderer is ParticleSystemRenderer) && renderer.shadowCastingMode != ShadowCastingMode.ShadowsOnly)
			{
				if (renderer.shadowCastingMode == ShadowCastingMode.Off)
				{
					if (renderer.enabled)
					{
						if (!this.previousValues.ContainsKey(renderer))
						{
							this.previousValues.Add(renderer, renderer.shadowCastingMode);
						}
						renderer.enabled = false;
					}
				}
				else
				{
					if (!this.previousValues.ContainsKey(renderer))
					{
						this.previousValues.Add(renderer, renderer.shadowCastingMode);
					}
					renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
			}
		}
		Pool.FreeList<Renderer>(ref list);
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x0007BC9C File Offset: 0x00079E9C
	private void RevertShadowsOnly(GameObject root)
	{
		foreach (KeyValuePair<Renderer, ShadowCastingMode> keyValuePair in this.previousValues)
		{
			if (!(keyValuePair.Key == null))
			{
				if (keyValuePair.Value == ShadowCastingMode.Off)
				{
					keyValuePair.Key.enabled = true;
					return;
				}
				keyValuePair.Key.shadowCastingMode = keyValuePair.Value;
			}
		}
		this.previousValues.Clear();
	}
}
