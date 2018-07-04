using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace Utils
{
    public enum LogType { NORMAL, ABNORMAL, ERROR };

    /// <summary>
    /// Provides writing a logs to "Logs" directory by days.
    /// Logs separates for LogType types.
    /// Used local DateTime value in yyyy-MM-dd HH:mm:ss format.
    /// </summary>
    public static class Log
    {
        private static string path;

        /// <summary>
        /// Writes a log to file.
        /// </summary>
        /// <param name="type">Type of log record.</param>
        /// <param name="ex">Exception object to write into log. Nullable.</param>
        /// <param name="msg">Description message.</param>
        public static void Write(LogType type, Exception ex, string msg)
        {
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Logs\" + DateTime.Now.ToString("yyyyMM") + @"\log" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            FileInfo log = new FileInfo(path);
            if (!(log.Directory.Exists))
                Directory.CreateDirectory(log.DirectoryName);
            lock ("lock")
            {
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine("{0}: {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), type.ToString().PadRight(10), ex == null ? msg : (msg + "\r\n" + ex.ToString()).Replace("    ", "\n").Replace("\n", "\n\t"));
                }
            }
        }
    }
}
