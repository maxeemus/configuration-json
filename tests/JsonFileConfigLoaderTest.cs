using System;
using System.IO;
using Maxeemus.Utilities.Configuration.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Maxeemus.Utilities.Configuration.Json.Tests
{
    [TestClass]
    public class JsonFileConfigLoaderTest
    {
        public class Section
        {
            public string Value { get; set; }
        }

        [TestMethod]
        public void JsonFileConfigLoader_LoadJsonText_ConfigFileIsUpdated_ReturnUpdatedConfig()
        {
            var fn = Path.GetFullPath(".\\JsonFileConfigLoaderTest.json");
            var loader = new JsonFileConfigLoader(fn);


            Action<Section> test = expected =>
            {

                using (var writer = new StreamWriter(fn))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(expected));
                }
                var actual = JsonConvert.DeserializeObject<Section>(loader.LoadJsonText());
                actual.ShouldBeEquivalentTo(expected);
            };

            test(new Section {Value = "Hello"});
            test(new Section {Value = "Hello World"});
        }
    }
}
