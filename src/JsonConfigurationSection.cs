using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Maxeemus.Utilities.Configuration.Json
{
    /// <summary>
    /// Json Configuration Section
    /// Loads/Reloads and provides section resent value         
    /// </summary>
    public class JsonConfigurationSection<T> : IJsonConfigurationSection<T> where T : class
    {
        private readonly IJsonConfigurationLoader loader;
        private readonly Func<dynamic, object> sectionMap;

        public JsonConfigurationSection(IJsonConfigurationLoader loader, Func<dynamic, object> sectionMap)
        {
            Debug.Assert(loader != null);
            Debug.Assert(sectionMap != null);

            this.loader = loader;
            this.sectionMap = sectionMap;
        }

        public T Value
        {
            get
            {
                var jsonText = loader.LoadJsonText() ?? "{}";
                var desirializedJson = JObject.Parse(jsonText);
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(sectionMap(desirializedJson)), new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            }
        }
    }
}
