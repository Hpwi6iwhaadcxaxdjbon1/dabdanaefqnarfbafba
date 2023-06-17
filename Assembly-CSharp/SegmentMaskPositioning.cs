using System;
using UnityEngine;

// Token: 0x020006B3 RID: 1715
public class SegmentMaskPositioning : MonoBehaviour
{
	// Token: 0x04002251 RID: 8785
	public PlayerModel source;

	// Token: 0x04002252 RID: 8786
	public GameObject headMask;

	// Token: 0x04002253 RID: 8787
	public GameObject chestMask;

	// Token: 0x04002254 RID: 8788
	public GameObject legsMask;

	// Token: 0x04002255 RID: 8789
	public float xOffset = 0.75f;

	// Token: 0x06002629 RID: 9769 RVA: 0x000C9528 File Offset: 0x000C7728
	[ContextMenu("RefreshPositions")]
	public void Refresh()
	{
		Transform transform = this.source.FindBone("neck");
		float num = 0.8f;
		this.headMask.transform.localScale = new Vector3(0.1f, num, 0.1f);
		this.headMask.transform.localPosition = base.transform.InverseTransformPoint(transform.transform.position + new Vector3(0f, num * 0.5f, 0f));
		Transform transform2 = this.source.FindBone("l_hand");
		float num2 = Mathf.Abs(transform2.transform.position.y - transform.transform.position.y);
		this.chestMask.transform.localScale = new Vector3(0.1f, num2, 0.1f);
		this.chestMask.transform.localPosition = base.transform.InverseTransformPoint(transform2.transform.position + new Vector3(0f, num2 * 0.5f, 0f));
		ref Vector3 position = transform2.transform.position;
		Vector3 position2 = this.source.transform.position;
		position2.y -= 0.25f;
		float num3 = Mathf.Abs(position.y - position2.y);
		this.legsMask.transform.localScale = new Vector3(0.1f, num3, 0.1f);
		this.legsMask.transform.localPosition = base.transform.InverseTransformPoint(position2 + new Vector3(0f, num3 * 0.5f, 0f));
		this.headMask.transform.localPosition = new Vector3(this.xOffset, this.headMask.transform.localPosition.y, 0f);
		this.chestMask.transform.localPosition = new Vector3(this.xOffset, this.chestMask.transform.localPosition.y, 0f);
		this.legsMask.transform.localPosition = new Vector3(this.xOffset, this.legsMask.transform.localPosition.y, 0f);
		bool active = (PaperDollSegment.selectedAreas & HitArea.Head) > (HitArea)0;
		bool active2 = (PaperDollSegment.selectedAreas & HitArea.Chest) > (HitArea)0;
		bool active3 = (PaperDollSegment.selectedAreas & HitArea.Leg) > (HitArea)0;
		this.headMask.SetActive(active);
		this.chestMask.SetActive(active2);
		this.legsMask.SetActive(active3);
	}
}
