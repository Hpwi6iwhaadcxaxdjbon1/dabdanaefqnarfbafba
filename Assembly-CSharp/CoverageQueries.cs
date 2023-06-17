using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000791 RID: 1937
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class CoverageQueries : MonoBehaviour
{
	// Token: 0x0400259D RID: 9629
	public float depthBias = -0.1f;

	// Token: 0x0400259E RID: 9630
	private static List<CoverageQueries.Query> pool = new List<CoverageQueries.Query>(128);

	// Token: 0x0400259F RID: 9631
	private static List<CoverageQueries.Query> added = new List<CoverageQueries.Query>(32);

	// Token: 0x040025A0 RID: 9632
	private static List<CoverageQueries.Query> reused = new List<CoverageQueries.Query>(32);

	// Token: 0x040025A1 RID: 9633
	private static List<int> removed = new List<int>(128);

	// Token: 0x040025A2 RID: 9634
	private static List<int> changed = new List<int>(128);

	// Token: 0x040025A3 RID: 9635
	private static Queue<int> freed = new Queue<int>(16);

	// Token: 0x040025A4 RID: 9636
	private static CoverageQueries.BufferSet buffer = new CoverageQueries.BufferSet();

	// Token: 0x040025A5 RID: 9637
	private Camera camera;

	// Token: 0x040025A6 RID: 9638
	private Material coverageMat;

	// Token: 0x040025A7 RID: 9639
	private static CoverageQueries instance;

	// Token: 0x040025A8 RID: 9640
	private static bool _debugShow = false;

	// Token: 0x040025A9 RID: 9641
	public bool debug;

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06002A13 RID: 10771 RVA: 0x00020A5B File Offset: 0x0001EC5B
	public static CoverageQueries Instance
	{
		get
		{
			return CoverageQueries.instance;
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06002A14 RID: 10772 RVA: 0x00020A62 File Offset: 0x0001EC62
	public static bool Supported
	{
		get
		{
			return SystemInfo.supportsAsyncGPUReadback;
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x06002A15 RID: 10773 RVA: 0x00020A69 File Offset: 0x0001EC69
	// (set) Token: 0x06002A16 RID: 10774 RVA: 0x00020A70 File Offset: 0x0001EC70
	public static bool DebugShow
	{
		get
		{
			return CoverageQueries._debugShow;
		}
		set
		{
			CoverageQueries._debugShow = value;
		}
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x00020A78 File Offset: 0x0001EC78
	private void Awake()
	{
		CoverageQueries.instance = this;
		this.camera = base.GetComponent<Camera>();
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x000D6FCC File Offset: 0x000D51CC
	private void OnEnable()
	{
		if (!CoverageQueries.Supported)
		{
			Debug.LogWarning("[CoverageQueries] Disabled due to unsupported Async GPU Reads on device " + SystemInfo.graphicsDeviceType);
			base.enabled = false;
			return;
		}
		this.coverageMat = new Material(Shader.Find("Hidden/CoverageQueries/Coverage"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		CoverageQueries.buffer.Attach(this.coverageMat);
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x00020A8C File Offset: 0x0001EC8C
	private void OnDisable()
	{
		if (this.coverageMat != null)
		{
			Object.DestroyImmediate(this.coverageMat);
			this.coverageMat = null;
		}
		CoverageQueries.buffer.Dispose(true);
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x00020AB9 File Offset: 0x0001ECB9
	private void Update()
	{
		this.FetchAndAnalyseResults();
		this.UpdateCollection();
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x00020AC7 File Offset: 0x0001ECC7
	private void OnPostRender()
	{
		this.PrepareAndDispatch();
		this.IssueRead();
	}

	// Token: 0x06002A1C RID: 10780 RVA: 0x000D7030 File Offset: 0x000D5230
	private void UpdateCollection()
	{
		if (CoverageQueries.reused.Count > 0)
		{
			foreach (CoverageQueries.Query query in CoverageQueries.reused)
			{
				int id = query.intern.id;
				Debug.Assert(id >= 0 && id < CoverageQueries.pool.Count, "Reusing invalid query id ");
				CoverageQueries.pool[id] = query;
				CoverageQueries.changed.Add(id);
			}
			CoverageQueries.reused.Clear();
		}
		if (CoverageQueries.added.Count > 0)
		{
			foreach (CoverageQueries.Query query2 in CoverageQueries.added)
			{
				int id2 = query2.intern.id;
				Debug.Assert(id2 >= 0 && id2 <= CoverageQueries.pool.Count + CoverageQueries.added.Count, "Adding invalid query id");
				CoverageQueries.pool.Add(query2);
				CoverageQueries.changed.Add(id2);
			}
			CoverageQueries.added.Clear();
		}
		if (CoverageQueries.removed.Count > 0)
		{
			for (int i = 0; i < CoverageQueries.removed.Count; i++)
			{
				int num = CoverageQueries.removed[i];
				Debug.Assert(num >= 0 && num < CoverageQueries.pool.Count, "Removing invalid query id");
				CoverageQueries.pool[num].intern.Reset();
				CoverageQueries.pool[num].result.Reset();
				CoverageQueries.pool[num] = null;
				CoverageQueries.freed.Enqueue(num);
			}
			CoverageQueries.removed.Clear();
		}
		CoverageQueries.buffer.CheckResize(CoverageQueries.pool.Count);
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x000D7238 File Offset: 0x000D5438
	private void PrepareAndDispatch()
	{
		if (CoverageQueries.pool.Count > 0 && CoverageQueries.pool.Count <= CoverageQueries.buffer.inputData.Length)
		{
			Matrix4x4 worldToCameraMatrix = this.camera.worldToCameraMatrix;
			Matrix4x4 value = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, false) * worldToCameraMatrix;
			Matrix4x4 inverse = value.inverse;
			this.coverageMat.SetMatrix("_ViewProjMatrix", value);
			this.coverageMat.SetMatrix("_InvViewProjMatrix", inverse);
			this.coverageMat.SetFloat("_DepthBias", this.depthBias);
			this.coverageMat.SetFloat("_RcpCameraAspect", 1f / this.camera.aspect);
			if (CoverageQueries.changed.Count > 0)
			{
				for (int i = 0; i < CoverageQueries.changed.Count; i++)
				{
					int num = CoverageQueries.changed[i];
					Debug.Assert(CoverageQueries.changed[i] >= 0 && CoverageQueries.changed[i] < CoverageQueries.pool.Count);
					CoverageQueries.Query query = CoverageQueries.pool[num];
					if (query != null)
					{
						float x = query.input.position.x;
						float y = query.input.position.y;
						float z = query.input.position.z;
						float num2 = (float)Mathf.RoundToInt(query.input.radius * 10000f) + (float)query.input.sampleCount / 255f;
						num2 *= (float)((query.input.radiusSpace == CoverageQueries.RadiusSpace.ScreenNormalized) ? 1 : -1);
						CoverageQueries.buffer.inputData[num] = new Vector4(x, y, z, num2);
					}
				}
				CoverageQueries.changed.Clear();
			}
			CoverageQueries.buffer.UploadData();
			CoverageQueries.buffer.Dispatch(CoverageQueries.pool.Count);
		}
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x00020AD5 File Offset: 0x0001ECD5
	private void IssueRead()
	{
		if (CoverageQueries.pool.Count > 0)
		{
			CoverageQueries.buffer.IssueRead();
		}
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000D743C File Offset: 0x000D563C
	private void FetchAndAnalyseResults()
	{
		if (CoverageQueries.pool.Count > 0 && CoverageQueries.pool.Count <= CoverageQueries.buffer.resultData.Length)
		{
			CoverageQueries.buffer.GetResults();
			for (int i = 0; i < CoverageQueries.pool.Count; i++)
			{
				CoverageQueries.Query query = CoverageQueries.pool[i];
				if (query != null)
				{
					Debug.Assert(query.intern.id == i);
					query.result.passed = (int)CoverageQueries.buffer.resultData[i].r;
					query.result.coverage = (float)CoverageQueries.buffer.resultData[i].g / 255f;
					query.result.weightedCoverage = (float)CoverageQueries.buffer.resultData[i].b / 255f;
					query.result.originOccluded = (CoverageQueries.buffer.resultData[i].a > 0);
					query.result.originVisibility = 2f * Mathf.Clamp01(0.5f - query.result.coverage) * (float)(query.result.originOccluded ? 0 : 1);
					if (query.result.frame < 0)
					{
						query.result.smoothCoverage = query.result.coverage;
						query.result.weightedSmoothCoverage = query.result.weightedCoverage;
						query.result.originSmoothVisibility = query.result.originVisibility;
					}
					else
					{
						float t = Time.deltaTime * query.input.smoothingSpeed;
						query.result.smoothCoverage = Mathf.Lerp(query.result.smoothCoverage, query.result.coverage, t);
						query.result.weightedSmoothCoverage = Mathf.Lerp(query.result.weightedSmoothCoverage, query.result.weightedCoverage, t);
						query.result.originSmoothVisibility = Mathf.Lerp(query.result.originSmoothVisibility, query.result.originVisibility, t);
					}
					query.result.frame = Time.frameCount;
				}
			}
		}
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x000D7678 File Offset: 0x000D5878
	public static void RegisterQuery(CoverageQueries.Query query)
	{
		Debug.Assert(query.input.sampleCount >= 1 && query.input.sampleCount <= 256, "RegisterQuery failed sample count check");
		Debug.Assert(query.input.radius >= 0f, "RegisterQuery failed with negative radius");
		int num = query.intern.id;
		if (num < 0)
		{
			if (CoverageQueries.freed.Count > 0)
			{
				num = CoverageQueries.freed.Dequeue();
				CoverageQueries.reused.Add(query);
			}
			else
			{
				num = CoverageQueries.pool.Count + CoverageQueries.added.Count;
				CoverageQueries.added.Add(query);
			}
			query.intern.id = num;
		}
	}

	// Token: 0x06002A21 RID: 10785 RVA: 0x000D7738 File Offset: 0x000D5938
	public static void UnregisterQuery(CoverageQueries.Query query)
	{
		int id = query.intern.id;
		if (id >= 0 && id < CoverageQueries.pool.Count)
		{
			CoverageQueries.removed.Add(id);
		}
	}

	// Token: 0x06002A22 RID: 10786 RVA: 0x000D7770 File Offset: 0x000D5970
	public static void UpdateQuery(CoverageQueries.Query query)
	{
		int id = query.intern.id;
		if (id >= 0 && id < CoverageQueries.pool.Count)
		{
			CoverageQueries.changed.Add(id);
		}
	}

	// Token: 0x02000792 RID: 1938
	public class BufferSet
	{
		// Token: 0x040025AA RID: 9642
		public int width;

		// Token: 0x040025AB RID: 9643
		public int height;

		// Token: 0x040025AC RID: 9644
		public Texture2D inputTexture;

		// Token: 0x040025AD RID: 9645
		public RenderTexture resultTexture;

		// Token: 0x040025AE RID: 9646
		public Color[] inputData = new Color[0];

		// Token: 0x040025AF RID: 9647
		public Color32[] resultData = new Color32[0];

		// Token: 0x040025B0 RID: 9648
		private Material coverageMat;

		// Token: 0x040025B1 RID: 9649
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x040025B2 RID: 9650
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		// Token: 0x06002A25 RID: 10789 RVA: 0x00020B01 File Offset: 0x0001ED01
		public void Attach(Material coverageMat)
		{
			this.coverageMat = coverageMat;
		}

		// Token: 0x06002A26 RID: 10790 RVA: 0x000D7818 File Offset: 0x000D5A18
		public void Dispose(bool data = true)
		{
			if (this.inputTexture != null)
			{
				Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
			}
		}

		// Token: 0x06002A27 RID: 10791 RVA: 0x000D7888 File Offset: 0x000D5A88
		public bool CheckResize(int count)
		{
			if (count > this.inputData.Length || (this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				this.width = Mathf.CeilToInt(Mathf.Sqrt((float)count));
				this.height = Mathf.CeilToInt((float)count / (float)this.width);
				this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true);
				this.inputTexture.name = "_Input";
				this.inputTexture.filterMode = FilterMode.Point;
				this.inputTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				this.resultTexture.name = "_Result";
				this.resultTexture.filterMode = FilterMode.Point;
				this.resultTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture.useMipMap = false;
				this.resultTexture.Create();
				int num = this.resultData.Length;
				int num2 = this.width * this.height;
				Array.Resize<Color>(ref this.inputData, num2);
				Array.Resize<Color32>(ref this.resultData, num2);
				Color32 color = new Color32(byte.MaxValue, 0, 0, 0);
				for (int i = num; i < num2; i++)
				{
					this.resultData[i] = color;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06002A28 RID: 10792 RVA: 0x00020B0A File Offset: 0x0001ED0A
		public void UploadData()
		{
			if (this.inputData.Length != 0)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
			}
		}

		// Token: 0x06002A29 RID: 10793 RVA: 0x000D79E4 File Offset: 0x000D5BE4
		public void Dispatch(int count)
		{
			if (this.inputData.Length != 0)
			{
				RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
				this.coverageMat.SetTexture("_Input", this.inputTexture);
				Graphics.Blit(this.inputTexture, this.resultTexture, this.coverageMat, 0);
				Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			}
		}

		// Token: 0x06002A2A RID: 10794 RVA: 0x00020B31 File Offset: 0x0001ED31
		public void IssueRead()
		{
			if (this.asyncRequests.Count < 10)
			{
				this.asyncRequests.Enqueue(AsyncGPUReadback.Request(this.resultTexture, 0, null));
			}
		}

		// Token: 0x06002A2B RID: 10795 RVA: 0x000D7A3C File Offset: 0x000D5C3C
		public void GetResults()
		{
			if (this.resultData.Length != 0)
			{
				while (this.asyncRequests.Count > 0)
				{
					AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
					if (asyncGPUReadbackRequest.hasError)
					{
						this.asyncRequests.Dequeue();
					}
					else
					{
						if (!asyncGPUReadbackRequest.done)
						{
							break;
						}
						NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
						for (int i = 0; i < data.Length; i++)
						{
							this.resultData[i] = data[i];
						}
						this.asyncRequests.Dequeue();
					}
				}
			}
		}
	}

	// Token: 0x02000793 RID: 1939
	public enum RadiusSpace
	{
		// Token: 0x040025B4 RID: 9652
		ScreenNormalized,
		// Token: 0x040025B5 RID: 9653
		World
	}

	// Token: 0x02000794 RID: 1940
	public class Query
	{
		// Token: 0x040025B6 RID: 9654
		public CoverageQueries.Query.Input input;

		// Token: 0x040025B7 RID: 9655
		public CoverageQueries.Query.Internal intern;

		// Token: 0x040025B8 RID: 9656
		public CoverageQueries.Query.Result result;

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06002A2D RID: 10797 RVA: 0x00020B85 File Offset: 0x0001ED85
		public bool IsRegistered
		{
			get
			{
				return this.intern.id >= 0;
			}
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x00020B98 File Offset: 0x0001ED98
		private void Reset()
		{
			this.intern.Reset();
			this.result.Reset();
		}

		// Token: 0x06002A2F RID: 10799 RVA: 0x00020BB0 File Offset: 0x0001EDB0
		public Query()
		{
			this.Reset();
		}

		// Token: 0x06002A30 RID: 10800 RVA: 0x000D7ACC File Offset: 0x000D5CCC
		public Query(Vector3 position, CoverageQueries.RadiusSpace radiusSpace, float radius, int sampleCount, float smoothingSpeed = 15f)
		{
			this.Reset();
			this.input.position = position;
			this.input.radiusSpace = radiusSpace;
			this.input.radius = radius;
			this.input.sampleCount = sampleCount;
			this.input.smoothingSpeed = smoothingSpeed;
		}

		// Token: 0x06002A31 RID: 10801 RVA: 0x00020BBE File Offset: 0x0001EDBE
		public void Register()
		{
			CoverageQueries.RegisterQuery(this);
		}

		// Token: 0x06002A32 RID: 10802 RVA: 0x00020BC6 File Offset: 0x0001EDC6
		public void Update(Vector3 position, float radius)
		{
			if (this.intern.id >= 0)
			{
				this.input.position = position;
				this.input.radius = radius;
				CoverageQueries.UpdateQuery(this);
			}
		}

		// Token: 0x06002A33 RID: 10803 RVA: 0x00020BF4 File Offset: 0x0001EDF4
		public void UpdatePosition(Vector3 position)
		{
			this.input.position = position;
			CoverageQueries.UpdateQuery(this);
		}

		// Token: 0x06002A34 RID: 10804 RVA: 0x00020C08 File Offset: 0x0001EE08
		public void UpdateRadius(float radius)
		{
			this.input.radius = radius;
			CoverageQueries.UpdateQuery(this);
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x00020C1C File Offset: 0x0001EE1C
		public void Unregister()
		{
			CoverageQueries.UnregisterQuery(this);
		}

		// Token: 0x02000795 RID: 1941
		public struct Input
		{
			// Token: 0x040025B9 RID: 9657
			public Vector3 position;

			// Token: 0x040025BA RID: 9658
			public CoverageQueries.RadiusSpace radiusSpace;

			// Token: 0x040025BB RID: 9659
			public float radius;

			// Token: 0x040025BC RID: 9660
			public int sampleCount;

			// Token: 0x040025BD RID: 9661
			public float smoothingSpeed;
		}

		// Token: 0x02000796 RID: 1942
		public struct Internal
		{
			// Token: 0x040025BE RID: 9662
			public int id;

			// Token: 0x06002A36 RID: 10806 RVA: 0x00020C24 File Offset: 0x0001EE24
			public void Reset()
			{
				this.id = -1;
			}
		}

		// Token: 0x02000797 RID: 1943
		public struct Result
		{
			// Token: 0x040025BF RID: 9663
			public int passed;

			// Token: 0x040025C0 RID: 9664
			public float coverage;

			// Token: 0x040025C1 RID: 9665
			public float smoothCoverage;

			// Token: 0x040025C2 RID: 9666
			public float weightedCoverage;

			// Token: 0x040025C3 RID: 9667
			public float weightedSmoothCoverage;

			// Token: 0x040025C4 RID: 9668
			public bool originOccluded;

			// Token: 0x040025C5 RID: 9669
			public int frame;

			// Token: 0x040025C6 RID: 9670
			public float originVisibility;

			// Token: 0x040025C7 RID: 9671
			public float originSmoothVisibility;

			// Token: 0x06002A37 RID: 10807 RVA: 0x000D7B24 File Offset: 0x000D5D24
			public void Reset()
			{
				this.passed = 0;
				this.coverage = 1f;
				this.smoothCoverage = 1f;
				this.weightedCoverage = 1f;
				this.weightedSmoothCoverage = 1f;
				this.originOccluded = false;
				this.frame = -1;
				this.originVisibility = 0f;
				this.originSmoothVisibility = 0f;
			}
		}
	}
}
