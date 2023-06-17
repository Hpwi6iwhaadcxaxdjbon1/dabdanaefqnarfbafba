using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class v_chainsaw : MonoBehaviour
{
	// Token: 0x0400083F RID: 2111
	public bool bAttacking;

	// Token: 0x04000840 RID: 2112
	public bool bHitMetal;

	// Token: 0x04000841 RID: 2113
	public bool bHitWood;

	// Token: 0x04000842 RID: 2114
	public bool bHitFlesh;

	// Token: 0x04000843 RID: 2115
	public bool bEngineOn;

	// Token: 0x04000844 RID: 2116
	public ParticleSystem[] hitMetalFX;

	// Token: 0x04000845 RID: 2117
	public ParticleSystem[] hitWoodFX;

	// Token: 0x04000846 RID: 2118
	public ParticleSystem[] hitFleshFX;

	// Token: 0x04000847 RID: 2119
	public SoundDefinition hitMetalSoundDef;

	// Token: 0x04000848 RID: 2120
	public SoundDefinition hitWoodSoundDef;

	// Token: 0x04000849 RID: 2121
	public SoundDefinition hitFleshSoundDef;

	// Token: 0x0400084A RID: 2122
	public Sound hitSound;

	// Token: 0x0400084B RID: 2123
	public GameObject hitSoundTarget;

	// Token: 0x0400084C RID: 2124
	public float hitSoundFadeTime = 0.1f;

	// Token: 0x0400084D RID: 2125
	public ParticleSystem smokeEffect;

	// Token: 0x0400084E RID: 2126
	public Animator chainsawAnimator;

	// Token: 0x0400084F RID: 2127
	public Renderer chainRenderer;

	// Token: 0x04000850 RID: 2128
	public Material chainlink;

	// Token: 0x04000851 RID: 2129
	private MaterialPropertyBlock block;

	// Token: 0x04000852 RID: 2130
	private Vector2 saveST;

	// Token: 0x04000853 RID: 2131
	private float chainSpeed;

	// Token: 0x04000854 RID: 2132
	private float chainAmount;

	// Token: 0x04000855 RID: 2133
	public float temp1;

	// Token: 0x04000856 RID: 2134
	public float temp2;

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0000B2D1 File Offset: 0x000094D1
	public void OnEnable()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.saveST = this.chainRenderer.sharedMaterial.GetVector("_MainTex_ST");
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0000B306 File Offset: 0x00009506
	private void Awake()
	{
		this.chainlink = this.chainRenderer.sharedMaterial;
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0005A72C File Offset: 0x0005892C
	private void ScrollChainTexture()
	{
		float z = this.chainAmount = (this.chainAmount + Time.deltaTime * this.chainSpeed) % 1f;
		this.block.Clear();
		this.block.SetVector("_MainTex_ST", new Vector4(this.saveST.x, this.saveST.y, z, 0f));
		this.chainRenderer.SetPropertyBlock(this.block);
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x0005A7AC File Offset: 0x000589AC
	private void Update()
	{
		this.chainsawAnimator.SetBool("attacking", this.bAttacking);
		this.smokeEffect.enableEmission = this.bEngineOn;
		ParticleSystem[] array;
		if (this.bHitMetal)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitMetalSoundDef);
			return;
		}
		if (this.bHitWood)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitWoodSoundDef);
			return;
		}
		if (this.bHitFlesh)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			this.DoHitSound(this.hitFleshSoundDef);
			return;
		}
		this.chainsawAnimator.SetBool("attackHit", false);
		array = this.hitMetalFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitWoodFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitFleshFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		if (this.hitSound != null)
		{
			this.hitSound.FadeOutAndRecycle(this.hitSoundFadeTime);
			this.hitSound = null;
		}
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x0005A9F4 File Offset: 0x00058BF4
	private void DoHitSound(SoundDefinition soundDef)
	{
		if (this.hitSound != null && this.hitSound.definition != soundDef)
		{
			this.hitSound.FadeOutAndRecycle(this.hitSoundFadeTime);
			this.hitSound = null;
		}
		if (this.hitSound == null)
		{
			this.hitSound = SoundManager.RequestSoundInstance(soundDef, this.hitSoundTarget, default(Vector3), false);
			this.hitSound.FadeInAndPlay(this.hitSoundFadeTime);
		}
	}
}
