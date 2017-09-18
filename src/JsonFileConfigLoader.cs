using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Maxeemus.Utilities.Configuration.Json
{
   /// <summary>
   /// Read and cache json file
   /// </summary>
    public class JsonFileConfigLoader : IJsonConfigurationLoader
    {
        private readonly string path;     
                   
        public JsonFileConfigLoader(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            this.path = path;
            
        }

        private string CacheKey => $"{typeof(JsonFileConfigLoader).FullName}.{GetHashCode()}.{path}";

        public string LoadJsonText()
        {
            if (!MemoryCache.Default.Contains(CacheKey))
            {
                using (var reader = new StreamReader(path))
                {
                    var json = reader.ReadToEnd();

                    var cachePolicy = new CacheItemPolicy();
                    cachePolicy.ChangeMonitors.Add(new HostFileChangeMonitor(new[] { path }));
                    MemoryCache.Default.Set(new CacheItem(CacheKey, json), cachePolicy);
                }
            }
            return MemoryCache.Default.Get(CacheKey).ToString();
        }
    }
}
