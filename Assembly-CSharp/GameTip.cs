using System;
using System.Collections;
using GameTips;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200062D RID: 1581
public class GameTip : SingletonComponent<GameTip>
{
	// Token: 0x04001F67 RID: 8039
	public CanvasGroup canvasGroup;

	// Token: 0x04001F68 RID: 8040
	public Text text;

	// Token: 0x04001F69 RID: 8041
	public static BaseTip[] Tips = new BaseTip[]
	{
		new HowToVoiceChat(),
		new TipCannotHarvest(),
		new TipEquipTorch(),
		new TipRads(),
		new TipRemoveRads(),
		new TipTooHot(),
		new TipTooCold(),
		new HowToWorldDrink(),
		new TipPlaceSleepingBag(),
		new HowToThrow(),
		new HowToUseBow(),
		new HowToRetrieveThrown(),
		new HowToHammerUpgrade(),
		new HowToOreMinigame(),
		new HowToTreeMinigame(),
		new TipNoBuild(),
		new HowToOpenBuildOptions(),
		new TipPlaceToolCupboard(),
		new TipFillToolCupboard(),
		new TipConsumeFood(),
		new HowToRegenWithComfort(),
		new HowToOpenInventory(),
		new HowToOpenCrafting(),
		new HowToOpenMap(),
		new TipHealAtCampfire(),
		new HowToExamineHeld()
	};

	// Token: 0x04001F6A RID: 8042
	[ClientVar(Saved = true)]
	public static bool showgametips = true;

	// Token: 0x06002348 RID: 9032 RVA: 0x0001BEB7 File Offset: 0x0001A0B7
	public void OnEnable()
	{
		this.canvasGroup.alpha = 0f;
		base.StartCoroutine(this.Think());
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x0001BED6 File Offset: 0x0001A0D6
	private void ClientConnected()
	{
		this.canvasGroup.alpha = 0f;
		base.StopAllCoroutines();
		base.StartCoroutine(this.Think());
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x0001BEFB File Offset: 0x0001A0FB
	public void Update()
	{
		if (this.canvasGroup.alpha > 0f)
		{
			this.text.SetAllDirty();
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x0001BF1A File Offset: 0x0001A11A
	private IEnumerator Think()
	{
		yield return CoroutineEx.waitForSecondsRealtime(10f);
		for (;;)
		{
			foreach (BaseTip tip in GameTip.Tips)
			{
				if (GameTip.showgametips && tip.ShouldShow)
				{
					this.text.text = tip.GetPhrase().translated;
					LeanTween.alphaCanvas(this.canvasGroup, 1f, 1f);
					yield return CoroutineEx.waitForSecondsRealtime(2f);
					while (GameTip.showgametips && tip.ShouldShow)
					{
						yield return CoroutineEx.waitForSecondsRealtime(1f);
					}
					LeanTween.alphaCanvas(this.canvasGroup, 0f, 2f);
					yield return CoroutineEx.waitForSecondsRealtime(2f);
				}
				tip = null;
			}
			BaseTip[] array = null;
			yield return CoroutineEx.waitForSecondsRealtime(2f);
		}
		yield break;
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000BB78C File Offset: 0x000B998C
	[ClientVar(AllowRunFromServer = true)]
	public static void ShowGameTip(string text)
	{
		SingletonComponent<GameTip>.Instance.StopAllCoroutines();
		SingletonComponent<GameTip>.Instance.text.text = text;
		SingletonComponent<GameTip>.Instance.text.SetLayoutDirty();
		LeanTween.alphaCanvas(SingletonComponent<GameTip>.Instance.canvasGroup, 1f, 1f);
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x0001BF29 File Offset: 0x0001A129
	[ClientVar(AllowRunFromServer = true)]
	public static void HideGameTip()
	{
		SingletonComponent<GameTip>.Instance.StopAllCoroutines();
		LeanTween.alphaCanvas(SingletonComponent<GameTip>.Instance.canvasGroup, 0f, 1f);
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x0001BF4F File Offset: 0x0001A14F
	[ClientVar]
	public static BaseTip[] ListGameTips()
	{
		return GameTip.Tips;
	}
}
