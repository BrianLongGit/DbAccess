using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DbAccess.Tools;

namespace DbAccess.DbAdapter
{
    public class DbAdapter //: IDbAdapter
    {
        public IDbConnection Conn { get; private set; }//now more flexible; doens't have to be a sql command
        public IDbCommand Cmd { get; private set; }

        public DbAdapter(IDbCommand command, IDbConnection conn)
        {
            Cmd = command;
            Conn = conn;
        }

        public List<T> LoadObject<T>(string storedProcedure, 
            IDataParameter[] parameters = null) where T: class
        {
            List<T> list = new List<T>();

            using (IDbConnection conn = Conn)//using statement does a try/catch inside; cleanup; when it's done it will close connection and command
            using (IDbCommand cmd = Cmd)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;//every ado.net needes connection, commandTimeout, commandType(string or storedprocedure/sql),commandText, then execute reader
                cmd.CommandTimeout = 5000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedure;
                if(parameters != null)
                {
                    foreach(IDbDataParameter parameter in parameters)
                        cmd.Parameters.Add(parameter);
                }
                IDataReader reader = cmd.ExecuteReader();//reader is where actual talking to the database happens
                while (reader.Read());//Data reader is really fast at reading data; faster than data mapper
                {
                    list.Add(DataMapper<T>.Instance.MapToObject(reader));
                }
            }
                return list;//TODO: Map to Object; we don't leave this function until list is returned
        }

        public T ExecuteDbScalar<T>(string storedProcedure, 
            IDbDataParameter[] parameters = null) where T: class//T generic type
        {
            using (IDbConnection conn = Conn)
                using (IDbCommand cmd = Cmd)
            {
                if (Conn.State != ConnectionState.Open)//first step is always to open connection
                    Conn.Open();

                cmd.Connection = conn;
                Cmd.CommandTimeout = 5000;
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = storedProcedure;
                foreach (IDbDataParameter parameter in parameters)
                    cmd.Parameters.Add(parameter);

                object obj = cmd.ExecuteScalar();
                return (T)Convert.ChangeType(obj,typeof(T));//parentheses T converts;whatever you return will be of type T
                //return (T)obj;//you can also do this
            }
        }

        public int ExecuteQuery(string storedProcedure, 
            IDbDataParameter[] parameters, 
            Action<IDbDataParameter[]> returnParameters = null)//basic setup for insert, update, and delete
        {
            using (IDbConnection conn = Conn)
            using (IDbCommand cmd = Cmd)
            {
                if (Conn.State != ConnectionState.Open)//first step is always to open connection
                    Conn.Open();

                cmd.Connection = conn;
                Cmd.CommandTimeout = 5000;
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = storedProcedure;
                foreach (IDbDataParameter parameter in parameters)
                    cmd.Parameters.Add(parameter);

                int returnValue = cmd.ExecuteNonQuery();//return how many rows not executed
                returnParameters?.Invoke(parameters);

                return returnValue;
            }
        }

    }
}
