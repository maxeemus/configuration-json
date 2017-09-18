# configuration-json
Small .Net Framework library provides a way of reading configuration from json. Library supports multiple files configuration with merging and overriding values.
It is similar as .Net Core ConfigurationBuilder but for .Net Frameforks and only support json.

## Example
### Json configuration files
Main configuration: appsettings.json
```json
{
    "sampleConfig":{
      "nestedObj":{
        "strValue": "str2 value"
      },
      "str": "str value",
      "int": 1213234      
    },
    "anotherConfig":{
      "name": "maxeemus"
    }
}
```
Particular environment configuration: appsettings.prod.json.
Values from appsettings.json will be merged\overridden with a values from this file.
```json
{
    "sampleConfig":{
      "nestedObj":{
        "strValue": "production value"
      }
}
```
### How to use
```c#
// Class of "sampleConfig" section
public class ConfigA
{
    public class Nested
    {
        public string StrValue { get; set; }
    }

    public string Str { get; set; }
    public int Int { get; set; }
    public DateTime? DateTime { get; set; }
    public Nested NestedObj { get; set; }
}

// Class of "anotherConfig" section
public class ConfigB
{
    public string Name { get; set; }
}

// Build configuration
var envName = "prod";
var configuration = new JsonConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json") // main configuration
    .AddJsonFile($"appsettings.{envName}.json") // particular environment configuration
    .SectionMap<ConfigA>(config => config.sampleConfig)  // Use "sampleConfig" bacause it defined in *.json
    .SectionMap<ConfigB>(config => config.anotherConfig) // Use anotherConfig bacause it defined in *.json
    .Build(); 
    
// get configuration sections
var sectionA = configuration.Section<ConfigA>();    
var sectionB = configuration.Section<ConfigB>();    

// get configuration instances
ConfigA configA = section1.Value;
ConfigB configB = section2.Value;
```