using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000275 RID: 629
public static class EffectLibrary
{
	// Token: 0x06001227 RID: 4647 RVA: 0x00077364 File Offset: 0x00075564
	private static void SetupEffect(this GameObject go, Effect effect)
	{
		List<IEffect> list = Pool.GetList<IEffect>();
		go.GetComponentsInChildren<IEffect>(true, list);
		foreach (IEffect effect2 in list)
		{
			effect2.SetupEffect(effect);
		}
		Pool.FreeList<IEffect>(ref list);
		if (!effect.pooledString.StartsWith("Assets/Prefabs/", 5))
		{
			BaseEntity baseEntity = go.ToBaseEntity();
			ParticleSystemSimulationSpace simulationSpace = (baseEntity != null && baseEntity is CargoShip) ? 0 : 1;
			List<ParticleSystem> list2 = Pool.GetList<ParticleSystem>();
			go.GetComponentsInChildren<ParticleSystem>(true, list2);
			for (int i = 0; i < list2.Count; i++)
			{
				list2[i].main.simulationSpace = simulationSpace;
			}
			Pool.FreeList<ParticleSystem>(ref list2);
		}
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x0000FBB9 File Offset: 0x0000DDB9
	private static void GenericEffectSpawn(Effect effect)
	{
		EffectLibrary.CreateEffect(effect.pooledString, effect);
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x0007743C File Offset: 0x0007563C
	private static void ProjectileEffectSpawn(Effect effect)
	{
		GameObject gameObject = GameManager.client.CreatePrefab(effect.pooledString, effect.worldPos, Quaternion.LookRotation(effect.worldNrm.normalized), false);
		if (gameObject == null)
		{
			return;
		}
		if (gameObject.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject, Rust.Client.EffectScene);
		}
		Projectile component = gameObject.GetComponent<Projectile>();
		if (component)
		{
			BasePlayer owner = (effect.source != 0UL) ? BasePlayer.FindByID_Clientside(effect.source) : null;
			component.owner = owner;
			component.seed = effect.number;
			component.invisible = (effect.scale == 0f);
			component.InitializeVelocity(effect.worldNrm);
			component.clientsideEffect = (effect.scale == 2f);
		}
		gameObject.SetupEffect(effect);
		gameObject.AwakeFromInstantiate();
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x0000FBC8 File Offset: 0x0000DDC8
	public static void Run(Effect fx)
	{
		if (fx.type == 0U)
		{
			EffectLibrary.GenericEffectSpawn(fx);
			return;
		}
		if (fx.type == 1U)
		{
			EffectLibrary.ProjectileEffectSpawn(fx);
		}
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x00077514 File Offset: 0x00075714
	public static GameObject CreateEffect(string strPrefab, Transform parent = null, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		GameObject gameObject = GameManager.client.CreatePrefab(strPrefab, pos, rot, false);
		if (gameObject == null)
		{
			return null;
		}
		if (gameObject.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject, Rust.Client.EffectScene);
		}
		if (parent)
		{
			gameObject.transform.parent = parent;
		}
		gameObject.AwakeFromInstantiate();
		return gameObject;
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x00077574 File Offset: 0x00075774
	public static GameObject CreateEffect(string strPrefab, Effect effect)
	{
		Quaternion localRotation = Quaternion.identity;
		Quaternion rot = Quaternion.identity;
		if (effect.worldNrm != Vector3.zero)
		{
			rot = QuaternionEx.LookRotationNormal(effect.worldNrm, effect.Up);
		}
		if (effect.normal != Vector3.zero)
		{
			localRotation = QuaternionEx.LookRotationNormal(effect.normal, effect.Up);
		}
		GameObject gameObject = GameManager.client.FindPrefab(strPrefab);
		if (gameObject == null)
		{
			return null;
		}
		if (gameObject.GetComponent<IEffectRecycle>() == null)
		{
			Debug.LogWarning("Tried to spawn an effect without a EffectRecycle component (" + strPrefab + ")", gameObject);
			return null;
		}
		MaxSpawnDistance component = gameObject.GetComponent<MaxSpawnDistance>();
		if (component != null)
		{
			Vector3 pos = effect.worldPos;
			if (effect.attached)
			{
				pos = effect.transform.localToWorldMatrix.MultiplyPoint3x4(effect.origin);
			}
			if (effect.gameObject)
			{
				pos = effect.gameObject.transform.localToWorldMatrix.MultiplyPoint3x4(effect.origin);
			}
			if (MainCamera.Distance(pos) > component.maxDistance)
			{
				return null;
			}
		}
		GameObject gameObject2 = GameManager.client.CreatePrefab(strPrefab, effect.worldPos, rot, false);
		if (gameObject2 == null)
		{
			return null;
		}
		if (gameObject2.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject2, Rust.Client.EffectScene);
		}
		if (effect.attached)
		{
			gameObject2.transform.parent = effect.transform;
			gameObject2.transform.localPosition = effect.origin;
			gameObject2.transform.localRotation = localRotation;
		}
		else if (effect.gameObject)
		{
			gameObject2.transform.parent = effect.gameObject.transform;
			gameObject2.transform.localPosition = effect.origin;
			gameObject2.transform.localRotation = localRotation;
		}
		gameObject2.SetupEffect(effect);
		gameObject2.AwakeFromInstantiate();
		return gameObject2;
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x00077758 File Offset: 0x00075958
	public static GameObject CreateEffect(string strPrefab, Vector3 vPos, Quaternion aAngle)
	{
		GameObject gameObject = GameManager.client.CreatePrefab(strPrefab, vPos, aAngle, false);
		if (gameObject == null)
		{
			return null;
		}
		if (gameObject.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject, Rust.Client.EffectScene);
		}
		gameObject.AwakeFromInstantiate();
		return gameObject;
	}
}
