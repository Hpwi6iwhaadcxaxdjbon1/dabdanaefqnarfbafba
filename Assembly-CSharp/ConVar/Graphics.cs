using System;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConVar
{
	// Token: 0x02000865 RID: 2149
	[ConsoleSystem.Factory("graphics")]
	public class Graphics : ConsoleSystem
	{
		// Token: 0x04002990 RID: 10640
		private const float MinShadowDistance = 40f;

		// Token: 0x04002991 RID: 10641
		private const float MaxShadowDistance2Split = 180f;

		// Token: 0x04002992 RID: 10642
		private const float MaxShadowDistance4Split = 800f;

		// Token: 0x04002993 RID: 10643
		private static float _shadowdistance = 800f;

		// Token: 0x04002994 RID: 10644
		[ClientVar(Saved = true)]
		public static int shadowmode = 2;

		// Token: 0x04002995 RID: 10645
		[ClientVar(Saved = true)]
		public static int shadowlights = 1;

		// Token: 0x04002996 RID: 10646
		private static int _shadowquality = 1;

		// Token: 0x04002997 RID: 10647
		[ClientVar(Saved = true)]
		public static bool grassshadows = false;

		// Token: 0x04002998 RID: 10648
		[ClientVar(Saved = true)]
		public static bool contactshadows = false;

		// Token: 0x04002999 RID: 10649
		[ClientVar(Saved = true)]
		public static float drawdistance = 2500f;

		// Token: 0x0400299A RID: 10650
		private static float _fov = 75f;

		// Token: 0x0400299B RID: 10651
		[ClientVar]
		public static bool hud = true;

		// Token: 0x0400299C RID: 10652
		[ClientVar(Saved = true)]
		public static bool chat = true;

		// Token: 0x0400299D RID: 10653
		[ClientVar(Saved = true)]
		public static bool branding = true;

		// Token: 0x0400299E RID: 10654
		[ClientVar(Saved = true)]
		public static int compass = 1;

		// Token: 0x0400299F RID: 10655
		[ClientVar(Saved = true)]
		public static bool dof = false;

		// Token: 0x040029A0 RID: 10656
		[ClientVar(Saved = true)]
		public static float dof_aper = 12f;

		// Token: 0x040029A1 RID: 10657
		[ClientVar(Saved = true)]
		public static float dof_blur = 1f;

		// Token: 0x040029A2 RID: 10658
		private static float _uiscale = 1f;

		// Token: 0x040029A3 RID: 10659
		private static int _anisotropic = 1;

		// Token: 0x040029A4 RID: 10660
		private static int _parallax = 0;

		// Token: 0x040029A5 RID: 10661
		private static bool _impostorshadows = false;

		// Token: 0x040029A6 RID: 10662
		private static int _showtexeldensity = 0;

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06002ED4 RID: 11988 RVA: 0x0002407A File Offset: 0x0002227A
		// (set) Token: 0x06002ED5 RID: 11989 RVA: 0x00024081 File Offset: 0x00022281
		[ClientVar(Help = "The currently selected quality level")]
		public static int quality
		{
			get
			{
				return QualitySettings.GetQualityLevel();
			}
			set
			{
				int shadowcascades = Graphics.shadowcascades;
				QualitySettings.SetQualityLevel(value, true);
				Graphics.shadowcascades = shadowcascades;
			}
		}

		// Token: 0x06002ED6 RID: 11990 RVA: 0x000E8E98 File Offset: 0x000E7098
		public static float EnforceShadowDistanceBounds(float distance)
		{
			if (QualitySettings.shadowCascades == 1)
			{
				distance = Mathf.Clamp(distance, 40f, 40f);
			}
			else if (QualitySettings.shadowCascades == 2)
			{
				distance = Mathf.Clamp(distance, 40f, 180f);
			}
			else
			{
				distance = Mathf.Clamp(distance, 40f, 800f);
			}
			return distance;
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06002ED7 RID: 11991 RVA: 0x00024094 File Offset: 0x00022294
		// (set) Token: 0x06002ED8 RID: 11992 RVA: 0x0002409B File Offset: 0x0002229B
		[ClientVar(Saved = true)]
		public static float shadowdistance
		{
			get
			{
				return Graphics._shadowdistance;
			}
			set
			{
				Graphics._shadowdistance = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics._shadowdistance);
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06002ED9 RID: 11993 RVA: 0x000240B2 File Offset: 0x000222B2
		// (set) Token: 0x06002EDA RID: 11994 RVA: 0x000240B9 File Offset: 0x000222B9
		[ClientVar(Saved = true)]
		public static int shadowcascades
		{
			get
			{
				return QualitySettings.shadowCascades;
			}
			set
			{
				QualitySettings.shadowCascades = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics.shadowdistance);
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06002EDB RID: 11995 RVA: 0x000240D0 File Offset: 0x000222D0
		// (set) Token: 0x06002EDC RID: 11996 RVA: 0x000E8EF0 File Offset: 0x000E70F0
		[ClientVar(Saved = true)]
		public static int shadowquality
		{
			get
			{
				return Graphics._shadowquality;
			}
			set
			{
				Graphics._shadowquality = Mathf.Clamp(value, 0, 2);
				Graphics.shadowmode = Graphics._shadowquality + 1;
				bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore;
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_HIGH", !flag && Graphics._shadowquality >= 2);
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06002EDD RID: 11997 RVA: 0x000240D7 File Offset: 0x000222D7
		// (set) Token: 0x06002EDE RID: 11998 RVA: 0x000240DE File Offset: 0x000222DE
		[ClientVar(Saved = true)]
		public static float fov
		{
			get
			{
				return Graphics._fov;
			}
			set
			{
				Graphics._fov = Mathf.Clamp(value, 60f, 90f);
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06002EDF RID: 11999 RVA: 0x000240F5 File Offset: 0x000222F5
		// (set) Token: 0x06002EE0 RID: 12000 RVA: 0x000240FC File Offset: 0x000222FC
		[ClientVar]
		public static float lodbias
		{
			get
			{
				return QualitySettings.lodBias;
			}
			set
			{
				QualitySettings.lodBias = Mathf.Clamp(value, 0.25f, 5f);
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06002EE1 RID: 12001 RVA: 0x00024113 File Offset: 0x00022313
		// (set) Token: 0x06002EE2 RID: 12002 RVA: 0x0002411A File Offset: 0x0002231A
		[ClientVar(Saved = true)]
		public static int shaderlod
		{
			get
			{
				return Shader.globalMaximumLOD;
			}
			set
			{
				Shader.globalMaximumLOD = Mathf.Clamp(value, 100, 600);
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06002EE3 RID: 12003 RVA: 0x0002412E File Offset: 0x0002232E
		// (set) Token: 0x06002EE4 RID: 12004 RVA: 0x00024135 File Offset: 0x00022335
		[ClientVar(Saved = true)]
		public static float uiscale
		{
			get
			{
				return Graphics._uiscale;
			}
			set
			{
				Graphics._uiscale = Mathf.Clamp(value, 0.5f, 1f);
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06002EE5 RID: 12005 RVA: 0x0002414C File Offset: 0x0002234C
		// (set) Token: 0x06002EE6 RID: 12006 RVA: 0x00024153 File Offset: 0x00022353
		[ClientVar(Saved = true)]
		public static int af
		{
			get
			{
				return Graphics._anisotropic;
			}
			set
			{
				value = Mathf.Clamp(value, 1, 16);
				Texture.SetGlobalAnisotropicFilteringLimits(1, value);
				if (value <= 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Disable;
				}
				if (value > 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Enable;
				}
				Graphics._anisotropic = value;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06002EE7 RID: 12007 RVA: 0x00024181 File Offset: 0x00022381
		// (set) Token: 0x06002EE8 RID: 12008 RVA: 0x000E8F3C File Offset: 0x000E713C
		[ClientVar(Saved = true)]
		public static int parallax
		{
			get
			{
				return Graphics._parallax;
			}
			set
			{
				if (value != 1)
				{
					if (value != 2)
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
					else
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.EnableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
				}
				else
				{
					Shader.EnableKeyword("TERRAIN_PARALLAX_OFFSET");
					Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
				}
				Graphics._parallax = value;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06002EE9 RID: 12009 RVA: 0x00024188 File Offset: 0x00022388
		// (set) Token: 0x06002EEA RID: 12010 RVA: 0x0002418F File Offset: 0x0002238F
		[ClientVar]
		public static bool itemskins
		{
			get
			{
				return WorkshopSkin.AllowApply;
			}
			set
			{
				WorkshopSkin.AllowApply = value;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06002EEB RID: 12011 RVA: 0x00024197 File Offset: 0x00022397
		// (set) Token: 0x06002EEC RID: 12012 RVA: 0x0002419E File Offset: 0x0002239E
		[ClientVar]
		public static float itemskintimeout
		{
			get
			{
				return WorkshopSkin.DownloadTimeout;
			}
			set
			{
				WorkshopSkin.DownloadTimeout = value;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06002EED RID: 12013 RVA: 0x000241A6 File Offset: 0x000223A6
		// (set) Token: 0x06002EEE RID: 12014 RVA: 0x000241AD File Offset: 0x000223AD
		[ClientVar(Saved = true)]
		public static bool impostorshadows
		{
			get
			{
				return Graphics._impostorshadows;
			}
			set
			{
				Graphics._impostorshadows = ImpostorShadows.TryToggle(value);
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06002EEF RID: 12015 RVA: 0x000241BA File Offset: 0x000223BA
		// (set) Token: 0x06002EF0 RID: 12016 RVA: 0x000241DC File Offset: 0x000223DC
		[ClientVar(ClientAdmin = true, Help = "Texture density visualization tool: 0=disabled, 1=256, 2=512, 3=1024 texel/meter")]
		public static int showtexeldensity
		{
			get
			{
				if (!(LocalPlayer.Entity != null) || !LocalPlayer.Entity.IsDeveloper)
				{
					return 0;
				}
				return Graphics._showtexeldensity;
			}
			set
			{
				Graphics._showtexeldensity = Mathf.Clamp(value, 0, 3);
			}
		}
	}
}
