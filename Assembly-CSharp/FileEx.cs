using System;
using System.IO;
using System.Threading;

// Token: 0x02000726 RID: 1830
public static class FileEx
{
	// Token: 0x0600280A RID: 10250 RVA: 0x000CE7C8 File Offset: 0x000CC9C8
	public static void Backup(DirectoryInfo parent, params string[] names)
	{
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = Path.Combine(parent.FullName, names[i]);
		}
		FileEx.Backup(names);
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000CE7FC File Offset: 0x000CC9FC
	public static bool MoveToSafe(this FileInfo parent, string target, int retries = 10)
	{
		for (int i = 0; i < retries; i++)
		{
			try
			{
				parent.MoveTo(target);
			}
			catch (Exception)
			{
				Thread.Sleep(5);
				goto IL_19;
			}
			return true;
			IL_19:;
		}
		return false;
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000CE83C File Offset: 0x000CCA3C
	public static void Backup(params string[] names)
	{
		for (int i = names.Length - 2; i >= 0; i--)
		{
			FileInfo fileInfo = new FileInfo(names[i]);
			FileInfo fileInfo2 = new FileInfo(names[i + 1]);
			if (fileInfo.Exists)
			{
				if (fileInfo2.Exists)
				{
					double totalHours = (DateTime.Now - fileInfo2.LastWriteTime).TotalHours;
					int num = (i == 0) ? 0 : (1 << i - 1);
					if (totalHours >= (double)num)
					{
						fileInfo2.Delete();
						fileInfo.MoveToSafe(fileInfo2.FullName, 10);
					}
				}
				else
				{
					if (!fileInfo2.Directory.Exists)
					{
						fileInfo2.Directory.Create();
					}
					fileInfo.MoveToSafe(fileInfo2.FullName, 10);
				}
			}
		}
	}
}
