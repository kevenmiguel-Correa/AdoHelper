using AdoHelperLib.Mapping;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdoHelperLib.Ado
{
    public class MultipleResult : IAsyncDisposable
    {
        private readonly SqlConnection _sqlConnection;
        private readonly SqlDataReader _reader;
        public MultipleResult(SqlConnection sqlConnection, SqlDataReader reader)
        {
            _sqlConnection = sqlConnection;
            _reader = reader;
        }

        public async Task<List<T>> ReadResultSetAsync<T>() where T : new()
        {
            var result = await DataReaderMapper.MapToListAsync<T>(_reader);

            //next result set
            await _reader.NextResultAsync();
            return result;
        }
        public async ValueTask DisposeAsync()
        {
            await _reader.DisposeAsync();
            await _sqlConnection.DisposeAsync();
        }
    }
}
