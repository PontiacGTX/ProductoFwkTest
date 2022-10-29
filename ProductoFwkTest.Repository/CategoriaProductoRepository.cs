using Microsoft.Extensions.Logging;
using ProductoFwkTest.App_Start;
using ProductoFwkTest.Entities;
using ProductoFwkTest.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProductoFwkTest.Repository
{
    public class CategoriaProductoRepository :IRepository<ProductoCat>
    {
        private ILogger<CategoriaProductoRepository> _logger;
        string _conString { get; }

        public CategoriaProductoRepository(StringCon strCon, ILogger<CategoriaProductoRepository> logger = null)
        {
            _logger = logger;
            _conString = strCon.String;

        }
        public async Task<ProductoCat> FirstOrDefault<Tid>(Tid id)
        {

            string cmd = $"Select TOP 1 * From ProductoCat Where ProductoCatId = @prodId";
            ProductoCat prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(Shared.SqlParamFactory.NewParam<int>(id, "@prodId", sizeof(int), System.Data.SqlDbType.Int));
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    if (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<ProductoCat>();
                    }
                }
            }

            return prod;
        }

        public async Task<ProductoCat> FirstOrDefault(Func<ProductoCat, bool> selector)
        {
            string cmd = $"Select * From ProductoCat";
            ProductoCat prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<ProductoCat>();

                        if (selector(prod))
                            break;
                    }
                }
            }

            return prod;
        }

        public async Task<IList<ProductoCat>> GetAll()
        {
            string cmd = $"Select * From ProductoCat";
            List<ProductoCat> prods = new List<ProductoCat>();
            Producto prod = null;
            try
            {
                using (var con = new SqlConnection(_conString))
                using (var command = new SqlCommand(cmd, con))
                {
                    await con.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            prods.Add(reader.GetDataToEntity<ProductoCat>());

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return prods;
        }

        public async Task<IList<ProductoCat>> GetAll(Func<ProductoCat, bool> selector)
        {
            string cmd = $"Select * From ProductoCat";
            List<ProductoCat> prods = new List<ProductoCat>();
            ProductoCat prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<ProductoCat>();

                        if (selector(prod))
                            prods.Add(prod);

                    }
                }
            }
            return prods;
        }

        public async Task<bool> Remove(ProductoCat s)
        {
            string cmd = $"Delete From ProductoCat Where ProductoCatId = @prodId";
            ProductoCat prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(Shared.SqlParamFactory.NewParam<int>(s.ProductoCatId, "@prodId", sizeof(int), System.Data.SqlDbType.Int));
                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            return false;
        }

        public async Task<bool> Remove<Tid>(Tid id)
        {
            string cmd = $"Delete From ProductoCat Where ProductoCatId = @prodId";
            Producto prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(Shared.SqlParamFactory.NewParam<int>(id, "@prodId", sizeof(int), System.Data.SqlDbType.Int));
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    if (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<Producto>();
                    }
                }
            }

            return !await Any(id);
        }

        public async Task<bool> RemoveRange(Func<ProductoCat, bool> selector)
        {
            List<int> ids = new List<int>();
            foreach (var item in await GetAll(selector))
            {
                try
                {
                    ids.Add(item.ProductoCatId);
                    await Remove(item.ProductoCatId);
                }
                catch (Exception ex)
                {

                }
            }
            return (await Task.WhenAll(ids.Select(async x => await Any(x)))).ToList().Any(y => y);
        }
        private string BuildUpdateQuery<T>(T s, out List<PropertyInfo> fields, out PropertyInfo pKey)
        {
            var res = s.GetType().GetCustomAttributes(typeof(TableAttribute), false).Select(x => (TableAttribute)x).FirstOrDefault();
            pKey = s.GetType().GetProperties()
              .FirstOrDefault(x => x.GetCustomAttributes(false).Any(y => y.GetType() == typeof(KeyAttribute)));
            var temp = pKey;
            fields = s.GetType().GetProperties()
                .Where(x => x != temp && !x.GetCustomAttributes(false).Any(y => y.GetType() == typeof(NotMappedAttribute))).Select(x => x).ToList();
            var Key = $"{pKey.Name}_Id";
            var list = string.Concat(fields.Select(x => $"{x.Name} = @{x.Name},"));
            list = list.Substring(0, list.Length - 1);
            string query = $"Update {res.Name} Set {list} " +
                $"where {pKey.Name} = @{Key}";
            return query;
        }
        public async Task<ProductoCat> Update<Tid>(Tid id, ProductoCat s)
        {
            List<PropertyInfo> listParams = new List<PropertyInfo>();
            string cmd = "";
            try
            {

                PropertyInfo pKey = null;
                cmd = BuildUpdateQuery(s, out listParams, out pKey);
                using (var con = new SqlConnection(_conString))
                using (var command = new SqlCommand(cmd, con))
                {
                    await con.OpenAsync();
                    command.Parameters.Add(SqlParamFactory.NewParam<int>(id, $"@{pKey.Name}_Id", sizeof(int), pKey.SqlDbType()));
                    foreach (var x in listParams)
                    {
                        command.Parameters.Add(SqlParamFactory.NewParam<object>(x.GetValue(s), $"@{x.Name}", x.SizeOf(s), x.SqlDbType()));
                    }
                    var reader = await command.ExecuteNonQueryAsync();

                }
                return await FirstOrDefault(id);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<int> Count()
        {
            string cmd = $"Select Count(*) as CT From ProductoCat";
            int count = 0;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(SqlParamFactory.NewParam<int>(count, "CT", sizeof(int), System.Data.SqlDbType.Int, System.Data.ParameterDirection.Output));
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    if (await reader.ReadAsync())
                    {
                        count = reader.GetDataToType<int>("CT");
                    }
                }
            }
            return count;
        }
        public async Task<int> Count(Func<ProductoCat, bool> selector)
        {
            string cmd = $"Select * From ProductoCat";
            ProductoCat prod = null;
            int count = 0;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    if (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<ProductoCat>();
                    }
                }
            }
            return count;
        }
        public async Task<bool> Any<Tid>(Tid id)
        {
            string cmd = $"Select Count(*) as CT From ProductoCat Where ProductoCatId = @prodId";
            
            int count = 0;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(SqlParamFactory.NewParam<int>(id, "@prodId", Marshal.SizeOf(typeof(Tid)), System.Data.SqlDbType.Int));
                command.Parameters.Add(SqlParamFactory.NewParam<int>(count, "CT", sizeof(int), System.Data.SqlDbType.Int, System.Data.ParameterDirection.Output));
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    if (await reader.ReadAsync())
                    {
                        count = reader.GetDataToType<int>("CT");
                    }
                }
            }
            return count > 0;
        }

        public async Task<bool> Any<Tid>(Func<ProductoCat, bool> selector)
        {
            string cmd = $"Select * From ProductoCat";
            ProductoCat prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<ProductoCat>();

                        if (selector(prod))
                            return true;

                    }
                }
            }
            return false;
        }
    }
}
