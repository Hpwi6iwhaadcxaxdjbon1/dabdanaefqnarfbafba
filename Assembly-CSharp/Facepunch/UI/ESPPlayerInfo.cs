using System;
using System.Collections.Generic;
using ConVar;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

namespace Facepunch.UI
{
	// Token: 0x020008A1 RID: 2209
	public class ESPPlayerInfo : MonoBehaviour
	{
		// Token: 0x04002A7E RID: 10878
		public Vector3 WorldOffset;

		// Token: 0x04002A7F RID: 10879
		protected Text Text;

		// Token: 0x04002A80 RID: 10880
		protected Image Image;

		// Token: 0x04002A81 RID: 10881
		public Gradient gradientNormal;

		// Token: 0x04002A82 RID: 10882
		public Gradient gradientTeam;

		// Token: 0x04002A83 RID: 10883
		public QueryVis visCheck;

		// Token: 0x04002A85 RID: 10885
		private float distanceFromCamera = float.PositiveInfinity;

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06002FB4 RID: 12212 RVA: 0x0002498E File Offset: 0x00022B8E
		// (set) Token: 0x06002FB5 RID: 12213 RVA: 0x00024996 File Offset: 0x00022B96
		public BasePlayer Entity { get; set; }

		// Token: 0x06002FB6 RID: 12214 RVA: 0x0002499F File Offset: 0x00022B9F
		private void Awake()
		{
			ComponentExtensions.GetComponentInChildren<ESPPlayerInfo, Text>(this, ref this.Text);
			ComponentExtensions.GetComponentInChildren<ESPPlayerInfo, Image>(this, ref this.Image);
		}

		// Token: 0x06002FB7 RID: 12215 RVA: 0x000EB124 File Offset: 0x000E9324
		public void Clear()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			this.Entity = null;
			base.gameObject.SetActive(false);
			if (this.visCheck != null)
			{
				Object.Destroy(this.visCheck.gameObject);
			}
		}

		// Token: 0x06002FB8 RID: 12216 RVA: 0x000EB170 File Offset: 0x000E9370
		internal void Init(BasePlayer entity)
		{
			base.gameObject.SetActive(true);
			this.Entity = entity;
			this.Text.text = entity.displayName;
			this.Text.text = (Global.streamermode ? RandomUsernames.Get(this.Entity.userID) : entity.displayName);
			this.visCheck = new GameObject().AddComponent<QueryVis>();
			this.visCheck.name = "NameTagVisCheck";
			this.visCheck.transform.position = this.GetVisCheckPosition();
		}

		// Token: 0x06002FB9 RID: 12217 RVA: 0x000249BB File Offset: 0x00022BBB
		private void SetColor(Color color)
		{
			if (this.Image.color.a == color.a)
			{
				return;
			}
			this.Text.color = color;
			this.Image.color = color;
		}

		// Token: 0x06002FBA RID: 12218 RVA: 0x000249EE File Offset: 0x00022BEE
		private void LateUpdate()
		{
			this.VisCheckPosition();
			this.UpdateColor();
			if (this.Image.color.a > 0f)
			{
				this.Position();
			}
		}

		// Token: 0x06002FBB RID: 12219 RVA: 0x000EB204 File Offset: 0x000E9404
		public Vector3 GetVisCheckPosition()
		{
			if (!(this.Entity == null) && !(this.Entity.playerModel == null))
			{
				return this.Entity.playerModel.headBone.transform.position + this.WorldOffset;
			}
			return Vector3.zero;
		}

		// Token: 0x06002FBC RID: 12220 RVA: 0x00024A19 File Offset: 0x00022C19
		public void VisCheckPosition()
		{
			if (this.visCheck != null)
			{
				this.visCheck.transform.position = this.GetVisCheckPosition();
			}
		}

		// Token: 0x06002FBD RID: 12221 RVA: 0x000EB260 File Offset: 0x000E9460
		public bool IsTeamMember()
		{
			if (LocalPlayer.Entity == null)
			{
				return false;
			}
			if (LocalPlayer.Entity.clientTeam == null)
			{
				return false;
			}
			using (List<PlayerTeam.TeamMember>.Enumerator enumerator = LocalPlayer.Entity.clientTeam.members.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.userID == this.Entity.userID)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06002FBE RID: 12222 RVA: 0x00024A3F File Offset: 0x00022C3F
		public void SetTextEnabled(bool wantsOn)
		{
			this.Text.enabled = wantsOn;
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x000EB2EC File Offset: 0x000E94EC
		private void UpdateColor()
		{
			if (this.Entity == null || this.Entity.playerModel == null || this.Entity.IsDead() || SingletonComponent<MainCamera>.Instance == null)
			{
				this.SetColor(new Color(0f, 0f, 0f, 0f));
				return;
			}
			Vector3 pos = this.Entity.playerModel.headBone.transform.position + this.WorldOffset;
			this.distanceFromCamera = MainCamera.Distance(pos);
			float num = Mathf.InverseLerp(4f, 3.75f, this.distanceFromCamera);
			Color color = this.gradientNormal.Evaluate(0.5f);
			if (TOD_Sky.Instance != null)
			{
				color = this.gradientNormal.Evaluate(TOD_Sky.Instance.Cycle.Hour / 24f);
			}
			Color b = this.gradientTeam.Evaluate(TOD_Sky.Instance.Cycle.Hour / 24f);
			Color color2 = Color.white;
			if (this.IsTeamMember())
			{
				float num2 = Mathf.InverseLerp(152f, 150f, this.distanceFromCamera);
				color2 = Color.green * b;
				color2.a = (nametags.enabled ? num2 : 0f);
				float num3 = Vector3.Dot((this.Entity.eyes.position - MainCamera.position).normalized, MainCamera.rotation * Vector3.forward);
				this.SetTextEnabled(this.distanceFromCamera < 20f || num3 >= 0.999f);
			}
			else
			{
				color2 = (this.IsTeamMember() ? (Color.green * b) : color);
				color2.a = (nametags.enabled ? num : 0f);
				this.SetTextEnabled(true);
			}
			if (this.visCheck != null)
			{
				float num4 = (this.visCheck.SampleVisibility() > 0f) ? 1f : 0f;
				color2.a *= num4;
			}
			this.SetColor(color2);
		}

		// Token: 0x06002FC0 RID: 12224 RVA: 0x000EB520 File Offset: 0x000E9720
		private void Position()
		{
			if (this.Entity == null || this.Entity.playerModel == null)
			{
				return;
			}
			RectTransform rectTransform = base.transform as RectTransform;
			Vector3 position = this.Entity.playerModel.headBone.transform.position + this.WorldOffset;
			if (Mathf.Approximately(this.Image.color.a, 0f))
			{
				return;
			}
			Vector3 vector = MainCamera.mainCamera.WorldToScreenPoint(position);
			if (vector.z < 0f)
			{
				Vector3 vector2 = new Vector3(-10000f, -10000f, 0f);
				if (rectTransform.localPosition != vector2)
				{
					rectTransform.localPosition = vector2;
				}
				return;
			}
			Vector3 lossyScale = rectTransform.lossyScale;
			Vector3 localPosition = new Vector3(vector.x * (1f / lossyScale.x), vector.y * (1f / lossyScale.y), 0f);
			rectTransform.localPosition = localPosition;
		}
	}
}
