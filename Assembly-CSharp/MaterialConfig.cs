using System;
using UnityEngine;

// Token: 0x0200070E RID: 1806
[CreateAssetMenu(menuName = "Rust/Material Config")]
public class MaterialConfig : ScriptableObject
{
	// Token: 0x0400238D RID: 9101
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersFloat[] Floats;

	// Token: 0x0400238E RID: 9102
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersColor[] Colors;

	// Token: 0x0400238F RID: 9103
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersTexture[] Textures;

	// Token: 0x04002390 RID: 9104
	public string[] ScaleUV;

	// Token: 0x04002391 RID: 9105
	private MaterialPropertyBlock properties;

	// Token: 0x060027AF RID: 10159 RVA: 0x000CD6B4 File Offset: 0x000CB8B4
	public MaterialPropertyBlock GetMaterialPropertyBlock(Material mat, Vector3 pos, Vector3 scale)
	{
		if (this.properties == null)
		{
			this.properties = new MaterialPropertyBlock();
		}
		this.properties.Clear();
		for (int i = 0; i < this.Floats.Length; i++)
		{
			MaterialConfig.ShaderParametersFloat shaderParametersFloat = this.Floats[i];
			float a;
			float b;
			float t = shaderParametersFloat.FindBlendParameters(pos, out a, out b);
			this.properties.SetFloat(shaderParametersFloat.Name, Mathf.Lerp(a, b, t));
		}
		for (int j = 0; j < this.Colors.Length; j++)
		{
			MaterialConfig.ShaderParametersColor shaderParametersColor = this.Colors[j];
			Color a2;
			Color b2;
			float t2 = shaderParametersColor.FindBlendParameters(pos, out a2, out b2);
			this.properties.SetColor(shaderParametersColor.Name, Color.Lerp(a2, b2, t2));
		}
		for (int k = 0; k < this.Textures.Length; k++)
		{
			MaterialConfig.ShaderParametersTexture shaderParametersTexture = this.Textures[k];
			Texture texture = shaderParametersTexture.FindBlendParameters(pos);
			if (texture)
			{
				this.properties.SetTexture(shaderParametersTexture.Name, texture);
			}
		}
		for (int l = 0; l < this.ScaleUV.Length; l++)
		{
			Vector4 vector = mat.GetVector(this.ScaleUV[l]);
			vector = new Vector4(vector.x * scale.y, vector.y * scale.y, vector.z, vector.w);
			this.properties.SetVector(this.ScaleUV[l], vector);
		}
		return this.properties;
	}

	// Token: 0x0200070F RID: 1807
	public class ShaderParameters<T>
	{
		// Token: 0x04002392 RID: 9106
		public string Name;

		// Token: 0x04002393 RID: 9107
		public T Arid;

		// Token: 0x04002394 RID: 9108
		public T Temperate;

		// Token: 0x04002395 RID: 9109
		public T Tundra;

		// Token: 0x04002396 RID: 9110
		public T Arctic;

		// Token: 0x04002397 RID: 9111
		private T[] climates;

		// Token: 0x060027B1 RID: 10161 RVA: 0x000CD830 File Offset: 0x000CBA30
		public float FindBlendParameters(Vector3 pos, out T src, out T dst)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				src = this.Temperate;
				dst = this.Tundra;
				return 0f;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[]
				{
					this.Arid,
					this.Temperate,
					this.Tundra,
					this.Arctic
				};
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
			src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
			dst = this.climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
			return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
		}

		// Token: 0x060027B2 RID: 10162 RVA: 0x000CD910 File Offset: 0x000CBB10
		public T FindBlendParameters(Vector3 pos)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				return this.Temperate;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[]
				{
					this.Arid,
					this.Temperate,
					this.Tundra,
					this.Arctic
				};
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			return this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		}
	}

	// Token: 0x02000710 RID: 1808
	[Serializable]
	public class ShaderParametersFloat : MaterialConfig.ShaderParameters<float>
	{
	}

	// Token: 0x02000711 RID: 1809
	[Serializable]
	public class ShaderParametersColor : MaterialConfig.ShaderParameters<Color>
	{
	}

	// Token: 0x02000712 RID: 1810
	[Serializable]
	public class ShaderParametersTexture : MaterialConfig.ShaderParameters<Texture>
	{
	}
}
