using System;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020006C8 RID: 1736
public class ToggleGroupCookie : MonoBehaviour
{
	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06002686 RID: 9862 RVA: 0x0001E082 File Offset: 0x0001C282
	public ToggleGroup group
	{
		get
		{
			return base.GetComponent<ToggleGroup>();
		}
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000CA82C File Offset: 0x000C8A2C
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("ToggleGroupCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			Transform transform = base.transform.Find(@string);
			if (transform)
			{
				Toggle component = transform.GetComponent<Toggle>();
				if (component)
				{
					Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].isOn = false;
					}
					component.isOn = false;
					component.isOn = true;
					this.SetupListeners();
					return;
				}
			}
		}
		Toggle toggle = Enumerable.FirstOrDefault<Toggle>(this.group.ActiveToggles(), (Toggle x) => x.isOn);
		if (toggle)
		{
			toggle.isOn = false;
			toggle.isOn = true;
		}
		this.SetupListeners();
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x000CA904 File Offset: 0x000C8B04
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000CA948 File Offset: 0x000C8B48
	private void SetupListeners()
	{
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000CA984 File Offset: 0x000C8B84
	private void OnToggleChanged(bool b)
	{
		Toggle toggle = Enumerable.FirstOrDefault<Toggle>(base.GetComponentsInChildren<Toggle>(), (Toggle x) => x.isOn);
		if (toggle)
		{
			PlayerPrefs.SetString("ToggleGroupCookie_" + base.name, toggle.gameObject.name);
		}
	}
}
