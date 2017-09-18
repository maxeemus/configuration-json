using System;
using Maxeemus.Utilities.Configuration.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Maxeemus.Utilities.Configuration.Json.Tests
{
    [TestClass]
    public class JsonMergeFileConfigLoaderTest
    {
        [TestMethod]
        public void JsonMergeFileConfigLoader_LoadJsonText_SetOfJson_ReturnsCorrectMergedJson()
        {
            #region testdata
            var expectedContent = new
            {
                section1_c = new
                {
                    p1 = "p1",
                    p4 = "p4n",
                    p6 = "p6c",
                    p8 = new[] { new { p9 = "3" }, new { p9 = "3" } },
                    sub_1_1_c = new
                    {
                        p2 = "p2",
                        p5 = "p5n",
                        p7 = "p7c"
                    },
                    sub_1_2_n = new
                    {
                        p3 = "p3"
                    }
                },

                section2_n = new
                {
                    p6 = "p6",
                    sub_2_1 = new
                    {
                        p7 = "p7"
                    }
                }
            };
            var expectedJson = JObject.Parse(JsonConvert.SerializeObject(expectedContent));

            var file1Content = new
            {
                section1_c = new
                {
                    p1 = "p1",
                    p6 = "p6",
                    p8 = new[] { new { p9 = "1" }, new { p9 = "2" } },
                    sub_1_1_c = new
                    {
                        p2 = "p2",
                        p7 = "p7"
                    }
                }
            };

            var file2Content = new
            {
                section1_c = new
                {
                    p4 = "p4n",
                    p6 = "p6c",
                    p8 = new[] { new { p9 = "3" }, new { p9 = "3" } },
                    sub_1_1_c = new
                    {
                        p5 = "p5n",
                        p7 = "p7c"
                    },
                    sub_1_2_n = new
                    {
                        p3 = "p3"
                    }
                },

                section2_n = new
                {
                    p6 = "p6",
                    sub_2_1 = new
                    {
                        p7 = "p7"
                    }
                }
            };
            #endregion

            var loader1 = new JsonConfigurationBuilderTest.FakeLoader { Json = JsonConvert.SerializeObject(file1Content) };
            var loader2 = new JsonConfigurationBuilderTest.FakeLoader { Json = JsonConvert.SerializeObject(file2Content) };
            var loader3 = new JsonConfigurationBuilderTest.FakeLoader { Json = "{}" };

            var actualJsonText = new JsonMergeFileConfigLoader(loader1, loader2, loader3).LoadJsonText();
            var actualJson = JObject.Parse(actualJsonText);

            JToken.DeepEquals(actualJson, expectedJson).ShouldBeEquivalentTo(true);
        }
        
        [TestMethod]
        public void JsonMergeFileConfigLoader_LoadJsonText_SingleInnerLoader_ReturnsCorrectJson()
        {
            var expectedContent = new
            {
                section1_c = new
                {
                    p1 = "p1_updated",
                    p4 = "p4n"
                }                    
            };
            var expectedJson = JObject.Parse(JsonConvert.SerializeObject(expectedContent));

            var loader1 = new JsonConfigurationBuilderTest.FakeLoader { Json = JsonConvert.SerializeObject(expectedContent) };
            var loader = new JsonMergeFileConfigLoader(loader1);

            var actualJsonText = loader.LoadJsonText();
            var actualJson = JObject.Parse(actualJsonText);

            JToken.DeepEquals(actualJson, expectedJson).ShouldBeEquivalentTo(true);
        }

        [TestMethod]
        public void JsonMergeFileConfigLoader_LoadJsonText_NoInnerLoader_ReturnsNull()
        {            
            new JsonMergeFileConfigLoader().LoadJsonText().Should().BeNull();          
        }
    }
}
