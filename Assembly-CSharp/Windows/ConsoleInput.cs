using System;
using UnityEngine;

namespace Windows
{
	// Token: 0x0200081F RID: 2079
	public class ConsoleInput
	{
		// Token: 0x040028CD RID: 10445
		public string inputString = "";

		// Token: 0x040028CE RID: 10446
		public string[] statusText = new string[]
		{
			"",
			"",
			""
		};

		// Token: 0x040028CF RID: 10447
		internal float nextUpdate;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06002D28 RID: 11560 RVA: 0x000E31D4 File Offset: 0x000E13D4
		// (remove) Token: 0x06002D29 RID: 11561 RVA: 0x000E320C File Offset: 0x000E140C
		public event Action<string> OnInputText;

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06002D2A RID: 11562 RVA: 0x00023338 File Offset: 0x00021538
		public bool valid
		{
			get
			{
				return Console.BufferWidth > 0;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06002D2B RID: 11563 RVA: 0x00023342 File Offset: 0x00021542
		public int lineWidth
		{
			get
			{
				return Console.BufferWidth;
			}
		}

		// Token: 0x06002D2C RID: 11564 RVA: 0x00023349 File Offset: 0x00021549
		public void ClearLine(int numLines)
		{
			Console.CursorLeft = 0;
			Console.Write(new string(' ', this.lineWidth * numLines));
			Console.CursorTop -= numLines;
			Console.CursorLeft = 0;
		}

		// Token: 0x06002D2D RID: 11565 RVA: 0x000E3244 File Offset: 0x000E1444
		public void RedrawInputLine()
		{
			try
			{
				Console.ForegroundColor = 15;
				Console.CursorTop++;
				for (int i = 0; i < this.statusText.Length; i++)
				{
					Console.CursorLeft = 0;
					Console.Write(this.statusText[i].PadRight(this.lineWidth));
				}
				Console.CursorTop -= this.statusText.Length + 1;
				Console.CursorLeft = 0;
				Console.BackgroundColor = 0;
				Console.ForegroundColor = 10;
				this.ClearLine(1);
				if (this.inputString.Length != 0)
				{
					if (this.inputString.Length < this.lineWidth - 2)
					{
						Console.Write(this.inputString);
					}
					else
					{
						Console.Write(this.inputString.Substring(this.inputString.Length - (this.lineWidth - 2)));
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06002D2E RID: 11566 RVA: 0x00023377 File Offset: 0x00021577
		internal void OnBackspace()
		{
			if (this.inputString.Length < 1)
			{
				return;
			}
			this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
			this.RedrawInputLine();
		}

		// Token: 0x06002D2F RID: 11567 RVA: 0x000233AD File Offset: 0x000215AD
		internal void OnEscape()
		{
			this.inputString = "";
			this.RedrawInputLine();
		}

		// Token: 0x06002D30 RID: 11568 RVA: 0x000E3330 File Offset: 0x000E1530
		internal void OnEnter()
		{
			this.ClearLine(this.statusText.Length);
			Console.ForegroundColor = 10;
			Console.WriteLine("> " + this.inputString);
			string text = this.inputString;
			this.inputString = "";
			if (this.OnInputText != null)
			{
				this.OnInputText.Invoke(text);
			}
			this.RedrawInputLine();
		}

		// Token: 0x06002D31 RID: 11569 RVA: 0x000E3394 File Offset: 0x000E1594
		public void Update()
		{
			if (!this.valid)
			{
				return;
			}
			if (this.nextUpdate < Time.realtimeSinceStartup)
			{
				this.RedrawInputLine();
				this.nextUpdate = Time.realtimeSinceStartup + 0.5f;
			}
			try
			{
				if (!Console.KeyAvailable)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
			if (consoleKeyInfo.Key == 13)
			{
				this.OnEnter();
				return;
			}
			if (consoleKeyInfo.Key == 8)
			{
				this.OnBackspace();
				return;
			}
			if (consoleKeyInfo.Key == 27)
			{
				this.OnEscape();
				return;
			}
			if (consoleKeyInfo.KeyChar != '\0')
			{
				this.inputString += consoleKeyInfo.KeyChar.ToString();
				this.RedrawInputLine();
				return;
			}
		}
	}
}
