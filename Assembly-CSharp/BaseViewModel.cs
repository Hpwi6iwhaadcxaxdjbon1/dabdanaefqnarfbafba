using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000771 RID: 1905
public class BaseViewModel : MonoBehaviour
{
	// Token: 0x0400247C RID: 9340
	public static bool HideViewmodel = false;

	// Token: 0x0400247D RID: 9341
	public static List<BaseViewModel> ActiveModels = new List<BaseViewModel>();

	// Token: 0x0400247E RID: 9342
	[Header("BaseViewModel")]
	public LazyAimProperties lazyaimRegular;

	// Token: 0x0400247F RID: 9343
	public LazyAimProperties lazyaimIronsights;

	// Token: 0x04002480 RID: 9344
	public Transform pivot;

	// Token: 0x04002481 RID: 9345
	public bool wantsHeldItemFlags;

	// Token: 0x04002482 RID: 9346
	public GameObject[] hideSightMeshes;

	// Token: 0x04002483 RID: 9347
	public Transform MuzzlePoint;

	// Token: 0x04002484 RID: 9348
	[Header("Skin")]
	public SubsurfaceProfile subsurfaceProfile;

	// Token: 0x04002485 RID: 9349
	internal Animator animator;

	// Token: 0x04002486 RID: 9350
	internal AnimationEvents animationEvent;

	// Token: 0x04002487 RID: 9351
	internal IronSights ironSights;

	// Token: 0x04002488 RID: 9352
	internal ViewmodelSway sway;

	// Token: 0x04002489 RID: 9353
	internal ViewmodelLower lower;

	// Token: 0x0400248A RID: 9354
	internal ViewmodelBob bob;

	// Token: 0x0400248B RID: 9355
	internal Model model;

	// Token: 0x0400248C RID: 9356
	private Skeleton Skeleton;

