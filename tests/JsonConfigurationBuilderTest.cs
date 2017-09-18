using System;
using System.Dynamic;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using Maxeemus.Utilities.Configuration.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Omu.ValueInjecter;

namespace Maxeemus.Utilities.Configuration.Json.Tests
{
    [TestClass]
    public class JsonConfigurationBuilderTest
    {
        public class FakeLoader : IJsonConfigurationLoader
        {
            public string Json { get; set; }

            public string LoadJsonText()
            {
                return Json;
            }
        }

        public class Section1
        {
            public class Nested
            {
                public string Str2 { get; set; }
            }

            public string Str { get; set; }
            public int Int { get; set; }
            public DateTime? DateTime { get; set; }
            public Nested NestedObj { get; set; }
        }

        public class Section2
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public void JsonConfigurationBuilder_FullTest_CorrectInitializing_CorrectResult()
        {
            var section1Expected = new Section1
            {
                Str = "str1",
                Int = 123,
                DateTime = DateTime.Now,
                NestedObj = new Section1.Nested
                {
                    Str2 = "str2"
                }
            };
            var section2Expected = new Section2
            {
                Name = "djkahfjkxcz,"
            };
            
            var loader = new FakeLoader
            {
                Json = JsonConvert.SerializeObject(new
                {
                    section1 = section1Expected,
                    section2 = section2Expected
                })
            };
            IJsonConfiguration configuration = new JsonConfigurationBuilder()
                .AddJsonLoader(loader)
                .SectionMap<Section1>(config => config.section1)
                .SectionMap<Section2>(config => config.section2)
                .Build();

            for (var i = 0; i < 2; i++)
            {
                var section1Actual = configuration.Section<Section1>();
                section1Actual.Value.ShouldBeEquivalentTo(section1Expected);
                var section2Actual = configuration.Section<Section2>();
                section2Actual.Value.ShouldBeEquivalentTo(section2Expected);

                section1Actual = configuration.Section<Section1>();
                section1Actual.Value.ShouldBeEquivalentTo(section1Expected);
                section2Actual = configuration.Section<Section2>();
                section2Actual.Value.ShouldBeEquivalentTo(section2Expected);

                // test reload
                section1Actual.Value.Int = section1Expected.Int *= 100;
                section2Actual.Value.Name = section2Expected.Name += "ksadksdaksda";
                loader.Json = JsonConvert.SerializeObject(new
                {
                    section1 = section1Expected,
                    section2 = section2Expected
                });
            }
        }

        [TestMethod]
        public void JsonConfigurationBuilder_Test_RequestMissingSection_ReturnNullValueSection()
        {
            var loader = new FakeLoader {Json = "{\"aaa\": \"123\"}"};
            IJsonConfiguration configuration = new JsonConfigurationBuilder()
                .AddJsonLoader(loader)
                .SectionMap<Section1>(config => config.section1)                
                .Build();
            var section1Actual = configuration.Section<Section1>();
            section1Actual.Should().NotBeNull();
            section1Actual.Value.Should().BeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(JsonSerializationException))]
        public void JsonConfigurationBuilder_Test_SectionIsOtherType_ThrowJsonSerializationException()
        {
            var loader = new FakeLoader { Json = "{\"section1\": \"123\"}" };
            IJsonConfiguration configuration = new JsonConfigurationBuilder()
                .AddJsonLoader(loader)
                .SectionMap<Section1>(config => config.section1)
                .Build();
            var section1Actual = configuration.Section<Section1>();
            section1Actual.Should().NotBeNull();
            section1Actual.Value.Should().BeNull();
        }

        [TestMethod]
        public void JsonConfigurationBuilder_Test_ReadRealFiles_ReturnCorrectSection()
        {
            var fn = "fn.json";
            var fnProd = "fn.prod.json";

            #region prepare files
            var section1 = new Section1
            {
                Str = "str1",
                Int = 123,
                DateTime = DateTime.Now,
                NestedObj = new Section1.Nested
                {
                    Str2 = "str2"
                }
            };
            var section2 = new Section2
            {
                Name = "Name"
            };

            using (var writer = new StreamWriter(fn))
            {
                writer.WriteLine(JsonConvert.SerializeObject(new { section1 }));
            }

            section1.NestedObj.Str2 += "_prod";
            using (var writer = new StreamWriter(fnProd))
            {
                writer.WriteLine(JsonConvert.SerializeObject(new
                {
                    section1 = new
                    {
                        NestedObj = new
                        {
                            Str2 = section1.NestedObj.Str2
                        }
                    }
                    ,
                    section2
                }));
            } 
            #endregion


            var configuration = new JsonConfigurationBuilder()
                .SetBasePath(".")
                .AddJsonFile(fn)
                .AddJsonFile(fnProd)
                .AddJsonFile("nope.json")
                .SectionMap<Section1>(config => config.section1)
                .SectionMap<Section2>(config => config.section2)
                .Build();
            var section1Actual = configuration.Section<Section1>();
            section1Actual.Value.ShouldBeEquivalentTo(section1);

            var section1Actua2 = configuration.Section<Section2>();
            section1Actua2.Value.ShouldBeEquivalentTo(section2);

        }
    }
}
