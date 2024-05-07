using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MovieWatch.Services.Extensions
{
    public class BaseFirstContractResolver : DefaultContractResolver
    {
        // As of 7.0.1, Json.NET suggests using a static instance for "stateless" contract resolvers, for performance reasons.
        // http://www.newtonsoft.com/json/help/html/ContractResolver.htm
        // http://www.newtonsoft.com/json/help/html/M_Newtonsoft_Json_Serialization_DefaultContractResolver__ctor_1.htm
        // "Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance."
        static BaseFirstContractResolver()
        {
            Instance = new BaseFirstContractResolver();
        }

        public static BaseFirstContractResolver Instance { get; }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            NamingStrategy = new CamelCaseNamingStrategy();

            var properties = base.CreateProperties(type, memberSerialization);

            return properties.ToList().OrderBy(p => p.DeclaringType.BaseTypesAndSelf().Count()).ToList();
        }
    }

    public static class TypeExtensions
    {
        public static IEnumerable<Type?> BaseTypesAndSelf(this Type? type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}