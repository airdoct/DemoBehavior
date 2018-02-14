using System;
using System.IO;
using System.Text;

namespace DemoBehavior
{
	public static class Utils
	{
		public static string mFichLog;

		public static void LogToFile(string Message)
		{
			try
			{
				if (String.IsNullOrEmpty(mFichLog))
				{
					String vFich = System.IO.Path.GetTempPath();
					if (!vFich.EndsWith("\\"))
						vFich += "\\";
					vFich += "Test_SII_";
					vFich += System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
					vFich += ".LOG";
					mFichLog = vFich;
				}
				StreamWriter sw = new StreamWriter(mFichLog, true);
				StringBuilder sb = new StringBuilder();
				sb.Append(DateTime.Now.ToString());
				sb.Append(" - ");
				sb.Append(Message);
				sw.WriteLine(sb.ToString());
				sw.Close();
				sw = null;
			}
			catch (Exception)
			{
				// throw ex;
			}
		}
	}
}
