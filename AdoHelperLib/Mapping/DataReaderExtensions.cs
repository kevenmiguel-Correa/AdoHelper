using System;
using System.Collections.Generic;
using System.Text;

namespace AdoHelperLib.Mapping
{

    public static class DataReaderExtensions
    {
        public static bool HasColumn(this System.Data.IDataRecord reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
