using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000663 RID: 1635
public class LootPanelContainer : MonoBehaviour
{
	// Token: 0x04002071 RID: 8305
	public static string containerName = "generic";

	// Token: 0x04002072 RID: 8306
	public GameObject NoLootPanel;

	// Token: 0x04002073 RID: 8307
	internal bool wasShowingPanel = true;

	// Token: 0x04002074 RID: 8308
	internal GameObject currentLootPanel;

	// Token: 0x04002075 RID: 8309
	private string _lastLootPanel = "";

	// Token: 0x0600246D RID: 9325 RVA: 0x000C0B40 File Offset: 0x000BED40
	private void Update()
	{
		bool shouldShowPanel = this.shouldShowPanel;
		if (shouldShowPanel != this.wasShowingPanel)
		{
			this.wasShowingPanel = shouldShowPanel;
			if (this.wasShowingPanel)
			{
				this.CreateLootPanel();
				base.GetComponent<LayoutElement>().ignoreLayout = false;
				base.GetComponent<CanvasGroup>().alpha = 1f;
				base.GetComponent<CanvasGroup>().interactable = true;
			}
			else
			{
				this.DestroyLootPanel();
				LootPanelContainer.containerName = "generic";
				base.GetComponent<LayoutElement>().ignoreLayout = true;
				base.GetComponent<CanvasGroup>().alpha = 0f;
				base.GetComponent<CanvasGroup>().interactable = false;
			}
		}
		if (shouldShowPanel && this._lastLootPanel != LootPanelContainer.containerName)
		{
			this.CreateLootPanel();
		}
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000C0BF0 File Offset: 0x000BEDF0
	private void CreateLootPanel()
	{
		this.DestroyLootPanel();
		string text = string.Format("assets/bundled/prefabs/ui/lootpanels/lootpanel.{0}.prefab", LootPanelContainer.containerName);
		this.currentLootPanel = GameManager.client.CreatePrefab(text, base.transform, true);
		this._lastLootPanel = LootPanelContainer.containerName;
		if (this.currentLootPanel == null)
		{
			Debug.LogError("Missing Loot Panel: " + text);
		}
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x0001CBB9 File Offset: 0x0001ADB9
	private void DestroyLootPanel()
	{
		if (this.currentLootPanel == null)
		{
			return;
		}
		GameManager.client.Retire(this.currentLootPanel);
		this.currentLootPanel = null;
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06002470 RID: 9328 RVA: 0x0001CBE1 File Offset: 0x0001ADE1
	private bool shouldShowPanel
	{
		get
		{
			return LocalPlayer.Entity && LocalPlayer.Entity.inventory.loot.IsLooting();
		}
	}
}
