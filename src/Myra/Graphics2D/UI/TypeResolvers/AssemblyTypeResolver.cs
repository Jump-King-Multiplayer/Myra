using System;
using System.Collections.Generic;
using System.Reflection;

namespace Myra.Graphics2D.UI.TypeResolvers
{
    public class AssemblyTypeResolver : ITypeResolver
    {
        private readonly List<Tuple<Assembly, string>> _searchAssemblies = new List<Tuple<Assembly, string>>();

        public AssemblyTypeResolver(IEnumerable<Tuple<Assembly, string>> searchAssemblies) : this()
        {
            _searchAssemblies.AddRange(searchAssemblies);
        }

        public AssemblyTypeResolver()
        {
        }

        public void AddAssembly(Assembly assembly, string namespacePrefix)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrEmpty(namespacePrefix)) throw new ArgumentException("namespacePrefix is null or empty", nameof(namespacePrefix));

            _searchAssemblies.Add(new Tuple<Assembly, string>(assembly, namespacePrefix));
        }

        Type ITypeResolver.ResolveType(string tagName)
        {
            foreach (Tuple<Assembly, string> searchAssembly in _searchAssemblies)
            {
                Assembly assembly = searchAssembly.Item1;
                string namespacePrefix = searchAssembly.Item2;

                Type type = assembly.GetType(namespacePrefix + "." + tagName, throwOnError: false, ignoreCase: true);

                if (type == null || !typeof(Widget).IsAssignableFrom(type) || type.IsAbstract)
                    continue;

                return type;
            }

            return null;
        }
    }
}