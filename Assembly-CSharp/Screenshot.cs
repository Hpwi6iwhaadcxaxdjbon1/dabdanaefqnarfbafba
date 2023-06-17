using System;
using System.Collections;
using System.IO;
using ConVar;
using UnityEngine;

// Token: 0x0200071F RID: 1823
public class Screenshot : MonoBehaviour
{
	// Token: 0x040023B8 RID: 9144
	public string screenshotPath;

	// Token: 0x040023B9 RID: 9145
	public int sizeMultiplier = 4;

	// Token: 0x040023BA RID: 9146
	private bool takingScreenshot;

	// Token: 0x060027E9 RID: 10217 RVA: 0x000CDF30 File Offset: 0x000CC130
	private void Update()
	{
		if (this.takingScreenshot)
		{
			return;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
		{
			base.StartCoroutine(this.SaveScreenshot("normal", 1, true, true));
			return;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F9))
		{
			base.StartCoroutine(this.SaveScreenshot("large", 4, false, true));
			return;
		}
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x0001F1FB File Offset: 0x0001D3FB
	private IEnumerator SaveScreenshot(string ext, int size, bool withHud, bool withoutHud)
	{
		this.takingScreenshot = true;
		string path = Application.dataPath + "/../" + this.screenshotPath;
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		bool oldHud = ConVar.Graphics.hud;
		bool oldNametags = nametags.enabled;
		bool oldDof = ConVar.Graphics.dof;
		int oldAA = Effects.antialiasing;
		bool oldSharpen = Effects.sharpen;
		if (size > 1)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, "graphics.dof 0", Array.Empty<object>());
			ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, "effects.aa 0", Array.Empty<object>());
			ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, "effects.sharpen 0", Array.Empty<object>());
		}
		ConVar.Graphics.branding = false;
		if (withHud)
		{
			ConVar.Graphics.hud = true;
			nametags.enabled = true;
			string name = string.Format("{0}/{1:yyyy-MM-dd-hhmmss}.{2}.png", path, DateTime.Now, ext);
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			Debug.Log("Saving " + name);
			ScreenCapture.CaptureScreenshot(name, size);
			name = null;
		}
		if (withoutHud)
		{
			ConVar.Graphics.hud = false;
			nametags.enabled = false;
			string name = string.Format("{0}/{1:yyyy-MM-dd-hhmmss}.{2}.nohud.png", path, DateTime.Now, ext);
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			Debug.Log("Saving " + name);
			ScreenCapture.CaptureScreenshot(name, size);
			name = null;
		}
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		ConVar.Graphics.hud = oldHud;
		nametags.enabled = oldNametags;
		ConVar.Graphics.branding = true;
		ConVar.Graphics.dof = oldDof;
		Effects.antialiasing = oldAA;
		Effects.sharpen = oldSharpen;
		ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, "graphics.dof " + oldDof.ToString(), Array.Empty<object>());
		ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, "effects.aa " + oldAA, Array.Empty<object>());
		ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, "effects.sharpen " + oldSharpen.ToString(), Array.Empty<object>());
		this.takingScreenshot = false;
		yield break;
	}
}
