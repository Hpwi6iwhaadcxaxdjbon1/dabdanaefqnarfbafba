using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class SimpleFlare : BaseMonoBehaviour, IClientComponent, IOnParentDestroying
{
	// Token: 0x04000775 RID: 1909
	public bool timeShimmer;

	// Token: 0x04000776 RID: 1910
	public bool positionalShimmer;

	// Token: 0x04000777 RID: 1911
	public bool rotate;

	// Token: 0x04000778 RID: 1912
	public float fadeSpeed = 35f;

	// Token: 0x04000779 RID: 1913
	public Collider checkCollider;

	// Token: 0x0400077A RID: 1914
	public float maxVisibleDistance = 30f;

	// Token: 0x0400077B RID: 1915
	public bool lightScaled;

	// Token: 0x0400077C RID: 1916
	public bool alignToCameraViaScript;

	// Token: 0x0400077D RID: 1917
	protected float tickRate = 0.33f;

	// Token: 0x0400077E RID: 1918
	private Vector3 fullSize;

	// Token: 0x0400077F RID: 1919
	public bool faceCameraPos = true;

	// Token: 0x04000780 RID: 1920
	public bool billboardViaShader;

	// Token: 0x04000781 RID: 1921
	private float artificialLightExposure;

	// Token: 0x04000782 RID: 1922
	private float privateRand;

	// Token: 0x04000783 RID: 1923
	private List<BasePlayer> players;

	// Token: 0x04000784 RID: 1924
	private Renderer myRenderer;

	// Token: 0x04000785 RID: 1925
	private static MaterialPropertyBlock block;

	// Token: 0x04000786 RID: 1926
	public float dotMin = -1f;

	// Token: 0x04000787 RID: 1927
	public float dotMax = -1f;

	// Token: 0x04000788 RID: 1928
	private float visibleFraction;

	// Token: 0x04000789 RID: 1929
	private bool destroying;

	// Token: 0x0400078A RID: 1930
	private float nextVisUpdateTime = -1f;

	// Token: 0x06000B28 RID: 2856 RVA: 0x0000AD04 File Offset: 0x00008F04
	public void OnParentDestroying()
	{
		this.destroying = true;
		this.visibleFraction = 0f;
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0000AD18 File Offset: 0x00008F18
	protected void Awake()
	{
		this.fullSize = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
		this.myRenderer = base.GetComponent<Renderer>();
		SimpleFlare.block = new MaterialPropertyBlock();
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0000AD51 File Offset: 0x00008F51
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.GatherLights));
		Pool.FreeList<BasePlayer>(ref this.players);
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00057438 File Offset: 0x00055638
	protected void OnEnable()
	{
		this.destroying = false;
		base.InvokeRepeating(new Action(this.GatherLights), Random.Range(1f, 2f), 2f);
		this.privateRand = Random.Range(0f, 100f);
		this.nextVisUpdateTime = Random.Range(0f, 1f);
		this.players = Pool.GetList<BasePlayer>();
		this.GatherLights();
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x000574B0 File Offset: 0x000556B0
	private void Update()
	{
		if (UnityEngine.Time.realtimeSinceStartup >= this.nextVisUpdateTime)
		{
			this.UpdateOcclusion();
			this.nextVisUpdateTime = UnityEngine.Time.realtimeSinceStartup + this.tickRate;
		}
		if (this.visibleFraction <= 0.01f && base.transform.localScale.magnitude <= 0.01f)
		{
			if (base.transform.localScale != Vector3.zero)
			{
				base.transform.localScale = Vector3.zero;
			}
			return;
		}
		float num = 1f;
		float num2 = 1f;
		if (this.timeShimmer)
		{
			num = Mathf.PerlinNoise((this.privateRand + UnityEngine.Time.time) * 1f, 0f);
		}
		if (this.positionalShimmer)
		{
			num2 = Mathf.PerlinNoise(this.privateRand + MainCamera.mainCamera.transform.position.x, this.privateRand + MainCamera.mainCamera.transform.position.z);
		}
		float num3 = (num + num2) / 2f;
		float d = 1f;
		if (this.lightScaled)
		{
			float value = TOD_Sky.Instance.LerpValue * TOD_Sky.Instance.Day.LightIntensity;
			float num4 = Mathf.InverseLerp(1f, 7f, value);
			if (num4 < 0.2f)
			{
				this.CalculateLights();
				d = Mathf.Max(num4, this.artificialLightExposure);
			}
			else
			{
				d = num4;
			}
		}
		float d2 = 1f;
		if (MainCamera.mainCamera && this.dotMin != -1f && this.dotMax != -1f)
		{
			float value2 = Vector3.Dot(base.transform.forward, (MainCamera.position - base.transform.position).normalized);
			d2 = Mathf.InverseLerp(this.dotMin, this.dotMax, value2);
		}
		Vector3 vector = this.fullSize * this.visibleFraction * (0.25f + 0.75f * num3) * d * d2;
		if (base.transform.localScale != vector)
		{
			Vector3 localScale = Vector3.MoveTowards(base.transform.localScale, vector, UnityEngine.Time.deltaTime * this.fadeSpeed);
			base.transform.localScale = localScale;
		}
		SimpleFlare.block.Clear();
		if (this.billboardViaShader)
		{
			Vector3 position = base.transform.position;
			Vector3 lossyScale = base.transform.lossyScale;
			SimpleFlare.block.SetVector("_Flare_WorldCenterAndTime", new Vector4(position.x, position.y, position.z, UnityEngine.Time.realtimeSinceStartup));
			SimpleFlare.block.SetVector("_Flare_WorldScaleAndRotate", new Vector4(lossyScale.x, lossyScale.y, lossyScale.z, (float)(this.rotate ? 1 : 0)));
		}
		else
		{
			if (this.faceCameraPos && MainCamera.mainCamera != null)
			{
				base.transform.rotation = Quaternion.LookRotation((MainCamera.mainCamera.transform.position - base.transform.position).normalized, this.rotate ? MainCamera.mainCamera.transform.up : Vector3.up);
			}
			if (this.rotate)
			{
				Quaternion quaternion = base.transform.rotation;
				quaternion *= Quaternion.Euler(0f, 0f, 360f * (UnityEngine.Time.time * 0.15f % 1f));
				base.transform.rotation = quaternion;
			}
			SimpleFlare.block.SetVector("_Flare_WorldCenterAndTime", new Vector4(0f, 0f, 0f, -1f));
		}
		this.myRenderer.SetPropertyBlock(SimpleFlare.block);
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x00057884 File Offset: 0x00055A84
	public void GatherLights()
	{
		this.players.Clear();
		if (Effects.otherplayerslightflares)
		{
			global::Vis.Entities<BasePlayer>(base.transform.position, 15f, this.players, 2048, 2);
		}
		if (LocalPlayer.Entity)
		{
			this.players.Add(LocalPlayer.Entity);
		}
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x000578E0 File Offset: 0x00055AE0
	public void CalculateLights()
	{
		this.artificialLightExposure = 0f;
		foreach (BasePlayer basePlayer in this.players)
		{
			foreach (Item item in basePlayer.inventory.containerWear.itemList)
			{
				if (item.HasFlag(Item.Flag.IsOn))
				{
					if (item.info.shortname == "hat.miner")
					{
						float num = Vector3.Dot(basePlayer.eyes.BodyForward(), (base.transform.position - basePlayer.eyes.position).normalized);
						num = Mathf.InverseLerp(0.72f, 1f, num);
						this.artificialLightExposure += num * (1f - Mathf.InverseLerp(3f, 12f, Vector3.Distance(base.transform.position, basePlayer.eyes.position)));
					}
					else
					{
						this.artificialLightExposure += 0.5f - Mathf.InverseLerp(2f, 6f, Vector3.Distance(base.transform.position, basePlayer.eyes.position));
					}
				}
			}
		}
		this.artificialLightExposure = Mathf.Clamp01(this.artificialLightExposure);
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00057AA0 File Offset: 0x00055CA0
	public virtual float SampleVisibility()
	{
		Camera mainCamera = MainCamera.mainCamera;
		Vector3 normalized = (base.transform.position - mainCamera.transform.position).normalized;
		float num = Vector3.Distance(mainCamera.transform.position, base.transform.position);
		RaycastHit raycastHit;
		if (!Physics.Raycast(new Ray(mainCamera.transform.position, normalized), ref raycastHit, num, 1252206849) || (this.checkCollider != null && raycastHit.collider == this.checkCollider))
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00057B44 File Offset: 0x00055D44
	public virtual void UpdateOcclusion()
	{
		if (this.destroying)
		{
			this.visibleFraction = 0f;
			return;
		}
		float num = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
		if (num <= this.maxVisibleDistance)
		{
			this.visibleFraction = this.SampleVisibility();
			float num2 = Mathf.InverseLerp(this.maxVisibleDistance, this.maxVisibleDistance - 2f, num);
			this.visibleFraction *= num2;
			return;
		}
		this.visibleFraction = 0f;
	}
}
