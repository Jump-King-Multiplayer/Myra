using System;
using System.Collections.Generic;

namespace Myra.Graphics2D.UI.TypeResolvers
{
    public class ManualTypeResolver : ITypeResolver
    {
        private List<Type> _types = new List<Type>();

        public ManualTypeResolver(params Type[] types) : this()
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            AddTypes(types);
        }

        public ManualTypeResolver()
        {
        }

        public void AddTypes(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            
            foreach (var type in types)
            {
                AddType(type);
            }
        }

        public void AddType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!typeof(Widget).IsAssignableFrom(type))
                throw new ArgumentException("Type is not inherited from Widget", nameof(type));

            if (type.IsAbstract)
                throw new ArgumentException("Type is abstract");
            
            _types.Add(type);
        }

        Type ITypeResolver.ResolveType(string tagName)
        {
            foreach (Type type in _types)
            {
                if (type.Name == tagName)
                    return type;
            }

            return null;
        }
    }
}