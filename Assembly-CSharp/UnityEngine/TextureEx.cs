using System;

namespace UnityEngine
{
	// Token: 0x02000838 RID: 2104
	public static class TextureEx
	{
		// Token: 0x040028F4 RID: 10484
		private static Color32[] buffer = new Color32[8192];

		// Token: 0x06002D8E RID: 11662 RVA: 0x000E4DD4 File Offset: 0x000E2FD4
		public static void Clear(this Texture2D tex, Color32 color)
		{
			if (tex.width > TextureEx.buffer.Length)
			{
				Debug.LogError("Trying to clear texture that is too big: " + tex.width);
				return;
			}
			for (int i = 0; i < tex.width; i++)
			{
				TextureEx.buffer[i] = color;
			}
			for (int j = 0; j < tex.height; j++)
			{
				tex.SetPixels32(0, j, tex.width, 1, TextureEx.buffer);
			}
			tex.Apply();
		}

		// Token: 0x06002D8F RID: 11663 RVA: 0x000E4E54 File Offset: 0x000E3054
		public static int GetSizeInBytes(this Texture texture)
		{
			int num = texture.width;
			int num2 = texture.height;
			if (texture is Texture2D)
			{
				Texture2D texture2D = texture as Texture2D;
				int bitsPerPixel = TextureEx.GetBitsPerPixel(texture2D.format);
				int mipmapCount = texture2D.mipmapCount;
				int i = 1;
				int num3 = 0;
				while (i <= mipmapCount)
				{
					num3 += num * num2 * bitsPerPixel / 8;
					num /= 2;
					num2 /= 2;
					i++;
				}
				return num3;
			}
			if (texture is Texture2DArray)
			{
				Texture2DArray texture2DArray = texture as Texture2DArray;
				int bitsPerPixel2 = TextureEx.GetBitsPerPixel(texture2DArray.format);
				int num4 = 10;
				int j = 1;
				int num5 = 0;
				int depth = texture2DArray.depth;
				while (j <= num4)
				{
					num5 += num * num2 * bitsPerPixel2 / 8;
					num /= 2;
					num2 /= 2;
					j++;
				}
				return num5 * depth;
			}
			if (texture is Cubemap)
			{
				int bitsPerPixel3 = TextureEx.GetBitsPerPixel((texture as Cubemap).format);
				int num6 = num * num2 * bitsPerPixel3 / 8;
				int num7 = 6;
				return num6 * num7;
			}
			return 0;
		}

		// Token: 0x06002D90 RID: 11664 RVA: 0x000E4F38 File Offset: 0x000E3138
		public static int GetBitsPerPixel(TextureFormat format)
		{
			switch (format)
			{
			case TextureFormat.Alpha8:
				return 8;
			case TextureFormat.ARGB4444:
				return 16;
			case TextureFormat.RGB24:
				return 24;
			case TextureFormat.RGBA32:
				return 32;
			case TextureFormat.ARGB32:
				return 32;
			case (TextureFormat)6:
			case (TextureFormat)8:
			case TextureFormat.R16:
			case (TextureFormat)11:
				return 0;
			case TextureFormat.RGB565:
				return 16;
			case TextureFormat.DXT1:
				return 4;
			case TextureFormat.DXT5:
				break;
			case TextureFormat.RGBA4444:
				return 16;
			case TextureFormat.BGRA32:
				return 32;
			default:
				switch (format)
				{
				case TextureFormat.BC7:
					break;
				case TextureFormat.BC4:
				case TextureFormat.BC5:
				case TextureFormat.DXT1Crunched:
				case TextureFormat.DXT5Crunched:
					return 0;
				case TextureFormat.PVRTC_RGB2:
					return 2;
				case TextureFormat.PVRTC_RGBA2:
					return 2;
				case TextureFormat.PVRTC_RGB4:
					return 4;
				case TextureFormat.PVRTC_RGBA4:
					return 4;
				case TextureFormat.ETC_RGB4:
					return 4;
				default:
					if (format != TextureFormat.ETC2_RGBA8)
					{
						return 0;
					}
					return 8;
				}
				break;
			}
			return 8;
		}
	}
}
