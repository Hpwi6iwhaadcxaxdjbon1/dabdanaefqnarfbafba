using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020005B2 RID: 1458
public class SpecialPurposeCamera
{
	// Token: 0x04001D65 RID: 7525
	public bool dirty;

	// Token: 0x04001D66 RID: 7526
	public string name;

	// Token: 0x04001D67 RID: 7527
	public Camera camera;

	// Token: 0x04001D68 RID: 7528
	public RenderTexture texture;

	// Token: 0x04001D69 RID: 7529
	public CommandBuffer commands;

	// Token: 0x04001D6A RID: 7530
	internal Vector3 position = Vector3.zero;

	// Token: 0x04001D6B RID: 7531
	internal Quaternion rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

	// Token: 0x04001D6C RID: 7532
	internal float orthographicSize = 32f;

	// Token: 0x04001D6D RID: 7533
	internal float nearClipPlane = 1f;

	// Token: 0x04001D6E RID: 7534
	internal float farClipPlane = 128f;

	// Token: 0x04001D6F RID: 7535
	internal bool created;

	// Token: 0x04001D70 RID: 7536
	private ListHashSet<SpecialPurposeCamera.RenderEntry> dynamicEntries = new ListHashSet<SpecialPurposeCamera.RenderEntry>(8);

	// Token: 0x04001D71 RID: 7537
	private InstancingContainer dynamicBuffer = new InstancingContainer(128);

	// Token: 0x04001D72 RID: 7538
	private ListHashSet<SpecialPurposeCamera.RenderEntry> staticEntries = new ListHashSet<SpecialPurposeCamera.RenderEntry>(8);

	// Token: 0x04001D73 RID: 7539
	private InstancingContainer staticBuffer = new InstancingContainer(128);

	// Token: 0x04001D74 RID: 7540
	private CameraEvent cameraEvent = CameraEvent.BeforeGBuffer;

	// Token: 0x04001D75 RID: 7541
	private MaterialPropertyBlock block;

