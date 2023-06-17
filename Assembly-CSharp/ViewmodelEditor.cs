using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000782 RID: 1922
public class ViewmodelEditor : SingletonComponent<ViewmodelEditor>
{
	// Token: 0x040024E3 RID: 9443
	private Vector3 view;

	// Token: 0x040024E4 RID: 9444
	private bool meleeHit;

	// Token: 0x060029CB RID: 10699 RVA: 0x000D4A78 File Offset: 0x000D2C78
	private void OnGUI()
	{
		this.DrawWeaponSwitch(new Rect((float)(Screen.width - 220), 20f, 200f, (float)(Screen.height - 40)));
		if (BaseViewModel.ActiveModel == null)
		{
			return;
		}
		int num = 30;
		int num2 = 10;
		int num3 = 120;
		int num4 = 20;
		if (BaseViewModel.ActiveModel.ironSights != null)
		{
			if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "Ironsights On"))
			{
				BaseViewModel.ActiveModel.ironSights.Enabled = true;
			}
			num4 += num + num2;
			if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "Ironsights Off"))
			{
				BaseViewModel.ActiveModel.ironSights.Enabled = false;
			}
			num4 += num + num2;
		}
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "attack"))
		{
			BaseViewModel.ActiveModel.TriggerAttack();
			this.meleeHit = false;
		}
		num4 += num + num2;
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "attack2"))
		{
			BaseViewModel.ActiveModel.TriggerAttack2();
			this.meleeHit = false;
		}
		num4 += num + num2;
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "deploy"))
		{
			BaseViewModel.ActiveModel.TriggerDeploy();
		}
		num4 += num + num2;
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "reload"))
		{
			BaseViewModel.ActiveModel.TriggerReload();
		}
		num4 += num + num2;
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "melee hit"))
		{
			BaseViewModel.ActiveModel.TriggerAttack();
			this.meleeHit = true;
		}
		num4 += num + num2;
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "ready"))
		{
			BaseViewModel.ActiveModel.TriggerReady();
		}
		num4 += num + num2;
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "cancel"))
		{
			BaseViewModel.ActiveModel.TriggerCancel();
		}
		num4 += num + num2;
		if (GUI.Button(new Rect(20f, (float)num4, (float)num3, 30f), "empty"))
		{
			BaseViewModel.ActiveModel.TriggerEmpty();
		}
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x000D4CCC File Offset: 0x000D2ECC
	protected override void Awake()
	{
		base.Awake();
		string @string = PlayerPrefs.GetString("EditorViewmodel");
		foreach (BaseViewModel baseViewModel in base.GetComponentsInChildren<BaseViewModel>(true))
		{
			baseViewModel.gameObject.SetActive(false);
			if (@string == baseViewModel.name)
			{
				baseViewModel.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x000D4D2C File Offset: 0x000D2F2C
	private void DrawWeaponSwitch(Rect rect)
	{
		BaseViewModel[] array = Enumerable.ToArray<BaseViewModel>(Enumerable.OrderBy<BaseViewModel, string>(base.GetComponentsInChildren<BaseViewModel>(true), (BaseViewModel x) => x.name));
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.activeSelf)
			{
				num = i;
			}
		}
		string[] array2 = Enumerable.ToArray<string>(Enumerable.Select<BaseViewModel, string>(array, (BaseViewModel x) => x.name));
		int num2 = GUI.SelectionGrid(rect, num, array2, 1);
		if (num2 != num && num2 >= 0)
		{
			if (num >= 0)
			{
				array[num].gameObject.SetActive(false);
			}
			array[num2].gameObject.SetActive(true);
			PlayerPrefs.SetString("EditorViewmodel", array[num2].name);
		}
	}

	// Token: 0x060029CE RID: 10702 RVA: 0x000D4E00 File Offset: 0x000D3000
	private void Update()
	{
		if (!BaseViewModel.ActiveModel)
		{
			return;
		}
		Camera.main.fieldOfView = 75f;
		if (Input.GetKey(KeyCode.Mouse2))
		{
			this.view.y = this.view.y + Input.GetAxisRaw("Mouse X") * 3f;
			this.view.x = this.view.x - Input.GetAxisRaw("Mouse Y") * 3f;
		}
		Camera.main.transform.rotation = Quaternion.Euler(this.view);
		Vector3 vector = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vector += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += Vector3.right;
		}
		Camera.main.transform.position += base.transform.rotation * vector * 0.2f;
		BaseViewModel.ActiveModel.OnCameraPositionChanged(Camera.main);
		BaseScreenShake.Apply(Camera.main, BaseViewModel.ActiveModel);
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x00020783 File Offset: 0x0001E983
	public void DoAnimationEvent(string name)
	{
		if (BaseViewModel.ActiveModel == null)
		{
			return;
		}
		if (name == "Strike")
		{
			if (this.meleeHit)
			{
				BaseViewModel.ActiveModel.TriggerAttack2();
			}
			this.meleeHit = false;
		}
	}
}
