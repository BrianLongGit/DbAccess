using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccess.Tools
{
    public sealed class SqlDbParameter
    {
        private static readonly SqlDbParameter _instance = new SqlDbParameter();
        private SqlDbParameter() { }
        static SqlDbParameter() { }
        public static SqlDbParameter Instance { get { return _instance; } }

        public SqlParameter BuildParameter<T>(string paramName,
            T paramValue, SqlDbType paramType, int paramSize = 0,
            ParameterDirection paramDirection = ParameterDirection.Input)
        {
            SqlParameter sqlp = new SqlParameter();
            sqlp.ParameterName = paramName;
            sqlp.SqlDbType = paramType;
            sqlp.Direction = paramDirection;
            if (paramSize > 0)
                sqlp.Size = paramSize;

            //TODO: Add Date Validation here.

            if (paramValue == null)
                sqlp.Value = DBNull.Value;
            else
                sqlp.Value = paramValue;

            return sqlp;
        }

    }
}
