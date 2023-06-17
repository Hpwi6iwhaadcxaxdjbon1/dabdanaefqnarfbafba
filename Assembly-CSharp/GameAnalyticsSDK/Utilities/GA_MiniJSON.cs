using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameAnalyticsSDK.Utilities
{
	// Token: 0x02000911 RID: 2321
	public class GA_MiniJSON
	{
		// Token: 0x0600319D RID: 12701 RVA: 0x00025D5D File Offset: 0x00023F5D
		public static object Deserialize(string json)
		{
			if (json == null)
			{
				return null;
			}
			return GA_MiniJSON.Parser.Parse(json);
		}

		// Token: 0x0600319E RID: 12702 RVA: 0x00025D6A File Offset: 0x00023F6A
		public static string Serialize(object obj)
		{
			return GA_MiniJSON.Serializer.Serialize(obj);
		}

		// Token: 0x02000912 RID: 2322
		private sealed class Parser : IDisposable
		{
			// Token: 0x04002C40 RID: 11328
			private const string WORD_BREAK = "{}[],:\"";

			// Token: 0x04002C41 RID: 11329
			private StringReader json;

			// Token: 0x060031A0 RID: 12704 RVA: 0x00025D72 File Offset: 0x00023F72
			public static bool IsWordBreak(char c)
			{
				return char.IsWhiteSpace(c) || "{}[],:\"".IndexOf(c) != -1;
			}

			// Token: 0x060031A1 RID: 12705 RVA: 0x00025D8F File Offset: 0x00023F8F
			private Parser(string jsonString)
			{
				this.json = new StringReader(jsonString);
			}

			// Token: 0x060031A2 RID: 12706 RVA: 0x000ED994 File Offset: 0x000EBB94
			public static object Parse(string jsonString)
			{
				object result;
				using (GA_MiniJSON.Parser parser = new GA_MiniJSON.Parser(jsonString))
				{
					result = parser.ParseValue();
				}
				return result;
			}

			// Token: 0x060031A3 RID: 12707 RVA: 0x00025DA3 File Offset: 0x00023FA3
			public void Dispose()
			{
				this.json.Dispose();
				this.json = null;
			}

			// Token: 0x060031A4 RID: 12708 RVA: 0x000ED9CC File Offset: 0x000EBBCC
			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				this.json.Read();
				for (;;)
				{
					GA_MiniJSON.Parser.TOKEN nextToken = this.NextToken;
					if (nextToken == GA_MiniJSON.Parser.TOKEN.NONE)
					{
						break;
					}
					if (nextToken == GA_MiniJSON.Parser.TOKEN.CURLY_CLOSE)
					{
						return dictionary;
					}
					if (nextToken != GA_MiniJSON.Parser.TOKEN.COMMA)
					{
						string text = this.ParseString();
						if (text == null)
						{
							goto Block_4;
						}
						if (this.NextToken != GA_MiniJSON.Parser.TOKEN.COLON)
						{
							goto Block_5;
						}
						this.json.Read();
						dictionary[text] = this.ParseValue();
					}
				}
				return null;
				Block_4:
				return null;
				Block_5:
				return null;
			}

			// Token: 0x060031A5 RID: 12709 RVA: 0x000EDA34 File Offset: 0x000EBC34
			private List<object> ParseArray()
			{
				List<object> list = new List<object>();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					GA_MiniJSON.Parser.TOKEN nextToken = this.NextToken;
					if (nextToken == GA_MiniJSON.Parser.TOKEN.NONE)
					{
						return null;
					}
					if (nextToken != GA_MiniJSON.Parser.TOKEN.SQUARED_CLOSE)
					{
						if (nextToken != GA_MiniJSON.Parser.TOKEN.COMMA)
						{
							object obj = this.ParseByToken(nextToken);
							list.Add(obj);
						}
					}
					else
					{
						flag = false;
					}
				}
				return list;
			}

			// Token: 0x060031A6 RID: 12710 RVA: 0x000EDA84 File Offset: 0x000EBC84
			private object ParseValue()
			{
				GA_MiniJSON.Parser.TOKEN nextToken = this.NextToken;
				return this.ParseByToken(nextToken);
			}

			// Token: 0x060031A7 RID: 12711 RVA: 0x000EDAA0 File Offset: 0x000EBCA0
			private object ParseByToken(GA_MiniJSON.Parser.TOKEN token)
			{
				switch (token)
				{
				case GA_MiniJSON.Parser.TOKEN.CURLY_OPEN:
					return this.ParseObject();
				case GA_MiniJSON.Parser.TOKEN.SQUARED_OPEN:
					return this.ParseArray();
				case GA_MiniJSON.Parser.TOKEN.STRING:
					return this.ParseString();
				case GA_MiniJSON.Parser.TOKEN.NUMBER:
					return this.ParseNumber();
				case GA_MiniJSON.Parser.TOKEN.TRUE:
					return true;
				case GA_MiniJSON.Parser.TOKEN.FALSE:
					return false;
				case GA_MiniJSON.Parser.TOKEN.NULL:
					return null;
				}
				return null;
			}

			// Token: 0x060031A8 RID: 12712 RVA: 0x000EDB10 File Offset: 0x000EBD10
			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					if (this.json.Peek() == -1)
					{
						break;
					}
					char nextChar = this.NextChar;
					if (nextChar != '"')
					{
						if (nextChar != '\\')
						{
							stringBuilder.Append(nextChar);
						}
						else if (this.json.Peek() == -1)
						{
							flag = false;
						}
						else
						{
							nextChar = this.NextChar;
							if (nextChar <= '\\')
							{
								if (nextChar == '"' || nextChar == '/' || nextChar == '\\')
								{
									stringBuilder.Append(nextChar);
								}
							}
							else if (nextChar <= 'f')
							{
								if (nextChar != 'b')
								{
									if (nextChar == 'f')
									{
										stringBuilder.Append('\f');
									}
								}
								else
								{
									stringBuilder.Append('\b');
								}
							}
							else if (nextChar != 'n')
							{
								switch (nextChar)
								{
								case 'r':
									stringBuilder.Append('\r');
									break;
								case 't':
									stringBuilder.Append('\t');
									break;
								case 'u':
								{
									char[] array = new char[4];
									for (int i = 0; i < 4; i++)
									{
										array[i] = this.NextChar;
									}
									stringBuilder.Append((char)Convert.ToInt32(new string(array), 16));
									break;
								}
								}
							}
							else
							{
								stringBuilder.Append('\n');
							}
						}
					}
					else
					{
						flag = false;
					}
				}
				return stringBuilder.ToString();
			}

			// Token: 0x060031A9 RID: 12713 RVA: 0x000EDC60 File Offset: 0x000EBE60
			private object ParseNumber()
			{
				string nextWord = this.NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long num;
					long.TryParse(nextWord, ref num);
					return num;
				}
				double num2;
				double.TryParse(nextWord, ref num2);
				return num2;
			}

			// Token: 0x060031AA RID: 12714 RVA: 0x00025DB7 File Offset: 0x00023FB7
			private void EatWhitespace()
			{
				while (char.IsWhiteSpace(this.PeekChar))
				{
					this.json.Read();
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
			}

			// Token: 0x17000422 RID: 1058
			// (get) Token: 0x060031AB RID: 12715 RVA: 0x00025DE2 File Offset: 0x00023FE2
			private char PeekChar
			{
				get
				{
					return Convert.ToChar(this.json.Peek());
				}
			}

			// Token: 0x17000423 RID: 1059
			// (get) Token: 0x060031AC RID: 12716 RVA: 0x00025DF4 File Offset: 0x00023FF4
			private char NextChar
			{
				get
				{
					return Convert.ToChar(this.json.Read());
				}
			}

			// Token: 0x17000424 RID: 1060
			// (get) Token: 0x060031AD RID: 12717 RVA: 0x000EDCA0 File Offset: 0x000EBEA0
			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (!GA_MiniJSON.Parser.IsWordBreak(this.PeekChar))
					{
						stringBuilder.Append(this.NextChar);
						if (this.json.Peek() == -1)
						{
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}

			// Token: 0x17000425 RID: 1061
			// (get) Token: 0x060031AE RID: 12718 RVA: 0x000EDCE4 File Offset: 0x000EBEE4
			private GA_MiniJSON.Parser.TOKEN NextToken
			{
				get
				{
					this.EatWhitespace();
					if (this.json.Peek() == -1)
					{
						return GA_MiniJSON.Parser.TOKEN.NONE;
					}
					char peekChar = this.PeekChar;
					if (peekChar <= '[')
					{
						switch (peekChar)
						{
						case '"':
							return GA_MiniJSON.Parser.TOKEN.STRING;
						case '#':
						case '$':
						case '%':
						case '&':
						case '\'':
						case '(':
						case ')':
						case '*':
						case '+':
						case '.':
						case '/':
							break;
						case ',':
							this.json.Read();
							return GA_MiniJSON.Parser.TOKEN.COMMA;
						case '-':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							return GA_MiniJSON.Parser.TOKEN.NUMBER;
						case ':':
							return GA_MiniJSON.Parser.TOKEN.COLON;
						default:
							if (peekChar == '[')
							{
								return GA_MiniJSON.Parser.TOKEN.SQUARED_OPEN;
							}
							break;
						}
					}
					else
					{
						if (peekChar == ']')
						{
							this.json.Read();
							return GA_MiniJSON.Parser.TOKEN.SQUARED_CLOSE;
						}
						if (peekChar == '{')
						{
							return GA_MiniJSON.Parser.TOKEN.CURLY_OPEN;
						}
						if (peekChar == '}')
						{
							this.json.Read();
							return GA_MiniJSON.Parser.TOKEN.CURLY_CLOSE;
						}
					}
					string nextWord = this.NextWord;
					if (nextWord == "false")
					{
						return GA_MiniJSON.Parser.TOKEN.FALSE;
					}
					if (nextWord == "true")
					{
						return GA_MiniJSON.Parser.TOKEN.TRUE;
					}
					if (!(nextWord == "null"))
					{
						return GA_MiniJSON.Parser.TOKEN.NONE;
					}
					return GA_MiniJSON.Parser.TOKEN.NULL;
				}
			}

			// Token: 0x02000913 RID: 2323
			private enum TOKEN
			{
				// Token: 0x04002C43 RID: 11331
				NONE,
				// Token: 0x04002C44 RID: 11332
				CURLY_OPEN,
				// Token: 0x04002C45 RID: 11333
				CURLY_CLOSE,
				// Token: 0x04002C46 RID: 11334
				SQUARED_OPEN,
				// Token: 0x04002C47 RID: 11335
				SQUARED_CLOSE,
				// Token: 0x04002C48 RID: 11336
				COLON,
				// Token: 0x04002C49 RID: 11337
				COMMA,
				// Token: 0x04002C4A RID: 11338
				STRING,
				// Token: 0x04002C4B RID: 11339
				NUMBER,
				// Token: 0x04002C4C RID: 11340
				TRUE,
				// Token: 0x04002C4D RID: 11341
				FALSE,
				// Token: 0x04002C4E RID: 11342
				NULL
			}
		}

		// Token: 0x02000914 RID: 2324
		private sealed class Serializer
		{
			// Token: 0x04002C4F RID: 11343
			private StringBuilder builder;

			// Token: 0x060031AF RID: 12719 RVA: 0x00025E06 File Offset: 0x00024006
			private Serializer()
			{
				this.builder = new StringBuilder();
			}

			// Token: 0x060031B0 RID: 12720 RVA: 0x00025E19 File Offset: 0x00024019
			public static string Serialize(object obj)
			{
				GA_MiniJSON.Serializer serializer = new GA_MiniJSON.Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			// Token: 0x060031B1 RID: 12721 RVA: 0x000EDE08 File Offset: 0x000EC008
			private void SerializeValue(object value)
			{
				if (value == null)
				{
					this.builder.Append("null");
					return;
				}
				string str;
				if ((str = (value as string)) != null)
				{
					this.SerializeString(str);
					return;
				}
				if (value is bool)
				{
					this.builder.Append(((bool)value) ? "true" : "false");
					return;
				}
				IList anArray;
				if ((anArray = (value as IList)) != null)
				{
					this.SerializeArray(anArray);
					return;
				}
				IDictionary obj;
				if ((obj = (value as IDictionary)) != null)
				{
					this.SerializeObject(obj);
					return;
				}
				if (value is char)
				{
					this.SerializeString(new string((char)value, 1));
					return;
				}
				this.SerializeOther(value);
			}

			// Token: 0x060031B2 RID: 12722 RVA: 0x000EDEAC File Offset: 0x000EC0AC
			private void SerializeObject(IDictionary obj)
			{
				bool flag = true;
				this.builder.Append('{');
				foreach (object obj2 in obj.Keys)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeString(obj2.ToString());
					this.builder.Append(':');
					this.SerializeValue(obj[obj2]);
					flag = false;
				}
				this.builder.Append('}');
			}

			// Token: 0x060031B3 RID: 12723 RVA: 0x000EDF54 File Offset: 0x000EC154
			private void SerializeArray(IList anArray)
			{
				this.builder.Append('[');
				bool flag = true;
				foreach (object value in anArray)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeValue(value);
					flag = false;
				}
				this.builder.Append(']');
			}

			// Token: 0x060031B4 RID: 12724 RVA: 0x000EDFD4 File Offset: 0x000EC1D4
			private void SerializeString(string str)
			{
				this.builder.Append('"');
				char[] array = str.ToCharArray();
				int i = 0;
				while (i < array.Length)
				{
					char c = array[i];
					switch (c)
					{
					case '\b':
						this.builder.Append("\\b");
						break;
					case '\t':
						this.builder.Append("\\t");
						break;
					case '\n':
						this.builder.Append("\\n");
						break;
					case '\v':
						goto IL_E0;
					case '\f':
						this.builder.Append("\\f");
						break;
					case '\r':
						this.builder.Append("\\r");
						break;
					default:
						if (c != '"')
						{
							if (c != '\\')
							{
								goto IL_E0;
							}
							this.builder.Append("\\\\");
						}
						else
						{
							this.builder.Append("\\\"");
						}
						break;
					}
					IL_129:
					i++;
					continue;
					IL_E0:
					int num = Convert.ToInt32(c);
					if (num >= 32 && num <= 126)
					{
						this.builder.Append(c);
						goto IL_129;
					}
					this.builder.Append("\\u");
					this.builder.Append(num.ToString("x4"));
					goto IL_129;
				}
				this.builder.Append('"');
			}

			// Token: 0x060031B5 RID: 12725 RVA: 0x000EE128 File Offset: 0x000EC328
			private void SerializeOther(object value)
			{
				if (value is float)
				{
					this.builder.Append(((float)value).ToString("R"));
					return;
				}
				if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					this.builder.Append(value);
					return;
				}
				if (value is double || value is decimal)
				{
					this.builder.Append(Convert.ToDouble(value).ToString("R"));
					return;
				}
				this.SerializeString(value.ToString());
			}
		}
	}
}
