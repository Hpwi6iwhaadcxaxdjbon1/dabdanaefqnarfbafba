using System;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class ViewmodelScope : MonoBehaviour
{
	// Token: 0x04000839 RID: 2105
	public float smoothSpeed = 0.05f;

	// Token: 0x0400083A RID: 2106
	public Material scopeMaterialOverride;

	// Token: 0x0400083B RID: 2107
	private bool wasVisible;

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0000B27E File Offset: 0x0000947E
	public void OnEnable()
	{
		this.UpdateScope();
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0000B286 File Offset: 0x00009486
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		UIScopeOverlay.instance.SetAlpha(0f);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0000B27E File Offset: 0x0000947E
	public void Update()
	{
		this.UpdateScope();
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0005A390 File Offset: 0x00058590
	public void UpdateScope()
	{
		bool flag = this.ShouldShow();
		bool flag2 = this.wasVisible != flag;
		this.wasVisible = flag;
		if (flag && this.scopeMaterialOverride != null)
		{
			UIScopeOverlay.instance.SetScopeMaterial(this.scopeMaterialOverride);
		}
		UIScopeOverlay.instance.SetAlpha(Mathf.Lerp(UIScopeOverlay.instance.GetAlpha(), (float)(flag ? 1 : 0), Time.deltaTime * 20f));
		UIScopeOverlay.instance.SetPosition(this.GetScreenPos(UIScopeOverlay.instance.GetPosition()));
		if (flag2)
		{
			ProjectileWeaponMod scopeMod = this.GetScopeMod();
			if (scopeMod)
			{
				ViewmodelAttachment component = scopeMod.GetComponent<ViewmodelAttachment>();
				if (component)
				{
					GameObject spawnedGameObject = component.spawnedGameObject;
					if (spawnedGameObject)
					{
						this.HideAttachments(spawnedGameObject.transform.root, !flag);
						this.ShowVMParts<SkinnedMeshRenderer>(spawnedGameObject.transform.root, !flag);
					}
				}
			}
		}
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0005A478 File Offset: 0x00058678
	public Vector3 GetScreenPos(Vector3 oldPos)
	{
		ProjectileWeaponMod scopeMod = this.GetScopeMod();
		if (scopeMod == null)
		{
			return Vector3.zero;
		}
		Transform targetPoint = scopeMod.GetComponent<ViewmodelAttachment>().spawnedGameObject.GetComponent<IronSightOverride>().aimPoint.targetPoint;
		Vector3 vector = MainCamera.mainCamera.WorldToScreenPoint(targetPoint.transform.position);
		float num = 1f;
		Vector3 zero = Vector3.zero;
		return Vector3.SmoothDamp(oldPos, new Vector3(vector.x * num, vector.y * num, 0f), ref zero, this.smoothSpeed);
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0005A504 File Offset: 0x00058704
	public void HideAttachments(Transform root, bool shouldShow)
	{
		HideIfScoped[] componentsInChildren = root.GetComponentsInChildren<HideIfScoped>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetVisible(shouldShow);
		}
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0005A530 File Offset: 0x00058730
	public void ShowVMParts<T>(Transform root, bool shouldShow) where T : Renderer
	{
		foreach (T t in root.GetComponentsInChildren<T>())
		{
			if (t.enabled != shouldShow)
			{
				t.enabled = shouldShow;
			}
		}
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0005A574 File Offset: 0x00058774
	public ProjectileWeaponMod GetScopeMod()
	{
		BaseProjectile localPlayerWeapon = this.GetLocalPlayerWeapon();
		if (localPlayerWeapon != null && localPlayerWeapon.children != null)
		{
			return Enumerable.FirstOrDefault<ProjectileWeaponMod>(Enumerable.Where<ProjectileWeaponMod>(Enumerable.Cast<ProjectileWeaponMod>(localPlayerWeapon.children), (ProjectileWeaponMod x) => x != null && x.isScope));
		}
		return null;
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0005A5D0 File Offset: 0x000587D0
	public BaseProjectile GetLocalPlayerWeapon()
	{
		if (LocalPlayer.Entity == null)
		{
			return null;
		}
		if (LocalPlayer.Entity.IsDead())
		{
			return null;
		}
		HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
		if (heldEntity == null)
		{
			return null;
		}
		BaseProjectile baseProjectile = heldEntity as BaseProjectile;
		if (baseProjectile == null)
		{
			return null;
		}
		return baseProjectile;
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0005A624 File Offset: 0x00058824
	public bool ShouldShow()
	{
		if (!LocalPlayer.Entity || LocalPlayer.Entity.currentViewMode != BasePlayer.CameraMode.FirstPerson)
		{
			return false;
		}
		BaseProjectile localPlayerWeapon = this.GetLocalPlayerWeapon();
		return !(localPlayerWeapon == null) && (localPlayerWeapon.ReadyToAim() && localPlayerWeapon.aiming);
	}
}
