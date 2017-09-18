using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxeemus.Utilities.Configuration.Json
{
    public interface IJsonConfigurationBuilder
    {
        IJsonConfigurationBuilder AddJsonLoader(IJsonConfigurationLoader loader);
        IJsonConfigurationBuilder SetBasePath(string dirPath);
        IJsonConfigurationBuilder AddJsonFile(string filePath);
        IJsonConfigurationBuilder SectionMap<T>(Func<dynamic, object> func) where T : class;        
        IJsonConfiguration Build();
    }

    public interface IJsonConfiguration
    {
        IJsonConfigurationSection<T> Section<T>() where T : class;
    }

    public interface IJsonConfigurationSection<out T> where T : class
    {        
        T Value { get; }
    }

    public interface IJsonConfigurationLoader
    {
        string LoadJsonText();        
    }
}
