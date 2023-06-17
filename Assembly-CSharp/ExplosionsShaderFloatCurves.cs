using System;
using UnityEngine;

// Token: 0x020007A6 RID: 1958
public class ExplosionsShaderFloatCurves : MonoBehaviour
{
	// Token: 0x04002615 RID: 9749
	public string ShaderProperty = "_BumpAmt";

	// Token: 0x04002616 RID: 9750
	public int MaterialID;

	// Token: 0x04002617 RID: 9751
	public AnimationCurve FloatPropertyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04002618 RID: 9752
	public float GraphTimeMultiplier = 1f;

	// Token: 0x04002619 RID: 9753
	public float GraphScaleMultiplier = 1f;

	// Token: 0x0400261A RID: 9754
	private bool canUpdate;

	// Token: 0x0400261B RID: 9755
	private Material matInstance;

	// Token: 0x0400261C RID: 9756
	private int propertyID;

	// Token: 0x0400261D RID: 9757
	private float startTime;

	// Token: 0x06002A80 RID: 10880 RVA: 0x000D8D58 File Offset: 0x000D6F58
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
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000210D8 File Offset: 0x0001F2D8
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x000D8DD4 File Offset: 0x000D6FD4
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float value = this.FloatPropertyCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphScaleMultiplier;
			this.matInstance.SetFloat(this.propertyID, value);
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}
