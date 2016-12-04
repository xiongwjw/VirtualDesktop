using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace VirtualDesktop
{
    public class Loger
    {
        public static void WriteFile(string message)
        {
            string file_name = System.Windows.Forms.Application.StartupPath + "\\Log\\" + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Log\\"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Log\\");
            }
            StreamWriter sw = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.Write);
                using (sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " : " + message);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close();
            }
            catch { }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }

        }


        
    }
}
