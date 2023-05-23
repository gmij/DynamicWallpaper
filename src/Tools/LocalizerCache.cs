using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;

namespace DynamicWallpaper.Tools
{
    public class LocalizerCache
    {
        private static readonly IStringLocalizer<ResourcesHelper> _service;
        private static readonly ConcurrentDictionary<string, IDictionary<string, string>> _localizerResources = new();

        static LocalizerCache()
        {
            _service = ServiceLocator.GetService<IStringLocalizer<ResourcesHelper>>();
            SwitchLang();
        }

        public static bool SwitchLang(string targetCulture = null)
        {
            var lang = targetCulture ?? Thread.CurrentThread.CurrentUICulture.Name;

            if (_localizerResources.TryGetValue(lang, out IDictionary<string, string> langDict))
            {
                DefaultCulture = lang;
                return langDict.Count > 0;
            }

            langDict = GetLocalizerResourceAsync(lang);
            _localizerResources[lang] = langDict;
            DefaultCulture = lang;
            return langDict.Count > 0;
        }

        public static string GetString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            if (_localizerResources.TryGetValue(DefaultCulture, out IDictionary<string, string> langDict) && langDict.TryGetValue(name, out string value))
            {
                return value;
            }

            throw new ArgumentOutOfRangeException(nameof(name));
        }

        private static Dictionary<string, string> GetLocalizerResourceAsync(string lang)
        {
            var allStrings = _service.GetAllStrings();
            return allStrings.ToDictionary(p => p.Name, p => p.Value);
        }

        private static string DefaultCulture { get; set; } = Thread.CurrentThread.CurrentUICulture.Name;
    }
}