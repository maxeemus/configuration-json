using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Maxeemus.Utilities.Configuration.Json
{
    /// <summary>
    /// Read and Merge json files
    /// </summary>
    public class JsonMergeFileConfigLoader : IJsonConfigurationLoader
    {
        private readonly IJsonConfigurationLoader[] loaders;
        private readonly JsonMergeSettings mergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };

        public JsonMergeFileConfigLoader(params IJsonConfigurationLoader[] loaders)
        {
            this.loaders = loaders ?? new IJsonConfigurationLoader[0];
        }

        public string LoadJsonText()
        {
            if(loaders.Length == 0) return null;
            return loaders.Aggregate(new JObject(), (merged, l) =>
            {
                merged.Merge(JObject.Parse(l.LoadJsonText()), mergeSettings);
                return merged;
            }).ToString();
        }
    }
}
