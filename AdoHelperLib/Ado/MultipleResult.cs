using AdoHelperLib.Mapping;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdoHelperLib.Ado
{
    public class MultipleResult : IAsyncDisposable
    {
        private readonly SqlCommand _command;
        private readonly SqlDataReader _reader;

        public MultipleResult(SqlCommand command, SqlDataReader reader)
        {
            _command = command;
            _reader = reader;
        }

        public async Task<List<T>> ReadAsync<T>() where T : new()
        {
            var result = await DataReaderMapper.MapToListAsync<T>(_reader);

            if (!_reader.IsClosed)
                await _reader.NextResultAsync();

            return result;
        }

        public async ValueTask DisposeAsync()
        {
            await _reader.DisposeAsync();   // close command and connection as well, since they are tied together
            await _command.DisposeAsync();
        }
    }
}
