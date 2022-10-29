using Microsoft.Extensions.Logging;
using ProductoFwkTest.App_Start;
using ProductoFwkTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ProductoFwkTest.Shared;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ProductoFwkTest.Repository
{
    public class ProductoRepository : IRepository<Producto>
    {
        private ILogger<ProductoRepository> _logger;
        string _conString { get; }

        public ProductoRepository(StringCon strCon, ILogger<ProductoRepository> logger=null)
        {
            _logger = logger;
            _conString = strCon.String;

        }
        public async Task<Producto> FirstOrDefault<Tid>(Tid id)
        {

            string cmd = $"Select TOP 1 * From Producto Where ProductoId = @prodId";
            Producto prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(Shared.SqlParamFactory.NewParam<int>(id, "@prodId",sizeof(int), System.Data.SqlDbType.Int));
                var reader =await command.ExecuteReaderAsync();
                if(reader.HasRows)
                {
                    if (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<Producto>();
                    }
                }
            }

            return prod;
        }

        public async Task<Producto> FirstOrDefault(Func<Producto, bool> selector)
        {
            string cmd = $"Select * From Producto";
            Producto prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<Producto>();

                        if (selector(prod))
                        break;
                    }
                }
            }

            return prod;
        }

        public async Task<IList<Producto>> GetAll()
        {
            string cmd = $"Select * From Producto";
            List<Producto> prods = new List<Producto>();
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
                            prods.Add( reader.GetDataToEntity<Producto>());

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

        public async Task<IList<Producto>> GetAll(Func<Producto, bool> selector)
        {
            string cmd = $"Select * From Producto";
            List<Producto> prods = new List<Producto>();
            Producto prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        prod = reader.GetDataToEntity<Producto>();

                        if (selector(prod))
                            prods.Add(prod);

                    }
                }
            }
            return prods;
        }

        public async Task<bool> Remove(Producto s)
        {
            string cmd = $"Delete From Producto Where ProductoId = @prodId";
            Producto prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(Shared.SqlParamFactory.NewParam<int>(s.ProductoId, "@prodId", sizeof(int), System.Data.SqlDbType.Int));
                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            return false;
        }

        public async Task<bool> Remove<Tid>(Tid id)
        {
            string cmd = $"Delete From Producto Where ProductoId = @prodId";
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

        public async Task<bool> RemoveRange(Func<Producto, bool> selector)
        {
            List<int> ids = new List<int>();
            foreach (var item in await GetAll(selector))
            {
                ids.Add(item.ProductoId);
                await Remove(item.ProductoId);
            }
            return (await Task.WhenAll(ids.Select(async x =>  await Any(x)))).ToList().Any(y => y);
        }
        private string BuildUpdateQuery<T>(T s,out List<PropertyInfo> fields,out PropertyInfo pKey)
        {
            var res = s.GetType().GetCustomAttributes(typeof(TableAttribute),false).Select(x=>(TableAttribute)x).FirstOrDefault();
             pKey = s.GetType().GetProperties()
               .FirstOrDefault(x => x.GetCustomAttributes(false).Any(y => y.GetType() == typeof(KeyAttribute)));
            var temp = pKey;
            fields = s.GetType().GetProperties()
                .Where(x =>  x != temp &&  !x.GetCustomAttributes(false).Any(y => y.GetType() == typeof(NotMappedAttribute))).Select(x => x).ToList();
            var Key = $"{pKey.Name}_Id";
            var list = string.Concat(fields.Select(x => $"{x.Name} = @{x.Name},"));
            list = list.Substring(0, list.Length - 1);
            string query = $"Update {res.Name} Set {list} " +
                $"where {pKey.Name} = @{Key}";
            return query;
        }
        public async Task<Producto> Update<Tid>(Tid id, Producto s)
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
            string cmd = $"Select Count(*) as CT From Producto";
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
        public async Task<int> Count(Func<Producto, bool> selector)
        {
            string cmd = $"Select * From Producto";
            Producto prod = null;
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
                        prod = reader.GetDataToEntity<Producto>();
                    }
                }
            }
            return count;
        }
        public async Task<bool> Any<Tid>(Tid id)
        {
            string cmd = $"Select Count(*) as CT From Producto Where ProductoId = @prodId";
            List<Producto> prods = new List<Producto>();
            int count = 0;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                command.Parameters.Add(SqlParamFactory.NewParam<int>(id, "@prodId", Marshal.SizeOf(typeof(Tid)),System.Data.SqlDbType.Int));
                command.Parameters.Add(SqlParamFactory.NewParam<int>(count, "CT", sizeof(int), System.Data.SqlDbType.Int, System.Data.ParameterDirection.Output));
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    if (await reader.ReadAsync())
                    {
                        count= reader.GetDataToType<int>("CT");
                    }
                }
            }
            return count >0;
        }

        public async Task<bool> Any<Tid>(Func<Producto, bool> selector)
        {
            string cmd = $"Select * From Producto";
            Producto prod = null;
            using (var con = new SqlConnection(_conString))
            using (var command = new SqlCommand(cmd, con))
            {
                await con.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        prod= reader.GetDataToEntity<Producto>();

                        if (selector(prod))
                            return true;

                    }
                }
            }
            return false;
        }
    }
}
