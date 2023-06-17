using System;
using UnityEngine;

// Token: 0x0200055F RID: 1375
[ExecuteInEditMode]
public class WaterInteraction : MonoBehaviour
{
	// Token: 0x04001B4D RID: 6989
	[SerializeField]
	private Texture2D texture;

	// Token: 0x04001B4E RID: 6990
	[Range(0f, 1f)]
	public float Displacement = 1f;

	// Token: 0x04001B4F RID: 6991
	[Range(0f, 1f)]
	public float Disturbance = 0.5f;

	// Token: 0x04001B54 RID: 6996
	private Transform cachedTransform;

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x06001F0D RID: 7949 RVA: 0x0001887B File Offset: 0x00016A7B
	// (set) Token: 0x06001F0E RID: 7950 RVA: 0x00018883 File Offset: 0x00016A83
	public Texture2D Texture
	{
		get
		{
			return this.texture;
		}
		set
		{
			this.texture = value;
			this.CheckRegister();
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x06001F0F RID: 7951 RVA: 0x00018892 File Offset: 0x00016A92
	// (set) Token: 0x06001F10 RID: 7952 RVA: 0x0001889A File Offset: 0x00016A9A
	public WaterDynamics.Image Image { get; private set; }

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x06001F11 RID: 7953 RVA: 0x000188A3 File Offset: 0x00016AA3
	// (set) Token: 0x06001F12 RID: 7954 RVA: 0x000188AB File Offset: 0x00016AAB
	public Vector2 Position { get; private set; } = Vector2.zero;

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x06001F13 RID: 7955 RVA: 0x000188B4 File Offset: 0x00016AB4
	// (set) Token: 0x06001F14 RID: 7956 RVA: 0x000188BC File Offset: 0x00016ABC
	public Vector2 Scale { get; private set; } = Vector2.one;

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x06001F15 RID: 7957 RVA: 0x000188C5 File Offset: 0x00016AC5
	// (set) Token: 0x06001F16 RID: 7958 RVA: 0x000188CD File Offset: 0x00016ACD
	public float Rotation { get; private set; }

	// Token: 0x06001F17 RID: 7959 RVA: 0x000188D6 File Offset: 0x00016AD6
	protected void OnEnable()
	{
		this.CheckRegister();
		this.UpdateTransform();
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000188E4 File Offset: 0x00016AE4
	protected void OnDisable()
	{
		this.Unregister();
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x000A987C File Offset: 0x000A7A7C
	public void CheckRegister()
	{
		if (!base.enabled || this.texture == null)
		{
			this.Unregister();
			return;
		}
		if (this.Image == null || this.Image.texture != this.texture)
		{
			this.Register();
		}
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x000188EC File Offset: 0x00016AEC
	private void UpdateImage()
	{
		this.Image = new WaterDynamics.Image(this.texture);
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000188FF File Offset: 0x00016AFF
	private void Register()
	{
		this.UpdateImage();
		WaterDynamics.RegisterInteraction(this);
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x0001890D File Offset: 0x00016B0D
	private void Unregister()
	{
		if (this.Image != null)
		{
			WaterDynamics.UnregisterInteraction(this);
			this.Image = null;
		}
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000A98CC File Offset: 0x000A7ACC
	public void UpdateTransform()
	{
		this.cachedTransform = ((this.cachedTransform != null) ? this.cachedTransform : base.transform);
		if (this.cachedTransform.hasChanged)
		{
			Vector3 position = this.cachedTransform.position;
			Vector3 lossyScale = this.cachedTransform.lossyScale;
			this.Position = new Vector2(position.x, position.z);
			this.Scale = new Vector2(lossyScale.x, lossyScale.z);
			this.Rotation = this.cachedTransform.rotation.eulerAngles.y;
			this.cachedTransform.hasChanged = false;
		}
	}
}
