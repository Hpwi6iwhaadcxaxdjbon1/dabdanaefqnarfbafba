using System;
using System.Collections.Generic;

namespace UnityEngine
{
	// Token: 0x02000821 RID: 2081
	public class DDraw : MonoBehaviour
	{
		// Token: 0x040028D3 RID: 10451
		public static GameObject store;

		// Token: 0x040028D4 RID: 10452
		public static DDraw singleton;

		// Token: 0x040028D5 RID: 10453
		public List<DDraw.BaseObject> list = new List<DDraw.BaseObject>();

		// Token: 0x040028D6 RID: 10454
		internal static int AutoYPosition;

		// Token: 0x040028D7 RID: 10455
		internal static float LastAutoY;

		// Token: 0x040028D8 RID: 10456
		private static Material lineMaterial;

		// Token: 0x040028D9 RID: 10457
		internal static GUISkin _skin;

		// Token: 0x06002D3C RID: 11580 RVA: 0x000E34E4 File Offset: 0x000E16E4
		public static DDraw Get()
		{
			if (MainCamera.mainCamera == null)
			{
				return null;
			}
			if (DDraw.singleton && DDraw.singleton.gameObject != MainCamera.mainCamera.gameObject)
			{
				DDraw.singleton = null;
			}
			if (!DDraw.singleton)
			{
				DDraw.singleton = Object.FindObjectOfType<DDraw>();
				if (!DDraw.singleton)
				{
					GameObject gameObject = MainCamera.mainCamera.gameObject;
					if (!gameObject)
					{
						return null;
					}
					DDraw.singleton = gameObject.AddComponent<DDraw>();
				}
			}
			return DDraw.singleton;
		}

		// Token: 0x06002D3D RID: 11581 RVA: 0x000E3574 File Offset: 0x000E1774
		public static void Sphere(Vector3 vPos, float fRadius, Color color, float fDuration = 0.5f, bool distanceFade = true)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.SphereObj sphereObj = new DDraw.SphereObj();
			sphereObj.position = vPos;
			sphereObj.transform = Matrix4x4.TRS(vPos, Quaternion.identity, Vector3.one * fRadius * 2f);
			sphereObj.color = color;
			sphereObj.distanceFade = distanceFade;
			sphereObj.end = Time.time + fDuration;
			sphereObj.start = Time.time;
			ddraw.list.Add(sphereObj);
		}

