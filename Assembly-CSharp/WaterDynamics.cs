using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000554 RID: 1364
[ExecuteInEditMode]
public class WaterDynamics : MonoBehaviour
{
	// Token: 0x04001AFF RID: 6911
	public bool ForceFallback;

	// Token: 0x04001B00 RID: 6912
	private WaterDynamics.Target target;

	// Token: 0x04001B01 RID: 6913
	private bool useNativePath;

	// Token: 0x04001B02 RID: 6914
	private static HashSet<WaterInteraction> interactions = new HashSet<WaterInteraction>();

	// Token: 0x04001B03 RID: 6915
	private const int maxRasterSize = 1024;

	// Token: 0x04001B04 RID: 6916
	private const int subStep = 256;

	// Token: 0x04001B05 RID: 6917
	private const int subShift = 8;

	// Token: 0x04001B06 RID: 6918
	private const int subMask = 255;

	// Token: 0x04001B07 RID: 6919
	private const float oneOverSubStep = 0.00390625f;

	// Token: 0x04001B08 RID: 6920
	private const float interp_subStep = 65536f;

	// Token: 0x04001B09 RID: 6921
	private const int interp_subShift = 16;

	// Token: 0x04001B0A RID: 6922
	private const int interp_subFracMask = 65535;

	// Token: 0x04001B0B RID: 6923
	private WaterDynamics.ImageDesc imageDesc;

	// Token: 0x04001B0C RID: 6924
	private byte[] imagePixels;

	// Token: 0x04001B0D RID: 6925
	private WaterDynamics.TargetDesc targetDesc;

	// Token: 0x04001B0E RID: 6926
	private byte[] targetPixels;

	// Token: 0x04001B0F RID: 6927
	private byte[] targetDrawTileTable;

	// Token: 0x04001B10 RID: 6928
	private SimpleList<ushort> targetDrawTileList;

	// Token: 0x04001B11 RID: 6929
	public bool ShowDebug;

	// Token: 0x04001B12 RID: 6930
	private Material material;

	// Token: 0x04001B13 RID: 6931
	private MaterialPropertyBlock block;

	// Token: 0x04001B14 RID: 6932
	private Mesh mesh;

