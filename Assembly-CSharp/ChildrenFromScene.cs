using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200060A RID: 1546
public class ChildrenFromScene : MonoBehaviour
{
	// Token: 0x04001EF7 RID: 7927
	public string SceneName;

	// Token: 0x04001EF8 RID: 7928
	public bool StartChildrenDisabled;

	// Token: 0x060022B0 RID: 8880 RVA: 0x0001B85B File Offset: 0x00019A5B
	private IEnumerator Start()
	{
		if (!SceneManager.GetSceneByName(this.SceneName).isLoaded)
		{
			yield return SceneManager.LoadSceneAsync(this.SceneName, LoadSceneMode.Additive);
		}
		Scene sceneByName = SceneManager.GetSceneByName(this.SceneName);
		foreach (GameObject gameObject in sceneByName.GetRootGameObjects())
		{
			gameObject.transform.SetParent(base.transform, false);
			gameObject.Identity();
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform)
			{
				rectTransform.pivot = Vector2.zero;
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.one;
			}
			SingletonComponent[] componentsInChildren = gameObject.GetComponentsInChildren<SingletonComponent>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].Setup();
			}
			if (this.StartChildrenDisabled)
			{
				gameObject.SetActive(false);
			}
		}
		SceneManager.UnloadSceneAsync(sceneByName);
		yield break;
	}
}
