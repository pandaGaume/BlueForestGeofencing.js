using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class GeofencingExtensionTypeManager : IGeofencingExtensionTypeFactory
    {
        public static readonly GeofencingExtensionTypeManager Shared = new GeofencingExtensionTypeManager();

        readonly bool _cs;
        readonly SemaphoreSlim _lock;
        IDictionary<string, Type> _types;
        IDictionary<Guid, string> _names;
        readonly IDictionary<string, string> _context;

        public GeofencingExtensionTypeManager(bool caseSensitive = false, IDictionary<string, string> context = null)
        {
            _cs = caseSensitive;
            _context = context;
            _lock = new SemaphoreSlim(1);
        }

        public string ForType(Type type)
        {
            try
            {
                _lock.Wait();
                if (_types == null && TryBuildTypeDirectory(out var types, out var names))
                {
                    _types = types;
                    _names = names;
                }
                if (_names.TryGetValue(type.GUID, out var n))
                {
                    if (_context != null && _context.TryGetValue(n, out string newName))
                    {
                        n = newName;
                    }
                    return n;
                }
                return default;
            }
            catch
            {
                return default;
            }
            finally
            {
                _lock.Release();
            }
        }

        public Type ForName(string typeName)
        {
            try
            {
                _lock.Wait();
                if (_types == null && TryBuildTypeDirectory( out var types, out var names))
                {
                    _types = types;
                    _names = names;
                }

                var n = typeName;
                if (_context != null && _context.TryGetValue(n, out string newName))
                {
                    n = newName;
                }
                if (_types.TryGetValue(_cs ? n : n.ToLower(), out var t))
                {
                    return t;
                }
                return default;
            }
            catch
            {
                return default;
            }
            finally
            {
                _lock.Release();
            }
        }

        protected virtual bool TryBuildTypeDirectory(out IDictionary<string, Type> types, out IDictionary<Guid, string> names)
        {
            types = new ConcurrentDictionary<string, Type>();
            names = new ConcurrentDictionary<Guid, string>();

            var assemblies = GetAssemblies().AsParallel();
            foreach (var a in assemblies)
            {
                try
                {
                    foreach (var type in a.GetTypes().Where(t => !t.IsInterface && typeof(IGeofencingExtension).IsAssignableFrom(t)))
                    {
                        var attributes = type.GetCustomAttributes(typeof(GeofencingExtensionTypeAttribute), true);
                        if (attributes.Length == 0)
                        {
                            // As remember, a class do NOT inherit interfaces, so we MUST also search the interface to 
                            // find the correct attributes
                            foreach (var i in type.GetInterfaces())
                            {
                                attributes = i.GetCustomAttributes(typeof(GeofencingExtensionTypeAttribute), true);
                                if (attributes.Length != 0)
                                {
                                    break;
                                }
                            }
                        }
                        var name = type.Name;
                        if (attributes.Length != 0 && attributes[0] is GeofencingExtensionTypeAttribute att)
                        {
                            name = att.Name;
                        }
                        name = _cs ? name : name.ToLower();

                        types[name] = type;
                        names[type.GUID] = name;
                    }
                }
                catch
                {
                    // do no let Assembly load stop the whole process.
                    continue;
                }
            }
            return true;
        }

        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            foreach (var p in referencedPaths)
            {
                yield return Assembly.LoadFrom(p);
            }
        }
    }
}
