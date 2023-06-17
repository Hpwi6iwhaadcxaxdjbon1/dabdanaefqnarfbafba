using System;
using System.Collections;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000601 RID: 1537
public class Achievements : SingletonComponent<Achievements>
{
	// Token: 0x04001ECF RID: 7887
	public SoundDefinition listComplete;

	// Token: 0x04001ED0 RID: 7888
	public SoundDefinition itemComplete;

	// Token: 0x04001ED1 RID: 7889
	public SoundDefinition popup;

	// Token: 0x04001ED2 RID: 7890
	private AchievementTodo[] items;

	// Token: 0x04001ED3 RID: 7891
	private Canvas canvas;

	// Token: 0x04001ED4 RID: 7892
	public Text titleText;

	// Token: 0x06002279 RID: 8825 RVA: 0x0001B6A5 File Offset: 0x000198A5
	public void OnEnable()
	{
		this.items = base.GetComponentsInChildren<AchievementTodo>(true);
		this.canvas = base.GetComponentInParent<Canvas>();
		this.Reset();
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x0001B6C6 File Offset: 0x000198C6
	private void ClientConnected()
	{
		this.Reset();
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x0001B6CE File Offset: 0x000198CE
	public void Reset()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.Think());
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x0001B6E3 File Offset: 0x000198E3
	private IEnumerator Think()
	{
		this.Hide();
		do
		{
			yield return CoroutineEx.waitForSecondsRealtime(3f);
		}
		while (LocalPlayer.Entity == null || !LocalPlayer.Entity.IsAlive() || !GameInfo.HasAchievements);
		this.Initialize();
		yield break;
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000B8D28 File Offset: 0x000B6F28
	private void Hide()
	{
		this.canvas.enabled = false;
		base.GetComponent<CanvasGroup>().alpha = 0f;
		(base.transform as RectTransform).anchoredPosition = new Vector3(-256f, 0f);
		AchievementTodo[] array = this.items;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000B8D98 File Offset: 0x000B6F98
	private void Show()
	{
		this.canvas.enabled = true;
		LeanTween.move(base.transform as RectTransform, new Vector2(0f, 0f), 0.4f).setEaseInOutCubic().setDelay(0.2f);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.4f).setDelay(0.2f);
		SoundManager.PlayOneshot(this.popup, null, true, default(Vector3));
	}

	// Token: 0x0600227F RID: 8831 RVA: 0x000B8E24 File Offset: 0x000B7024
	private void Initialize()
	{
		this.Hide();
		foreach (AchievementGroup achievementGroup in AchievementGroup.All)
		{
			if (!achievementGroup.Unlocked)
			{
				this.SwitchToGroup(achievementGroup);
				break;
			}
		}
		if (this.items[0].gameObject.activeSelf)
		{
			this.Show();
		}
	}

	// Token: 0x06002280 RID: 8832 RVA: 0x000B8E7C File Offset: 0x000B707C
	private void SwitchToGroup(AchievementGroup group)
	{
		if (group.groupTitle.translated == "")
		{
			this.titleText.text = "TASK LIST";
		}
		else
		{
			this.titleText.text = group.groupTitle.translated;
		}
		for (int i = 0; i < this.items.Length; i++)
		{
			if (i < group.Items.Length)
			{
				this.items[i].Init(group.Items[i]);
			}
			else
			{
				this.items[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000B8F10 File Offset: 0x000B7110
	public void OnItemComplete()
	{
		SoundManager.PlayOneshot(this.itemComplete, null, true, default(Vector3));
		base.Invoke(new Action(this.CheckForListComplete), 2f);
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000B8F4C File Offset: 0x000B714C
	public void CheckForListComplete()
	{
		foreach (AchievementTodo achievementTodo in this.items)
		{
			if (achievementTodo.gameObject.activeSelf && !achievementTodo.State)
			{
				return;
			}
		}
		LeanTween.scale(base.transform as RectTransform, Vector3.one * 1.5f, 0.7f).setEase(TweenMode.Punch).setDelay(0.2f);
		LeanTween.move(base.transform as RectTransform, new Vector2(-256f, 0f), 1f).setEaseInOutCubic().setDelay(2f);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 1f).setDelay(2f);
		SoundManager.PlayOneshot(this.listComplete, null, true, default(Vector3));
		base.Invoke(new Action(this.Initialize), 5f);
	}
}