	// Token: 0x06002193 RID: 8595 RVA: 0x000B6060 File Offset: 0x000B4260
	public SpecialPurposeCamera(string name)
	{
		this.name = name;
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x000B60FC File Offset: 0x000B42FC
	public void Create(int width, int height, int depth, RenderTextureFormat format)
	{
		this.created = true;
		this.texture = new RenderTexture(width, height, depth, format);
		this.texture.name = this.name;
		this.commands = new CommandBuffer();
		this.commands.name = this.name;
		this.camera = MainCamera.mainCamera;
		this.camera.AddCommandBuffer(this.cameraEvent, this.commands);
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x000B6170 File Offset: 0x000B4370
	public void Destroy()
	{
		this.created = false;
		if (this.camera)
		{
			this.camera.RemoveCommandBuffer(this.cameraEvent, this.commands);
			this.camera = null;
		}
		if (this.texture)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
		this.commands = null;
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x0001AAD5 File Offset: 0x00018CD5
	public void Add(Transform transform, Mesh mesh, Material material, bool isDynamic, bool isBillboard)
	{
		if (isDynamic)
		{
			this.dynamicEntries.Add(new SpecialPurposeCamera.RenderEntry(transform, mesh, material, isBillboard));
			return;
		}
		this.staticEntries.Add(new SpecialPurposeCamera.RenderEntry(transform, mesh, material, isBillboard));
		this.dirty = true;
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x0001AB0D File Offset: 0x00018D0D
	public void Remove(Transform transform, Mesh mesh, Material material, bool isDynamic, bool isBillboard)
	{
		if (isDynamic)
		{
			this.dynamicEntries.Remove(new SpecialPurposeCamera.RenderEntry(transform, mesh, material, isBillboard));
			return;
		}
		this.staticEntries.Remove(new SpecialPurposeCamera.RenderEntry(transform, mesh, material, isBillboard));
		this.dirty = true;
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000B61D8 File Offset: 0x000B43D8
	private Matrix4x4 ViewMatrix()
	{
		Matrix4x4 result = default(Matrix4x4);
		Vector3 vector = this.rotation * Vector3.right;
		Vector3 vector2 = this.rotation * Vector3.up;
		Vector3 vector3 = this.rotation * -Vector3.forward;
		Vector3 zero = Vector3.zero;
		result.SetRow(0, vector);
		result.SetRow(1, vector2);
		result.SetRow(2, vector3);
		result.SetRow(3, zero);
		Vector4 column = new Vector4(Vector3.Dot(-this.position, vector), Vector3.Dot(-this.position, vector2), Vector3.Dot(-this.position, vector3), 1f);
		result.SetColumn(3, column);
		return result;
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x0001AB47 File Offset: 0x00018D47
	private Matrix4x4 ProjMatrix()
	{
		return Matrix4x4.Ortho(-this.orthographicSize, this.orthographicSize, -this.orthographicSize, this.orthographicSize, this.nearClipPlane, this.farClipPlane);
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000B62B0 File Offset: 0x000B44B0
	public void Refresh()
	{
		this.dynamicBuffer.Clear();
		for (int i = 0; i < this.dynamicEntries.Count; i++)
		{
			SpecialPurposeCamera.RenderEntry renderEntry = this.dynamicEntries.Values[i];
			this.dynamicBuffer.Add(renderEntry.mesh, renderEntry.material, renderEntry.matrix, 0, -1);
		}
		if (this.dirty)
		{
			this.staticBuffer.Clear();
			for (int j = 0; j < this.staticEntries.Count; j++)
			{
				SpecialPurposeCamera.RenderEntry renderEntry2 = this.staticEntries.Values[j];
				this.staticBuffer.Add(renderEntry2.mesh, renderEntry2.material, renderEntry2.matrix, 0, -1);
			}
			this.dirty = false;
		}
		this.commands.Clear();
		this.commands.SetViewProjectionMatrices(this.ViewMatrix(), this.ProjMatrix());
		this.commands.SetRenderTarget(this.texture);
		this.commands.ClearRenderTarget(true, true, Color.clear);
		this.dynamicBuffer.Apply(this.commands, true, null);
		this.staticBuffer.Apply(this.commands, true, null);
	}

	// Token: 0x020005B3 RID: 1459
	private struct RenderEntry : IEquatable<SpecialPurposeCamera.RenderEntry>
	{
		// Token: 0x04001D76 RID: 7542
		public Transform transform;

		// Token: 0x04001D77 RID: 7543
		public Mesh mesh;

		// Token: 0x04001D78 RID: 7544
		public Material material;

		// Token: 0x04001D79 RID: 7545
		public bool billboard;

		// Token: 0x0600219B RID: 8603 RVA: 0x0001AB74 File Offset: 0x00018D74
		public RenderEntry(Transform transform, Mesh mesh, Material material, bool billboard)
		{
			this.transform = transform;
			this.material = material;
			this.mesh = mesh;
			this.billboard = billboard;
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x0600219C RID: 8604 RVA: 0x0001AB93 File Offset: 0x00018D93
		public Matrix4x4 matrix
		{
			get
			{
				if (this.billboard)
				{
					return Matrix4x4.TRS(this.transform.position, Quaternion.identity, this.transform.lossyScale);
				}
				return this.transform.localToWorldMatrix;
			}
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x0001ABC9 File Offset: 0x00018DC9
		public override int GetHashCode()
		{
			return this.transform.GetHashCode();
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x0001ABD6 File Offset: 0x00018DD6
		public override bool Equals(object other)
		{
			return other is SpecialPurposeCamera.RenderEntry && this.Equals((SpecialPurposeCamera.RenderEntry)other);
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x000B63E4 File Offset: 0x000B45E4
		public bool Equals(SpecialPurposeCamera.RenderEntry other)
		{
			return this.transform == other.transform && this.mesh == other.mesh && this.material == other.material && this.billboard == other.billboard;
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x0001ABEE File Offset: 0x00018DEE
		public static bool operator ==(SpecialPurposeCamera.RenderEntry a, SpecialPurposeCamera.RenderEntry b)
		{
			return a.Equals(b);
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x0001ABF8 File Offset: 0x00018DF8
		public static bool operator !=(SpecialPurposeCamera.RenderEntry a, SpecialPurposeCamera.RenderEntry b)
		{
			return !a.Equals(b);
		}
	}
}
