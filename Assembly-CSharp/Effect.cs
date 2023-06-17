using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class Effect : EffectData
{
	// Token: 0x04000EB2 RID: 3762
	public Vector3 Up;

	// Token: 0x04000EB3 RID: 3763
	public Vector3 worldPos;

	// Token: 0x04000EB4 RID: 3764
	public Vector3 worldNrm;

	// Token: 0x04000EB5 RID: 3765
	public bool attached;

	// Token: 0x04000EB6 RID: 3766
	public Transform transform;

	// Token: 0x04000EB7 RID: 3767
	public GameObject gameObject;

	// Token: 0x04000EB8 RID: 3768
	public string pooledString;

	// Token: 0x04000EB9 RID: 3769
	public bool broadcast;

	// Token: 0x04000EBA RID: 3770
	private static Effect reusableInstace = new Effect();

	// Token: 0x0600120B RID: 4619 RVA: 0x0000FA7D File Offset: 0x0000DC7D
	public Effect()
	{
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0000FA85 File Offset: 0x0000DC85
	public Effect(string effectName, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x0000FA9F File Offset: 0x0000DC9F
	public Effect(string effectName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x000769E0 File Offset: 0x00074BE0
	public void Init(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = true;
		this.origin = posLocal;
		this.normal = normLocal;
		this.gameObject = null;
		this.Up = Vector3.zero;
		this.transform = ent.FindBone(StringPool.Get(boneID));
		this.worldPos = this.transform.localToWorldMatrix.MultiplyPoint(posLocal);
		this.worldNrm = this.transform.localToWorldMatrix.MultiplyVector(normLocal);
		if (ent != null && !ent.IsValid())
		{
			Debug.LogWarning("Effect.Init - invalid entity");
		}
		this.entity = (ent.IsValid() ? ent.net.ID : 0U);
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
		this.bone = boneID;
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x00076AC0 File Offset: 0x00074CC0
	public void Init(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = false;
		this.worldPos = posWorld;
		this.worldNrm = normWorld;
		this.gameObject = null;
		this.Up = Vector3.zero;
		this.entity = 0U;
		this.origin = this.worldPos;
		this.normal = this.worldNrm;
		this.bone = 0U;
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x00076B3C File Offset: 0x00074D3C
	public bool NetworkConstruct()
	{
		this.pooledString = StringPool.Get(this.pooledstringid);
		if (this.entity <= 0U)
		{
			this.worldPos = this.origin;
			this.worldNrm = this.normal;
			return true;
		}
		BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(this.entity) as BaseEntity;
		if (baseEntity == null)
		{
			if (Global.developer > 0)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Missing entity ",
					this.entity,
					" for effect ",
					this.pooledString,
					" (",
					this.pooledstringid,
					")"
				}));
			}
			return false;
		}
		if (this.bone > 0U)
		{
			if (this.bone == StringPool.closest)
			{
				this.transform = baseEntity.FindClosestBone(this.origin);
				this.worldPos = this.transform.position;
				this.worldNrm = (this.worldPos - this.origin).normalized;
				this.worldPos += this.worldNrm * 0.1f;
				this.origin = this.transform.worldToLocalMatrix.MultiplyPoint(this.worldPos);
				this.normal = this.transform.worldToLocalMatrix.MultiplyVector(this.worldNrm);
				this.attached = true;
				return true;
			}
			this.transform = baseEntity.FindBone(StringPool.Get(this.bone));
			if (this.transform == null)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Missing bone ",
					this.bone,
					" on entity ",
					baseEntity,
					" for effect ",
					this.pooledString,
					" (",
					this.pooledstringid,
					")"
				}));
				return false;
			}
		}
		else
		{
			this.transform = baseEntity.transform;
		}
		this.worldPos = this.transform.localToWorldMatrix.MultiplyPoint(this.origin);
		this.worldNrm = this.transform.localToWorldMatrix.MultiplyVector(this.normal);
		this.attached = true;
		return true;
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x0000FABD File Offset: 0x0000DCBD
	public void Clear()
	{
		this.worldPos = Vector3.zero;
		this.worldNrm = Vector3.zero;
		this.attached = false;
		this.transform = null;
		this.gameObject = null;
		this.pooledString = null;
		this.broadcast = false;
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x00076DA0 File Offset: 0x00074FA0
	public static void Strip(GameObject obj)
	{
		EffectRecycle[] componentsInChildren = obj.GetComponentsInChildren<EffectRecycle>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Recycle();
		}
	}

	// Token: 0x0200026F RID: 623
	public enum Type : uint
	{
		// Token: 0x04000EBC RID: 3772
		Generic,
		// Token: 0x04000EBD RID: 3773
		Projectile
	}

	// Token: 0x02000270 RID: 624
	public static class client
	{
		// Token: 0x06001214 RID: 4628 RVA: 0x0000FB04 File Offset: 0x0000DD04
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
			Effect.reusableInstace.Init(fxtype, ent, boneID, posLocal, normLocal, null);
			EffectLibrary.Run(Effect.reusableInstace);
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x0000FB21 File Offset: 0x0000DD21
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, null);
			Effect.reusableInstace.pooledString = strName;
			EffectLibrary.Run(Effect.reusableInstace);
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x0000FB52 File Offset: 0x0000DD52
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Vector3 up = default(Vector3))
		{
			Effect.reusableInstace.Init(fxtype, posWorld, normWorld, null);
			Effect.reusableInstace.Up = up;
			EffectLibrary.Run(Effect.reusableInstace);
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x0000FB77 File Offset: 0x0000DD77
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Vector3 up = default(Vector3))
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, posWorld, normWorld, null);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.Up = up;
			EffectLibrary.Run(Effect.reusableInstace);
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00076DCC File Offset: 0x00074FCC
		public static void Run(string strName, GameObject obj)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, Vector3.zero, Vector3.zero, null);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.gameObject = obj;
			EffectLibrary.Run(Effect.reusableInstace);
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x00076E18 File Offset: 0x00075018
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.client.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal + info.HitNormalLocal * 0.1f, info.HitNormalLocal);
				return;
			}
			Effect.client.Run(effectName, info.HitPositionWorld + info.HitNormalWorld * 0.1f, info.HitNormalWorld, default(Vector3));
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x00076E98 File Offset: 0x00075098
		public static void ImpactEffect(HitInfo info)
		{
			string materialName = StringPool.Get(info.HitMaterial);
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld))
			{
				return;
			}
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Effect.client.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				if (info.DoDecals)
				{
					Effect.client.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				}
			}
			else
			{
				Effect.client.Run(strName, info.HitPositionWorld, info.HitNormalWorld, default(Vector3));
				Effect.client.Run(decal, info.HitPositionWorld, info.HitNormalWorld, default(Vector3));
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Effect.client.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
					}
					else
					{
						Effect.client.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, default(Vector3));
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}

	// Token: 0x02000271 RID: 625
	public static class server
	{
		// Token: 0x0600121B RID: 4635 RVA: 0x00002ECE File Offset: 0x000010CE
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0000FBB0 File Offset: 0x0000DDB0
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00002ECE File Offset: 0x000010CE
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null, bool broadcast = false)
		{
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0000FBB0 File Offset: 0x0000DDB0
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Connection sourceConnection = null, bool broadcast = false)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x00077070 File Offset: 0x00075270
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.server.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				return;
			}
			Effect.server.Run(effectName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x000770CC File Offset: 0x000752CC
		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string materialName = StringPool.Get(info.HitMaterial);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < WaterLevel.GetWaterDepth(info.HitPositionWorld))
			{
				return;
			}
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Effect.server.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				Effect.server.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
			}
			else
			{
				Effect.server.Run(strName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
				Effect.server.Run(decal, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Effect.server.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
					}
					else
					{
						Effect.server.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}
}
