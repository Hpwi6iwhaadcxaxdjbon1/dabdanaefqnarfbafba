using System;
using Facepunch.Extend;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200064A RID: 1610
public class CraftingQueueIcon : MonoBehaviour
{
	// Token: 0x04001FFB RID: 8187
	public CanvasGroup canvasGroup;

	// Token: 0x04001FFC RID: 8188
	public Image icon;

	// Token: 0x04001FFD RID: 8189
	public GameObject timeLeft;

	// Token: 0x04001FFE RID: 8190
	public GameObject craftingCount;

	// Token: 0x04001FFF RID: 8191
	[NonSerialized]
	public int taskid;

	// Token: 0x04002000 RID: 8192
	[NonSerialized]
	public float endTime;

	// Token: 0x04002001 RID: 8193
	[NonSerialized]
	public ItemDefinition item;

	// Token: 0x04002002 RID: 8194
	[NonSerialized]
	public int amount;

	// Token: 0x060023DA RID: 9178 RVA: 0x0001C547 File Offset: 0x0001A747
	private void Awake()
	{
		this.timeLeft.SetActive(false);
		this.craftingCount.SetActive(false);
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x0001C561 File Offset: 0x0001A761
	private void Update()
	{
		if (this.timeLeft.activeInHierarchy)
		{
			this.timeLeft.GetComponentInChildren<Text>(true).text = this.timeLeftString;
		}
	}

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x060023DC RID: 9180 RVA: 0x0001C587 File Offset: 0x0001A787
	public string timeLeftString
	{
		get
		{
			return NumberExtensions.FormatSeconds((long)Mathf.Clamp(this.endTime - Time.realtimeSinceStartup, 0f, 3600f));
		}
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x0001C5AA File Offset: 0x0001A7AA
	internal void OnTaskStart(float endTime)
	{
		this.endTime = endTime;
		this.timeLeft.SetActive(true);
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000BE030 File Offset: 0x000BC230
	internal void Finished(bool success, int amountleft)
	{
		if (amountleft == 0)
		{
			Object.DestroyImmediate(base.gameObject, false);
			return;
		}
		this.amount = amountleft;
		Text componentInChildren = this.craftingCount.GetComponentInChildren<Text>(true);
		if (componentInChildren)
		{
			componentInChildren.text = "x" + amountleft.ToString("N0");
		}
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000BE088 File Offset: 0x000BC288
	internal void Init(int taskid, int itemid, int amount, int skinid)
	{
		this.amount = amount;
		if (amount > 1)
		{
			this.craftingCount.SetActive(true);
			this.craftingCount.GetComponentInChildren<Text>(true).text = "x" + amount.ToString("N0");
		}
		this.taskid = taskid;
		this.item = ItemManager.FindItemDefinition(itemid);
		if (this.item != null)
		{
			this.icon.sprite = this.item.FindIconSprite(skinid);
		}
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x0001C5BF File Offset: 0x0001A7BF
	public void Cancel()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "craft.canceltask", new object[]
		{
			this.taskid
		});
	}
}
