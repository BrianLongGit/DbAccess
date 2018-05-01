using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace DbAccess.Tools 
{
    public class DataMapper<T> where T: class//take a generic
    {
        private static readonly DataMapper<T>_instance = new DataMapper<T>();//singleton design pattern
        PropertyInfo[] properties;

        private DataMapper()
        {
            properties = typeof(T).GetProperties();
        }
        static DataMapper() { }

        public static DataMapper<T> Instance { get { return _instance; } }

        public T MapToObject(IDataReader reader)
        {
            IEnumerable<string> columns = reader.GetSchemaTable()
                .Rows.Cast<DataRow>().Select(
                c => c["ColumnName"].ToString().ToLower()).ToList();

            T obj = Activator.CreateInstance<T>();//heh?

            foreach (PropertyInfo pinfo in properties)
            {
                if (columns.Contains(pinfo.Name.ToLower()))
                {
                    if (reader[pinfo.Name] != DBNull.Value)
                    {
                        if (reader[pinfo.Name].GetType() == typeof(decimal))
                        {
                            pinfo.SetValue(obj, reader.GetDouble(pinfo.Name));
                        }
                        else
                        {
                            pinfo.SetValue(obj, (reader.GetValue(reader.GetOrdinal(pinfo.Name)) ?? null),
                                null);
                        }
                    }
                }
            }
            return obj;
        }
    }
    
    public static class DataHelper
    {
        public static double GetDouble(this IDataReader reader, string columnName)
        {
            double dbl = 0;
            double.TryParse(reader[columnName].ToString(), out dbl);
            return dbl;
        }

        public static double GetDouble(this IDataReader reader, int columnIndex)//overriding means giving to options for GetDouble
        {
            double dbl = 0;
            double.TryParse(reader[columnIndex].ToString(), out dbl);
            return dbl;
        }
    }
}
