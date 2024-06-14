using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotePad.Helper
{
    public class Logger
    {
        private static readonly string logFilePath = "log.txt";

        public static void SaveInfo(LoggerLevel level, object payload)
        {
            EnsureLogFileExists();
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} \t[{level}] \t{payload}";

            try
            {
                using StreamWriter writer = new StreamWriter(logFilePath, true);
                writer.WriteLine(logEntry);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка записи в лог: {ex.Message}");
            }
        }

        private static void EnsureLogFileExists()
        {
            try
            {
                if (!File.Exists(logFilePath))
                {
                    using FileStream fs = File.Create(logFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка создания файла лога: {ex.Message}");
            }
        }
    }

    public enum LoggerLevel
    {
        DEBUG,
        INFO,
        WARN
    }
}
