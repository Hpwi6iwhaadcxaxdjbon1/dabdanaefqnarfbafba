using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ConVar;
using RustNative;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007B1 RID: 1969
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class OcclusionCulling : MonoBehaviour
{
	// Token: 0x0400265D RID: 9821
	public ComputeShader computeShader;

	// Token: 0x0400265E RID: 9822
	public bool usePixelShaderFallback = true;

	// Token: 0x0400265F RID: 9823
	public bool useAsyncReadAPI;

	// Token: 0x04002660 RID: 9824
	private Camera camera;

	// Token: 0x04002661 RID: 9825
	private const int ComputeThreadsPerGroup = 64;

	// Token: 0x04002662 RID: 9826
	private const int InputBufferStride = 16;

	// Token: 0x04002663 RID: 9827
	private const int ResultBufferStride = 4;

	// Token: 0x04002664 RID: 9828
	private const int OccludeeMaxSlotsPerPool = 1048576;

	// Token: 0x04002665 RID: 9829
	private const int OccludeePoolGranularity = 2048;

	// Token: 0x04002666 RID: 9830
	private const int StateBufferGranularity = 2048;

	// Token: 0x04002667 RID: 9831
	private const int GridBufferGranularity = 256;

	// Token: 0x04002668 RID: 9832
	private static Queue<OccludeeState> statePool = new Queue<OccludeeState>();

	// Token: 0x04002669 RID: 9833
	private static OcclusionCulling.SimpleList<OccludeeState> staticOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x0400266A RID: 9834
	private static OcclusionCulling.SimpleList<OccludeeState.State> staticStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x0400266B RID: 9835
	private static OcclusionCulling.SimpleList<int> staticVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x0400266C RID: 9836
	private static OcclusionCulling.SimpleList<OccludeeState> dynamicOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x0400266D RID: 9837
	private static OcclusionCulling.SimpleList<OccludeeState.State> dynamicStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x0400266E RID: 9838
	private static OcclusionCulling.SimpleList<int> dynamicVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x0400266F RID: 9839
	private static List<int> staticChanged = new List<int>(256);

	// Token: 0x04002670 RID: 9840
	private static Queue<int> staticRecycled = new Queue<int>();

	// Token: 0x04002671 RID: 9841
	private static List<int> dynamicChanged = new List<int>(1024);

	// Token: 0x04002672 RID: 9842
	private static Queue<int> dynamicRecycled = new Queue<int>();

	// Token: 0x04002673 RID: 9843
	private static OcclusionCulling.BufferSet staticSet = new OcclusionCulling.BufferSet();

	// Token: 0x04002674 RID: 9844
	private static OcclusionCulling.BufferSet dynamicSet = new OcclusionCulling.BufferSet();

	// Token: 0x04002675 RID: 9845
	private static OcclusionCulling.BufferSet gridSet = new OcclusionCulling.BufferSet();

	// Token: 0x04002676 RID: 9846
	private Vector4[] frustumPlanes = new Vector4[6];

	// Token: 0x04002677 RID: 9847
	private string[] frustumPropNames = new string[6];

	// Token: 0x04002678 RID: 9848
	private float[] matrixToFloatTemp = new float[16];

	// Token: 0x04002679 RID: 9849
	private Material fallbackMat;

	// Token: 0x0400267A RID: 9850
	private Material depthCopyMat;

	// Token: 0x0400267B RID: 9851
	private Matrix4x4 viewMatrix;

	// Token: 0x0400267C RID: 9852
	private Matrix4x4 projMatrix;

	// Token: 0x0400267D RID: 9853
	private Matrix4x4 viewProjMatrix;

	// Token: 0x0400267E RID: 9854
	private Matrix4x4 prevViewProjMatrix;

	// Token: 0x0400267F RID: 9855
	private Matrix4x4 invViewProjMatrix;

	// Token: 0x04002680 RID: 9856
	private bool useNativePath = true;

	// Token: 0x04002681 RID: 9857
	private static OcclusionCulling instance;

	// Token: 0x04002682 RID: 9858
	private static GraphicsDeviceType[] supportedDeviceTypes;

	// Token: 0x04002683 RID: 9859
	private static bool _enabled;

	// Token: 0x04002684 RID: 9860
	private static bool _safeMode;

	// Token: 0x04002685 RID: 9861
	private static OcclusionCulling.DebugFilter _debugShow;

	// Token: 0x04002686 RID: 9862
	public OcclusionCulling.DebugSettings debugSettings = new OcclusionCulling.DebugSettings();

	// Token: 0x04002687 RID: 9863
	private Material debugMipMat;

	// Token: 0x04002688 RID: 9864
	private const float debugDrawDuration = 0.0334f;

	// Token: 0x04002689 RID: 9865
	private Material downscaleMat;

	// Token: 0x0400268A RID: 9866
	private Material blitCopyMat;

	// Token: 0x0400268B RID: 9867
	private int hiZLevelCount;

	// Token: 0x0400268C RID: 9868
	private int hiZWidth;

	// Token: 0x0400268D RID: 9869
	private int hiZHeight;

	// Token: 0x0400268E RID: 9870
	private RenderTexture depthTexture;

	// Token: 0x0400268F RID: 9871
	private RenderTexture hiZTexture;

	// Token: 0x04002690 RID: 9872
	private RenderTexture[] hiZLevels;

	// Token: 0x04002691 RID: 9873
	private const int GridCellsPerAxis = 2097152;

	// Token: 0x04002692 RID: 9874
	private const int GridHalfCellsPerAxis = 1048576;

	// Token: 0x04002693 RID: 9875
	private const int GridMinHalfCellsPerAxis = -1048575;

	// Token: 0x04002694 RID: 9876
	private const int GridMaxHalfCellsPerAxis = 1048575;

	// Token: 0x04002695 RID: 9877
	private const float GridCellSize = 100f;

	// Token: 0x04002696 RID: 9878
	private const float GridHalfCellSize = 50f;

	// Token: 0x04002697 RID: 9879
	private const float GridRcpCellSize = 0.01f;

	// Token: 0x04002698 RID: 9880
	private const int GridPoolCapacity = 16384;

	// Token: 0x04002699 RID: 9881
	private const int GridPoolGranularity = 4096;

	// Token: 0x0400269A RID: 9882
	private static OcclusionCulling.HashedPool<OcclusionCulling.Cell> grid;

	// Token: 0x0400269B RID: 9883
	private static Queue<OcclusionCulling.Cell> gridChanged;

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06002AB4 RID: 10932 RVA: 0x0002142E File Offset: 0x0001F62E
	public static OcclusionCulling Instance
	{
		get
		{
			return OcclusionCulling.instance;
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06002AB5 RID: 10933 RVA: 0x00021435 File Offset: 0x0001F635
	public static bool Supported
	{
		get
		{
			return Enumerable.Contains<GraphicsDeviceType>(OcclusionCulling.supportedDeviceTypes, SystemInfo.graphicsDeviceType);
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06002AB6 RID: 10934 RVA: 0x00021446 File Offset: 0x0001F646
	// (set) Token: 0x06002AB7 RID: 10935 RVA: 0x0002144D File Offset: 0x0001F64D
	public static bool Enabled
	{
		get
		{
			return OcclusionCulling._enabled;
		}
		set
		{
			OcclusionCulling._enabled = value;
			if (OcclusionCulling.instance != null)
			{
				OcclusionCulling.instance.enabled = value;
			}
		}
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06002AB8 RID: 10936 RVA: 0x0002146D File Offset: 0x0001F66D
	// (set) Token: 0x06002AB9 RID: 10937 RVA: 0x00021474 File Offset: 0x0001F674
	public static bool SafeMode
	{
		get
		{
			return OcclusionCulling._safeMode;
		}
		set
		{
			OcclusionCulling._safeMode = value;
		}
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06002ABA RID: 10938 RVA: 0x0002147C File Offset: 0x0001F67C
	// (set) Token: 0x06002ABB RID: 10939 RVA: 0x00021483 File Offset: 0x0001F683
	public static OcclusionCulling.DebugFilter DebugShow
	{
		get
		{
			return OcclusionCulling._debugShow;
		}
		set
		{
			OcclusionCulling._debugShow = value;
		}
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000D9A20 File Offset: 0x000D7C20
	private static void GrowStatePool()
	{
		for (int i = 0; i < 2048; i++)
		{
			OcclusionCulling.statePool.Enqueue(new OccludeeState());
		}
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x0002148B File Offset: 0x0001F68B
	private static OccludeeState Allocate()
	{
		if (OcclusionCulling.statePool.Count == 0)
		{
			OcclusionCulling.GrowStatePool();
		}
		return OcclusionCulling.statePool.Dequeue();
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x000214A8 File Offset: 0x0001F6A8
	private static void Release(OccludeeState state)
	{
		OcclusionCulling.statePool.Enqueue(state);
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000D9A4C File Offset: 0x000D7C4C
	private void Awake()
	{
		OcclusionCulling.instance = this;
		this.camera = base.GetComponent<Camera>();
		for (int i = 0; i < 6; i++)
		{
			this.frustumPropNames[i] = "_FrustumPlane" + i;
		}
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x000D9A90 File Offset: 0x000D7C90
	private void OnEnable()
	{
		if (!OcclusionCulling.Enabled)
		{
			OcclusionCulling.Enabled = false;
			return;
		}
		if (!OcclusionCulling.Supported)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to graphics device type " + SystemInfo.graphicsDeviceType + " not supported.");
			OcclusionCulling.Enabled = false;
			return;
		}
		this.usePixelShaderFallback = (this.usePixelShaderFallback || !SystemInfo.supportsComputeShaders || this.computeShader == null || !this.computeShader.HasKernel("compute_cull"));
		this.useNativePath = (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 && this.SupportsNativePath());
		this.useAsyncReadAPI = (!this.useNativePath && SystemInfo.supportsAsyncGPUReadback);
		if (!this.useNativePath && !this.useAsyncReadAPI)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to unsupported Async GPU Reads on device " + SystemInfo.graphicsDeviceType);
			OcclusionCulling.Enabled = false;
			return;
		}
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			OcclusionCulling.staticChanged.Add(i);
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			OcclusionCulling.dynamicChanged.Add(j);
		}
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat = new Material(Shader.Find("Hidden/OcclusionCulling/Culling"))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		OcclusionCulling.staticSet.Attach(this);
		OcclusionCulling.dynamicSet.Attach(this);
		OcclusionCulling.gridSet.Attach(this);
		this.depthCopyMat = new Material(Shader.Find("Hidden/OcclusionCulling/DepthCopy"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.InitializeHiZMap();
		this.UpdateCameraMatrices(true);
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x000D9C20 File Offset: 0x000D7E20
	private bool SupportsNativePath()
	{
		bool result = true;
		try
		{
			OccludeeState.State state = default(OccludeeState.State);
			Color32 color = new Color32(0, 0, 0, 0);
			Vector4 zero = Vector4.zero;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			OcclusionCulling.ProcessOccludees_Native(ref state, ref num, 0, ref color, 0, ref num2, ref num3, ref zero, 0f, 0U);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log("[OcclusionCulling] Fast native path not available. Reverting to managed fallback.");
			result = false;
		}
		return result;
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x000D9C90 File Offset: 0x000D7E90
	private void OnDisable()
	{
		if (this.fallbackMat != null)
		{
			Object.DestroyImmediate(this.fallbackMat);
			this.fallbackMat = null;
		}
		if (this.depthCopyMat != null)
		{
			Object.DestroyImmediate(this.depthCopyMat);
			this.depthCopyMat = null;
		}
		OcclusionCulling.staticSet.Dispose(true);
		OcclusionCulling.dynamicSet.Dispose(true);
		OcclusionCulling.gridSet.Dispose(true);
		this.FinalizeHiZMap();
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x000D9D04 File Offset: 0x000D7F04
	public static void MakeAllVisible()
	{
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			if (OcclusionCulling.staticOccludees[i] != null)
			{
				OcclusionCulling.staticOccludees[i].MakeVisible();
			}
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			if (OcclusionCulling.dynamicOccludees[j] != null)
			{
				OcclusionCulling.dynamicOccludees[j].MakeVisible();
			}
		}
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x000214B5 File Offset: 0x0001F6B5
	private void Update()
	{
		if (!OcclusionCulling.Enabled)
		{
			base.enabled = false;
			return;
		}
		this.CheckResizeHiZMap();
		this.DebugUpdate();
		this.DebugDraw();
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x000D9D78 File Offset: 0x000D7F78
	public static void RecursiveAddOccludees<T>(Transform transform, float minTimeVisible = 0.1f, bool isStatic = true, bool stickyGizmos = false) where T : Occludee
	{
		Renderer component = transform.GetComponent<Renderer>();
		Collider component2 = transform.GetComponent<Collider>();
		if (component != null && component2 != null)
		{
			T t = component.gameObject.GetComponent<T>();
			t = ((t == null) ? component.gameObject.AddComponent<T>() : t);
			t.minTimeVisible = minTimeVisible;
			t.isStatic = isStatic;
			t.stickyGizmos = stickyGizmos;
			t.Register();
		}
		foreach (object obj in transform)
		{
			OcclusionCulling.RecursiveAddOccludees<T>((Transform)obj, minTimeVisible, isStatic, stickyGizmos);
		}
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x000D9E48 File Offset: 0x000D8048
	private static int FindFreeSlot(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled)
	{
		int result;
		if (recycled.Count > 0)
		{
			result = recycled.Dequeue();
		}
		else
		{
			if (occludees.Count == occludees.Capacity)
			{
				int num = Mathf.Min(occludees.Capacity + 2048, 1048576);
				if (num > 0)
				{
					occludees.Capacity = num;
					states.Capacity = num;
				}
			}
			if (occludees.Count < occludees.Capacity)
			{
				result = occludees.Count;
				occludees.Add(null);
				states.Add(default(OccludeeState.State));
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x000D9ED0 File Offset: 0x000D80D0
	public static OccludeeState GetStateById(int id)
	{
		if (id < 0 || id >= 2097152)
		{
			return null;
		}
		bool flag = id < 1048576;
		int index = flag ? id : (id - 1048576);
		if (flag)
		{
			return OcclusionCulling.staticOccludees[index];
		}
		return OcclusionCulling.dynamicOccludees[index];
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x000D9F1C File Offset: 0x000D811C
	public static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged = null)
	{
		int num;
		if (isStatic)
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged, OcclusionCulling.staticSet, OcclusionCulling.staticVisibilityChanged);
		}
		else
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicVisibilityChanged);
		}
		if (num >= 0 && !isStatic)
		{
			return num + 1048576;
		}
		return num;
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x000D9FA0 File Offset: 0x000D81A0
	private static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged, OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled, List<int> changed, OcclusionCulling.BufferSet set, OcclusionCulling.SimpleList<int> visibilityChanged)
	{
		int num = OcclusionCulling.FindFreeSlot(occludees, states, recycled);
		if (num >= 0)
		{
			Vector4 sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OccludeeState occludeeState = OcclusionCulling.Allocate().Initialize(states, set, num, sphereBounds, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged);
			occludeeState.cell = OcclusionCulling.RegisterToGrid(occludeeState);
			occludees[num] = occludeeState;
			changed.Add(num);
			if (states.array[num].isVisible > 0 != occludeeState.cell.isVisible)
			{
				visibilityChanged.Add(num);
			}
		}
		return num;
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x000DA038 File Offset: 0x000D8238
	public static void UnregisterOccludee(int id)
	{
		if (id >= 0 && id < 2097152)
		{
			bool flag = id < 1048576;
			int slot = flag ? id : (id - 1048576);
			if (flag)
			{
				OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.staticOccludees, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged);
				return;
			}
			OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged);
		}
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x000214D8 File Offset: 0x0001F6D8
	private static void UnregisterOccludee(int slot, OcclusionCulling.SimpleList<OccludeeState> occludees, Queue<int> recycled, List<int> changed)
	{
		OccludeeState occludeeState = occludees[slot];
		OcclusionCulling.UnregisterFromGrid(occludeeState);
		recycled.Enqueue(slot);
		changed.Add(slot);
		OcclusionCulling.Release(occludeeState);
		occludees[slot] = null;
		occludeeState.Invalidate();
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x000DA094 File Offset: 0x000D8294
	public static void UpdateDynamicOccludee(int id, Vector3 center, float radius)
	{
		int num = id - 1048576;
		if (num >= 0 && num < 1048576)
		{
			OcclusionCulling.dynamicStates.array[num].sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OcclusionCulling.dynamicChanged.Add(num);
		}
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x000DA0F0 File Offset: 0x000D82F0
	private void UpdateBuffers(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, List<int> changed, bool isStatic)
	{
		int count = occludees.Count;
		bool flag = changed.Count > 0;
		set.CheckResize(count, 2048);
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				if (!isStatic)
				{
					OcclusionCulling.UpdateInGrid(occludeeState);
				}
				set.inputData[num] = states[num].sphereBounds;
			}
			else
			{
				set.inputData[num] = Vector4.zero;
			}
		}
		changed.Clear();
		if (flag)
		{
			set.UploadData();
		}
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x000DA194 File Offset: 0x000D8394
	private void UpdateCameraMatrices(bool starting = false)
	{
		if (!starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
		Matrix4x4 proj = Matrix4x4.Perspective(this.camera.fieldOfView, this.camera.aspect, this.camera.nearClipPlane, this.camera.farClipPlane);
		this.viewMatrix = this.camera.worldToCameraMatrix;
		this.projMatrix = GL.GetGPUProjectionMatrix(proj, false);
		this.viewProjMatrix = this.projMatrix * this.viewMatrix;
		this.invViewProjMatrix = Matrix4x4.Inverse(this.viewProjMatrix);
		if (starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x000DA238 File Offset: 0x000D8438
	private void OnPreCull()
	{
		this.UpdateCameraMatrices(false);
		this.GenerateHiZMipChain();
		this.PrepareAndDispatch();
		this.IssueRead();
		if (OcclusionCulling.grid.Size <= OcclusionCulling.gridSet.resultData.Length)
		{
			this.RetrieveAndApplyVisibility();
			return;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"[OcclusionCulling] Grid size and result capacity are out of sync: ",
			OcclusionCulling.grid.Size,
			", ",
			OcclusionCulling.gridSet.resultData.Length
		}));
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x000DA2C4 File Offset: 0x000D84C4
	private void OnPostRender()
	{
		bool sRGBWrite = GL.sRGBWrite;
		RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
		RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
		this.GrabDepthTexture();
		UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
		GL.sRGBWrite = sRGBWrite;
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x000DA2F4 File Offset: 0x000D84F4
	private float[] MatrixToFloatArray(Matrix4x4 m)
	{
		int i = 0;
		int num = 0;
		while (i < 4)
		{
			for (int j = 0; j < 4; j++)
			{
				this.matrixToFloatTemp[num++] = m[j, i];
			}
			i++;
		}
		return this.matrixToFloatTemp;
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x000DA338 File Offset: 0x000D8538
	private void PrepareAndDispatch()
	{
		Vector2 v = new Vector2((float)this.hiZWidth, (float)this.hiZHeight);
		OcclusionCulling.ExtractFrustum(this.viewProjMatrix, ref this.frustumPlanes);
		bool noDataVisible = Culling.noDataVisible;
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat.SetTexture("_HiZMap", this.hiZTexture);
			this.fallbackMat.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.fallbackMat.SetMatrix("_ViewMatrix", this.viewMatrix);
			this.fallbackMat.SetMatrix("_ProjMatrix", this.projMatrix);
			this.fallbackMat.SetMatrix("_ViewProjMatrix", this.viewProjMatrix);
			this.fallbackMat.SetVector("_CameraWorldPos", base.transform.position);
			this.fallbackMat.SetVector("_ViewportSize", v);
			this.fallbackMat.SetFloat("_FrustumCull", noDataVisible ? 0f : 1f);
			for (int i = 0; i < 6; i++)
			{
				this.fallbackMat.SetVector(this.frustumPropNames[i], this.frustumPlanes[i]);
			}
		}
		else
		{
			this.computeShader.SetTexture(0, "_HiZMap", this.hiZTexture);
			this.computeShader.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.computeShader.SetFloats("_ViewMatrix", this.MatrixToFloatArray(this.viewMatrix));
			this.computeShader.SetFloats("_ProjMatrix", this.MatrixToFloatArray(this.projMatrix));
			this.computeShader.SetFloats("_ViewProjMatrix", this.MatrixToFloatArray(this.viewProjMatrix));
			this.computeShader.SetVector("_CameraWorldPos", base.transform.position);
			this.computeShader.SetVector("_ViewportSize", v);
			this.computeShader.SetFloat("_FrustumCull", noDataVisible ? 0f : 1f);
			for (int j = 0; j < 6; j++)
			{
				this.computeShader.SetVector(this.frustumPropNames[j], this.frustumPlanes[j]);
			}
		}
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticSet, OcclusionCulling.staticChanged, true);
			OcclusionCulling.staticSet.Dispatch(OcclusionCulling.staticOccludees.Count);
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicChanged, false);
			OcclusionCulling.dynamicSet.Dispatch(OcclusionCulling.dynamicOccludees.Count);
		}
		this.UpdateGridBuffers();
		OcclusionCulling.gridSet.Dispatch(OcclusionCulling.grid.Size);
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x000DA60C File Offset: 0x000D880C
	private void IssueRead()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.IssueRead();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.IssueRead();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.IssueRead();
		}
		GL.IssuePluginEvent(RustNative.Graphics.GetRenderEventFunc(), 2);
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x000DA66C File Offset: 0x000D886C
	public void ResetTiming(OcclusionCulling.SmartList bucket)
	{
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null)
			{
				occludeeState.states.array[occludeeState.slot].waitTime = 0f;
			}
		}
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x000DA6B8 File Offset: 0x000D88B8
	public void ResetTiming()
	{
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null)
			{
				this.ResetTiming(cell.staticBucket);
				this.ResetTiming(cell.dynamicBucket);
			}
		}
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x000DA704 File Offset: 0x000D8904
	private static bool FrustumCull(Vector4[] planes, Vector4 testSphere)
	{
		for (int i = 0; i < 6; i++)
		{
			if (planes[i].x * testSphere.x + planes[i].y * testSphere.y + planes[i].z * testSphere.z + planes[i].w < -testSphere.w)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x000DA774 File Offset: 0x000D8974
	private static int ProcessOccludees_Safe(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SmartList bucket, Color32[] results, OcclusionCulling.SimpleList<int> changed, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null && occludeeState.slot < results.Length)
			{
				int slot = occludeeState.slot;
				OccludeeState.State state = states[slot];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[slot].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = (time < state.waitTime);
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						changed.Add(slot);
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[slot] = state;
				num += (int)state.isVisible;
			}
		}
		return num;
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x000DA860 File Offset: 0x000D8A60
	private static int ProcessOccludees_Fast(OccludeeState.State[] states, int[] bucket, int bucketCount, Color32[] results, int resultCount, int[] changed, ref int changedCount, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucketCount; i++)
		{
			int num2 = bucket[i];
			if (num2 >= 0 && num2 < resultCount && states[num2].active != 0)
			{
				OccludeeState.State state = states[num2];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[num2].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = (time < state.waitTime);
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						int num3 = changedCount;
						changedCount = num3 + 1;
						changed[num3] = num2;
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[num2] = state;
				num += (flag2 ? 0 : 1);
			}
		}
		return num;
	}

	// Token: 0x06002AD9 RID: 10969
	[DllImport("Renderer", EntryPoint = "CULL_ProcessOccludees")]
	private static extern int ProcessOccludees_Native(ref OccludeeState.State states, ref int bucket, int bucketCount, ref Color32 results, int resultCount, ref int changed, ref int changedCount, ref Vector4 frustumPlanes, float time, uint frame);

	// Token: 0x06002ADA RID: 10970 RVA: 0x000DA94C File Offset: 0x000D8B4C
	private void ApplyVisibility_Safe(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.staticStates, cell.staticBucket, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticVisibilityChanged, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.dynamicStates, cell.dynamicBucket, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicVisibilityChanged, this.frustumPlanes, time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x000DAA90 File Offset: 0x000D8C90
	private void ApplyVisibility_Fast(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.staticStates.array, cell.staticBucket.Slots, cell.staticBucket.Size, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticSet.resultData.Length, OcclusionCulling.staticVisibilityChanged.array, ref OcclusionCulling.staticVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.dynamicStates.array, cell.dynamicBucket.Slots, cell.dynamicBucket.Size, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicSet.resultData.Length, OcclusionCulling.dynamicVisibilityChanged.array, ref OcclusionCulling.dynamicVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x000DAC34 File Offset: 0x000D8E34
	private void ApplyVisibility_Native(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.staticStates.array[0], ref cell.staticBucket.Slots[0], cell.staticBucket.Size, ref OcclusionCulling.staticSet.resultData[0], OcclusionCulling.staticSet.resultData.Length, ref OcclusionCulling.staticVisibilityChanged.array[0], ref OcclusionCulling.staticVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.dynamicStates.array[0], ref cell.dynamicBucket.Slots[0], cell.dynamicBucket.Size, ref OcclusionCulling.dynamicSet.resultData[0], OcclusionCulling.dynamicSet.resultData.Length, ref OcclusionCulling.dynamicVisibilityChanged.array[0], ref OcclusionCulling.dynamicVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x000DAE1C File Offset: 0x000D901C
	private void ProcessCallbacks(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SimpleList<int> changed)
	{
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				bool flag = states.array[num].isVisible == 0;
				OcclusionCulling.OnVisibilityChanged onVisibilityChanged = occludeeState.onVisibilityChanged;
				if (onVisibilityChanged != null && (Object)onVisibilityChanged.Target != null)
				{
					onVisibilityChanged(flag);
				}
				if (occludeeState.slot >= 0)
				{
					states.array[occludeeState.slot].isVisible = (flag ? 1 : 0);
				}
			}
		}
		changed.Clear();
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x000DAEBC File Offset: 0x000D90BC
	public void RetrieveAndApplyVisibility()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.GetResults();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.GetResults();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.GetResults();
		}
		if (this.debugSettings.showAllVisible)
		{
			for (int i = 0; i < OcclusionCulling.staticSet.resultData.Length; i++)
			{
				OcclusionCulling.staticSet.resultData[i].r = 1;
			}
			for (int j = 0; j < OcclusionCulling.dynamicSet.resultData.Length; j++)
			{
				OcclusionCulling.dynamicSet.resultData[j].r = 1;
			}
			for (int k = 0; k < OcclusionCulling.gridSet.resultData.Length; k++)
			{
				OcclusionCulling.gridSet.resultData[k].r = 1;
			}
		}
		OcclusionCulling.staticVisibilityChanged.EnsureCapacity(OcclusionCulling.staticOccludees.Count);
		OcclusionCulling.dynamicVisibilityChanged.EnsureCapacity(OcclusionCulling.dynamicOccludees.Count);
		float time = UnityEngine.Time.time;
		uint frameCount = (uint)UnityEngine.Time.frameCount;
		if (this.useNativePath)
		{
			this.ApplyVisibility_Native(time, frameCount);
		}
		else
		{
			this.ApplyVisibility_Fast(time, frameCount);
		}
		this.ProcessCallbacks(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticVisibilityChanged);
		this.ProcessCallbacks(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicVisibilityChanged);
	}

	// Token: 0x06002ADF RID: 10975 RVA: 0x00021508 File Offset: 0x0001F708
	public static bool DebugFilterIsDynamic(int filter)
	{
		return filter == 1 || filter == 4;
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x00021514 File Offset: 0x0001F714
	public static bool DebugFilterIsStatic(int filter)
	{
		return filter == 2 || filter == 4;
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x00021520 File Offset: 0x0001F720
	public static bool DebugFilterIsGrid(int filter)
	{
		return filter == 3 || filter == 4;
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x0002152C File Offset: 0x0001F72C
	private void DebugInitialize()
	{
		this.debugMipMat = new Material(Shader.Find("Hidden/OcclusionCulling/DebugMip"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0002154B File Offset: 0x0001F74B
	private void DebugShutdown()
	{
		if (this.debugMipMat != null)
		{
			Object.DestroyImmediate(this.debugMipMat);
			this.debugMipMat = null;
		}
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x0002156D File Offset: 0x0001F76D
	private void DebugUpdate()
	{
		if (this.HiZReady)
		{
			this.debugSettings.showMainLod = Mathf.Clamp(this.debugSettings.showMainLod, 0, this.hiZLevels.Length - 1);
		}
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x000DB024 File Offset: 0x000D9224
	private void DebugDraw()
	{
		int num = (MainCamera.mainCamera != null) ? MainCamera.mainCamera.cullingMask : -1;
		if (this.debugSettings.showScreenBounds)
		{
			Matrix4x4 worldToCameraMatrix = this.camera.worldToCameraMatrix;
			Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, false);
			Matrix4x4 viewProjection = gpuprojectionMatrix * worldToCameraMatrix;
			Vector2 viewportSize = new Vector2((float)this.hiZWidth, (float)this.hiZHeight);
			OcclusionCulling.ExtractFrustum(viewProjection, ref this.frustumPlanes);
			for (int i = 0; i < OcclusionCulling.dynamicOccludees.Count; i++)
			{
				if (OcclusionCulling.dynamicOccludees[i] != null)
				{
					OcclusionCPUTest.CullSphere(this.camera, OcclusionCulling.dynamicStates[i].sphereBounds, worldToCameraMatrix, gpuprojectionMatrix, viewProjection, this.frustumPlanes, viewportSize);
				}
			}
			for (int j = 0; j < OcclusionCulling.staticOccludees.Count; j++)
			{
				if (OcclusionCulling.staticOccludees[j] != null)
				{
					OcclusionCPUTest.CullSphere(this.camera, OcclusionCulling.staticStates[j].sphereBounds, worldToCameraMatrix, gpuprojectionMatrix, viewProjection, this.frustumPlanes, viewportSize);
				}
			}
			for (int k = 0; k < OcclusionCulling.grid.Size; k++)
			{
				OcclusionCulling.Cell cell = OcclusionCulling.grid[k];
				if (cell != null)
				{
					OcclusionCPUTest.CullSphere(this.camera, cell.sphereBounds, worldToCameraMatrix, gpuprojectionMatrix, viewProjection, this.frustumPlanes, viewportSize);
				}
			}
		}
		if ((this.debugSettings.showMask & OcclusionCulling.DebugMask.Dynamic) != OcclusionCulling.DebugMask.Off || OcclusionCulling.DebugShow == OcclusionCulling.DebugFilter.Dynamic || OcclusionCulling.DebugShow == OcclusionCulling.DebugFilter.All)
		{
			for (int l = 0; l < OcclusionCulling.dynamicOccludees.Count; l++)
			{
				OccludeeState occludeeState = OcclusionCulling.dynamicOccludees[l];
				if (occludeeState != null)
				{
					bool flag = (num & 1 << occludeeState.layer) != 0;
					bool flag2 = (this.debugSettings.layerFilter & 1 << occludeeState.layer) != 0;
					if (flag && flag2)
					{
						OccludeeState.State state = OcclusionCulling.dynamicStates[l];
						float num2 = state.sphereBounds.w * 2f;
						UnityEngine.DDraw.Bounds(new Bounds(state.sphereBounds, new Vector3(num2, num2, num2)), (state.isVisible != 0) ? Color.green : Color.red, 0.0334f, false, false);
					}
				}
			}
		}
		if ((this.debugSettings.showMask & OcclusionCulling.DebugMask.Static) != OcclusionCulling.DebugMask.Off || OcclusionCulling.DebugShow == OcclusionCulling.DebugFilter.Static || OcclusionCulling.DebugShow == OcclusionCulling.DebugFilter.All)
		{
			for (int m = 0; m < OcclusionCulling.staticOccludees.Count; m++)
			{
				OccludeeState occludeeState2 = OcclusionCulling.staticOccludees[m];
				if (occludeeState2 != null)
				{
					bool flag3 = (num & 1 << occludeeState2.layer) != 0;
					bool flag4 = (this.debugSettings.layerFilter & 1 << occludeeState2.layer) != 0;
					if (flag3 && flag4)
					{
						OccludeeState.State state2 = OcclusionCulling.staticStates[m];
						float num3 = state2.sphereBounds.w * 2f;
						UnityEngine.DDraw.Bounds(new Bounds(state2.sphereBounds, new Vector3(num3, num3, num3)), (state2.isVisible != 0) ? Color.green : Color.red, 0.0334f, false, false);
					}
				}
			}
		}
		if ((this.debugSettings.showMask & OcclusionCulling.DebugMask.Grid) != OcclusionCulling.DebugMask.Off || OcclusionCulling.DebugShow == OcclusionCulling.DebugFilter.Grid || OcclusionCulling.DebugShow == OcclusionCulling.DebugFilter.All)
		{
			for (int n = 0; n < OcclusionCulling.grid.Size; n++)
			{
				OcclusionCulling.Cell cell2 = OcclusionCulling.grid[n];
				if (cell2 != null)
				{
					UnityEngine.DDraw.Bounds(cell2.bounds, cell2.isVisible ? Color.green : Color.red, 0.0334f, false, false);
				}
			}
		}
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x000DB3D8 File Offset: 0x000D95D8
	public static void NormalizePlane(ref Vector4 plane)
	{
		float num = Mathf.Sqrt(plane.x * plane.x + plane.y * plane.y + plane.z * plane.z);
		plane.x /= num;
		plane.y /= num;
		plane.z /= num;
		plane.w /= num;
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x000DB440 File Offset: 0x000D9640
	public static void ExtractFrustum(Matrix4x4 viewProjMatrix, ref Vector4[] planes)
	{
		planes[0].x = viewProjMatrix.m30 + viewProjMatrix.m00;
		planes[0].y = viewProjMatrix.m31 + viewProjMatrix.m01;
		planes[0].z = viewProjMatrix.m32 + viewProjMatrix.m02;
		planes[0].w = viewProjMatrix.m33 + viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[0]);
		planes[1].x = viewProjMatrix.m30 - viewProjMatrix.m00;
		planes[1].y = viewProjMatrix.m31 - viewProjMatrix.m01;
		planes[1].z = viewProjMatrix.m32 - viewProjMatrix.m02;
		planes[1].w = viewProjMatrix.m33 - viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[1]);
		planes[2].x = viewProjMatrix.m30 - viewProjMatrix.m10;
		planes[2].y = viewProjMatrix.m31 - viewProjMatrix.m11;
		planes[2].z = viewProjMatrix.m32 - viewProjMatrix.m12;
		planes[2].w = viewProjMatrix.m33 - viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[2]);
		planes[3].x = viewProjMatrix.m30 + viewProjMatrix.m10;
		planes[3].y = viewProjMatrix.m31 + viewProjMatrix.m11;
		planes[3].z = viewProjMatrix.m32 + viewProjMatrix.m12;
		planes[3].w = viewProjMatrix.m33 + viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[3]);
		planes[4].x = viewProjMatrix.m20;
		planes[4].y = viewProjMatrix.m21;
		planes[4].z = viewProjMatrix.m22;
		planes[4].w = viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[4]);
		planes[5].x = viewProjMatrix.m30 - viewProjMatrix.m20;
		planes[5].y = viewProjMatrix.m31 - viewProjMatrix.m21;
		planes[5].z = viewProjMatrix.m32 - viewProjMatrix.m22;
		planes[5].w = viewProjMatrix.m33 - viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[5]);
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06002AE8 RID: 10984 RVA: 0x0002159D File Offset: 0x0001F79D
	public bool HiZReady
	{
		get
		{
			return this.hiZTexture != null && this.hiZWidth > 0 && this.hiZHeight > 0;
		}
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x000DB6F0 File Offset: 0x000D98F0
	public void CheckResizeHiZMap()
	{
		int pixelWidth = this.camera.pixelWidth;
		int pixelHeight = this.camera.pixelHeight;
		if (pixelWidth > 0 && pixelHeight > 0)
		{
			int num = pixelWidth / 4;
			int num2 = pixelHeight / 4;
			if (this.hiZLevels == null || this.hiZWidth != num || this.hiZHeight != num2)
			{
				this.InitializeHiZMap(num, num2);
				this.hiZWidth = num;
				this.hiZHeight = num2;
				if (this.debugSettings.log)
				{
					Debug.Log(string.Concat(new object[]
					{
						"[OcclusionCulling] Resized HiZ Map to ",
						this.hiZWidth,
						" x ",
						this.hiZHeight
					}));
				}
			}
		}
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x000DB7A4 File Offset: 0x000D99A4
	private void InitializeHiZMap()
	{
		Shader shader = Shader.Find("Hidden/OcclusionCulling/DepthDownscale");
		Shader shader2 = Shader.Find("Hidden/OcclusionCulling/BlitCopy");
		this.downscaleMat = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blitCopyMat = new Material(shader2)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.CheckResizeHiZMap();
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x000DB7F8 File Offset: 0x000D99F8
	private void FinalizeHiZMap()
	{
		this.DestroyHiZMap();
		if (this.downscaleMat != null)
		{
			Object.DestroyImmediate(this.downscaleMat);
			this.downscaleMat = null;
		}
		if (this.blitCopyMat != null)
		{
			Object.DestroyImmediate(this.blitCopyMat);
			this.blitCopyMat = null;
		}
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000DB84C File Offset: 0x000D9A4C
	private void InitializeHiZMap(int width, int height)
	{
		this.DestroyHiZMap();
		width = Mathf.Clamp(width, 1, 65536);
		height = Mathf.Clamp(height, 1, 65536);
		int num = Mathf.Min(width, height);
		this.hiZLevelCount = (int)(Mathf.Log((float)num, 2f) + 1f);
		this.hiZLevels = new RenderTexture[this.hiZLevelCount];
		this.depthTexture = this.CreateDepthTexture("DepthTex", width, height, false);
		this.hiZTexture = this.CreateDepthTexture("HiZMapTex", width, height, true);
		for (int i = 0; i < this.hiZLevelCount; i++)
		{
			this.hiZLevels[i] = this.CreateDepthTextureMip("HiZMap" + i, width, height, i);
		}
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x000DB908 File Offset: 0x000D9B08
	private void DestroyHiZMap()
	{
		if (this.depthTexture != null)
		{
			RenderTexture.active = null;
			Object.DestroyImmediate(this.depthTexture);
			this.depthTexture = null;
		}
		if (this.hiZTexture != null)
		{
			RenderTexture.active = null;
			Object.DestroyImmediate(this.hiZTexture);
			this.hiZTexture = null;
		}
		if (this.hiZLevels != null)
		{
			for (int i = 0; i < this.hiZLevels.Length; i++)
			{
				Object.DestroyImmediate(this.hiZLevels[i]);
			}
			this.hiZLevels = null;
		}
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x000215C1 File Offset: 0x0001F7C1
	private RenderTexture CreateDepthTexture(string name, int width, int height, bool mips = false)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = mips;
		renderTexture.autoGenerateMips = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x000DB990 File Offset: 0x000D9B90
	private RenderTexture CreateDepthTextureMip(string name, int width, int height, int mip)
	{
		int width2 = width >> mip;
		int height2 = height >> mip;
		int depth = (mip == 0) ? 24 : 0;
		RenderTexture renderTexture = new RenderTexture(width2, height2, depth, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x000215F9 File Offset: 0x0001F7F9
	public void GrabDepthTexture()
	{
		if (this.depthTexture != null)
		{
			UnityEngine.Graphics.Blit(null, this.depthTexture, this.depthCopyMat, 0);
		}
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x000DB9E4 File Offset: 0x000D9BE4
	public void GenerateHiZMipChain()
	{
		if (this.HiZReady)
		{
			bool noDataVisible = Culling.noDataVisible;
			this.depthCopyMat.SetMatrix("_CameraReprojection", this.prevViewProjMatrix * this.invViewProjMatrix);
			this.depthCopyMat.SetFloat("_FrustumNoDataDepth", noDataVisible ? 1f : 0f);
			UnityEngine.Graphics.Blit(this.depthTexture, this.hiZLevels[0], this.depthCopyMat, 1);
			for (int i = 1; i < this.hiZLevels.Length; i++)
			{
				RenderTexture renderTexture = this.hiZLevels[i - 1];
				RenderTexture dest = this.hiZLevels[i];
				int pass = ((renderTexture.width & 1) == 0 && (renderTexture.height & 1) == 0) ? 0 : 1;
				this.downscaleMat.SetTexture("_MainTex", renderTexture);
				UnityEngine.Graphics.Blit(renderTexture, dest, this.downscaleMat, pass);
			}
			for (int j = 0; j < this.hiZLevels.Length; j++)
			{
				UnityEngine.Graphics.SetRenderTarget(this.hiZTexture, j);
				UnityEngine.Graphics.Blit(this.hiZLevels[j], this.blitCopyMat);
			}
		}
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x000DBAF8 File Offset: 0x000D9CF8
	private void DebugDrawGizmos()
	{
		Camera component = base.GetComponent<Camera>();
		Gizmos.color = new Color(0.75f, 0.75f, 0f, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, component.fieldOfView, component.farClipPlane, component.nearClipPlane, component.aspect);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.identity;
		Matrix4x4 worldToCameraMatrix = component.worldToCameraMatrix;
		Matrix4x4 matrix4x = GL.GetGPUProjectionMatrix(component.projectionMatrix, false) * worldToCameraMatrix;
		Vector4[] array = new Vector4[6];
		OcclusionCulling.ExtractFrustum(matrix4x, ref array);
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 a = new Vector3(array[i].x, array[i].y, array[i].z);
			float w = array[i].w;
			Vector3 vector = -a * w;
			Gizmos.DrawLine(vector, vector * 2f);
		}
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000DBC10 File Offset: 0x000D9E10
	private static int floor(float x)
	{
		int num = (int)x;
		if (x >= (float)num)
		{
			return num;
		}
		return num - 1;
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x000DBC2C File Offset: 0x000D9E2C
	public static OcclusionCulling.Cell RegisterToGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		int num4 = Mathf.Clamp(num, -1048575, 1048575);
		int num5 = Mathf.Clamp(num2, -1048575, 1048575);
		int num6 = Mathf.Clamp(num3, -1048575, 1048575);
		ulong num7 = (ulong)((long)((num4 >= 0) ? num4 : (num4 + 1048575)));
		ulong num8 = (ulong)((long)((num5 >= 0) ? num5 : (num5 + 1048575)));
		ulong num9 = (ulong)((long)((num6 >= 0) ? num6 : (num6 + 1048575)));
		ulong key = num7 << 42 | num8 << 21 | num9;
		OcclusionCulling.Cell cell;
		bool flag = OcclusionCulling.grid.TryGetValue(key, out cell);
		if (!flag)
		{
			Vector3 center = default(Vector3);
			center.x = (float)num * 100f + 50f;
			center.y = (float)num2 * 100f + 50f;
			center.z = (float)num3 * 100f + 50f;
			Vector3 size = new Vector3(100f, 100f, 100f);
			cell = OcclusionCulling.grid.Add(key, 16).Initialize(num, num2, num3, new Bounds(center, size));
		}
		OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
		if (!flag || !smartList.Contains(occludee))
		{
			occludee.cell = cell;
			smartList.Add(occludee, 16);
			OcclusionCulling.gridChanged.Enqueue(cell);
		}
		return cell;
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000DBE0C File Offset: 0x000DA00C
	public static void UpdateInGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		if (num != occludee.cell.x || num2 != occludee.cell.y || num3 != occludee.cell.z)
		{
			OcclusionCulling.UnregisterFromGrid(occludee);
			OcclusionCulling.RegisterToGrid(occludee);
		}
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000DBED4 File Offset: 0x000DA0D4
	public static void UnregisterFromGrid(OccludeeState occludee)
	{
		OcclusionCulling.Cell cell = occludee.cell;
		OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
		OcclusionCulling.gridChanged.Enqueue(cell);
		smartList.Remove(occludee);
		if (cell.staticBucket.Count == 0 && cell.dynamicBucket.Count == 0)
		{
			OcclusionCulling.grid.Remove(cell);
			cell.Reset();
		}
		occludee.cell = null;
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000DBF44 File Offset: 0x000DA144
	public void UpdateGridBuffers()
	{
		if (OcclusionCulling.gridSet.CheckResize(OcclusionCulling.grid.Size, 256))
		{
			if (this.debugSettings.log)
			{
				Debug.Log("[OcclusionCulling] Resized grid to " + OcclusionCulling.grid.Size);
			}
			for (int i = 0; i < OcclusionCulling.grid.Size; i++)
			{
				if (OcclusionCulling.grid[i] != null)
				{
					OcclusionCulling.gridChanged.Enqueue(OcclusionCulling.grid[i]);
				}
			}
		}
		bool flag = OcclusionCulling.gridChanged.Count > 0;
		while (OcclusionCulling.gridChanged.Count > 0)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.gridChanged.Dequeue();
			OcclusionCulling.gridSet.inputData[cell.hashedPoolIndex] = cell.sphereBounds;
		}
		if (flag)
		{
			OcclusionCulling.gridSet.UploadData();
		}
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x000DC078 File Offset: 0x000DA278
	// Note: this type is marked as 'beforefieldinit'.
	static OcclusionCulling()
	{
		GraphicsDeviceType[] array = new GraphicsDeviceType[3];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.82BEEBBCC8BD8731B58FAE2B7BF8B77906BBF7D2).FieldHandle);
		OcclusionCulling.supportedDeviceTypes = array;
		OcclusionCulling._enabled = true;
		OcclusionCulling._safeMode = false;
		OcclusionCulling._debugShow = OcclusionCulling.DebugFilter.Off;
		OcclusionCulling.grid = new OcclusionCulling.HashedPool<OcclusionCulling.Cell>(16384, 4096);
		OcclusionCulling.gridChanged = new Queue<OcclusionCulling.Cell>();
	}

	// Token: 0x020007B2 RID: 1970
	public class BufferSet
	{
		// Token: 0x0400269C RID: 9884
		public ComputeBuffer inputBuffer;

		// Token: 0x0400269D RID: 9885
		public ComputeBuffer resultBuffer;

		// Token: 0x0400269E RID: 9886
		public int width;

		// Token: 0x0400269F RID: 9887
		public int height;

		// Token: 0x040026A0 RID: 9888
		public int capacity;

		// Token: 0x040026A1 RID: 9889
		public int count;

		// Token: 0x040026A2 RID: 9890
		public Texture2D inputTexture;

		// Token: 0x040026A3 RID: 9891
		public RenderTexture resultTexture;

		// Token: 0x040026A4 RID: 9892
		public Texture2D resultReadTexture;

		// Token: 0x040026A5 RID: 9893
		public Color[] inputData = new Color[0];

		// Token: 0x040026A6 RID: 9894
		public Color32[] resultData = new Color32[0];

		// Token: 0x040026A7 RID: 9895
		private OcclusionCulling culling;

		// Token: 0x040026A8 RID: 9896
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x040026A9 RID: 9897
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		// Token: 0x040026AA RID: 9898
		public IntPtr readbackInst = IntPtr.Zero;

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06002AFA RID: 11002 RVA: 0x0002161C File Offset: 0x0001F81C
		public bool Ready
		{
			get
			{
				return this.resultData.Length != 0;
			}
		}

		// Token: 0x06002AFB RID: 11003 RVA: 0x00021628 File Offset: 0x0001F828
		public void Attach(OcclusionCulling culling)
		{
			this.culling = culling;
		}

		// Token: 0x06002AFC RID: 11004 RVA: 0x000DC180 File Offset: 0x000DA380
		public void Dispose(bool data = true)
		{
			if (this.inputBuffer != null)
			{
				this.inputBuffer.Dispose();
				this.inputBuffer = null;
			}
			if (this.resultBuffer != null)
			{
				this.resultBuffer.Dispose();
				this.resultBuffer = null;
			}
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
			if (this.resultReadTexture != null)
			{
				Object.DestroyImmediate(this.resultReadTexture);
				this.resultReadTexture = null;
			}
			if (this.readbackInst != IntPtr.Zero)
			{
				RustNative.Graphics.BufferReadback.Destroy(this.readbackInst);
				this.readbackInst = IntPtr.Zero;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
				this.capacity = 0;
				this.count = 0;
			}
		}

		// Token: 0x06002AFD RID: 11005 RVA: 0x000DC278 File Offset: 0x000DA478
		public bool CheckResize(int count, int granularity)
		{
			if (count > this.capacity || (this.culling.usePixelShaderFallback && this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				int num = this.capacity;
				int num2 = count / granularity * granularity + granularity;
				if (this.culling.usePixelShaderFallback)
				{
					this.width = Mathf.CeilToInt(Mathf.Sqrt((float)num2));
					this.height = Mathf.CeilToInt((float)num2 / (float)this.width);
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
					this.resultReadTexture = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false, true);
					this.resultReadTexture.name = "_ResultRead";
					this.resultReadTexture.filterMode = FilterMode.Point;
					this.resultReadTexture.wrapMode = TextureWrapMode.Clamp;
					if (!this.culling.useAsyncReadAPI)
					{
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForTexture(this.resultTexture.GetNativeTexturePtr(), (uint)this.width, (uint)this.height, (uint)this.resultTexture.format);
					}
					this.capacity = this.width * this.height;
				}
				else
				{
					this.inputBuffer = new ComputeBuffer(num2, 16);
					this.resultBuffer = new ComputeBuffer(num2, 4);
					if (!this.culling.useAsyncReadAPI)
					{
						uint num3 = (uint)(this.capacity * 4);
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForBuffer(this.resultBuffer.GetNativeBufferPtr(), num3);
					}
					this.capacity = num2;
				}
				Array.Resize<Color>(ref this.inputData, this.capacity);
				Array.Resize<Color32>(ref this.resultData, this.capacity);
				Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				for (int i = num; i < this.capacity; i++)
				{
					this.resultData[i] = color;
				}
				this.count = count;
				return true;
			}
			return false;
		}

		// Token: 0x06002AFE RID: 11006 RVA: 0x00021631 File Offset: 0x0001F831
		public void UploadData()
		{
			if (this.culling.usePixelShaderFallback)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
				return;
			}
			this.inputBuffer.SetData(this.inputData);
		}

		// Token: 0x06002AFF RID: 11007 RVA: 0x0002166E File Offset: 0x0001F86E
		private int AlignDispatchSize(int dispatchSize)
		{
			return (dispatchSize + 63) / 64;
		}

		// Token: 0x06002B00 RID: 11008 RVA: 0x000DC4EC File Offset: 0x000DA6EC
		public void Dispatch(int count)
		{
			if (this.culling.usePixelShaderFallback)
			{
				RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
				this.culling.fallbackMat.SetTexture("_Input", this.inputTexture);
				UnityEngine.Graphics.Blit(this.inputTexture, this.resultTexture, this.culling.fallbackMat, 0);
				UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
				return;
			}
			this.culling.computeShader.SetBuffer(0, "_Input", this.inputBuffer);
			this.culling.computeShader.SetBuffer(0, "_Result", this.resultBuffer);
			this.culling.computeShader.Dispatch(0, this.AlignDispatchSize(count), 1, 1);
		}

		// Token: 0x06002B01 RID: 11009 RVA: 0x000DC5A4 File Offset: 0x000DA7A4
		public void IssueRead()
		{
			if (!OcclusionCulling.SafeMode)
			{
				if (this.culling.useAsyncReadAPI)
				{
					if (this.asyncRequests.Count < 10)
					{
						AsyncGPUReadbackRequest asyncGPUReadbackRequest;
						if (this.culling.usePixelShaderFallback)
						{
							asyncGPUReadbackRequest = AsyncGPUReadback.Request(this.resultTexture, 0, null);
						}
						else
						{
							asyncGPUReadbackRequest = AsyncGPUReadback.Request(this.resultBuffer, null);
						}
						this.asyncRequests.Enqueue(asyncGPUReadbackRequest);
						return;
					}
				}
				else if (this.readbackInst != IntPtr.Zero)
				{
					RustNative.Graphics.BufferReadback.IssueRead(this.readbackInst);
				}
			}
		}

		// Token: 0x06002B02 RID: 11010 RVA: 0x000DC628 File Offset: 0x000DA828
		public void GetResults()
		{
			if (this.resultData != null && this.resultData.Length != 0)
			{
				if (!OcclusionCulling.SafeMode)
				{
					if (this.culling.useAsyncReadAPI)
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
									return;
								}
								NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
								for (int i = 0; i < data.Length; i++)
								{
									this.resultData[i] = data[i];
								}
								this.asyncRequests.Dequeue();
							}
						}
						return;
					}
					if (this.readbackInst != IntPtr.Zero)
					{
						RustNative.Graphics.BufferReadback.GetData(this.readbackInst, ref this.resultData[0]);
						return;
					}
				}
				else
				{
					if (this.culling.usePixelShaderFallback)
					{
						RenderTexture.active = this.resultTexture;
						this.resultReadTexture.ReadPixels(new Rect(0f, 0f, (float)this.width, (float)this.height), 0, 0);
						this.resultReadTexture.Apply();
						Array.Copy(this.resultReadTexture.GetPixels32(), this.resultData, this.resultData.Length);
						return;
					}
					this.resultBuffer.GetData(this.resultData);
				}
			}
		}
	}

	// Token: 0x020007B3 RID: 1971
	// (Invoke) Token: 0x06002B05 RID: 11013
	public delegate void OnVisibilityChanged(bool visible);

	// Token: 0x020007B4 RID: 1972
	public enum DebugFilter
	{
		// Token: 0x040026AC RID: 9900
		Off,
		// Token: 0x040026AD RID: 9901
		Dynamic,
		// Token: 0x040026AE RID: 9902
		Static,
		// Token: 0x040026AF RID: 9903
		Grid,
		// Token: 0x040026B0 RID: 9904
		All
	}

	// Token: 0x020007B5 RID: 1973
	[Flags]
	public enum DebugMask
	{
		// Token: 0x040026B2 RID: 9906
		Off = 0,
		// Token: 0x040026B3 RID: 9907
		Dynamic = 1,
		// Token: 0x040026B4 RID: 9908
		Static = 2,
		// Token: 0x040026B5 RID: 9909
		Grid = 4,
		// Token: 0x040026B6 RID: 9910
		All = 7
	}

	// Token: 0x020007B6 RID: 1974
	[Serializable]
	public class DebugSettings
	{
		// Token: 0x040026B7 RID: 9911
		public bool log;

		// Token: 0x040026B8 RID: 9912
		public bool showAllVisible;

		// Token: 0x040026B9 RID: 9913
		public bool showMipChain;

		// Token: 0x040026BA RID: 9914
		public bool showMain;

		// Token: 0x040026BB RID: 9915
		public int showMainLod;

		// Token: 0x040026BC RID: 9916
		public bool showFallback;

		// Token: 0x040026BD RID: 9917
		public bool showStats;

		// Token: 0x040026BE RID: 9918
		public bool showScreenBounds;

		// Token: 0x040026BF RID: 9919
		public OcclusionCulling.DebugMask showMask;

		// Token: 0x040026C0 RID: 9920
		public LayerMask layerFilter = -1;
	}

	// Token: 0x020007B7 RID: 1975
	public class HashedPoolValue
	{
		// Token: 0x040026C1 RID: 9921
		public ulong hashedPoolKey = ulong.MaxValue;

		// Token: 0x040026C2 RID: 9922
		public int hashedPoolIndex = -1;
	}

	// Token: 0x020007B8 RID: 1976
	public class HashedPool<ValueType> where ValueType : OcclusionCulling.HashedPoolValue, new()
	{
		// Token: 0x040026C3 RID: 9923
		private int granularity;

		// Token: 0x040026C4 RID: 9924
		private Dictionary<ulong, ValueType> dict;

		// Token: 0x040026C5 RID: 9925
		private List<ValueType> pool;

		// Token: 0x040026C6 RID: 9926
		private List<ValueType> list;

		// Token: 0x040026C7 RID: 9927
		private Queue<ValueType> recycled;

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06002B0A RID: 11018 RVA: 0x000216D8 File Offset: 0x0001F8D8
		public int Size
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06002B0B RID: 11019 RVA: 0x000216E5 File Offset: 0x0001F8E5
		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		// Token: 0x170002CF RID: 719
		public ValueType this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x06002B0E RID: 11022 RVA: 0x0002170F File Offset: 0x0001F90F
		public HashedPool(int capacity, int granularity)
		{
			this.granularity = granularity;
			this.dict = new Dictionary<ulong, ValueType>(capacity);
			this.pool = new List<ValueType>(capacity);
			this.list = new List<ValueType>(capacity);
			this.recycled = new Queue<ValueType>();
		}

		// Token: 0x06002B0F RID: 11023 RVA: 0x0002174D File Offset: 0x0001F94D
		public void Clear()
		{
			this.dict.Clear();
			this.pool.Clear();
			this.list.Clear();
			this.recycled.Clear();
		}

		// Token: 0x06002B10 RID: 11024 RVA: 0x000DC788 File Offset: 0x000DA988
		public ValueType Add(ulong key, int capacityGranularity = 16)
		{
			ValueType valueType;
			if (this.recycled.Count > 0)
			{
				valueType = this.recycled.Dequeue();
				this.list[valueType.hashedPoolIndex] = valueType;
			}
			else
			{
				int count = this.pool.Count;
				if (count == this.pool.Capacity)
				{
					this.pool.Capacity += this.granularity;
				}
				valueType = Activator.CreateInstance<ValueType>();
				valueType.hashedPoolIndex = count;
				this.pool.Add(valueType);
				this.list.Add(valueType);
			}
			valueType.hashedPoolKey = key;
			this.dict.Add(key, valueType);
			return valueType;
		}

		// Token: 0x06002B11 RID: 11025 RVA: 0x000DC840 File Offset: 0x000DAA40
		public void Remove(ValueType value)
		{
			this.dict.Remove(value.hashedPoolKey);
			this.list[value.hashedPoolIndex] = default(ValueType);
			this.recycled.Enqueue(value);
			value.hashedPoolKey = ulong.MaxValue;
		}

		// Token: 0x06002B12 RID: 11026 RVA: 0x0002177B File Offset: 0x0001F97B
		public bool TryGetValue(ulong key, out ValueType value)
		{
			return this.dict.TryGetValue(key, ref value);
		}

		// Token: 0x06002B13 RID: 11027 RVA: 0x0002178A File Offset: 0x0001F98A
		public bool ContainsKey(ulong key)
		{
			return this.dict.ContainsKey(key);
		}
	}

	// Token: 0x020007B9 RID: 1977
	public class SimpleList<T>
	{
		// Token: 0x040026C8 RID: 9928
		private const int defaultCapacity = 16;

		// Token: 0x040026C9 RID: 9929
		private static readonly T[] emptyArray = new T[0];

		// Token: 0x040026CA RID: 9930
		public T[] array;

		// Token: 0x040026CB RID: 9931
		public int count;

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06002B14 RID: 11028 RVA: 0x00021798 File Offset: 0x0001F998
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06002B15 RID: 11029 RVA: 0x000217A0 File Offset: 0x0001F9A0
		// (set) Token: 0x06002B16 RID: 11030 RVA: 0x000DC89C File Offset: 0x000DAA9C
		public int Capacity
		{
			get
			{
				return this.array.Length;
			}
			set
			{
				if (value != this.array.Length)
				{
					if (value > 0)
					{
						T[] array = new T[value];
						if (this.count > 0)
						{
							Array.Copy(this.array, 0, array, 0, this.count);
						}
						this.array = array;
						return;
					}
					this.array = OcclusionCulling.SimpleList<T>.emptyArray;
				}
			}
		}

		// Token: 0x170002D2 RID: 722
		public T this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		// Token: 0x06002B19 RID: 11033 RVA: 0x000217C7 File Offset: 0x0001F9C7
		public SimpleList()
		{
			this.array = OcclusionCulling.SimpleList<T>.emptyArray;
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x000217DA File Offset: 0x0001F9DA
		public SimpleList(int capacity)
		{
			this.array = ((capacity == 0) ? OcclusionCulling.SimpleList<T>.emptyArray : new T[capacity]);
		}

		// Token: 0x06002B1B RID: 11035 RVA: 0x000DC8F0 File Offset: 0x000DAAF0
		public void Add(T item)
		{
			if (this.count == this.array.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			T[] array = this.array;
			int num = this.count;
			this.count = num + 1;
			array[num] = item;
		}

		// Token: 0x06002B1C RID: 11036 RVA: 0x000217F8 File Offset: 0x0001F9F8
		public void Clear()
		{
			if (this.count > 0)
			{
				Array.Clear(this.array, 0, this.count);
				this.count = 0;
			}
		}

		// Token: 0x06002B1D RID: 11037 RVA: 0x000DC938 File Offset: 0x000DAB38
		public bool Contains(T item)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.array[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002B1E RID: 11038 RVA: 0x0002181C File Offset: 0x0001FA1C
		public void CopyTo(T[] array)
		{
			Array.Copy(this.array, 0, array, 0, this.count);
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x000DC97C File Offset: 0x000DAB7C
		public void EnsureCapacity(int min)
		{
			if (this.array.Length < min)
			{
				int num = (this.array.Length == 0) ? 16 : (this.array.Length * 2);
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}
	}

	// Token: 0x020007BA RID: 1978
	public class SmartListValue
	{
		// Token: 0x040026CC RID: 9932
		public int hashedListIndex = -1;
	}

	// Token: 0x020007BB RID: 1979
	public class SmartList
	{
		// Token: 0x040026CD RID: 9933
		private const int defaultCapacity = 16;

		// Token: 0x040026CE RID: 9934
		private static readonly OccludeeState[] emptyList = new OccludeeState[0];

		// Token: 0x040026CF RID: 9935
		private static readonly int[] emptySlots = new int[0];

		// Token: 0x040026D0 RID: 9936
		private OccludeeState[] list;

		// Token: 0x040026D1 RID: 9937
		private int[] slots;

		// Token: 0x040026D2 RID: 9938
		private Queue<int> recycled;

		// Token: 0x040026D3 RID: 9939
		private int count;

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06002B22 RID: 11042 RVA: 0x0002184E File Offset: 0x0001FA4E
		public OccludeeState[] List
		{
			get
			{
				return this.list;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06002B23 RID: 11043 RVA: 0x00021856 File Offset: 0x0001FA56
		public int[] Slots
		{
			get
			{
				return this.slots;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06002B24 RID: 11044 RVA: 0x0002185E File Offset: 0x0001FA5E
		public int Size
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06002B25 RID: 11045 RVA: 0x00021866 File Offset: 0x0001FA66
		public int Count
		{
			get
			{
				return this.count - this.recycled.Count;
			}
		}

		// Token: 0x170002D7 RID: 727
		public OccludeeState this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06002B28 RID: 11048 RVA: 0x0002188F File Offset: 0x0001FA8F
		// (set) Token: 0x06002B29 RID: 11049 RVA: 0x000DC9BC File Offset: 0x000DABBC
		public int Capacity
		{
			get
			{
				return this.list.Length;
			}
			set
			{
				if (value != this.list.Length)
				{
					if (value > 0)
					{
						OccludeeState[] array = new OccludeeState[value];
						int[] array2 = new int[value];
						if (this.count > 0)
						{
							Array.Copy(this.list, array, this.count);
							Array.Copy(this.slots, array2, this.count);
						}
						this.list = array;
						this.slots = array2;
						return;
					}
					this.list = OcclusionCulling.SmartList.emptyList;
					this.slots = OcclusionCulling.SmartList.emptySlots;
				}
			}
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x00021899 File Offset: 0x0001FA99
		public SmartList(int capacity)
		{
			this.list = new OccludeeState[capacity];
			this.slots = new int[capacity];
			this.recycled = new Queue<int>();
			this.count = 0;
		}

		// Token: 0x06002B2B RID: 11051 RVA: 0x000DCA38 File Offset: 0x000DAC38
		public void Add(OccludeeState value, int capacityGranularity = 16)
		{
			int num;
			if (this.recycled.Count > 0)
			{
				num = this.recycled.Dequeue();
				this.list[num] = value;
				this.slots[num] = value.slot;
			}
			else
			{
				num = this.count;
				if (num == this.list.Length)
				{
					this.EnsureCapacity(this.count + 1);
				}
				this.list[num] = value;
				this.slots[num] = value.slot;
				this.count++;
			}
			value.hashedListIndex = num;
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x000DCAC4 File Offset: 0x000DACC4
		public void Remove(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			this.list[hashedListIndex] = null;
			this.slots[hashedListIndex] = -1;
			this.recycled.Enqueue(hashedListIndex);
			value.hashedListIndex = -1;
		}

		// Token: 0x06002B2D RID: 11053 RVA: 0x000DCB00 File Offset: 0x000DAD00
		public bool Contains(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			return hashedListIndex >= 0 && this.list[hashedListIndex] != null;
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x000DCB28 File Offset: 0x000DAD28
		public void EnsureCapacity(int min)
		{
			if (this.list.Length < min)
			{
				int num = (this.list.Length == 0) ? 16 : (this.list.Length * 2);
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}
	}

	// Token: 0x020007BC RID: 1980
	[Serializable]
	public class Cell : OcclusionCulling.HashedPoolValue
	{
		// Token: 0x040026D4 RID: 9940
		public int x;

		// Token: 0x040026D5 RID: 9941
		public int y;

		// Token: 0x040026D6 RID: 9942
		public int z;

		// Token: 0x040026D7 RID: 9943
		public Bounds bounds;

		// Token: 0x040026D8 RID: 9944
		public Vector4 sphereBounds;

		// Token: 0x040026D9 RID: 9945
		public bool isVisible;

		// Token: 0x040026DA RID: 9946
		public OcclusionCulling.SmartList staticBucket;

		// Token: 0x040026DB RID: 9947
		public OcclusionCulling.SmartList dynamicBucket;

		// Token: 0x06002B30 RID: 11056 RVA: 0x000DCB68 File Offset: 0x000DAD68
		public void Reset()
		{
			this.x = (this.y = (this.z = 0));
			this.bounds = default(Bounds);
			this.sphereBounds = Vector4.zero;
			this.isVisible = true;
			this.staticBucket = null;
			this.dynamicBucket = null;
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x000DCBBC File Offset: 0x000DADBC
		public OcclusionCulling.Cell Initialize(int x, int y, int z, Bounds bounds)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.bounds = bounds;
			this.sphereBounds = new Vector4(bounds.center.x, bounds.center.y, bounds.center.z, bounds.extents.magnitude);
			this.isVisible = true;
			this.staticBucket = new OcclusionCulling.SmartList(32);
			this.dynamicBucket = new OcclusionCulling.SmartList(32);
			return this;
		}
	}

	// Token: 0x020007BD RID: 1981
	public struct Sphere
	{
		// Token: 0x040026DC RID: 9948
		public Vector3 position;

		// Token: 0x040026DD RID: 9949
		public float radius;

		// Token: 0x06002B33 RID: 11059 RVA: 0x000218EB File Offset: 0x0001FAEB
		public bool IsValid()
		{
			return this.radius > 0f;
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x000218FA File Offset: 0x0001FAFA
		public Sphere(Vector3 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}
	}
}
