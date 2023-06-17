using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200078F RID: 1935
internal class UVTextureAnimator : MonoBehaviour
{
	// Token: 0x0400258B RID: 9611
	public int Rows = 4;

	// Token: 0x0400258C RID: 9612
	public int Columns = 4;

	// Token: 0x0400258D RID: 9613
	public float Fps = 20f;

	// Token: 0x0400258E RID: 9614
	public int OffsetMat;

	// Token: 0x0400258F RID: 9615
	public bool IsLoop = true;

	// Token: 0x04002590 RID: 9616
	public float StartDelay;

	// Token: 0x04002591 RID: 9617
	private bool isInizialised;

	// Token: 0x04002592 RID: 9618
	private int index;

	// Token: 0x04002593 RID: 9619
	private int count;

	// Token: 0x04002594 RID: 9620
	private int allCount;

	// Token: 0x04002595 RID: 9621
	private float deltaFps;

	// Token: 0x04002596 RID: 9622
	private bool isVisible;

	// Token: 0x04002597 RID: 9623
	private bool isCorutineStarted;

	// Token: 0x04002598 RID: 9624
	private Renderer currentRenderer;

	// Token: 0x04002599 RID: 9625
	private Material instanceMaterial;

	// Token: 0x06002A03 RID: 10755 RVA: 0x00020936 File Offset: 0x0001EB36
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06002A04 RID: 10756 RVA: 0x000D6D7C File Offset: 0x000D4F7C
	private void InitDefaultVariables()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		if (this.currentRenderer == null)
		{
			throw new Exception("UvTextureAnimator can't get renderer");
		}
		if (!this.currentRenderer.enabled)
		{
			this.currentRenderer.enabled = true;
		}
		this.allCount = 0;
		this.deltaFps = 1f / this.Fps;
		this.count = this.Rows * this.Columns;
		this.index = this.Columns - 1;
		Vector3 zero = Vector3.zero;
		this.OffsetMat -= this.OffsetMat / this.count * this.count;
		Vector2 value = new Vector2(1f / (float)this.Columns, 1f / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", value);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x0002095E File Offset: 0x0001EB5E
	private void Play()
	{
		if (this.isCorutineStarted)
		{
			return;
		}
		if (this.StartDelay > 0.0001f)
		{
			base.Invoke("PlayDelay", this.StartDelay);
		}
		else
		{
			base.StartCoroutine(this.UpdateCorutine());
		}
		this.isCorutineStarted = true;
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x0002099D File Offset: 0x0001EB9D
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x000209AC File Offset: 0x0001EBAC
	private void OnEnable()
	{
		if (!this.isInizialised)
		{
			return;
		}
		this.InitDefaultVariables();
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x000209CA File Offset: 0x0001EBCA
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x000209EB File Offset: 0x0001EBEB
	private IEnumerator UpdateCorutine()
	{
		while (this.isVisible && (this.IsLoop || this.allCount != this.count))
		{
			this.UpdateCorutineFrame();
			if (!this.IsLoop && this.allCount == this.count)
			{
				break;
			}
			yield return new WaitForSeconds(this.deltaFps);
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x000D6E90 File Offset: 0x000D5090
	private void UpdateCorutineFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		Vector2 value = new Vector2((float)this.index / (float)this.Columns - (float)(this.index / this.Columns), 1f - (float)(this.index / this.Columns) / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", value);
		}
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x000209FA File Offset: 0x0001EBFA
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}
}
