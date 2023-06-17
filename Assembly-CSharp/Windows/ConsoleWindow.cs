using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Windows
{
	// Token: 0x02000820 RID: 2080
	[SuppressUnmanagedCodeSecurity]
	public class ConsoleWindow
	{
		// Token: 0x040028D0 RID: 10448
		private TextWriter oldOutput;

		// Token: 0x040028D1 RID: 10449
		private const int STD_INPUT_HANDLE = -10;

		// Token: 0x040028D2 RID: 10450
		private const int STD_OUTPUT_HANDLE = -11;

		// Token: 0x06002D33 RID: 11571 RVA: 0x000E3458 File Offset: 0x000E1658
		public void Initialize()
		{
			ConsoleWindow.FreeConsole();
			if (!ConsoleWindow.AttachConsole(4294967295U))
			{
				ConsoleWindow.AllocConsole();
			}
			this.oldOutput = Console.Out;
			try
			{
				Console.OutputEncoding = Encoding.UTF8;
				Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), 2), Encoding.UTF8)
				{
					AutoFlush = true
				});
			}
			catch (Exception ex)
			{
				Debug.Log("Couldn't redirect output: " + ex.Message);
			}
		}

		// Token: 0x06002D34 RID: 11572 RVA: 0x000233F7 File Offset: 0x000215F7
		public void Shutdown()
		{
			Console.SetOut(this.oldOutput);
			ConsoleWindow.FreeConsole();
		}

		// Token: 0x06002D35 RID: 11573 RVA: 0x0002340A File Offset: 0x0002160A
		public void SetTitle(string strName)
		{
			ConsoleWindow.SetConsoleTitleA(strName);
		}

		// Token: 0x06002D36 RID: 11574
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AttachConsole(uint dwProcessId);

		// Token: 0x06002D37 RID: 11575
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AllocConsole();

		// Token: 0x06002D38 RID: 11576
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		// Token: 0x06002D39 RID: 11577
		[DllImport("kernel32.dll", CallingConvention = 3, CharSet = 4, SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		// Token: 0x06002D3A RID: 11578
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleTitleA(string lpConsoleTitle);
	}
}
