using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ProductoFwkTest.Shared
{
    public static class SqlParamFactory
    {
        public static int SizeOf(this PropertyInfo pi,object o)
        {
            if(typeof(string) == pi.PropertyType)
            {
                return sizeof(char) * (pi.GetValue(o)).ToString().Length;
            }
            else  if (typeof(int) == pi.PropertyType)
            {
                return sizeof(int);
            }
            else if (typeof(long) == pi.PropertyType)
            {
                return sizeof(long);
            }
            else if (typeof(byte) == pi.PropertyType)
            {
                return sizeof(byte);
            }
            else if (typeof(bool) == pi.PropertyType)
            {
                return sizeof(bool);
            }
            else if (typeof(double) == pi.PropertyType)
            {
                return sizeof(double);
            }
            else if (typeof(float) == pi.PropertyType)
            {
                return sizeof(float);
            }
            return -1;
        }
        public static System.Data.SqlDbType SqlDbType(this PropertyInfo pi)
        {
            if (typeof(string) == pi.PropertyType)
            {
                return System.Data.SqlDbType.NVarChar;
            }
            else if (typeof(int) == pi.PropertyType)
            {
                return System.Data.SqlDbType.Int; 
            }
            else if (typeof(long) == pi.PropertyType)
            {
                return System.Data.SqlDbType.BigInt;
            }
            else if (typeof(byte) == pi.PropertyType)
            {
                return System.Data.SqlDbType.TinyInt;
            }
            else if (typeof(bool) == pi.PropertyType)
            {
                return System.Data.SqlDbType.Bit;
            }
            else if (typeof(double) == pi.PropertyType)
            {
                return System.Data.SqlDbType.Float;
            }
            else if (typeof(float) == pi.PropertyType)
            {
                return System.Data.SqlDbType.Float; ;
            }
            return default;
        }
        public static System.Data.SqlClient.SqlParameter NewParam<TVal>(object value,string paramName,int size, System.Data.SqlDbType ty,
            System.Data.ParameterDirection dir = System.Data.ParameterDirection.Input)
        {
            System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter();
            param.ParameterName = paramName;
            var t = value.GetType();
            param.Value= Convert.ChangeType(value, value.GetType());
            param.Size = size;
            param.SqlDbType = ty;
            param.Direction = dir;
            return param;
        }

        public static T GetDataToEntity<T>(this System.Data.SqlClient.SqlDataReader reader) where T: new()
        {
            T obj = new T();
            foreach(var prop in obj.GetType().GetProperties())
            {
                prop.SetValue(obj, reader[prop.Name]);
            }
            return obj;
        }

        public static T GetDataToType<T>(this System.Data.SqlClient.SqlDataReader reader,string name) where T : new()
        {
            T obj = default;
            obj =(T)Convert.ChangeType(reader[name], typeof(T));
            return obj;
        }
    }
}