	// Token: 0x04001B15 RID: 6933
	private static Dictionary<Texture2D, WaterDynamics.InstanceBatch> Batches = new Dictionary<Texture2D, WaterDynamics.InstanceBatch>();

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x06001EB0 RID: 7856 RVA: 0x000183B0 File Offset: 0x000165B0
	// (set) Token: 0x06001EAF RID: 7855 RVA: 0x000183A7 File Offset: 0x000165A7
	public bool IsInitialized { get; private set; }

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000183B8 File Offset: 0x000165B8
	public static void RegisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Add(interaction);
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000183C6 File Offset: 0x000165C6
	public static void UnregisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Remove(interaction);
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000A7F08 File Offset: 0x000A6108
	private bool SupportsNativePath()
	{
		bool result = true;
		try
		{
			WaterDynamics.ImageDesc imageDesc = default(WaterDynamics.ImageDesc);
			byte[] array = new byte[1];
			WaterDynamics.RasterBindImage_Native(ref imageDesc, ref array[0]);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log("[WaterDynamics] Fast native path not available. Reverting to managed fallback.");
			result = false;
		}
		return result;
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000183D4 File Offset: 0x000165D4
	public void Initialize(Vector3 areaPosition, Vector3 areaSize)
	{
		this.target = new WaterDynamics.Target(this, areaPosition, areaSize);
		this.InitializeRender();
		this.useNativePath = this.SupportsNativePath();
		this.IsInitialized = true;
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000183FD File Offset: 0x000165FD
	public bool TryInitialize()
	{
		if (!this.IsInitialized && TerrainMeta.Data != null)
		{
			this.Initialize(TerrainMeta.Position, TerrainMeta.Data.size);
			return true;
		}
		return false;
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x0001842C File Offset: 0x0001662C
	public void Shutdown()
	{
		this.ShutdownRender();
		if (this.target != null)
		{
			this.target.Destroy();
			this.target = null;
		}
		this.IsInitialized = false;
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x00018455 File Offset: 0x00016655
	public void OnEnable()
	{
		this.TryInitialize();
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x0001845E File Offset: 0x0001665E
	public void OnDisable()
	{
		this.Shutdown();
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x00018466 File Offset: 0x00016666
	public void Update()
	{
		if (!this.IsInitialized && !this.TryInitialize())
		{
			return;
		}
		this.target.Prepare();
		this.ProcessInteractions();
		this.target.Update();
		this.target.UpdateGlobalShaderProperties();
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000A7F58 File Offset: 0x000A6158
	private void ProcessInteractions()
	{
		this.RenderIssueBindTarget(this.target);
		foreach (WaterInteraction waterInteraction in WaterDynamics.interactions)
		{
			if (!(waterInteraction == null))
			{
				waterInteraction.UpdateTransform();
				this.RenderIssueInteraction(waterInteraction.Texture, waterInteraction.Position, waterInteraction.Scale, waterInteraction.Rotation, waterInteraction.Displacement, waterInteraction.Disturbance);
			}
		}
		this.RenderFlushInteractions();
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x0000481F File Offset: 0x00002A1F
	public float SampleHeight(Vector3 pos)
	{
		return 0f;
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x000184A0 File Offset: 0x000166A0
	private void RasterBindImage(WaterDynamics.Image image)
	{
		this.imageDesc = image.desc;
		this.imagePixels = image.pixels;
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x000184BA File Offset: 0x000166BA
	private void RasterBindTarget(WaterDynamics.Target target)
	{
		this.targetDesc = target.Desc;
		this.targetPixels = target.Pixels;
		this.targetDrawTileTable = target.DrawTileTable;
		this.targetDrawTileList = target.DrawTileList;
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000A7FF0 File Offset: 0x000A61F0
	private void RasterInteraction(Vector2 pos, Vector2 scale, float rotation, float disp, float dist)
	{
		Vector2 a = this.targetDesc.WorldToRaster(pos);
		float f = -rotation * 0.017453292f;
		float s = Mathf.Sin(f);
		float c = Mathf.Cos(f);
		float num = Mathf.Min((float)this.imageDesc.width * scale.x, 1024f) * 0.5f;
		float num2 = Mathf.Min((float)this.imageDesc.height * scale.y, 1024f) * 0.5f;
		Vector2 vector = a + this.Rotate2D(new Vector2(-num, -num2), s, c);
		Vector2 vector2 = a + this.Rotate2D(new Vector2(num, -num2), s, c);
		Vector2 vector3 = a + this.Rotate2D(new Vector2(num, num2), s, c);
		Vector2 vector4 = a + this.Rotate2D(new Vector2(-num, num2), s, c);
		WaterDynamics.Point2D p = new WaterDynamics.Point2D(vector.x * 256f, vector.y * 256f);
		WaterDynamics.Point2D p2 = new WaterDynamics.Point2D(vector2.x * 256f, vector2.y * 256f);
		WaterDynamics.Point2D point2D = new WaterDynamics.Point2D(vector3.x * 256f, vector3.y * 256f);
		WaterDynamics.Point2D p3 = new WaterDynamics.Point2D(vector4.x * 256f, vector4.y * 256f);
		Vector2 uv = new Vector2(-0.5f, -0.5f);
		Vector2 uv2 = new Vector2((float)this.imageDesc.width - 0.5f, -0.5f);
		Vector2 vector5 = new Vector2((float)this.imageDesc.width - 0.5f, (float)this.imageDesc.height - 0.5f);
		Vector2 uv3 = new Vector2(-0.5f, (float)this.imageDesc.height - 0.5f);
		byte disp2 = (byte)(disp * 255f);
		byte dist2 = (byte)(dist * 255f);
		this.RasterizeTriangle(p, p2, point2D, uv, uv2, vector5, disp2, dist2);
		this.RasterizeTriangle(p, point2D, p3, uv, vector5, uv3, disp2, dist2);
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x000184EC File Offset: 0x000166EC
	private float Frac(float x)
	{
		return x - (float)((int)x);
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000A8204 File Offset: 0x000A6404
	private Vector2 Rotate2D(Vector2 v, float s, float c)
	{
		Vector2 result;
		result.x = v.x * c - v.y * s;
		result.y = v.y * c + v.x * s;
		return result;
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000184F3 File Offset: 0x000166F3
	private int Min3(int a, int b, int c)
	{
		return Mathf.Min(a, Mathf.Min(b, c));
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x00018502 File Offset: 0x00016702
	private int Max3(int a, int b, int c)
	{
		return Mathf.Max(a, Mathf.Max(b, c));
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x000A8244 File Offset: 0x000A6444
	private int EdgeFunction(WaterDynamics.Point2D a, WaterDynamics.Point2D b, WaterDynamics.Point2D c)
	{
		return (int)(((long)(b.x - a.x) * (long)(c.y - a.y) >> 8) - ((long)(b.y - a.y) * (long)(c.x - a.x) >> 8));
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x00018511 File Offset: 0x00016711
	private bool IsTopLeft(WaterDynamics.Point2D a, WaterDynamics.Point2D b)
	{
		return (a.y == b.y && a.x < b.x) || a.y > b.y;
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000A8294 File Offset: 0x000A6494
	private void RasterizeTriangle(WaterDynamics.Point2D p0, WaterDynamics.Point2D p1, WaterDynamics.Point2D p2, Vector2 uv0, Vector2 uv1, Vector2 uv2, byte disp, byte dist)
	{
		int width = this.imageDesc.width;
		int widthShift = this.imageDesc.widthShift;
		int maxWidth = this.imageDesc.maxWidth;
		int maxHeight = this.imageDesc.maxHeight;
		int size = this.targetDesc.size;
		int tileCount = this.targetDesc.tileCount;
		int num = Mathf.Max(this.Min3(p0.x, p1.x, p2.x), 0);
		int num2 = Mathf.Max(this.Min3(p0.y, p1.y, p2.y), 0);
		int num3 = Mathf.Min(this.Max3(p0.x, p1.x, p2.x), this.targetDesc.maxSizeSubStep);
		int num4 = Mathf.Min(this.Max3(p0.y, p1.y, p2.y), this.targetDesc.maxSizeSubStep);
		int num5 = Mathf.Max(num >> 8 >> this.targetDesc.tileSizeShift, 0);
		int num6 = Mathf.Min(num3 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
		int num7 = Mathf.Max(num2 >> 8 >> this.targetDesc.tileSizeShift, 0);
		int num8 = Mathf.Min(num4 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
		for (int i = num7; i <= num8; i++)
		{
			int num9 = i * tileCount;
			for (int j = num5; j <= num6; j++)
			{
				int num10 = num9 + j;
				if (this.targetDrawTileTable[num10] == 0)
				{
					this.targetDrawTileTable[num10] = 1;
					this.targetDrawTileList.Add((ushort)num10);
				}
			}
		}
		num = (num + 255 & -256);
		num2 = (num2 + 255 & -256);
		int num11 = this.IsTopLeft(p1, p2) ? 0 : -1;
		int num12 = this.IsTopLeft(p2, p0) ? 0 : -1;
		int num13 = this.IsTopLeft(p0, p1) ? 0 : -1;
		WaterDynamics.Point2D c = new WaterDynamics.Point2D(num, num2);
		int num14 = this.EdgeFunction(p1, p2, c) + num11;
		int num15 = this.EdgeFunction(p2, p0, c) + num12;
		int num16 = this.EdgeFunction(p0, p1, c) + num13;
		int num17 = p1.y - p2.y;
		int num18 = p2.y - p0.y;
		int num19 = p0.y - p1.y;
		int num20 = p2.x - p1.x;
		int num21 = p0.x - p2.x;
		int num22 = p1.x - p0.x;
		float num23 = 16777216f / (float)this.EdgeFunction(p0, p1, p2);
		float num24 = uv0.x * 65536f;
		float num25 = uv0.y * 65536f;
		float num26 = (uv1.x - uv0.x) * num23;
		float num27 = (uv1.y - uv0.y) * num23;
		float num28 = (uv2.x - uv0.x) * num23;
		float num29 = (uv2.y - uv0.y) * num23;
		int num30 = (int)((float)num18 * 0.00390625f * num26 + (float)num19 * 0.00390625f * num28);
		int num31 = (int)((float)num18 * 0.00390625f * num27 + (float)num19 * 0.00390625f * num29);
		for (int k = num2; k <= num4; k += 256)
		{
			int num32 = num14;
			int num33 = num15;
			int num34 = num16;
			int num35 = (int)(num24 + num26 * 0.00390625f * (float)num33 + num28 * 0.00390625f * (float)num34);
			int num36 = (int)(num25 + num27 * 0.00390625f * (float)num33 + num29 * 0.00390625f * (float)num34);
			for (int l = num; l <= num3; l += 256)
			{
				if ((num32 | num33 | num34) >= 0)
				{
					int num37 = (num35 > 0) ? num35 : 0;
					object obj = (num36 > 0) ? num36 : 0;
					int num38 = num37 >> 16;
					object obj2 = obj;
					int num39 = obj2 >> 16;
					byte b = (byte)((num37 & 65535) >> 8);
					byte b2 = (obj2 & 65535) >> 8;
					num38 = ((num38 > 0) ? num38 : 0);
					num39 = ((num39 > 0) ? num39 : 0);
					num38 = ((num38 < maxWidth) ? num38 : maxWidth);
					num39 = ((num39 < maxHeight) ? num39 : maxHeight);
					int num40 = (num38 < maxWidth) ? 1 : 0;
					int num41 = (num39 < maxHeight) ? width : 0;
					int num42 = (num39 << widthShift) + num38;
					int num43 = num42 + num40;
					int num44 = num42 + num41;
					int num45 = num44 + num40;
					byte b3 = this.imagePixels[num42];
					byte b4 = this.imagePixels[num43];
					byte b5 = this.imagePixels[num44];
					byte b6 = this.imagePixels[num45];
					int num46 = (int)b3 + (b * (b4 - b3) >> 8);
					int num47 = (int)b5 + (b * (b6 - b5) >> 8);
					int num48 = num46 + ((int)b2 * (num47 - num46) >> 8);
					num48 = num48 * (int)disp >> 8;
					int num49 = (k >> 8) * size + (l >> 8);
					num48 = (int)this.targetPixels[num49] + num48;
					num48 = ((num48 < 255) ? num48 : 255);
					this.targetPixels[num49] = (byte)num48;
				}
				num32 += num17;
				num33 += num18;
				num34 += num19;
				num35 += num30;
				num36 += num31;
			}
			num14 += num20;
			num15 += num21;
			num16 += num22;
		}
	}

	// Token: 0x06001EC6 RID: 7878
	[DllImport("RustNative", EntryPoint = "Water_RasterClearTile")]
	private static extern void RasterClearTile_Native(ref byte pixels, int offset, int stride, int width, int height);

	// Token: 0x06001EC7 RID: 7879
	[DllImport("RustNative", EntryPoint = "Water_RasterBindImage")]
	private static extern void RasterBindImage_Native(ref WaterDynamics.ImageDesc desc, ref byte pixels);

	// Token: 0x06001EC8 RID: 7880
	[DllImport("RustNative", EntryPoint = "Water_RasterBindTarget")]
	private static extern void RasterBindTarget_Native(ref WaterDynamics.TargetDesc desc, ref byte pixels, ref byte drawTileTable, ref ushort drawTileList, ref int drawTileCount);

	// Token: 0x06001EC9 RID: 7881
	[DllImport("RustNative", EntryPoint = "Water_RasterInteraction")]
	private static extern void RasterInteraction_Native(Vector2 pos, Vector2 scale, float rotation, float disp, float dist);

	// Token: 0x06001ECA RID: 7882 RVA: 0x0001853F File Offset: 0x0001673F
	public void InitializeRender()
	{
		this.material = new Material(Shader.Find("Hidden/Water/Interaction"));
		this.block = new MaterialPropertyBlock();
		this.mesh = this.CreateMesh();
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000A87F4 File Offset: 0x000A69F4
	public void ShutdownRender()
	{
		WaterDynamics.SafeDestroy<Material>(ref this.material);
		WaterDynamics.SafeDestroy<Mesh>(ref this.mesh);
		foreach (KeyValuePair<Texture2D, WaterDynamics.InstanceBatch> keyValuePair in WaterDynamics.Batches)
		{
			WaterDynamics.InstanceBatch value = keyValuePair.Value;
			value.Release();
			Pool.Free<WaterDynamics.InstanceBatch>(ref value);
		}
		WaterDynamics.Batches.Clear();
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x000A8874 File Offset: 0x000A6A74
	private Mesh CreateMesh()
	{
		return new Mesh
		{
			vertices = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(0f, 1f, 0f)
			},
			triangles = new int[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			}
		};
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x0001856D File Offset: 0x0001676D
	private void RenderIssueBindTarget(WaterDynamics.Target target)
	{
		target.CommandBuffer.SetRenderTarget(target.InteractionTarget);
		target.CommandBuffer.ClearRenderTarget(false, true, new Color(0.5f, 0.5f, 0f, 0f));
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x000A8918 File Offset: 0x000A6B18
	private void RenderIssueInteraction(Texture2D texture, Vector2 pos, Vector2 scale, float rotation, float disp, float dist)
	{
		WaterDynamics.InstanceBatch instanceBatch = null;
		if (!WaterDynamics.Batches.TryGetValue(texture, ref instanceBatch))
		{
			instanceBatch = Pool.Get<WaterDynamics.InstanceBatch>();
			instanceBatch.Initialize(this.mesh, texture);
			WaterDynamics.Batches.Add(texture, instanceBatch);
		}
		instanceBatch.AddInstance(new WaterDynamics.InstanceData(pos, scale, rotation, disp, dist));
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x000A8968 File Offset: 0x000A6B68
	private void RenderFlushInteractions()
	{
		this.block.Clear();
		foreach (KeyValuePair<Texture2D, WaterDynamics.InstanceBatch> keyValuePair in WaterDynamics.Batches)
		{
			Texture key = keyValuePair.Key;
			WaterDynamics.InstanceBatch value = keyValuePair.Value;
			if (value.Count > 0)
			{
				value.UpdateBuffers();
				this.block.SetTexture("_Texture", key);
				this.block.SetVector("_TextureSize", new Vector2((float)key.width, (float)key.height));
				this.block.SetBuffer("_InstanceBuffer", value.InstanceBuffer);
				this.target.CommandBuffer.DrawMeshInstancedIndirect(this.mesh, 0, this.material, 0, value.ArgBuffer, 0, this.block);
			}
			value.Clear();
		}
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x000185AB File Offset: 0x000167AB
	public static void SafeDestroy<T>(ref T obj) where T : Object
	{
		if (obj != null)
		{
			Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x000A8A68 File Offset: 0x000A6C68
	public static T SafeDestroy<T>(T obj) where T : Object
	{
		if (obj != null)
		{
			Object.DestroyImmediate(obj);
		}
		return default(T);
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000185D7 File Offset: 0x000167D7
	public static void SafeRelease<T>(ref T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000A8A98 File Offset: 0x000A6C98
	public static T SafeRelease<T>(T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
		}
		return default(T);
	}

	// Token: 0x02000555 RID: 1365
	[StructLayout(0, Pack = 1)]
	public struct ImageDesc
	{
		// Token: 0x04001B16 RID: 6934
		public int width;

		// Token: 0x04001B17 RID: 6935
		public int height;

		// Token: 0x04001B18 RID: 6936
		public int maxWidth;

		// Token: 0x04001B19 RID: 6937
		public int maxHeight;

		// Token: 0x04001B1A RID: 6938
		public int widthShift;

		// Token: 0x06001ED6 RID: 7894 RVA: 0x000A8AC4 File Offset: 0x000A6CC4
		public ImageDesc(Texture2D tex)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.maxWidth = tex.width - 1;
			this.maxHeight = tex.height - 1;
			this.widthShift = (int)Mathf.Log((float)tex.width, 2f);
		}

		// Token: 0x06001ED7 RID: 7895 RVA: 0x00018613 File Offset: 0x00016813
		public void Clear()
		{
			this.width = 0;
			this.height = 0;
			this.maxWidth = 0;
			this.maxHeight = 0;
			this.widthShift = 0;
		}
	}

	// Token: 0x02000556 RID: 1366
	public class Image
	{
		// Token: 0x04001B1B RID: 6939
		public WaterDynamics.ImageDesc desc;

		// Token: 0x04001B1D RID: 6941
		public byte[] pixels;

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06001ED8 RID: 7896 RVA: 0x00018638 File Offset: 0x00016838
		// (set) Token: 0x06001ED9 RID: 7897 RVA: 0x00018640 File Offset: 0x00016840
		public Texture2D texture { get; private set; }

		// Token: 0x06001EDA RID: 7898 RVA: 0x00018649 File Offset: 0x00016849
		public Image(Texture2D tex)
		{
			this.desc = new WaterDynamics.ImageDesc(tex);
			this.texture = tex;
			this.pixels = this.GetDisplacementPixelsFromTexture(tex);
		}

		// Token: 0x06001EDB RID: 7899 RVA: 0x00018671 File Offset: 0x00016871
		public void Destroy()
		{
			this.desc.Clear();
			this.texture = null;
			this.pixels = null;
		}

		// Token: 0x06001EDC RID: 7900 RVA: 0x000A8B20 File Offset: 0x000A6D20
		private byte[] GetDisplacementPixelsFromTexture(Texture2D tex)
		{
			Color32[] pixels = tex.GetPixels32();
			byte[] array = new byte[pixels.Length];
			for (int i = 0; i < pixels.Length; i++)
			{
				array[i] = pixels[i].b;
			}
			return array;
		}
	}

	// Token: 0x02000557 RID: 1367
	private struct Point2D
	{
		// Token: 0x04001B1E RID: 6942
		public int x;

		// Token: 0x04001B1F RID: 6943
		public int y;

		// Token: 0x06001EDD RID: 7901 RVA: 0x0001868C File Offset: 0x0001688C
		public Point2D(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x0001869C File Offset: 0x0001689C
		public Point2D(float x, float y)
		{
			this.x = (int)x;
			this.y = (int)y;
		}
	}

	// Token: 0x02000558 RID: 1368
	public struct InstanceData
	{
		// Token: 0x04001B20 RID: 6944
		public Vector4 PositionScale;

		// Token: 0x04001B21 RID: 6945
		public Vector4 RotationDispDist;

		// Token: 0x06001EDF RID: 7903 RVA: 0x000186AE File Offset: 0x000168AE
		public InstanceData(Vector2 pos, Vector2 scale, float rotation, float disp, float dist)
		{
			this.PositionScale = new Vector4(pos.x, pos.y, scale.x, scale.y);
			this.RotationDispDist = new Vector4(rotation, disp, dist, 0f);
		}
	}

	// Token: 0x02000559 RID: 1369
	private class InstanceBatch
	{
		// Token: 0x04001B25 RID: 6949
		private uint[] args;

		// Token: 0x04001B27 RID: 6951
		private SimpleList<WaterDynamics.InstanceData> instances;

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06001EE1 RID: 7905 RVA: 0x000186F1 File Offset: 0x000168F1
		// (set) Token: 0x06001EE0 RID: 7904 RVA: 0x000186E8 File Offset: 0x000168E8
		public Mesh Mesh { get; private set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06001EE3 RID: 7907 RVA: 0x00018702 File Offset: 0x00016902
		// (set) Token: 0x06001EE2 RID: 7906 RVA: 0x000186F9 File Offset: 0x000168F9
		public Texture Texture { get; private set; }

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06001EE5 RID: 7909 RVA: 0x00018713 File Offset: 0x00016913
		// (set) Token: 0x06001EE4 RID: 7908 RVA: 0x0001870A File Offset: 0x0001690A
		public ComputeBuffer ArgBuffer { get; private set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06001EE7 RID: 7911 RVA: 0x00018724 File Offset: 0x00016924
		// (set) Token: 0x06001EE6 RID: 7910 RVA: 0x0001871B File Offset: 0x0001691B
		public ComputeBuffer InstanceBuffer { get; private set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06001EE8 RID: 7912 RVA: 0x0001872C File Offset: 0x0001692C
		public int Count
		{
			get
			{
				return this.instances.Count;
			}
		}

		// Token: 0x06001EE9 RID: 7913 RVA: 0x00018739 File Offset: 0x00016939
		private ComputeBuffer SafeRelease(ComputeBuffer buffer)
		{
			if (buffer != null)
			{
				buffer.Release();
			}
			return null;
		}

		// Token: 0x06001EEA RID: 7914 RVA: 0x000A8B5C File Offset: 0x000A6D5C
		public void Initialize(Mesh mesh, Texture texture)
		{
			this.Mesh = mesh;
			this.Texture = texture;
			this.instances = Pool.Get<SimpleList<WaterDynamics.InstanceData>>();
			this.instances.Clear();
			this.args = new uint[5];
			this.ArgBuffer = WaterDynamics.SafeRelease<ComputeBuffer>(this.ArgBuffer);
			this.ArgBuffer = new ComputeBuffer(1, this.args.Length * 4, ComputeBufferType.DrawIndirect);
			this.args[0] = mesh.GetIndexCount(0);
			this.args[2] = mesh.GetIndexStart(0);
			this.args[3] = mesh.GetBaseVertex(0);
		}

		// Token: 0x06001EEB RID: 7915 RVA: 0x00018745 File Offset: 0x00016945
		public void Release()
		{
			this.ArgBuffer = WaterDynamics.SafeRelease<ComputeBuffer>(this.ArgBuffer);
			this.args = null;
			this.InstanceBuffer = WaterDynamics.SafeRelease<ComputeBuffer>(this.InstanceBuffer);
			Pool.Free<SimpleList<WaterDynamics.InstanceData>>(ref this.instances);
		}

		// Token: 0x06001EEC RID: 7916 RVA: 0x0001877B File Offset: 0x0001697B
		public void Clear()
		{
			this.instances.Clear();
		}

		// Token: 0x06001EED RID: 7917 RVA: 0x00018788 File Offset: 0x00016988
		public void AddInstance(WaterDynamics.InstanceData data)
		{
			this.instances.Add(data);
		}

		// Token: 0x06001EEE RID: 7918 RVA: 0x000A8BF4 File Offset: 0x000A6DF4
		public void UpdateBuffers()
		{
			if (this.InstanceBuffer == null || this.InstanceBuffer.count < this.instances.Count)
			{
				this.InstanceBuffer = WaterDynamics.SafeRelease<ComputeBuffer>(this.InstanceBuffer);
				this.InstanceBuffer = new ComputeBuffer(this.instances.Count, 32);
			}
			if (this.InstanceBuffer != null)
			{
				this.InstanceBuffer.SetData(this.instances.Array, 0, 0, this.instances.Count);
			}
			if (this.ArgBuffer != null && (ulong)this.args[1] != (ulong)((long)this.instances.Count))
			{
				this.args[1] = (uint)this.instances.Count;
				this.ArgBuffer.SetData(this.args);
			}
		}
	}

	// Token: 0x0200055A RID: 1370
	[StructLayout(0, Pack = 1)]
	public struct TargetDesc
	{
		// Token: 0x04001B28 RID: 6952
		public int size;

		// Token: 0x04001B29 RID: 6953
		public int maxSize;

		// Token: 0x04001B2A RID: 6954
		public int maxSizeSubStep;

		// Token: 0x04001B2B RID: 6955
		public Vector2 areaOffset;

		// Token: 0x04001B2C RID: 6956
		public Vector2 areaToMapUV;

		// Token: 0x04001B2D RID: 6957
		public Vector2 areaToMapXY;

		// Token: 0x04001B2E RID: 6958
		public int tileSize;

		// Token: 0x04001B2F RID: 6959
		public int tileSizeShift;

		// Token: 0x04001B30 RID: 6960
		public int tileCount;

		// Token: 0x04001B31 RID: 6961
		public int tileMaxCount;

		// Token: 0x06001EF0 RID: 7920 RVA: 0x000A8CB8 File Offset: 0x000A6EB8
		public TargetDesc(Vector3 areaPosition, Vector3 areaSize)
		{
			this.size = 1024;
			this.maxSize = this.size - 1;
			this.maxSizeSubStep = this.maxSize * 256;
			this.areaOffset = new Vector2(areaPosition.x, areaPosition.z);
			this.areaToMapUV = new Vector2(1f / areaSize.x, 1f / areaSize.z);
			this.areaToMapXY = this.areaToMapUV * (float)this.size;
			this.tileSize = Mathf.NextPowerOfTwo(Mathf.Max(this.size, 4096)) / 256;
			this.tileSizeShift = (int)Mathf.Log((float)this.tileSize, 2f);
			this.tileCount = Mathf.CeilToInt((float)this.size / (float)this.tileSize);
			this.tileMaxCount = this.tileCount - 1;
		}

		// Token: 0x06001EF1 RID: 7921 RVA: 0x000A8DA4 File Offset: 0x000A6FA4
		public void Clear()
		{
			this.areaOffset = Vector2.zero;
			this.areaToMapUV = Vector2.zero;
			this.areaToMapXY = Vector2.zero;
			this.size = 0;
			this.maxSize = 0;
			this.maxSizeSubStep = 0;
			this.tileSize = 0;
			this.tileSizeShift = 0;
			this.tileCount = 0;
			this.tileMaxCount = 0;
		}

		// Token: 0x06001EF2 RID: 7922 RVA: 0x000A8E04 File Offset: 0x000A7004
		public ushort TileOffsetToXYOffset(ushort tileOffset, out int x, out int y, out int offset)
		{
			int num = (int)tileOffset % this.tileCount;
			int num2 = (int)tileOffset / this.tileCount;
			x = num * this.tileSize;
			y = num2 * this.tileSize;
			offset = y * this.size + x;
			return tileOffset;
		}

		// Token: 0x06001EF3 RID: 7923 RVA: 0x00018796 File Offset: 0x00016996
		public ushort TileOffsetToTileXYIndex(ushort tileOffset, out int tileX, out int tileY, out ushort tileIndex)
		{
			tileX = (int)tileOffset % this.tileCount;
			tileY = (int)tileOffset / this.tileCount;
			tileIndex = (ushort)(tileY * this.tileCount + tileX);
			return tileOffset;
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x000A8E48 File Offset: 0x000A7048
		public Vector2 WorldToRaster(Vector2 pos)
		{
			Vector2 result;
			result.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			result.y = (pos.y - this.areaOffset.y) * this.areaToMapXY.y;
			return result;
		}

		// Token: 0x06001EF5 RID: 7925 RVA: 0x000A8EA0 File Offset: 0x000A70A0
		public Vector3 WorldToRaster(Vector3 pos)
		{
			Vector2 v;
			v.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			v.y = (pos.z - this.areaOffset.y) * this.areaToMapXY.y;
			return v;
		}
	}

	// Token: 0x0200055B RID: 1371
	public class Target
	{
		// Token: 0x04001B32 RID: 6962
		public WaterDynamics owner;

		// Token: 0x04001B33 RID: 6963
		public WaterDynamics.TargetDesc desc;

		// Token: 0x04001B34 RID: 6964
		private byte[] pixels;

		// Token: 0x04001B35 RID: 6965
		private RenderTexture interactionTarget;

		// Token: 0x04001B36 RID: 6966
		private RenderTexture combinedTarget;

		// Token: 0x04001B37 RID: 6967
		private CommandBuffer commandBuffer;

		// Token: 0x04001B38 RID: 6968
		private Material combineMaterial;

		// Token: 0x04001B39 RID: 6969
		private byte[] clearTileTable;

		// Token: 0x04001B3A RID: 6970
		private SimpleList<ushort> clearTileList;

		// Token: 0x04001B3B RID: 6971
		private byte[] drawTileTable;

		// Token: 0x04001B3C RID: 6972
		private SimpleList<ushort> drawTileList;

		// Token: 0x04001B3D RID: 6973
		private const int MaxInteractionOffset = 100;

		// Token: 0x04001B3E RID: 6974
		private Vector3 prevCameraWorldPos;

		// Token: 0x04001B3F RID: 6975
		private Vector2i interactionOffset;

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06001EF6 RID: 7926 RVA: 0x000187BD File Offset: 0x000169BD
		public WaterDynamics.TargetDesc Desc
		{
			get
			{
				return this.desc;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06001EF7 RID: 7927 RVA: 0x000187C5 File Offset: 0x000169C5
		public byte[] Pixels
		{
			get
			{
				return this.pixels;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06001EF8 RID: 7928 RVA: 0x000187CD File Offset: 0x000169CD
		public RenderTexture InteractionTarget
		{
			get
			{
				return this.interactionTarget;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06001EF9 RID: 7929 RVA: 0x000187D5 File Offset: 0x000169D5
		public RenderTexture CombinedTarget
		{
			get
			{
				return this.combinedTarget;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06001EFA RID: 7930 RVA: 0x000187DD File Offset: 0x000169DD
		public CommandBuffer CommandBuffer
		{
			get
			{
				return this.commandBuffer;
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06001EFB RID: 7931 RVA: 0x000187E5 File Offset: 0x000169E5
		public byte[] DrawTileTable
		{
			get
			{
				return this.drawTileTable;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06001EFC RID: 7932 RVA: 0x000187ED File Offset: 0x000169ED
		public SimpleList<ushort> DrawTileList
		{
			get
			{
				return this.drawTileList;
			}
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x000A8F00 File Offset: 0x000A7100
		public Target(WaterDynamics owner, Vector3 areaPosition, Vector3 areaSize)
		{
			this.owner = owner;
			this.desc = new WaterDynamics.TargetDesc(areaPosition, areaSize);
			WaterDynamics.SafeDestroy<RenderTexture>(ref this.interactionTarget);
			this.interactionTarget = this.CreateRenderTexture(this.desc.size);
			WaterDynamics.SafeDestroy<RenderTexture>(ref this.combinedTarget);
			this.combinedTarget = this.CreateRenderTexture(this.desc.size);
			Graphics.SetRenderTarget(this.combinedTarget);
			GL.Clear(false, true, new Color(0.5f, 0.5f, 0f, 0f));
			this.commandBuffer = new CommandBuffer
			{
				name = "WaterInteraction"
			};
			this.combineMaterial = new Material(Shader.Find("Hidden/Water/CombineInteractions"));
		}

		// Token: 0x06001EFE RID: 7934 RVA: 0x000187F5 File Offset: 0x000169F5
		public void Destroy()
		{
			this.desc.Clear();
			WaterDynamics.SafeDestroy<RenderTexture>(ref this.interactionTarget);
			WaterDynamics.SafeDestroy<RenderTexture>(ref this.combinedTarget);
			WaterDynamics.SafeRelease<CommandBuffer>(ref this.commandBuffer);
			WaterDynamics.SafeDestroy<Material>(ref this.combineMaterial);
		}

		// Token: 0x06001EFF RID: 7935 RVA: 0x0001882E File Offset: 0x00016A2E
		private Texture2D CreateDynamicTexture(int size)
		{
			return new Texture2D(size, size, TextureFormat.ARGB32, false, true)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
		}

		// Token: 0x06001F00 RID: 7936 RVA: 0x00018848 File Offset: 0x00016A48
		private RenderTexture CreateRenderTexture(int size)
		{
			RenderTexture renderTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			renderTexture.Create();
			return renderTexture;
		}

		// Token: 0x06001F01 RID: 7937 RVA: 0x000A8FC4 File Offset: 0x000A71C4
		public void ClearTiles()
		{
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				int num;
				int num2;
				int num3;
				this.desc.TileOffsetToXYOffset(this.clearTileList[i], out num, out num2, out num3);
				int num4 = Mathf.Min(num + this.desc.tileSize, this.desc.size) - num;
				int num5 = Mathf.Min(num2 + this.desc.tileSize, this.desc.size) - num2;
				if (this.owner.useNativePath)
				{
					WaterDynamics.RasterClearTile_Native(ref this.pixels[0], num3, this.desc.size, num4, num5);
				}
				else
				{
					for (int j = 0; j < num5; j++)
					{
						Array.Clear(this.pixels, num3, num4);
						num3 += this.desc.size;
					}
				}
			}
		}

		// Token: 0x06001F02 RID: 7938 RVA: 0x000A90AC File Offset: 0x000A72AC
		public void ProcessTiles()
		{
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				int num2;
				int num3;
				ushort num4;
				ushort num = this.desc.TileOffsetToTileXYIndex(this.clearTileList[i], out num2, out num3, out num4);
				this.clearTileTable[(int)num] = 0;
				this.clearTileList[i] = ushort.MaxValue;
			}
			this.clearTileList.Clear();
			for (int j = 0; j < this.drawTileList.Count; j++)
			{
				int num2;
				int num3;
				ushort num4;
				ushort num5 = this.desc.TileOffsetToTileXYIndex(this.drawTileList[j], out num2, out num3, out num4);
				if (this.clearTileTable[(int)num4] == 0)
				{
					this.clearTileTable[(int)num4] = 1;
					this.clearTileList.Add(num4);
				}
				this.drawTileTable[(int)num5] = 0;
				this.drawTileList[j] = ushort.MaxValue;
			}
			this.drawTileList.Clear();
		}

		// Token: 0x06001F03 RID: 7939 RVA: 0x00002ECE File Offset: 0x000010CE
		public void UpdateTiles()
		{
		}

		// Token: 0x06001F04 RID: 7940 RVA: 0x000A9194 File Offset: 0x000A7394
		public void Prepare()
		{
			Camera mainCamera = MainCamera.mainCamera;
			if (mainCamera == null)
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			zero.x = Mathf.Floor(mainCamera.transform.position.x);
			zero.z = Mathf.Floor(mainCamera.transform.position.z);
			float num = (float)this.desc.size;
			float y = num / 2f;
			float z = 1f / num;
			float w = 0.5f / num;
			Shader.SetGlobalVector("_WaterInteraction_WorldOffset", zero);
			Shader.SetGlobalVector("_WaterInteractionMap_Desc", new Vector4(num, y, z, w));
			this.combineMaterial.SetTexture("_InteractionTex", this.interactionTarget);
			this.interactionOffset.x = (int)(zero.x - this.prevCameraWorldPos.x);
			this.interactionOffset.y = (int)(zero.z - this.prevCameraWorldPos.z);
			if (Mathf.Abs(this.interactionOffset.x) > 100 || Mathf.Abs(this.interactionOffset.y) > 100)
			{
				this.interactionOffset = Vector2i.zero;
			}
			this.commandBuffer.Clear();
			this.prevCameraWorldPos = zero;
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x000A92D4 File Offset: 0x000A74D4
		public void Update()
		{
			if (MainCamera.mainCamera == null)
			{
				return;
			}
			int nameID = Shader.PropertyToID("_TempRT");
			this.commandBuffer.GetTemporaryRT(nameID, this.combinedTarget.descriptor);
			if (this.interactionOffset.x == 0 && this.interactionOffset.y == 0)
			{
				this.commandBuffer.CopyTexture(this.combinedTarget, nameID);
			}
			else
			{
				int srcX = (this.interactionOffset.x > 0) ? this.interactionOffset.x : 0;
				int srcY = (this.interactionOffset.y > 0) ? this.interactionOffset.y : 0;
				int srcWidth = this.desc.size - Mathf.Abs(this.interactionOffset.x);
				int srcHeight = this.desc.size - Mathf.Abs(this.interactionOffset.y);
				int dstX = (this.interactionOffset.x < 0) ? (-this.interactionOffset.x) : 0;
				int dstY = (this.interactionOffset.y < 0) ? (-this.interactionOffset.y) : 0;
				this.commandBuffer.CopyTexture(this.combinedTarget, 0, 0, srcX, srcY, srcWidth, srcHeight, nameID, 0, 0, dstX, dstY);
			}
			this.commandBuffer.Blit(nameID, this.combinedTarget, this.combineMaterial);
			this.commandBuffer.ReleaseTemporaryRT(nameID);
			Graphics.ExecuteCommandBuffer(this.commandBuffer);
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x00018869 File Offset: 0x00016A69
		public void UpdateGlobalShaderProperties()
		{
			Shader.SetGlobalTexture("_WaterInteractionMap", this.combinedTarget);
		}
	}
}
