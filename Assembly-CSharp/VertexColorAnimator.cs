using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007C2 RID: 1986
public class VertexColorAnimator : MonoBehaviour
{
	// Token: 0x04002705 RID: 9989
	public List<MeshHolder> animationMeshes;

	// Token: 0x04002706 RID: 9990
	public List<float> animationKeyframes;

	// Token: 0x04002707 RID: 9991
	public float timeScale = 2f;

	// Token: 0x04002708 RID: 9992
	public int mode;

	// Token: 0x04002709 RID: 9993
	private float elapsedTime;

	// Token: 0x06002B4E RID: 11086 RVA: 0x000219A3 File Offset: 0x0001FBA3
	public void initLists()
	{
		this.animationMeshes = new List<MeshHolder>();
		this.animationKeyframes = new List<float>();
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000DDCB8 File Offset: 0x000DBEB8
	public void addMesh(Mesh mesh, float atPosition)
	{
		MeshHolder meshHolder = new MeshHolder();
		meshHolder.setAnimationData(mesh);
		this.animationMeshes.Add(meshHolder);
		this.animationKeyframes.Add(atPosition);
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x000219BB File Offset: 0x0001FBBB
	private void Start()
	{
		this.elapsedTime = 0f;
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x000219C8 File Offset: 0x0001FBC8
	public void replaceKeyframe(int frameIndex, Mesh mesh)
	{
		this.animationMeshes[frameIndex].setAnimationData(mesh);
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x000219DC File Offset: 0x0001FBDC
	public void deleteKeyframe(int frameIndex)
	{
		this.animationMeshes.RemoveAt(frameIndex);
		this.animationKeyframes.RemoveAt(frameIndex);
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x000DDCEC File Offset: 0x000DBEEC
	public void scrobble(float scrobblePos)
	{
		if (this.animationMeshes.Count == 0)
		{
			return;
		}
		Color[] array = new Color[base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (scrobblePos >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num >= this.animationKeyframes.Count - 1)
		{
			base.GetComponent<VertexColorStream>().setColors(this.animationMeshes[num]._colors);
			return;
		}
		float num2 = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
		float num3 = this.animationKeyframes[num];
		float t = (scrobblePos - num3) / num2;
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], t);
		}
		base.GetComponent<VertexColorStream>().setColors(array);
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x000DDE0C File Offset: 0x000DC00C
	private void Update()
	{
		if (this.mode == 0)
		{
			this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
		}
		else if (this.mode == 1)
		{
			this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
			if (this.elapsedTime > 1f)
			{
				this.elapsedTime = 0f;
			}
		}
		else if (this.mode == 2)
		{
			if (Mathf.FloorToInt(Time.fixedTime / this.timeScale) % 2 == 0)
			{
				this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
			}
			else
			{
				this.elapsedTime -= Time.fixedDeltaTime / this.timeScale;
			}
		}
		Color[] array = new Color[base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (this.elapsedTime >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num < this.animationKeyframes.Count - 1)
		{
			float num2 = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
			float num3 = this.animationKeyframes[num];
			float t = (this.elapsedTime - num3) / num2;
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], t);
			}
		}
		else
		{
			array = this.animationMeshes[num]._colors;
		}
		base.GetComponent<VertexColorStream>().setColors(array);
	}
}
