using System.Text.Json;

namespace DotNetAssignment.Services
{
    public class SimpleLocalizer
    {
        private readonly Dictionary<string, string> _en = new();
        private readonly Dictionary<string, string> _hi = new();

        public SimpleLocalizer(IWebHostEnvironment env)
        {
            var basePath = Path.Combine(env.ContentRootPath, "Resources", "i18n");
            LoadFile(Path.Combine(basePath, "en.json"), _en);
            LoadFile(Path.Combine(basePath, "hi.json"), _hi);
        }

        private static void LoadFile(string path, Dictionary<string, string> target)
        {
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data == null) return;
            foreach (var kv in data)
                target[kv.Key] = kv.Value;
        }

        public string Get(string key, HttpContext? httpContext)
        {
            var lang = "en";
            var header = httpContext?.Request?.Headers?["Accept-Language"].ToString();
            if (!string.IsNullOrWhiteSpace(header))
            {
                // take first language and normalize to two-letter code
                var first = header.Split(',')[0].Trim();
                if (first.Length >= 2) lang = first.Substring(0, 2).ToLowerInvariant();
            }

            if (lang == "hi" && _hi.TryGetValue(key, out var hiVal))
                return hiVal;

            if (_en.TryGetValue(key, out var enVal))
                return enVal;

            return key; // fallback to key if missing
        }
    }
}
