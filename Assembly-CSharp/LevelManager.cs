using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020003B0 RID: 944
public static class LevelManager
{
	// Token: 0x04001477 RID: 5239
	public static string CurrentLevelName;

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x0600179E RID: 6046 RVA: 0x0008B0C8 File Offset: 0x000892C8
	public static bool isLoaded
	{
		get
		{
			return LevelManager.CurrentLevelName != null && !(LevelManager.CurrentLevelName == "") && !(LevelManager.CurrentLevelName == "UIScene") && !(LevelManager.CurrentLevelName == "Empty") && !(LevelManager.CurrentLevelName == "MenuBackground") && !(LevelManager.CurrentLevelName == "UIWorkshop");
		}
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x00013BB9 File Offset: 0x00011DB9
	public static bool IsValid(string strName)
	{
		return Application.CanStreamedLevelBeLoaded(strName);
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x00013BC1 File Offset: 0x00011DC1
	public static void LoadLevel(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		LoadingScreen.Show();
		SceneManager.LoadScene(strName, LoadSceneMode.Single);
		if (!keepLoadingScreenOpen)
		{
			LoadingScreen.Hide();
		}
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x00013BF1 File Offset: 0x00011DF1
	public static IEnumerator LoadLevelAsync(string strName, bool keepLoadingScreenOpen = true)
	{
		LevelManager.CurrentLevelName = strName;
		LoadingScreen.Show();
		yield return null;
		yield return SceneManager.LoadSceneAsync(strName, LoadSceneMode.Single);
		yield return null;
		yield return null;
		if (!keepLoadingScreenOpen)
		{
			LoadingScreen.Hide();
		}
		yield break;
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x00013C07 File Offset: 0x00011E07
	public static void UnloadLevel()
	{
		LevelManager.CurrentLevelName = null;
		LoadingScreen.Show();
		Application.LoadLevel("MenuBackground");
		LoadingScreen.Hide();
		MainMenuSystem.Show();
	}
}
