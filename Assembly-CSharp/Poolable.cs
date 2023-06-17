using System;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x02000766 RID: 1894
public class Poolable : MonoBehaviour, IClientComponent, IPrefabPostProcess
{
	// Token: 0x0400245D RID: 9309
	[HideInInspector]
	public uint prefabID;

	// Token: 0x0400245E RID: 9310
	[HideInInspector]
	public Behaviour[] behaviours;

	// Token: 0x0400245F RID: 9311
	[HideInInspector]
	public Rigidbody[] rigidbodies;

	// Token: 0x04002460 RID: 9312
	[HideInInspector]
	public Collider[] colliders;

	// Token: 0x04002461 RID: 9313
	[HideInInspector]
	public LODGroup[] lodgroups;

	// Token: 0x04002462 RID: 9314
	[HideInInspector]
	public Renderer[] renderers;

	// Token: 0x04002463 RID: 9315
	[HideInInspector]
	public ParticleSystem[] particles;

	// Token: 0x04002464 RID: 9316
	[HideInInspector]
	public bool[] behaviourStates;

	// Token: 0x04002465 RID: 9317
	[HideInInspector]
	public bool[] rigidbodyStates;

	// Token: 0x04002466 RID: 9318
	[HideInInspector]
	public bool[] colliderStates;

	// Token: 0x04002467 RID: 9319
	[HideInInspector]
	public bool[] lodgroupStates;

	// Token: 0x04002468 RID: 9320
	[HideInInspector]
	public bool[] rendererStates;

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06002941 RID: 10561 RVA: 0x000D2848 File Offset: 0x000D0A48
	public int ClientCount
	{
		get
		{
			if (base.GetComponent<LootPanel>() != null)
			{
				return 1;
			}
			if (base.GetComponent<DecorComponent>() != null)
			{
				return 100;
			}
			if (base.GetComponent<BuildingBlock>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Door>() != null)
			{
				return 100;
			}
			return 10;
		}
	}

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06002942 RID: 10562 RVA: 0x0000508F File Offset: 0x0000328F
	public int ServerCount
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x00020280 File Offset: 0x0001E480
	public void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.Initialize(StringPool.Get(name));
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x000D289C File Offset: 0x000D0A9C
	public void Initialize(uint id)
	{
		this.prefabID = id;
		this.behaviours = Enumerable.ToArray<Behaviour>(Enumerable.OfType<Behaviour>(base.gameObject.GetComponentsInChildren(typeof(Behaviour), true)));
		this.rigidbodies = base.gameObject.GetComponentsInChildren<Rigidbody>(true);
		this.colliders = base.gameObject.GetComponentsInChildren<Collider>(true);
		this.lodgroups = base.gameObject.GetComponentsInChildren<LODGroup>(true);
		this.renderers = base.gameObject.GetComponentsInChildren<Renderer>(true);
		this.particles = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		this.behaviourStates = new bool[this.behaviours.Length];
		this.rigidbodyStates = new bool[this.rigidbodies.Length];
		this.colliderStates = new bool[this.colliders.Length];
		this.lodgroupStates = new bool[this.lodgroups.Length];
		this.rendererStates = new bool[this.renderers.Length];
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x000D2990 File Offset: 0x000D0B90
	public void EnterPool()
	{
		if (base.transform.parent != null)
		{
			base.transform.SetParent(null, false);
		}
		if (Pool.mode <= 1)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			this.SetBehaviourEnabled(false);
			this.SetComponentEnabled(false);
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x00020293 File Offset: 0x0001E493
	public void LeavePool()
	{
		if (Pool.mode > 1)
		{
			this.SetComponentEnabled(true);
		}
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x000D2A08 File Offset: 0x000D0C08
	public void SetBehaviourEnabled(bool state)
	{
		try
		{
			if (!state)
			{
				for (int i = 0; i < this.behaviours.Length; i++)
				{
					Behaviour behaviour = this.behaviours[i];
					this.behaviourStates[i] = behaviour.enabled;
					behaviour.enabled = false;
				}
				for (int j = 0; j < this.particles.Length; j++)
				{
					ParticleSystem particleSystem = this.particles[j];
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
			else
			{
				for (int k = 0; k < this.particles.Length; k++)
				{
					ParticleSystem particleSystem2 = this.particles[k];
					if (particleSystem2.playOnAwake)
					{
						particleSystem2.Play();
					}
				}
				for (int l = 0; l < this.behaviours.Length; l++)
				{
					this.behaviours[l].enabled = this.behaviourStates[l];
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Pooling error: ",
				base.name,
				" (",
				ex.Message,
				")"
			}));
		}
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000D2B20 File Offset: 0x000D0D20
	public void SetComponentEnabled(bool state)
	{
		try
		{
			if (!state)
			{
				for (int i = 0; i < this.renderers.Length; i++)
				{
					Renderer renderer = this.renderers[i];
					this.rendererStates[i] = renderer.enabled;
					renderer.enabled = false;
				}
				for (int j = 0; j < this.lodgroups.Length; j++)
				{
					LODGroup lodgroup = this.lodgroups[j];
					this.lodgroupStates[j] = lodgroup.enabled;
					lodgroup.enabled = false;
				}
				for (int k = 0; k < this.colliders.Length; k++)
				{
					Collider collider = this.colliders[k];
					this.colliderStates[k] = collider.enabled;
					collider.enabled = false;
				}
				for (int l = 0; l < this.rigidbodies.Length; l++)
				{
					Rigidbody rigidbody = this.rigidbodies[l];
					this.rigidbodyStates[l] = rigidbody.isKinematic;
					rigidbody.isKinematic = true;
					rigidbody.detectCollisions = false;
				}
			}
			else
			{
				for (int m = 0; m < this.renderers.Length; m++)
				{
					this.renderers[m].enabled = this.rendererStates[m];
				}
				for (int n = 0; n < this.lodgroups.Length; n++)
				{
					this.lodgroups[n].enabled = this.lodgroupStates[n];
				}
				for (int num = 0; num < this.colliders.Length; num++)
				{
					this.colliders[num].enabled = this.colliderStates[num];
				}
				for (int num2 = 0; num2 < this.rigidbodies.Length; num2++)
				{
					Rigidbody rigidbody2 = this.rigidbodies[num2];
					rigidbody2.isKinematic = this.rigidbodyStates[num2];
					rigidbody2.detectCollisions = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Pooling error: ",
				base.name,
				" (",
				ex.Message,
				")"
			}));
		}
	}
}
