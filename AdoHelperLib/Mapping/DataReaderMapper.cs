using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AdoHelperLib.Mapping
{
    //public static class DataReaderMapper
    //{
    //    public static async Task<List<T>> MapToListAsync<T>(SqlDataReader reader) where T : new()
    //    {
    //        var res = new List<T>();
    //        var props = typeof(T).GetProperties();

    //        while(await reader.ReadAsync())
    //        {
    //            var obj = new T();
    //            foreach(var prop in props)
    //            {
    //                if (!reader.HasColumn(prop.Name))
    //                {
    //                    continue; 
    //                }
    //                var value = reader[prop.Name];

    //                if(value == DBNull.Value)
    //                {
    //                    continue;
    //                }
    //                prop.SetValue(obj, value);
    //            }
    //            res.Add(obj);
    //        }
    //        return res;
    //    }
    //}

    /// <summary>
    /// DataReaderMapper provides functionality to map data from a SqlDataReader to a list of strongly-typed objects. It uses reflection to match column names from the reader to property names on the target type, and includes caching of property information for improved performance on repeated mappings of the same type.
    /// </summary>
    public static class DataReaderMapper
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _cache = new();

        /// <summary>
        /// Maps the data from a SqlDataReader to a list of objects of type T. It reads each row from the reader, creates an instance of T, and sets its properties based on the column names and values from the reader. The method handles null values and type conversions as needed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static async Task<List<T>> MapToListAsync<T>(SqlDataReader reader) where T : new()
        {
            var result = new List<T>();

            var props = GetProperties(typeof(T));

            while (await reader.ReadAsync())
            {
                var obj = new T();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);

                    if (!props.TryGetValue(columnName.ToLower(), out var prop))
                        continue;

                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                    if (value == null)
                    {
                        prop.SetValue(obj, null);
                        continue;
                    }

                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    var safeValue = Convert.ChangeType(value, targetType);

                    prop.SetValue(obj, safeValue);
                }

                result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// Obtains a dictionary of property names (lowercased) to PropertyInfo for the given type, using caching for performance.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            if (_cache.TryGetValue(type, out var cached))
                return cached;

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .ToDictionary(p => p.Name.ToLower(), p => p);

            _cache[type] = props;

            return props;
        }
    }
}
