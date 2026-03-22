using AdoHelperLib.Mapping;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AdoHelperLib.Ado
{
    public class AdoHelperContext
    {
        private readonly string _connectionString;
        public AdoHelperContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> QueryAsync<T>(string sp, params SqlParameter[] parameters)
              where T : new()
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = CreateCommand(conn, sp, parameters);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            return await DataReaderMapper.MapToListAsync<T>(reader);
        }

        public async Task<int> ExecuteAsync(string sp, params SqlParameter[] parameters)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = CreateCommand(conn, sp, parameters);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<T> ExecuteScalarAsync<T>(string sp, params SqlParameter[] parameters)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = CreateCommand(conn, sp, parameters);

            await conn.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public async Task<DataSet> QueryDataSetAsync(string sp, params SqlParameter[] parameters)
        {
            var ds = new DataSet();

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = CreateCommand(conn, sp, parameters);

            await conn.OpenAsync();

            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds);

            return ds;
        }

        public async Task<MultipleResult> QueryMultipleAsync(string sp, params SqlParameter[] parameters)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = CreateCommand(conn, sp, parameters);

            await conn.OpenAsync();

            var reader = await cmd.ExecuteReaderAsync();

            return new MultipleResult(conn, reader);
        }
        private SqlCommand CreateCommand(SqlConnection conn, string sp, SqlParameter[] parameters)
        {
            var cmd = new SqlCommand(sp, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd;
        }
    }
}
