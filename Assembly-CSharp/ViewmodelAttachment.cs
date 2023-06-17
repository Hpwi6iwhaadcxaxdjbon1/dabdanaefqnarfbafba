using System;
using Rust;
using UnityEngine;

// Token: 0x0200077F RID: 1919
public class ViewmodelAttachment : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged, IViewModelUpdated
{
	// Token: 0x040024CC RID: 9420
	public GameObjectRef modelObject;

	// Token: 0x040024CD RID: 9421
	public string targetBone;

	// Token: 0x040024CE RID: 9422
	public bool hideViewModelIronSights;

	// Token: 0x040024CF RID: 9423
	[NonSerialized]
	public GameObject spawnedGameObject;

	// Token: 0x060029BF RID: 10687 RVA: 0x0002075D File Offset: 0x0001E95D
	public void OnEnable()
	{
		GlobalMessages.onViewModeChanged.Add(this);
		GlobalMessages.onViewModelUpdated.Add(this);
		this.OnViewModeChanged();
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x000D42B0 File Offset: 0x000D24B0
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onViewModeChanged.Remove(this);
		GlobalMessages.onViewModelUpdated.Remove(this);
		if (this.spawnedGameObject)
		{
			if (BaseViewModel.ActiveModel != null && this.hideViewModelIronSights)
			{
				BaseViewModel.ActiveModel.HideSightMeshes(false);
			}
			GameManager.Destroy(this.spawnedGameObject, 0f);
			this.spawnedGameObject = null;
		}
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x0002077B File Offset: 0x0001E97B
	public void OnViewModeChanged()
	{
		this.CreateAttachment();
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x0002077B File Offset: 0x0001E97B
	public void OnViewModelUpdated()
	{
		this.CreateAttachment();
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x000D4324 File Offset: 0x000D2524
	public void CreateAttachment()
	{
		if (base.baseEntity == null)
		{
			return;
		}
		if (this.spawnedGameObject != null)
		{
			return;
		}
		if (!BaseViewModel.ActiveModel)
		{
			return;
		}
		if (!this.IsThisOurViewmodel(BaseViewModel.ActiveModel))
		{
			return;
		}
		if (BaseViewModel.ActiveModel.model == null)
		{
			Debug.LogWarning("Viewmodel without model component: " + BaseViewModel.ActiveModel);
			return;
		}
		this.spawnedGameObject = this.modelObject.Instantiate(null);
		Transform transform = BaseViewModel.ActiveModel.model.FindBone(this.targetBone);
		if (transform == null)
		{
			transform = BaseViewModel.ActiveModel.transform;
		}
		if (this.hideViewModelIronSights)
		{
			BaseViewModel.ActiveModel.HideSightMeshes(true);
		}
		this.spawnedGameObject.transform.SetParent(transform, false);
		this.spawnedGameObject.Identity();
		BaseEntityChild.Setup(this.spawnedGameObject, base.baseEntity);
		this.spawnedGameObject.AddComponent<BaseEntityChild>();
		this.spawnedGameObject.SetActive(true);
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000D4428 File Offset: 0x000D2628
	public virtual void RootEntFlagsChanged(BaseEntity flagCarrier)
	{
		if (this.spawnedGameObject == null)
		{
			return;
		}
		BaseEntity baseEntity = this.spawnedGameObject.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.flags = flagCarrier.flags;
			this.spawnedGameObject.BroadcastOnPostNetworkUpdate(baseEntity);
		}
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x000D4470 File Offset: 0x000D2670
	private bool IsThisOurViewmodel(BaseViewModel viewmodel)
	{
		BaseEntity parentEntity = base.baseEntity.GetParentEntity();
		return !(parentEntity == null) && !(LocalPlayer.Entity == null) && !(LocalPlayer.Entity.GetHeldEntity() != parentEntity);
	}
}
