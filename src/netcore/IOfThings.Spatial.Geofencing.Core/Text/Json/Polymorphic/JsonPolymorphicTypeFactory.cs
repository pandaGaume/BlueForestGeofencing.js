using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class JsonPolymorphicTypeFactory : IJsonPolymorphicTypeFactory 
    {
        public static IJsonPolymorphicTypeFactory Shared = new JsonPolymorphicTypeFactory();

        readonly bool _cs ;
        readonly SemaphoreSlim _lock;
        IDictionary<string, Type> _types;
        readonly IDictionary<string, string> _context;

        public JsonPolymorphicTypeFactory(bool caseSensitive = false, IDictionary<string, string> context = null)
        {
            _cs = caseSensitive;
            _context = context;
            _lock = new SemaphoreSlim(1);
        }
        public bool CaseSensitive => _cs;
        public virtual Type ForName(string typeName)
        {
            try
            {
                _lock.Wait();
                _types = _types ?? BuildTypeDirectory();
                var n = typeName;
                if ( _context != null && _context.TryGetValue(n, out string newName))
                {
                    n = newName;
                }
                if(_types.TryGetValue(_cs ? n : n.ToLower(), out var t))
                {
                    return t;
                }
                return default;
            }
            catch(Exception e)
            {
                return default;
            }
            finally
            {
                _lock.Release();
            }
        }

        protected virtual IDictionary<string, Type> BuildTypeDirectory()
        {
            var types = new ConcurrentDictionary<string, Type>();

            var assemblies = GetAssemblies().AsParallel();
            foreach(var a in assemblies)
            {
                foreach(var type in a.GetTypes().Where(t=>!t.IsInterface))
                {
                    var attributes = type.GetCustomAttributes(typeof(JsonPolymorphicTypeAttribute), true);
                    if(attributes.Length == 0)
                    {
                        // As remember, a class do NOT inherit interfaces, so we MUST also search the interface to 
                        // find the correct attributes
                        foreach(var i in type.GetInterfaces())
                        {
                            attributes = i.GetCustomAttributes(typeof(JsonPolymorphicTypeAttribute), true);
                            if( attributes.Length != 0)
                            {
                                break;
                            }
                        }
                    }
                    
                    if(attributes.Length != 0 && attributes[0] is JsonPolymorphicTypeAttribute att)
                    {
                        types[_cs? att.Name : att.Name.ToLower()] = type;
                    } 
                }
            }
            return types;
        }

        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            foreach(var p in referencedPaths)
            {
                //yield return Assembly.LoadFile(Path.GetFileName(p));
                yield return Assembly.LoadFrom(p);
            }
        }
    }
}
