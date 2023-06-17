using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020007D9 RID: 2009
	public static class Utils
	{
		// Token: 0x040027AF RID: 10159
		private static Utils.FloatPackingPrecision ms_FloatPackingPrecision;

		// Token: 0x040027B0 RID: 10160
		private const int kFloatPackingHighMinShaderLevel = 35;

		// Token: 0x06002BC5 RID: 11205 RVA: 0x00021F2C File Offset: 0x0002012C
		public static string GetPath(Transform current)
		{
			if (current.parent == null)
			{
				return "/" + current.name;
			}
			return Utils.GetPath(current.parent) + "/" + current.name;
		}

		// Token: 0x06002BC6 RID: 11206 RVA: 0x00021F68 File Offset: 0x00020168
		public static T NewWithComponent<T>(string name) where T : Component
		{
			return new GameObject(name, new Type[]
			{
				typeof(T)
			}).GetComponent<T>();
		}

		// Token: 0x06002BC7 RID: 11207 RVA: 0x000E041C File Offset: 0x000DE61C
		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			T t = self.GetComponent<T>();
			if (t == null)
			{
				t = self.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x06002BC8 RID: 11208 RVA: 0x00021F88 File Offset: 0x00020188
		public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
		{
			return self.gameObject.GetOrAddComponent<T>();
		}

		// Token: 0x06002BC9 RID: 11209 RVA: 0x00021F95 File Offset: 0x00020195
		public static bool HasFlag(this Enum mask, Enum flags)
		{
			return ((int)mask & (int)flags) == (int)flags;
		}

		// Token: 0x06002BCA RID: 11210 RVA: 0x00021FAC File Offset: 0x000201AC
		public static Vector2 xy(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.y);
		}

		// Token: 0x06002BCB RID: 11211 RVA: 0x00021FBF File Offset: 0x000201BF
		public static Vector2 xz(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.z);
		}

		// Token: 0x06002BCC RID: 11212 RVA: 0x00021FD2 File Offset: 0x000201D2
		public static Vector2 yz(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.z);
		}

		// Token: 0x06002BCD RID: 11213 RVA: 0x00021FE5 File Offset: 0x000201E5
		public static Vector2 yx(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.x);
		}

		// Token: 0x06002BCE RID: 11214 RVA: 0x00021FF8 File Offset: 0x000201F8
		public static Vector2 zx(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.x);
		}

		// Token: 0x06002BCF RID: 11215 RVA: 0x0002200B File Offset: 0x0002020B
		public static Vector2 zy(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.y);
		}

		// Token: 0x06002BD0 RID: 11216 RVA: 0x0002201E File Offset: 0x0002021E
		public static float GetVolumeCubic(this Bounds self)
		{
			return self.size.x * self.size.y * self.size.z;
		}

		// Token: 0x06002BD1 RID: 11217 RVA: 0x000E0448 File Offset: 0x000DE648
		public static float GetMaxArea2D(this Bounds self)
		{
			return Mathf.Max(Mathf.Max(self.size.x * self.size.y, self.size.y * self.size.z), self.size.x * self.size.z);
		}

		// Token: 0x06002BD2 RID: 11218 RVA: 0x00022046 File Offset: 0x00020246
		public static Color Opaque(this Color self)
		{
			return new Color(self.r, self.g, self.b, 1f);
		}

		// Token: 0x06002BD3 RID: 11219 RVA: 0x000E04AC File Offset: 0x000DE6AC
		public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
		{
			Vector3 vector = Vector3.Cross(normal, (Mathf.Abs(Vector3.Dot(normal, Vector3.forward)) < 0.999f) ? Vector3.forward : Vector3.up).normalized * size;
			Vector3 vector2 = position + vector;
			Vector3 vector3 = position - vector;
			vector = Quaternion.AngleAxis(90f, normal) * vector;
			Vector3 vector4 = position + vector;
			Vector3 vector5 = position - vector;
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = color;
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector4, vector5);
			Gizmos.DrawLine(vector2, vector4);
			Gizmos.DrawLine(vector4, vector3);
			Gizmos.DrawLine(vector3, vector5);
			Gizmos.DrawLine(vector5, vector2);
		}

		// Token: 0x06002BD4 RID: 11220 RVA: 0x00022064 File Offset: 0x00020264
		public static Plane TranslateCustom(this Plane plane, Vector3 translation)
		{
			plane.distance += Vector3.Dot(translation.normalized, plane.normal) * translation.magnitude;
			return plane;
		}

		// Token: 0x06002BD5 RID: 11221 RVA: 0x000E0564 File Offset: 0x000DE764
		public static bool IsValid(this Plane plane)
		{
			return plane.normal.sqrMagnitude > 0.5f;
		}

		// Token: 0x06002BD6 RID: 11222 RVA: 0x000E0588 File Offset: 0x000DE788
		public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
		{
			Matrix4x4 result = default(Matrix4x4);
			for (int i = 0; i < 16; i++)
			{
				Color color = self.Evaluate(Mathf.Clamp01((float)i / 15f));
				result[i] = color.PackToFloat(floatPackingPrecision);
			}
			return result;
		}

		// Token: 0x06002BD7 RID: 11223 RVA: 0x000E05D0 File Offset: 0x000DE7D0
		public static Color[] SampleInArray(this Gradient self, int samplesCount)
		{
			Color[] array = new Color[samplesCount];
			for (int i = 0; i < samplesCount; i++)
			{
				array[i] = self.Evaluate(Mathf.Clamp01((float)i / (float)(samplesCount - 1)));
			}
			return array;
		}

		// Token: 0x06002BD8 RID: 11224 RVA: 0x00022090 File Offset: 0x00020290
		private static Vector4 Vector4_Floor(Vector4 vec)
		{
			return new Vector4(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z), Mathf.Floor(vec.w));
		}

		// Token: 0x06002BD9 RID: 11225 RVA: 0x000E060C File Offset: 0x000DE80C
		public static float PackToFloat(this Color color, int floatPackingPrecision)
		{
			Vector4 vector = Utils.Vector4_Floor(color * (float)(floatPackingPrecision - 1));
			return 0f + vector.x * (float)floatPackingPrecision * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.y * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.z * (float)floatPackingPrecision + vector.w;
		}

		// Token: 0x06002BDA RID: 11226 RVA: 0x000220C3 File Offset: 0x000202C3
		public static Utils.FloatPackingPrecision GetFloatPackingPrecision()
		{
			if (Utils.ms_FloatPackingPrecision == Utils.FloatPackingPrecision.Undef)
			{
				Utils.ms_FloatPackingPrecision = ((SystemInfo.graphicsShaderLevel >= 35) ? Utils.FloatPackingPrecision.High : Utils.FloatPackingPrecision.Low);
			}
			return Utils.ms_FloatPackingPrecision;
		}

		// Token: 0x06002BDB RID: 11227 RVA: 0x00002ECE File Offset: 0x000010CE
		public static void MarkCurrentSceneDirty()
		{
		}

		// Token: 0x020007DA RID: 2010
		public enum FloatPackingPrecision
		{
			// Token: 0x040027B2 RID: 10162
			High = 64,
			// Token: 0x040027B3 RID: 10163
			Low = 8,
			// Token: 0x040027B4 RID: 10164
			Undef = 0
		}
	}
}
