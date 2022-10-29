
//Copyright(c) 2022 - Present, Cristofher Parada All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

//*Redistributions of source code must retain the above copyright notice, this list of conditions
//  and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, this list of conditions
//  and the following disclaimer in the documentation and/or other materials provided with the distribution.
//* Neither the name of Cristofher Parada,
// nor the names of its contributors may be used to endorse or promote products
//  derived from this software without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
