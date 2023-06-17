using System;
using Apex.AI;
using Apex.AI.Components;

namespace Rust.Ai.HTN
{
	// Token: 0x020008DE RID: 2270
	public class HTNUtilityAiClient : UtilityAIClient
	{
		// Token: 0x0600309B RID: 12443 RVA: 0x000250E7 File Offset: 0x000232E7
		public HTNUtilityAiClient(Guid aiId, IContextProvider contextProvider) : base(aiId, contextProvider)
		{
		}

		// Token: 0x0600309C RID: 12444 RVA: 0x000250F1 File Offset: 0x000232F1
		public HTNUtilityAiClient(IUtilityAI ai, IContextProvider contextProvider) : base(ai, contextProvider)
		{
		}

		// Token: 0x0600309D RID: 12445 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnPause()
		{
		}

		// Token: 0x0600309E RID: 12446 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnResume()
		{
		}

		// Token: 0x0600309F RID: 12447 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnStart()
		{
		}

		// Token: 0x060030A0 RID: 12448 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnStop()
		{
		}

		// Token: 0x060030A1 RID: 12449 RVA: 0x000250FB File Offset: 0x000232FB
		public void Initialize()
		{
			base.Start();
		}

		// Token: 0x060030A2 RID: 12450 RVA: 0x00025103 File Offset: 0x00023303
		public void Kill()
		{
			base.Stop();
		}
	}
}
