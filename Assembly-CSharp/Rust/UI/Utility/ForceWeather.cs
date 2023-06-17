﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
	// Token: 0x020008B6 RID: 2230
	[RequireComponent(typeof(Toggle))]
	internal class ForceWeather : MonoBehaviour
	{
		// Token: 0x04002AF3 RID: 10995
		private Toggle component;

		// Token: 0x04002AF4 RID: 10996
		public bool Rain;

		// Token: 0x04002AF5 RID: 10997
		public bool Fog;

		// Token: 0x04002AF6 RID: 10998
		public bool Wind;

		// Token: 0x04002AF7 RID: 10999
		public bool Clouds;

		// Token: 0x06003020 RID: 12320 RVA: 0x000250D9 File Offset: 0x000232D9
		public void OnEnable()
		{
			this.component = base.GetComponent<Toggle>();
		}

		// Token: 0x06003021 RID: 12321 RVA: 0x000EBFF0 File Offset: 0x000EA1F0
		public void Update()
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			if (this.Rain)
			{
				SingletonComponent<Climate>.Instance.Overrides.Rain = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Rain, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Fog)
			{
				SingletonComponent<Climate>.Instance.Overrides.Fog = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Fog, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Wind)
			{
				SingletonComponent<Climate>.Instance.Overrides.Wind = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Wind, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Clouds)
			{
				SingletonComponent<Climate>.Instance.Overrides.Clouds = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Clouds, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
		}
	}
}