	// Token: 0x0400248D RID: 9357
	private GameObject Clothing;

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x06002973 RID: 10611 RVA: 0x000203FD File Offset: 0x0001E5FD
	public static BaseViewModel ActiveModel
	{
		get
		{
			if (BaseViewModel.ActiveModels.Count <= 0)
			{
				return null;
			}
			return BaseViewModel.ActiveModels[0];
		}
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x00020419 File Offset: 0x0001E619
	public bool IsOK()
	{
		return this.animator;
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x0002042B File Offset: 0x0001E62B
	public void PreDestroy()
	{
		base.gameObject.BroadcastOnParentDestroying();
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x00020438 File Offset: 0x0001E638
	private void OnEnable()
	{
		if (BaseViewModel.ActiveModels.Count > 0)
		{
			Debug.LogWarning("More than one viewmodel active at the same time.");
		}
		BaseViewModel.ActiveModels.Add(this);
		GlobalMessages.OnViewModelUpdated();
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x00020461 File Offset: 0x0001E661
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		BaseViewModel.ActiveModels.Remove(this);
		GlobalMessages.OnViewModelUpdated();
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000D31F4 File Offset: 0x000D13F4
	private void Awake()
	{
		this.Skeleton = base.GetComponent<Skeleton>();
		this.animator = base.GetComponent<Animator>();
		this.animationEvent = base.GetComponent<AnimationEvents>();
		this.ironSights = base.GetComponent<IronSights>();
		this.sway = base.GetComponent<ViewmodelSway>();
		this.lower = base.GetComponent<ViewmodelLower>();
		this.bob = base.GetComponent<ViewmodelBob>();
		this.model = base.GetComponent<Model>();
		if (!this.animationEvent)
		{
			this.animationEvent = base.GetComponentInChildren<AnimationEvents>();
		}
		if (!this.animator)
		{
			this.animator = base.GetComponentInChildren<Animator>();
		}
		if (MainCamera.mainCamera)
		{
			base.transform.localPosition = MainCamera.mainCamera.transform.position;
			base.transform.localRotation = MainCamera.mainCamera.transform.rotation;
		}
		this.UpdateRenderers();
		if (LocalPlayer.Entity && LocalPlayer.Entity.playerModel)
		{
			this.SetSkinColor(LocalPlayer.Entity.playerModel.GetSkinColor());
		}
		else
		{
			this.SetSkinColor(new Color(1.0078431f, 0.37254903f, 0.28627452f));
		}
		this.animator.Update(Mathf.Epsilon);
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x0002047C File Offset: 0x0001E67C
	public void OnClothingChanged()
	{
		this.UpdateRenderers();
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000D3338 File Offset: 0x000D1538
	private void UpdateRenderers()
	{
		if (this.Clothing != null)
		{
			Object.Destroy(this.Clothing);
			this.Clothing = null;
		}
		this.Clothing = new GameObject();
		this.Clothing.transform.parent = base.transform;
		this.AddHandsModel();
		this.UpdateClothingModels();
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x00002ECE File Offset: 0x000010CE
	private void AddHandsModel()
	{
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000D3394 File Offset: 0x000D1594
	private void UpdateClothingModels()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (LocalPlayer.Entity.inventory == null)
		{
			return;
		}
		if (LocalPlayer.Entity.inventory.containerWear == null)
		{
			return;
		}
		foreach (Item item in LocalPlayer.Entity.inventory.containerWear.itemList)
		{
			if (!(item.info.ItemModWearable == null) && item.info.ItemModWearable.viewmodelAddition.isValid)
			{
				this.AddClothing(item.info.ItemModWearable.viewmodelAddition.Get().GetComponent<ViewmodelClothing>(), item);
			}
		}
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x00020484 File Offset: 0x0001E684
	private void AddClothing(ViewmodelClothing clothing, Item item)
	{
		if (this.Skeleton == null)
		{
			return;
		}
		clothing.CopyToSkeleton(this.Skeleton, this.Clothing, item);
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x000D346C File Offset: 0x000D166C
	private void SetSkinColor(Color skinColor)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor("_BaseColorTint", skinColor);
		if (this.subsurfaceProfile != null)
		{
			materialPropertyBlock.SetFloat("_SubsurfaceProfile", (float)this.subsurfaceProfile.Id);
		}
		Renderer[] array = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		array = array;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetPropertyBlock(materialPropertyBlock);
		}
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000D34D4 File Offset: 0x000D16D4
	public void OnCameraPositionChanged(Camera cam)
	{
		if (BaseViewModel.HideViewmodel)
		{
			base.transform.position = cam.transform.position + new Vector3(0f, 1000f, 0f);
			base.transform.rotation = Quaternion.identity;
			return;
		}
		CachedTransform<BaseViewModel> cachedTransform = new CachedTransform<BaseViewModel>(this);
		cachedTransform.position = cam.transform.position;
		cachedTransform.rotation = cam.transform.rotation;
		if (LocalPlayer.Entity)
		{
			Vector3 point = Vector3.Scale(base.transform.worldToLocalMatrix.MultiplyPoint3x4(this.pivot.transform.position), base.transform.localScale);
			Quaternion rotation = LocalPlayer.Entity.eyes.rotationLook * Quaternion.Inverse(LocalPlayer.Entity.eyes.headRotation);
			Vector3 vector = Vector3.zero;
			vector += rotation * point;
			vector -= LocalPlayer.Entity.eyes.rotation * point;
			cachedTransform.rotation = LocalPlayer.Entity.eyes.rotation;
			cachedTransform.position = cam.transform.position + vector;
		}
		if (this.ironSights)
		{
			this.ironSights.UpdateIronsights(ref cachedTransform, cam);
		}
		if (this.sway)
		{
			this.sway.Apply(ref cachedTransform);
		}
		if (this.lower)
		{
			this.lower.Apply(ref cachedTransform);
		}
		if (this.bob)
		{
			this.bob.Apply(ref cachedTransform, cam.fieldOfView);
		}
		cachedTransform.Apply();
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000204A8 File Offset: 0x0001E6A8
	public LazyAimProperties GetLazyAim()
	{
		if ((this.ironSights ? this.ironSights.GetDelta() : 0f) > 0.5f)
		{
			return this.lazyaimIronsights;
		}
		return this.lazyaimRegular;
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000204DD File Offset: 0x0001E6DD
	public void TriggerAttack()
	{
		this.Play("attack");
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x000204EA File Offset: 0x0001E6EA
	public void TriggerAttack2()
	{
		this.Play("attack2");
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x000204F7 File Offset: 0x0001E6F7
	public void TriggerReady()
	{
		this.Play("ready");
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x00020504 File Offset: 0x0001E704
	public void TriggerCancel()
	{
		this.Play("cancel");
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x00020511 File Offset: 0x0001E711
	public void TriggerDeploy()
	{
		this.Play("deploy");
	}

	// Token: 0x06002986 RID: 10630 RVA: 0x0002051E File Offset: 0x0001E71E
	public void TriggerReload()
	{
		this.Play("reload");
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x0002052B File Offset: 0x0001E72B
	public void TriggerHolster()
	{
		this.Play("holster");
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x00020538 File Offset: 0x0001E738
	public void TriggerEmpty()
	{
		this.Play("empty");
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x00020545 File Offset: 0x0001E745
	public void Trigger(string name)
	{
		if (!this.IsOK())
		{
			return;
		}
		this.animator.SetTrigger(name);
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x0002055C File Offset: 0x0001E75C
	public void SetBool(string name, bool bState)
	{
		if (!this.IsOK())
		{
			return;
		}
		this.animator.SetBool(name, bState);
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x00020574 File Offset: 0x0001E774
	public void SetFloat(string name, float fAmount)
	{
		if (!this.IsOK())
		{
			return;
		}
		this.animator.SetFloat(name, fAmount);
	}

	// Token: 0x0600298C RID: 10636 RVA: 0x000D3698 File Offset: 0x000D1898
	public void Play(string anim)
	{
		if (!this.IsOK())
		{
			return;
		}
		if (!this.animator.HasState(0, Animator.StringToHash(anim)))
		{
			Debug.LogError(this.animator.name + ": Missing state '" + anim + "'");
			return;
		}
		this.animator.Play(anim, 0, 0f);
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x000D36F8 File Offset: 0x000D18F8
	public void CrossFade(string anim, float fade)
	{
		if (!this.IsOK())
		{
			return;
		}
		if (!this.animator.HasState(0, Animator.StringToHash(anim)))
		{
			Debug.LogError(this.animator.name + ": Missing state '" + anim + "'");
			return;
		}
		this.animator.CrossFade(anim, fade, 0, 0f);
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x000D3758 File Offset: 0x000D1958
	public void HideSightMeshes(bool bHide)
	{
		GameObject[] array = this.hideSightMeshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!bHide);
		}
	}
}
