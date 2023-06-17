using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006AA RID: 1706
public class UIParticle : BaseMonoBehaviour
{
	// Token: 0x04002201 RID: 8705
	public Vector2 LifeTime;

	// Token: 0x04002202 RID: 8706
	public Vector2 Gravity = new Vector2(1000f, 1000f);

	// Token: 0x04002203 RID: 8707
	public Vector2 InitialX;

	// Token: 0x04002204 RID: 8708
	public Vector2 InitialY;

	// Token: 0x04002205 RID: 8709
	public Vector2 InitialScale = Vector2.one;

	// Token: 0x04002206 RID: 8710
	public Vector2 InitialDelay;

	// Token: 0x04002207 RID: 8711
	public Vector2 ScaleVelocity;

	// Token: 0x04002208 RID: 8712
	public Gradient InitialColor;

	// Token: 0x04002209 RID: 8713
	private float lifetime;

	// Token: 0x0400220A RID: 8714
	private float gravity;

	// Token: 0x0400220B RID: 8715
	private Vector2 velocity;

	// Token: 0x0400220C RID: 8716
	private float scaleVelocity;

	// Token: 0x06002606 RID: 9734 RVA: 0x000C8044 File Offset: 0x000C6244
	public static void Add(UIParticle particleSource, RectTransform spawnPosition, RectTransform particleCanvas)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(particleSource.gameObject);
		gameObject.transform.SetParent(spawnPosition, false);
		gameObject.transform.localPosition = new Vector3(Random.Range(0f, spawnPosition.rect.width) - spawnPosition.rect.width * spawnPosition.pivot.x, Random.Range(0f, spawnPosition.rect.height) - spawnPosition.rect.height * spawnPosition.pivot.y, 0f);
		gameObject.transform.SetParent(particleCanvas, true);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000C8110 File Offset: 0x000C6310
	private void Start()
	{
		base.transform.localScale *= Random.Range(this.InitialScale.x, this.InitialScale.y);
		this.velocity.x = Random.Range(this.InitialX.x, this.InitialX.y);
		this.velocity.y = Random.Range(this.InitialY.x, this.InitialY.y);
		this.gravity = Random.Range(this.Gravity.x, this.Gravity.y);
		this.scaleVelocity = Random.Range(this.ScaleVelocity.x, this.ScaleVelocity.y);
		Image component = base.GetComponent<Image>();
		if (component)
		{
			component.color = this.InitialColor.Evaluate(Random.Range(0f, 1f));
		}
		this.lifetime = Random.Range(this.InitialDelay.x, this.InitialDelay.y) * -1f;
		if (this.lifetime < 0f)
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}
		base.Invoke(new Action(this.Die), Random.Range(this.LifeTime.x, this.LifeTime.y) + this.lifetime * -1f);
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x000C828C File Offset: 0x000C648C
	private void Update()
	{
		if (this.lifetime < 0f)
		{
			this.lifetime += Time.deltaTime;
			if (this.lifetime < 0f)
			{
				return;
			}
			base.GetComponent<CanvasGroup>().alpha = 1f;
		}
		else
		{
			this.lifetime += Time.deltaTime;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.localScale;
		this.velocity.y = this.velocity.y - this.gravity * Time.deltaTime;
		position.x += this.velocity.x * Time.deltaTime;
		position.y += this.velocity.y * Time.deltaTime;
		vector += Vector3.one * this.scaleVelocity * Time.deltaTime;
		if (vector.x <= 0f || vector.y <= 0f)
		{
			this.Die();
			return;
		}
		base.transform.position = position;
		base.transform.localScale = vector;
	}

	// Token: 0x06002609 RID: 9737 RVA: 0x00008E27 File Offset: 0x00007027
	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
