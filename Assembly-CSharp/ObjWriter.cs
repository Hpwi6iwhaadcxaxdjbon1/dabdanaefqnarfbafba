using System;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x02000751 RID: 1873
public static class ObjWriter
{
	// Token: 0x060028CA RID: 10442 RVA: 0x000D0F1C File Offset: 0x000CF11C
	public static string MeshToString(Mesh mesh)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(mesh.name).Append("\n");
		foreach (Vector3 vector in mesh.vertices)
		{
			stringBuilder.Append(string.Format("v {0} {1} {2}\n", -vector.x, vector.y, vector.z));
		}
		stringBuilder.Append("\n");
		foreach (Vector3 vector2 in mesh.normals)
		{
			stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -vector2.x, vector2.y, vector2.z));
		}
		stringBuilder.Append("\n");
		Vector2[] uv = mesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			Vector3 vector3 = uv[i];
			stringBuilder.Append(string.Format("vt {0} {1}\n", vector3.x, vector3.y));
		}
		stringBuilder.Append("\n");
		int[] triangles = mesh.triangles;
		for (int j = 0; j < triangles.Length; j += 3)
		{
			int num = triangles[j] + 1;
			int num2 = triangles[j + 1] + 1;
			int num3 = triangles[j + 2] + 1;
			stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", num, num2, num3));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x000D10D0 File Offset: 0x000CF2D0
	public static void Write(Mesh mesh, string path)
	{
		using (StreamWriter streamWriter = new StreamWriter(path))
		{
			streamWriter.Write(ObjWriter.MeshToString(mesh));
		}
	}
}
