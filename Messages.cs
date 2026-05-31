using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DiplomaDefense.Core
{
    // тут беремо всі тексти з файлу messages.json
    // так зручно бо можна змінити мову не чіпаючи код
    public static class Messages
    {
        private static JsonDocument doc;
        private static readonly string FileName = "messages.json";

        // читаємо файл один раз на старті
        static Messages()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    string json = File.ReadAllText(FileName, Encoding.UTF8);
                    doc = JsonDocument.Parse(json);
                }
                else
                {
                    doc = null;
                }
            }
            catch
            {
                doc = null;
            }
        }

        // дістаємо текст за ключем
        // якщо файлу нема або ключа нема - просто покажемо ключ у дужках
        public static string Get(string key)
        {
            if (doc == null)
                return "[" + key + "]";
            try
            {
                return doc.RootElement.GetProperty(key).GetString() ?? ("[" + key + "]");
            }
            catch
            {
                return "[" + key + "]";
            }
        }
    }
}
