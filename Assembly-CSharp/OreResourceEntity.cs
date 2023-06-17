using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class OreResourceEntity : StagedResourceEntity
{
	// Token: 0x04000625 RID: 1573
	public GameObjectRef bonusPrefab;

	// Token: 0x04000626 RID: 1574
	public GameObjectRef finishEffect;

	// Token: 0x04000627 RID: 1575
	public GameObjectRef bonusFailEffect;

	// Token: 0x04000628 RID: 1576
	public OreHotSpot _hotSpot;

	// Token: 0x04000629 RID: 1577
	public SoundPlayer bonusSound;

	// Token: 0x0600097A RID: 2426 RVA: 0x00050BC8 File Offset: 0x0004EDC8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("OreResourceEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 3437749222U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: PlayBonusLevelSound ");
				}
				using (TimeWarning.New("PlayBonusLevelSound", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.PlayBonusLevelSound(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in PlayBonusLevelSound", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00009892 File Offset: 0x00007A92
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00050CE4 File Offset: 0x0004EEE4
	[BaseEntity.RPC_Client]
	public void PlayBonusLevelSound(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		Vector3 position = msg.read.Vector3();
		SoundModulation.Modulator modulator = SoundManager.PlayOneshot(this.bonusSound.soundDefinition, null, false, position).modulation.CreateModulator(SoundModulation.Parameter.Pitch);
		float num2 = Mathf.Clamp01((float)num / 4f);
		modulator.value = 0.5f + 1.5f * num2;
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00050D48 File Offset: 0x0004EF48
	public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
	{
		Vector2 vector = allowInside ? Random.insideUnitCircle : Random.insideUnitCircle.normalized;
		return new Vector3(vector.x, 0f, vector.y);
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00050D84 File Offset: 0x0004EF84
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset, bool allowInside = true, bool changeHeight = true)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 vector = allowInside ? Random.insideUnitCircle : Random.insideUnitCircle.normalized;
		Vector3 b = new Vector3(vector.x * degreesOffset, changeHeight ? (Random.Range(-1f, 1f) * degreesOffset) : 0f, vector.y * degreesOffset);
		return (input + b).normalized;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00050E04 File Offset: 0x0004F004
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec;
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00050E90 File Offset: 0x0004F090
	public static Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f, bool allowInside = false)
	{
		Vector2 vector = allowInside ? Random.insideUnitCircle : Random.insideUnitCircle.normalized;
		Vector3 result = new Vector3(vector.x, 0f, vector.y).normalized * distance;
		result.y = Random.Range(minHeight, maxHeight);
		return result;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0000989B File Offset: 0x00007A9B
	public Vector3 ClampToCylinder(Vector3 localPos, Vector3 cylinderAxis, float cylinderDistance, float minHeight = 0f, float maxHeight = 0f)
	{
		return Vector3.zero;
	}
}
