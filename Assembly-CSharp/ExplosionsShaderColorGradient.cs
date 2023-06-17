using System;
using UnityEngine;

// Token: 0x020007A5 RID: 1957
public class ExplosionsShaderColorGradient : MonoBehaviour
{
	// Token: 0x0400260C RID: 9740
	public string ShaderProperty = "_TintColor";

	// Token: 0x0400260D RID: 9741
	public int MaterialID;

	// Token: 0x0400260E RID: 9742
	public Gradient Color = new Gradient();

	// Token: 0x0400260F RID: 9743
	public float TimeMultiplier = 1f;

	// Token: 0x04002610 RID: 9744
	private bool canUpdate;

	// Token: 0x04002611 RID: 9745
	private Material matInstance;

	// Token: 0x04002612 RID: 9746
	private int propertyID;

	// Token: 0x04002613 RID: 9747
	private float startTime;

	// Token: 0x04002614 RID: 9748
	private Color oldColor;

	// Token: 0x06002A7C RID: 10876 RVA: 0x000D8C60 File Offset: 0x000D6E60
	private void Start()
	{
		Material[] materials = base.GetComponent<Renderer>().materials;
		if (this.MaterialID >= materials.Length)
		{
			Debug.Log("ShaderColorGradient: Material ID more than shader materials count.");
		}
		this.matInstance = materials[this.MaterialID];
		if (!this.matInstance.HasProperty(this.ShaderProperty))
		{
			Debug.Log("ShaderColorGradient: Shader not have \"" + this.ShaderProperty + "\" property");
		}
		this.propertyID = Shader.PropertyToID(this.ShaderProperty);
		this.oldColor = this.matInstance.GetColor(this.propertyID);
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x0002109B File Offset: 0x0001F29B
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x000D8CF4 File Offset: 0x000D6EF4
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			Color a = this.Color.Evaluate(num / this.TimeMultiplier);
			this.matInstance.SetColor(this.propertyID, a * this.oldColor);
		}
		if (num >= this.TimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}
