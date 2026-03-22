using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdoHelperLib.Mapping
{
    public static class DataReaderMapper
    {
        public static async Task<List<T>> MapToListAsync<T>(SqlDataReader reader) where T : new()
        {
            var res = new List<T>();
            var props = typeof(T).GetProperties();

            while(await reader.ReadAsync())
            {
                var obj = new T();
                foreach(var prop in props)
                {
                    if (!reader.HasColumn(prop.Name))
                    {
                        continue; 
                    }
                    var value = reader[prop.Name];

                    if(value == DBNull.Value)
                    {
                        continue;
                    }
                    prop.SetValue(obj, value);
                }
                res.Add(obj);
            }
            return res;
        }
    }
}
