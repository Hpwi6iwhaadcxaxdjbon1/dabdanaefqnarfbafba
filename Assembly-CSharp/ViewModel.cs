using System;
using Rust;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class ViewModel : MonoBehaviour, IClientComponent
{
	// Token: 0x04000DD7 RID: 3543
	public GameObjectRef viewModelPrefab;

	// Token: 0x04000DD8 RID: 3544
	[NonSerialized]
	public HeldEntity targetEntity;

	// Token: 0x04000DD9 RID: 3545
	[NonSerialized]
	public BaseViewModel instance;

	// Token: 0x060010F9 RID: 4345 RVA: 0x00072174 File Offset: 0x00070374
	public void Deploy(BaseEntity parent)
	{
		if (this.instance != null)
		{
			return;
		}
		GameObject gameObject = GameManager.client.CreatePrefab(this.viewModelPrefab.resourcePath, true);
		this.instance = gameObject.GetComponent<BaseViewModel>();
		if (this.instance == null)
		{
			Debug.LogError("Invalid viewmodel: " + this.instance);
			return;
		}
		if (parent.itemSkin != null)
		{
			parent.itemSkin.ApplySkin(this.instance.gameObject);
		}
		else if (parent.skinID != 0UL)
		{
			WorkshopSkin.Apply(gameObject, parent.skinID, null);
		}
		AnimationEvents componentInChildren = gameObject.GetComponentInChildren<AnimationEvents>();
		if (componentInChildren)
		{
			componentInChildren.targetEntity = this.targetEntity;
		}
		BaseEntityChild.Setup(gameObject, parent);
		gameObject.AddComponent<BaseEntityChild>();
		gameObject.BroadcastOnPostNetworkUpdate(parent);
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x0000EE02 File Offset: 0x0000D002
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Clear();
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x0000EE12 File Offset: 0x0000D012
	public void Holster()
	{
		if (this.instance == null)
		{
			return;
		}
		this.Clear();
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x0000EE29 File Offset: 0x0000D029
	public void Clear()
	{
		if (this.instance != null)
		{
			this.instance.PreDestroy();
			GameManager.Destroy(this.instance.gameObject, 0f);
			this.instance = null;
		}
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x0000EE60 File Offset: 0x0000D060
	public void OnFrame()
	{
		if (this.instance == null)
		{
			return;
		}
		if (!MainCamera.isValid)
		{
			return;
		}
		this.instance.OnCameraPositionChanged(MainCamera.mainCamera);
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x0000EE89 File Offset: 0x0000D089
	public void Play(string name)
	{
		if (this.instance == null)
		{
			return;
		}
		this.instance.Play(name);
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x0000EEA6 File Offset: 0x0000D0A6
	public void Trigger(string name)
	{
		if (this.instance == null)
		{
			return;
		}
		this.instance.Trigger(name);
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x0000EEC3 File Offset: 0x0000D0C3
	public void CrossFade(string name, float duration = 0.2f)
	{
		if (this.instance == null)
		{
			return;
		}
		this.instance.CrossFade(name, duration);
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x0000EEE1 File Offset: 0x0000D0E1
	public void SetBool(string name, bool bState)
	{
		if (this.instance == null)
		{
			return;
		}
		this.instance.SetBool(name, bState);
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x0000EEFF File Offset: 0x0000D0FF
	public void SetFloat(string name, float fAmount)
	{
		if (this.instance == null)
		{
			return;
		}
		this.instance.SetFloat(name, fAmount);
	}

	// Token: 0x170000D4 RID: 212
	// (set) Token: 0x06001103 RID: 4355 RVA: 0x0000EF1D File Offset: 0x0000D11D
	public bool ironSights
	{
		set
		{
			if (this.instance == null || this.instance.ironSights == null)
			{
				return;
			}
			this.instance.ironSights.SetEnabled(value);
		}
	}
}
