using System;
using Apex.AI;
using Apex.AI.Components;

namespace Rust.Ai
{
	// Token: 0x020008B7 RID: 2231
	public class BaseAiUtilityClient : UtilityAIClient
	{
		// Token: 0x06003023 RID: 12323 RVA: 0x000250E7 File Offset: 0x000232E7
		public BaseAiUtilityClient(Guid aiId, IContextProvider contextProvider) : base(aiId, contextProvider)
		{
		}

		// Token: 0x06003024 RID: 12324 RVA: 0x000250F1 File Offset: 0x000232F1
		public BaseAiUtilityClient(IUtilityAI ai, IContextProvider contextProvider) : base(ai, contextProvider)
		{
		}

		// Token: 0x06003025 RID: 12325 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnPause()
		{
		}

		// Token: 0x06003026 RID: 12326 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnResume()
		{
		}

		// Token: 0x06003027 RID: 12327 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnStart()
		{
		}

		// Token: 0x06003028 RID: 12328 RVA: 0x00002ECE File Offset: 0x000010CE
		protected override void OnStop()
		{
		}

		// Token: 0x06003029 RID: 12329 RVA: 0x000250FB File Offset: 0x000232FB
		public void Initialize()
		{
			base.Start();
		}

		// Token: 0x0600302A RID: 12330 RVA: 0x00025103 File Offset: 0x00023303
		public void Kill()
		{
			base.Stop();
		}
	}
}
