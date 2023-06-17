using System;
using UnityEngine;

// Token: 0x02000708 RID: 1800
public class LayerCullDistance : MonoBehaviour
{
	// Token: 0x04002364 RID: 9060
	public string Layer = "Default";

	// Token: 0x04002365 RID: 9061
	public float Distance = 1000f;

	// Token: 0x06002783 RID: 10115 RVA: 0x000CCFAC File Offset: 0x000CB1AC
	protected void OnEnable()
	{
		Camera component = base.GetComponent<Camera>();
		float[] layerCullDistances = component.layerCullDistances;
		layerCullDistances[LayerMask.NameToLayer(this.Layer)] = this.Distance;
		component.layerCullDistances = layerCullDistances;
	}
}
