using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plato.Internal.Cache
{

    public class CacheTokenStore
    {
        public static IDictionary<CacheKey, Type> Tokens { get; } = new Dictionary<CacheKey, Type>();

        public static IEnumerable<CacheKey> TryGet(Type type)
        {
            return Tokens
                .Where(t => t.Value == type)
                .Select(c => c.Key);
        }

        public static CacheKey GetOrAddToken(Type type, params object[] varyBy)
        {
            var key = new CacheKey(type, varyBy);
            if (Tokens.ContainsKey(key))
            {
                return key;
            }
            Tokens.Add(key, type);
            return key;
        }

        public static void Flush(Type type)
        {
            Tokens.Clear();
            foreach (var entry in Tokens)
            {
                if (entry.Value != type)
                {
                    Tokens.Add(entry.Key, entry.Value);
                }
            }

        }

    }

    public class CacheKey : IEquatable<CacheKey>
    {
        
        private readonly Type _type;
        private readonly object _varyBy;
        private readonly int _typeHashCode;
        private readonly string _varyByHashCode;

        public CacheKey(Type type, params object[] varyBy)
        {
            var sb = new StringBuilder();
            foreach (var vary in varyBy)
            {
                if (vary != null)
                {
                    sb.Append(vary.GetHashCode());
                }
            }
            
            _varyBy = varyBy;
            _type = type;
            _varyByHashCode = sb.ToString();
            _typeHashCode = _type.GetHashCode();
        }

        public static bool operator ==(CacheKey a, CacheKey b)
        {
            var areEqual = ReferenceEquals(a, b);
            if ((object)a != null && (object)b != null)
            {
                areEqual = a.Equals(b);
            }

            return areEqual;
        }

        public static bool operator !=(CacheKey a, CacheKey b) => !(a == b);

        public bool Equals(CacheKey other)
        {
            var areEqual = false;

            if (other != null)
            {
                areEqual = this.ToString() == other.ToString();
            }

            return areEqual;
        }

        public override bool Equals(object obj) => this.Equals(obj as CacheKey);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_type != null ? _type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _typeHashCode;
                hashCode = (hashCode * 397) ^ (_varyByHashCode != null ? _varyByHashCode.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{_type}_{GetHashCode()}";
        }
    }

}