		// Token: 0x06002D3E RID: 11582 RVA: 0x000E35F8 File Offset: 0x000E17F8
		public static void SphereGizmo(Vector3 vPos, float fRadius, Color color, float fDuration = 0.5f, bool distanceFade = true, bool ztest = true)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.SphereGizmoObj sphereGizmoObj = new DDraw.SphereGizmoObj();
			sphereGizmoObj.position = vPos;
			sphereGizmoObj.transform = Matrix4x4.TRS(vPos, Quaternion.identity, Vector3.one * fRadius * 2f);
			sphereGizmoObj.color = color;
			sphereGizmoObj.distanceFade = distanceFade;
			sphereGizmoObj.end = Time.time + fDuration;
			sphereGizmoObj.start = Time.time;
			sphereGizmoObj.ztest = ztest;
			ddraw.list.Add(sphereGizmoObj);
		}

		// Token: 0x06002D3F RID: 11583 RVA: 0x000E3684 File Offset: 0x000E1884
		public static void Line(Vector3 vPos, Vector3 vPosB, Color color, float fDuration = 0.5f, bool distanceFade = true, bool ztest = true)
		{
			Vector3 vector = vPosB - vPos;
			DDraw.Line(vPos, vector.normalized, vector.magnitude, color, fDuration, distanceFade, ztest);
		}

		// Token: 0x06002D40 RID: 11584 RVA: 0x000E36B4 File Offset: 0x000E18B4
		public static void Line(Vector3 vPos, Vector3 vNormal, float magnitude, Color color, float fDuration = 0.5f, bool distanceFade = true, bool ztest = true)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			Quaternion q = Quaternion.identity;
			if (vNormal.magnitude > 0.01f)
			{
				q = Quaternion.LookRotation(vNormal);
			}
			DDraw.LineObj lineObj = new DDraw.LineObj();
			lineObj.position = vPos;
			lineObj.transform = Matrix4x4.TRS(vPos, q, new Vector3(1f, 1f, magnitude));
			lineObj.color = color;
			lineObj.distanceFade = distanceFade;
			lineObj.ztest = ztest;
			lineObj.end = Time.time + fDuration;
			lineObj.start = Time.time;
			ddraw.list.Add(lineObj);
		}

		// Token: 0x06002D41 RID: 11585 RVA: 0x000E3754 File Offset: 0x000E1954
		public static void Arrow(Vector3 vPos, Vector3 vPosB, float headSize, Color color, float fDuration = 0.5f)
		{
			Vector3 vector = vPosB - vPos;
			if (Mathf.Approximately(vector.magnitude, 0f))
			{
				return;
			}
			DDraw.Arrow(vPos, vector.normalized, vector.magnitude, headSize, color, fDuration);
		}

		// Token: 0x06002D42 RID: 11586 RVA: 0x000E3798 File Offset: 0x000E1998
		public static void ArrowPadded(Vector3 vPos, Vector3 vPosB, float padding, float headSize, Color color, float fDuration = 0.5f)
		{
			Vector3 normalized = (vPosB - vPos).normalized;
			DDraw.Arrow(vPos + normalized * padding, vPosB - normalized * padding, headSize, color, fDuration);
		}

		// Token: 0x06002D43 RID: 11587 RVA: 0x000E37DC File Offset: 0x000E19DC
		public static void Arrow(Vector3 vPos, Vector3 vNormal, float magnitude, float headSize, Color color, float fDuration = 0.5f)
		{
			if (Vector3.zero == vNormal)
			{
				return;
			}
			DDraw.Line(vPos, vNormal, magnitude - headSize, color, fDuration, true, true);
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.ArrowHead arrowHead = new DDraw.ArrowHead();
			arrowHead.position = vPos;
			arrowHead.transform = Matrix4x4.TRS(vPos + vNormal * (magnitude - headSize), Quaternion.LookRotation(vNormal), Vector3.one * headSize);
			arrowHead.color = color;
			arrowHead.end = Time.time + fDuration;
			arrowHead.start = Time.time;
			ddraw.list.Add(arrowHead);
		}

		// Token: 0x06002D44 RID: 11588 RVA: 0x000E387C File Offset: 0x000E1A7C
		public static void Bounds(Bounds bnds, Color color, float fDuration = 0.5f, bool distanceFade = true, bool ztest = false)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.CubeObj cubeObj = new DDraw.CubeObj();
			cubeObj.position = bnds.center;
			cubeObj.transform = Matrix4x4.TRS(bnds.center, Quaternion.identity, bnds.size);
			cubeObj.color = color;
			cubeObj.distanceFade = distanceFade;
			cubeObj.ztest = ztest;
			cubeObj.end = Time.time + fDuration;
			cubeObj.start = Time.time;
			ddraw.list.Add(cubeObj);
		}

		// Token: 0x06002D45 RID: 11589 RVA: 0x000E3904 File Offset: 0x000E1B04
		public static void Bounds(Matrix4x4 matrix, Bounds bnds, Color color, float fDuration = 0.5f)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.CubeObj cubeObj = new DDraw.CubeObj();
			cubeObj.position = bnds.center;
			cubeObj.transform = matrix * Matrix4x4.TRS(bnds.center, Quaternion.identity, bnds.size);
			cubeObj.color = color;
			cubeObj.end = Time.time + fDuration;
			cubeObj.start = Time.time;
			ddraw.list.Add(cubeObj);
		}

		// Token: 0x06002D46 RID: 11590 RVA: 0x000E3984 File Offset: 0x000E1B84
		public static void Box(Vector3 vPos, float fSize, Color color, float fDuration = 1f, bool distanceFade = true)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.CubeObj cubeObj = new DDraw.CubeObj();
			cubeObj.position = vPos;
			cubeObj.transform = Matrix4x4.TRS(vPos, Quaternion.identity, Vector3.one * fSize);
			cubeObj.color = color;
			cubeObj.distanceFade = distanceFade;
			cubeObj.end = Time.time + fDuration;
			cubeObj.start = Time.time;
			ddraw.list.Add(cubeObj);
		}

		// Token: 0x06002D47 RID: 11591 RVA: 0x000E39FC File Offset: 0x000E1BFC
		public static void Box(Vector3 vPos, Quaternion rot, Vector3 size, Color color, float fDuration = 1f)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.CubeObj cubeObj = new DDraw.CubeObj();
			cubeObj.position = vPos;
			cubeObj.transform = Matrix4x4.TRS(vPos, rot, size);
			cubeObj.color = color;
			cubeObj.end = Time.time + fDuration;
			cubeObj.start = Time.time;
			ddraw.list.Add(cubeObj);
		}

		// Token: 0x06002D48 RID: 11592 RVA: 0x00023413 File Offset: 0x00021613
		public static void Box(Vector3 vPos)
		{
			DDraw.Box(vPos, 0.1f, Color.white, 1f, true);
		}

		// Token: 0x06002D49 RID: 11593 RVA: 0x000E3A60 File Offset: 0x000E1C60
		public static void Box(Matrix4x4 matrix, Color color, float fDuration = 1f)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.CubeObj cubeObj = new DDraw.CubeObj();
			cubeObj.position = matrix.MultiplyPoint3x4(Vector3.zero);
			cubeObj.transform = matrix;
			cubeObj.color = color;
			cubeObj.end = Time.time + fDuration;
			cubeObj.start = Time.time;
			ddraw.list.Add(cubeObj);
		}

		// Token: 0x06002D4A RID: 11594 RVA: 0x000E3AC8 File Offset: 0x000E1CC8
		public static void Text(string text, Vector3 vPos, Color color, float fDuration = 0.5f)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.TextObj textObj = new DDraw.TextObj();
			textObj.text = text;
			textObj.position = vPos;
			textObj.color = color;
			textObj.end = Time.time + fDuration;
			textObj.start = Time.time;
			ddraw.list.Add(textObj);
		}

		// Token: 0x06002D4B RID: 11595 RVA: 0x000E3B24 File Offset: 0x000E1D24
		public static void ScreenText(string text, int x, int y, Color color, float fDuration = 0.5f)
		{
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.ScreenTextObj screenTextObj = new DDraw.ScreenTextObj();
			screenTextObj.text = text;
			screenTextObj.x = x;
			screenTextObj.y = y;
			screenTextObj.color = color;
			screenTextObj.end = Time.time + fDuration;
			screenTextObj.start = Time.time;
			ddraw.list.Add(screenTextObj);
		}

		// Token: 0x06002D4C RID: 11596 RVA: 0x000E3B88 File Offset: 0x000E1D88
		public static void ScreenText(string text, int x, Color color, float fDuration = 0.5f)
		{
			if (DDraw.LastAutoY <= Time.time)
			{
				DDraw.AutoYPosition = 0;
			}
			DDraw.LastAutoY = Mathf.Max(DDraw.LastAutoY, Time.time + fDuration);
			DDraw ddraw = DDraw.Get();
			if (!ddraw)
			{
				return;
			}
			DDraw.ScreenTextObj screenTextObj = new DDraw.ScreenTextObj();
			screenTextObj.text = text;
			screenTextObj.x = x;
			screenTextObj.y = DDraw.AutoYPosition;
			screenTextObj.color = color;
			screenTextObj.end = Time.time + fDuration;
			screenTextObj.start = Time.time;
			ddraw.list.Add(screenTextObj);
			DDraw.AutoYPosition++;
		}

		// Token: 0x06002D4D RID: 11597 RVA: 0x0002342B File Offset: 0x0002162B
		private void Awake()
		{
			DDraw.singleton = this;
		}

		// Token: 0x06002D4E RID: 11598 RVA: 0x00023433 File Offset: 0x00021633
		private void OnPreRender()
		{
			if (this.list.Count == 0)
			{
				return;
			}
			this.list.RemoveAll((DDraw.BaseObject o) => o.end < Time.time);
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x000E3C24 File Offset: 0x000E1E24
		private void OnPostRender()
		{
			if (this.list.Count == 0)
			{
				return;
			}
			DDraw.CreateLineMaterial();
			GL.PushMatrix();
			foreach (DDraw.BaseObject baseObject in this.list)
			{
				baseObject.delta = Mathf.InverseLerp(baseObject.start, baseObject.end, Time.time);
				baseObject.Render();
			}
			GL.PopMatrix();
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x000E3CB0 File Offset: 0x000E1EB0
		private void OnGUI()
		{
			if (this.list.Count == 0)
			{
				return;
			}
			GUI.skin.label.alignment = 4;
			GUI.skin.label.fontSize = 10;
			GUI.contentColor = Color.white;
			foreach (DDraw.BaseObject baseObject in this.list)
			{
				baseObject.DrawGUI();
			}
		}

		// Token: 0x06002D51 RID: 11601 RVA: 0x0002346E File Offset: 0x0002166E
		private static void CreateLineMaterial()
		{
			if (DDraw.lineMaterial)
			{
				return;
			}
			DDraw.lineMaterial = new Material(Shader.Find("Hidden/DDraw/Colored"));
			DDraw.lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			DDraw.lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06002D52 RID: 11602 RVA: 0x000234AE File Offset: 0x000216AE
		internal static GUISkin skin
		{
			get
			{
				if (DDraw._skin == null)
				{
					DDraw._skin = Resources.Load<GUISkin>("DebugSkin");
				}
				return DDraw._skin;
			}
		}

		// Token: 0x02000822 RID: 2082
		public class BaseObject
		{
			// Token: 0x040028DA RID: 10458
			public Vector3 position;

			// Token: 0x040028DB RID: 10459
			public Matrix4x4 transform;

			// Token: 0x040028DC RID: 10460
			public Color color;

			// Token: 0x040028DD RID: 10461
			public float end;

			// Token: 0x040028DE RID: 10462
			public float start;

			// Token: 0x040028DF RID: 10463
			public float delta;

			// Token: 0x040028E0 RID: 10464
			public bool distanceFade = true;

			// Token: 0x040028E1 RID: 10465
			public bool ztest = true;

			// Token: 0x040028E2 RID: 10466
			public Stack<Matrix4x4> matrixStack = new Stack<Matrix4x4>();

			// Token: 0x040028E3 RID: 10467
			private static Vector3[] vDisc = new Vector3[]
			{
				new Vector3(0.191f, 0.462f, 0f),
				new Vector3(0.354f, 0.354f, 0f),
				new Vector3(0.462f, 0.191f, 0f),
				new Vector3(0.5f, 0f, 0f),
				new Vector3(0.462f, -0.191f, 0f),
				new Vector3(0.354f, -0.354f, 0f),
				new Vector3(0.191f, -0.462f, 0f),
				new Vector3(0f, -0.5f, 0f),
				new Vector3(-0.191f, -0.462f, 0f),
				new Vector3(-0.354f, -0.354f, 0f),
				new Vector3(-0.462f, -0.191f, 0f),
				new Vector3(-0.5f, 0f, 0f),
				new Vector3(-0.462f, 0.191f, 0f),
				new Vector3(-0.354f, 0.354f, 0f),
				new Vector3(-0.191f, 0.462f, 0f),
				new Vector3(0f, 0.5f, 0f)
			};

			// Token: 0x040028E4 RID: 10468
			private static Vector3[] vPlane = new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, 0f),
				new Vector3(0.5f, -0.5f, 0f),
				new Vector3(0.5f, 0.5f, 0f),
				new Vector3(-0.5f, 0.5f, 0f)
			};

			// Token: 0x06002D55 RID: 11605 RVA: 0x00002ECE File Offset: 0x000010CE
			public virtual void Draw()
			{
			}

			// Token: 0x06002D56 RID: 11606 RVA: 0x00002ECE File Offset: 0x000010CE
			public virtual void DrawGUI()
			{
			}

			// Token: 0x06002D57 RID: 11607 RVA: 0x000E3D3C File Offset: 0x000E1F3C
			public virtual void Render()
			{
				this.matrixStack.Push(this.transform);
				GL.PushMatrix();
				GL.MultMatrix(this.transform);
				DDraw.lineMaterial.SetPass(this.ztest ? 0 : 1);
				this.Draw();
				GL.PopMatrix();
			}

			// Token: 0x06002D58 RID: 11608 RVA: 0x000E3D8C File Offset: 0x000E1F8C
			public void PushMatrix(Matrix4x4 mat)
			{
				Matrix4x4 matrix4x = this.matrixStack.Peek() * mat;
				this.matrixStack.Push(matrix4x);
				GL.PushMatrix();
				GL.MultMatrix(matrix4x);
				DDraw.lineMaterial.SetPass(this.ztest ? 0 : 1);
			}

			// Token: 0x06002D59 RID: 11609 RVA: 0x000234E4 File Offset: 0x000216E4
			public void PopMatrix()
			{
				this.matrixStack.Pop();
				GL.PopMatrix();
			}

			// Token: 0x06002D5A RID: 11610 RVA: 0x000E3DDC File Offset: 0x000E1FDC
			public void DrawDisc()
			{
				this.Begin(1);
				for (int i = 1; i < DDraw.BaseObject.vDisc.Length; i++)
				{
					GL.Vertex(DDraw.BaseObject.vDisc[i - 1]);
					GL.Vertex(DDraw.BaseObject.vDisc[i]);
				}
				GL.Vertex(DDraw.BaseObject.vDisc[0]);
				GL.Vertex(DDraw.BaseObject.vDisc[DDraw.BaseObject.vDisc.Length - 1]);
				GL.End();
			}

			// Token: 0x06002D5B RID: 11611 RVA: 0x000E3E54 File Offset: 0x000E2054
			public void DrawPlane()
			{
				this.Begin(1);
				for (int i = 1; i < DDraw.BaseObject.vPlane.Length; i++)
				{
					GL.Vertex(DDraw.BaseObject.vDisc[i - 1]);
					GL.Vertex(DDraw.BaseObject.vDisc[i]);
				}
				GL.Vertex(DDraw.BaseObject.vDisc[0]);
				GL.Vertex(DDraw.BaseObject.vDisc[DDraw.BaseObject.vDisc.Length - 1]);
				GL.End();
			}

			// Token: 0x06002D5C RID: 11612 RVA: 0x000E3ECC File Offset: 0x000E20CC
			internal void Begin(int type)
			{
				GL.Begin(type);
				float value = Vector3.Distance(Camera.current.transform.position, this.position);
				Color c = this.color;
				if (this.distanceFade)
				{
					float num = Mathf.InverseLerp(30f, 0f, value);
					c = new Color(this.color.r * num, this.color.g * num, this.color.b * num, (1f - this.delta) * num);
				}
				GL.Color(c);
			}
		}

		// Token: 0x02000823 RID: 2083
		public class SphereObj : DDraw.BaseObject
		{
			// Token: 0x06002D5F RID: 11615 RVA: 0x000E41A4 File Offset: 0x000E23A4
			public override void Draw()
			{
				for (float num = 0f; num <= 1f; num += 0.0625f)
				{
					float num2 = Mathf.Cos(num * 3.1415927f * 1f);
					float d = Mathf.Sin(num * 3.1415927f * 1f);
					base.PushMatrix(Matrix4x4.TRS(new Vector3(0f, 0f, num2 * 0.5f), Quaternion.identity, Vector3.one * d));
					base.DrawDisc();
					base.PopMatrix();
				}
			}
		}

		// Token: 0x02000824 RID: 2084
		public class SphereGizmoObj : DDraw.BaseObject
		{
			// Token: 0x040028E5 RID: 10469
			private static Matrix4x4 XZ = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);

			// Token: 0x040028E6 RID: 10470
			private static Matrix4x4 YZ = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 90f, 0f), Vector3.one);

			// Token: 0x040028E7 RID: 10471
			private static Matrix4x4 XY = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90f, 0f, 0f), Vector3.one);

			// Token: 0x06002D61 RID: 11617 RVA: 0x000E422C File Offset: 0x000E242C
			public override void Draw()
			{
				base.PushMatrix(DDraw.SphereGizmoObj.XZ);
				base.DrawDisc();
				base.PopMatrix();
				base.PushMatrix(DDraw.SphereGizmoObj.YZ);
				base.DrawDisc();
				base.PopMatrix();
				base.PushMatrix(DDraw.SphereGizmoObj.XY);
				base.DrawDisc();
				base.PopMatrix();
			}
		}

		// Token: 0x02000825 RID: 2085
		public class LineObj : DDraw.BaseObject
		{
			// Token: 0x06002D64 RID: 11620 RVA: 0x00023520 File Offset: 0x00021720
			public override void Draw()
			{
				base.Begin(1);
				GL.Vertex(Vector3.zero);
				GL.Vertex(Vector3.forward);
				GL.End();
			}
		}

		// Token: 0x02000826 RID: 2086
		public class CubeObj : DDraw.BaseObject
		{
			// Token: 0x06002D66 RID: 11622 RVA: 0x000E4308 File Offset: 0x000E2508
			public override void Draw()
			{
				base.Begin(1);
				GL.Vertex(new Vector3(-0.5f, -0.5f, -0.5f));
				GL.Vertex(new Vector3(0.5f, -0.5f, -0.5f));
				GL.Vertex(new Vector3(-0.5f, -0.5f, -0.5f));
				GL.Vertex(new Vector3(-0.5f, -0.5f, 0.5f));
				GL.Vertex(new Vector3(-0.5f, -0.5f, 0.5f));
				GL.Vertex(new Vector3(0.5f, -0.5f, 0.5f));
				GL.Vertex(new Vector3(0.5f, -0.5f, -0.5f));
				GL.Vertex(new Vector3(0.5f, -0.5f, 0.5f));
				GL.Vertex(new Vector3(-0.5f, 0.5f, -0.5f));
				GL.Vertex(new Vector3(0.5f, 0.5f, -0.5f));
				GL.Vertex(new Vector3(-0.5f, 0.5f, -0.5f));
				GL.Vertex(new Vector3(-0.5f, 0.5f, 0.5f));
				GL.Vertex(new Vector3(-0.5f, 0.5f, 0.5f));
				GL.Vertex(new Vector3(0.5f, 0.5f, 0.5f));
				GL.Vertex(new Vector3(0.5f, 0.5f, -0.5f));
				GL.Vertex(new Vector3(0.5f, 0.5f, 0.5f));
				GL.Vertex(new Vector3(-0.5f, 0.5f, -0.5f));
				GL.Vertex(new Vector3(-0.5f, -0.5f, -0.5f));
				GL.Vertex(new Vector3(0.5f, 0.5f, -0.5f));
				GL.Vertex(new Vector3(0.5f, -0.5f, -0.5f));
				GL.Vertex(new Vector3(0.5f, 0.5f, 0.5f));
				GL.Vertex(new Vector3(0.5f, -0.5f, 0.5f));
				GL.Vertex(new Vector3(-0.5f, 0.5f, 0.5f));
				GL.Vertex(new Vector3(-0.5f, -0.5f, 0.5f));
				GL.End();
			}
		}

		// Token: 0x02000827 RID: 2087
		public class ArrowHead : DDraw.BaseObject
		{
			// Token: 0x06002D68 RID: 11624 RVA: 0x000E457C File Offset: 0x000E277C
			public override void Draw()
			{
				base.DrawDisc();
				base.Begin(1);
				for (float num = 0f; num <= 1f; num += 0.0625f)
				{
					float x = Mathf.Cos(num * 3.1415927f * 2f) * 0.5f;
					float y = Mathf.Sin(num * 3.1415927f * 2f) * 0.5f;
					GL.Vertex3(x, y, 0f);
					GL.Vertex(Vector3.forward);
				}
				GL.End();
			}
		}

		// Token: 0x02000828 RID: 2088
		public class TextObj : DDraw.BaseObject
		{
			// Token: 0x040028E8 RID: 10472
			public string text = "unset";

			// Token: 0x040028E9 RID: 10473
			protected Rect rect;

			// Token: 0x040028EA RID: 10474
			protected bool draw;

			// Token: 0x06002D6A RID: 11626 RVA: 0x000E45FC File Offset: 0x000E27FC
			public override void Draw()
			{
				Vector3 vector = MainCamera.mainCamera.WorldToScreenPoint(this.position);
				if (vector.z < 0f)
				{
					this.draw = false;
					return;
				}
				vector.y = (float)Screen.height - vector.y;
				this.rect.x = vector.x - 500f;
				this.rect.y = vector.y - 500f;
				this.rect.width = 1000f;
				this.rect.height = 1000f;
				this.draw = true;
			}

			// Token: 0x06002D6B RID: 11627 RVA: 0x000E4698 File Offset: 0x000E2898
			public override void DrawGUI()
			{
				if (!this.draw)
				{
					return;
				}
				GUI.contentColor = Color.black;
				GUI.Label(this.rect, this.text, DDraw.skin.label);
				Rect rect = new Rect(this.rect);
				rect.xMin -= 1f;
				rect.yMin -= 1f;
				rect.xMax -= 1f;
				rect.yMax -= 1f;
				GUI.contentColor = new Color(this.color.r, this.color.g, this.color.b, this.color.a);
				GUI.Label(rect, this.text, DDraw.skin.label);
			}
		}

		// Token: 0x02000829 RID: 2089
		public class ScreenTextObj : DDraw.BaseObject
		{
			// Token: 0x040028EB RID: 10475
			public string text = "unset";

			// Token: 0x040028EC RID: 10476
			public int x;

			// Token: 0x040028ED RID: 10477
			public int y;

			// Token: 0x040028EE RID: 10478
			protected Rect rect;

			// Token: 0x06002D6D RID: 11629 RVA: 0x00002ECE File Offset: 0x000010CE
			public override void Draw()
			{
			}

			// Token: 0x06002D6E RID: 11630 RVA: 0x000E4778 File Offset: 0x000E2978
			public override void DrawGUI()
			{
				float num = 20f;
				float num2 = (float)Screen.width - num * 2f;
				float num3 = (float)Screen.height - num * 2f;
				int num4 = 5;
				float num5 = num2 / (float)num4;
				int num6 = 16;
				float num7 = num3 / (float)num6;
				GUI.contentColor = new Color(this.color.r, this.color.g, this.color.b, 1f - this.delta);
				GUI.skin.label.fontSize = 11;
				GUI.skin.label.alignment = 0;
				this.rect.width = num5;
				this.rect.height = (float)num6;
				this.rect.x = num + (float)this.x * num5;
				this.rect.y = num + (float)this.y % num7 * (float)num6;
				GUI.Label(this.rect, this.text);
			}
		}
	}
}
