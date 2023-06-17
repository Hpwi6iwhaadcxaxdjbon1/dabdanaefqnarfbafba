using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000316 RID: 790
public class PlayerNameTag : MonoBehaviour
{
	// Token: 0x040011F1 RID: 4593
	public CanvasGroup canvasGroup;

	// Token: 0x040011F2 RID: 4594
	public Text text;

	// Token: 0x040011F3 RID: 4595
	public Gradient color;

	// Token: 0x040011F4 RID: 4596
	public float minDistance = 3f;

	// Token: 0x040011F5 RID: 4597
	public float maxDistance = 10f;

	// Token: 0x040011F6 RID: 4598
	public Vector3 positionOffset;

	// Token: 0x040011F7 RID: 4599
	public Transform parentBone;

	// Token: 0x040011F8 RID: 4600
	private float distanceFromCamera = float.MaxValue;

	// Token: 0x040011F9 RID: 4601
	internal string lastName = "";

	// Token: 0x060014FB RID: 5371 RVA: 0x000821EC File Offset: 0x000803EC
	public void Initialize(BasePlayer player, Transform parent)
	{
		if (player == null)
		{
			Debug.LogError("PlayerNameTag: player is NULL");
			return;
		}
		if (player.displayName == null)
		{
			Debug.LogError("PlayerNameTag: player displayName is NULL");
			return;
		}
		if (this.text == null)
		{
			Debug.LogError("PlayerNameTag: text is NULL");
			return;
		}
		this.parentBone = parent;
		this.UpdateFrom(player);
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x00082248 File Offset: 0x00080448
	public void UpdateFrom(BasePlayer player)
	{
		if (player.displayName == null || player.displayName.Length == 0)
		{
			return;
		}
		string text = player.displayName;
		if (text.Length >= 20)
		{
			text = text.Substring(0, 18) + "..";
		}
		if (text.Trim().Length < 2)
		{
			text = "Blaster :D";
		}
		if (this.text != null)
		{
			this.text.text = text;
		}
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x00011CFC File Offset: 0x0000FEFC
	private void LateUpdate()
	{
		if (this.parentBone == null)
		{
			return;
		}
		base.transform.localPosition = this.parentBone.position + this.positionOffset;
		this.UpdateNameColor();
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x000822C0 File Offset: 0x000804C0
	private void UpdateNameColor()
	{
		float num = Mathf.InverseLerp(this.maxDistance, this.minDistance, this.distanceFromCamera);
		Color color = this.color.Evaluate(0.5f);
		if (TOD_Sky.Instance != null)
		{
			color = this.color.Evaluate(TOD_Sky.Instance.Cycle.Hour / 24f);
		}
		color.a = 1f;
		this.canvasGroup.alpha = (nametags.enabled ? num : 0f);
		if (this.text != null)
		{
			this.text.color = color;
		}
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x00082364 File Offset: 0x00080564
	public void PositionUpdate(bool visible)
	{
		if (this.parentBone == null)
		{
			visible = false;
		}
		else
		{
			this.distanceFromCamera = MainCamera.Distance(this.parentBone.position);
		}
		base.gameObject.SetActive(visible && this.distanceFromCamera < this.maxDistance);
	}
}
