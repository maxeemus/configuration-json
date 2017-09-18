using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Maxeemus.Utilities.Configuration.Json
{
    /// <summary>
    /// IJsonConfigurationBuilder: Setup and Build JsonConfiguration 
    /// IJsonConfiguration: Provide IJsonConfigurationSection instance
    /// </summary>
    public class JsonConfigurationBuilder : IJsonConfigurationBuilder, IJsonConfiguration
    {        
        private readonly List<IJsonConfigurationLoader> jsonLoaders = new List<IJsonConfigurationLoader>();                        
        private readonly Dictionary<Type, Func<dynamic, object>> sectionMaps = new Dictionary<Type, Func<dynamic, object>>();
        private JsonMergeFileConfigLoader mergeLoader;
        private string baseFilePath;

        public IJsonConfigurationBuilder AddJsonLoader(IJsonConfigurationLoader loader)
        {
            if(loader == null)
                throw new ArgumentNullException(nameof(loader));

            jsonLoaders.Add(loader);
            return this;
        }

        public IJsonConfigurationBuilder SetBasePath(string path)
        {
            if(path == null)
                throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path))
                throw new ArgumentException($"{path} does not exist.");
            baseFilePath = new DirectoryInfo(path).FullName;
            return this;
        }

        public IJsonConfigurationBuilder AddJsonFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            var fullPath = Path.Combine(baseFilePath, path);
            if (File.Exists(fullPath))
            {
                jsonLoaders.Add(new JsonFileConfigLoader(fullPath));
            }
            return this;
        }

        /// <summary>
        /// Add section object accessor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IJsonConfigurationBuilder SectionMap<T>(Func<dynamic, object> func) where T : class 
        {

            if (sectionMaps.ContainsKey(typeof(T)))
                throw new ArgumentException($"Section of {typeof(T).Name} has been already defined.");
            sectionMaps[typeof(T)] = func;
            return this;
        }

        public IJsonConfiguration Build()
        {            
            mergeLoader = new JsonMergeFileConfigLoader(jsonLoaders.ToArray());
            return this;
        }

        /// <summary>
        /// Create IJsonConfigurationSection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IJsonConfigurationSection<T> Section<T>() where T : class
        {
            if (mergeLoader == null)
                throw new ApplicationException("Method Build() has not been invoked.");
            if (!sectionMaps.ContainsKey(typeof(T)))
                throw new ArgumentException($"Section of {typeof(T).Name} is not defined.");

            return new JsonConfigurationSection<T>(mergeLoader, sectionMaps[typeof(T)]);            
        }        
    }
}
