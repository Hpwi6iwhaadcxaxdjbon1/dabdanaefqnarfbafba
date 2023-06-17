using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200061C RID: 1564
public class DeveloperTools : SingletonComponent<DeveloperTools>
{
	// Token: 0x04001F28 RID: 7976
	public GameObject developerTools;

	// Token: 0x04001F29 RID: 7977
	public GameObject navButton;

	// Token: 0x04001F2A RID: 7978
	public GameObject panelContainer;

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x060022FF RID: 8959 RVA: 0x0001BB8E File Offset: 0x00019D8E
	public static bool isOpen
	{
		get
		{
			return SingletonComponent<DeveloperTools>.Instance && SingletonComponent<DeveloperTools>.Instance.developerTools.activeSelf;
		}
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x0001BBAD File Offset: 0x00019DAD
	public static void Close()
	{
		if (!SingletonComponent<DeveloperTools>.Instance)
		{
			return;
		}
		SingletonComponent<DeveloperTools>.Instance.developerTools.SetActive(false);
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000BA838 File Offset: 0x000B8A38
	private void Start()
	{
		this.developerTools.SetActive(false);
		for (int i = 0; i < this.panelContainer.transform.childCount; i++)
		{
			Transform child = this.panelContainer.transform.GetChild(i);
			child.gameObject.SetActive(false);
			GameObject newButton = this.navButton.transform.parent.gameObject.InstantiateChild(this.navButton);
			newButton.GetComponentsInChildren<Text>(true)[0].text = child.name;
			newButton.GetComponent<Button>().onClick.AddListener(delegate()
			{
				this.UnselectAll();
				newButton.GetComponent<Button>().interactable = false;
				child.gameObject.SetActive(true);
				child.gameObject.BroadcastMessage("OnPanelOpened", SendMessageOptions.DontRequireReceiver);
			});
			if (i == 0)
			{
				child.gameObject.SetActive(true);
				newButton.GetComponent<Button>().interactable = false;
				child.gameObject.BroadcastMessage("OnPanelOpened", SendMessageOptions.DontRequireReceiver);
			}
		}
		this.navButton.SetActive(false);
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000BA954 File Offset: 0x000B8B54
	private void UnselectAll()
	{
		for (int i = 0; i < this.navButton.transform.parent.childCount; i++)
		{
			this.navButton.transform.parent.GetChild(i).GetComponent<Button>().interactable = true;
		}
		for (int j = 0; j < this.panelContainer.transform.childCount; j++)
		{
			this.panelContainer.transform.GetChild(j).gameObject.SetActive(false);
		}
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x0001BBCC File Offset: 0x00019DCC
	public void ToggleConsole()
	{
		this.developerTools.SetActive(!this.developerTools.activeSelf);
		if (this.developerTools.activeSelf)
		{
			this.developerTools.BroadcastMessage("OnOpenDevTools", SendMessageOptions.DontRequireReceiver);
		}
	}
}
