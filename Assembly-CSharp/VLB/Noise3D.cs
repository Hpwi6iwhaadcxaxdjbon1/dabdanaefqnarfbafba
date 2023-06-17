using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020007D7 RID: 2007
	public static class Noise3D
	{
		// Token: 0x040027A6 RID: 10150
		private static bool ms_IsSupportedChecked;

		// Token: 0x040027A7 RID: 10151
		private static bool ms_IsSupported;

		// Token: 0x040027A8 RID: 10152
		private static Texture3D ms_NoiseTexture;

		// Token: 0x040027A9 RID: 10153
		private const HideFlags kHideFlags = HideFlags.HideAndDontSave;

		// Token: 0x040027AA RID: 10154
		private const int kMinShaderLevel = 35;

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06002BBC RID: 11196 RVA: 0x00021EAB File Offset: 0x000200AB
		public static bool isSupported
		{
			get
			{
				if (!Noise3D.ms_IsSupportedChecked)
				{
					Noise3D.ms_IsSupported = (SystemInfo.graphicsShaderLevel >= 35);
					if (!Noise3D.ms_IsSupported)
					{
						Debug.LogWarning(Noise3D.isNotSupportedString);
					}
					Noise3D.ms_IsSupportedChecked = true;
				}
				return Noise3D.ms_IsSupported;
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06002BBD RID: 11197 RVA: 0x00021EE1 File Offset: 0x000200E1
		public static bool isProperlyLoaded
		{
			get
			{
				return Noise3D.ms_NoiseTexture != null;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06002BBE RID: 11198 RVA: 0x00021EEE File Offset: 0x000200EE
		public static string isNotSupportedString
		{
			get
			{
				return string.Format("3D Noise requires higher shader capabilities (Shader Model 3.5 / OpenGL ES 3.0), which are not available on the current platform: graphicsShaderLevel (current/required) = {0} / {1}", SystemInfo.graphicsShaderLevel, 35);
			}
		}

		// Token: 0x06002BBF RID: 11199 RVA: 0x00021F0B File Offset: 0x0002010B
		[RuntimeInitializeOnLoadMethod]
		private static void OnStartUp()
		{
			Noise3D.LoadIfNeeded();
		}

		// Token: 0x06002BC0 RID: 11200 RVA: 0x000E0244 File Offset: 0x000DE444
		public static void LoadIfNeeded()
		{
			if (!Noise3D.isSupported)
			{
				return;
			}
			if (Noise3D.ms_NoiseTexture == null)
			{
				Noise3D.ms_NoiseTexture = Noise3D.LoadTexture3D(Config.Instance.noise3DData, Config.Instance.noise3DSize);
				if (Noise3D.ms_NoiseTexture)
				{
					Noise3D.ms_NoiseTexture.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			Shader.SetGlobalTexture("_VLB_NoiseTex3D", Noise3D.ms_NoiseTexture);
			Shader.SetGlobalVector("_VLB_NoiseGlobal", Config.Instance.globalNoiseParam);
		}

		// Token: 0x06002BC1 RID: 11201 RVA: 0x000E02C0 File Offset: 0x000DE4C0
		private static Texture3D LoadTexture3D(TextAsset textData, int size)
		{
			if (textData == null)
			{
				Debug.LogErrorFormat("Fail to open Noise 3D Data", Array.Empty<object>());
				return null;
			}
			byte[] bytes = textData.bytes;
			Debug.Assert(bytes != null);
			int num = Mathf.Max(0, size * size * size);
			if (bytes.Length != num)
			{
				Debug.LogErrorFormat("Noise 3D Data file has not the proper size {0}x{0}x{0}", new object[]
				{
					size
				});
				return null;
			}
			Texture3D texture3D = new Texture3D(size, size, size, TextureFormat.Alpha8, false);
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new Color32(0, 0, 0, bytes[i]);
			}
			texture3D.SetPixels(array);
			texture3D.Apply();
			return texture3D;
		}
	}
}
