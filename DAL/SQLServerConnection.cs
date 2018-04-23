using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SQLServerConnection : IDBConnection
    {
        string connectionString = string.Empty;

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                connectionString = value;
            }
        }

        public bool ExecCommand(string command, params DBParameter[] parameters)
        {
            bool result = false;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new SqlCommand(command, connection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                SqlParameter sqlParameter = new SqlParameter("@" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                sqlParameter.Value = parameter.Value;
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        sqlCommand.ExecuteNonQuery();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Create().Exception(ex);
                    }
                }
            }
            return result;
        }

        public List<object[]> ExecQuery(string query, params DBParameter[] parameters)
        {
            List<object[]> resultSet = new List<object[]>();
            Dictionary<string, DBParameter> outputs = new Dictionary<string, DBParameter>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                SqlParameter sqlParameter = new SqlParameter("@" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                if (parameter.SqlType == ParameterType.OUT)
                                {
                                    sqlParameter.Direction = ParameterDirection.Output;
                                    outputs.Add("@" + parameter.Name, parameter);
                                }
                                else
                                    sqlParameter.Value = parameter.Value;
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        SqlDataReader dataReader = sqlCommand.ExecuteReader();
                        while (dataReader.Read())
                        {
                            object[] fila = new object[dataReader.FieldCount];
                            dataReader.GetValues(fila);
                            resultSet.Add(fila);
                        }
                        dataReader.Close();

                        if (outputs.Count > 0)
                        {
                            foreach (string key in outputs.Keys)
                            {
                                outputs[key].Value = sqlCommand.Parameters[key].Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Create().Exception(ex);
                    }
                }
            }
            return resultSet;
        }

        public object ExecScalar(string query, params DBParameter[] parameters)
        {
            object result = null;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                SqlParameter sqlParameter = new SqlParameter("@" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                sqlParameter.Value = parameter.Value;
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        result = sqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Logger.Create().Exception(ex);
                    }
                }
            }
            return result;
        }

        public bool ExecStoredProc(string procedure, params DBParameter[] parameters)
        {
            bool result = false;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new SqlCommand(procedure, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                SqlParameter sqlParameter = new SqlParameter("@" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                sqlParameter.Value = parameter.Value;
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        sqlCommand.ExecuteNonQuery();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Create().Exception(ex);
                    }
                }
            }
            return result;
        }

        public long ExecStoredProcAdd(string procedure, params DBParameter[] parameters)
        {
            int Id = -1;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new SqlCommand(procedure, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                SqlParameter sqlParameter = new SqlParameter("@" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                sqlParameter.Value = parameter.Value;
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        SqlParameter returnedId = new SqlParameter();
                        returnedId.Direction = ParameterDirection.ReturnValue;
                        sqlCommand.Parameters.Add(returnedId);

                        sqlCommand.ExecuteNonQuery();
                        int.TryParse(returnedId.Value.ToString(), out Id);
                    }
                    catch (Exception ex)
                    {
                        Logger.Create().Exception(ex);
                    }
                }
            }
            return Id;
        }

        public List<object[]> ExecStoredProcQuery(string procedure, params DBParameter[] parameters)
        {
            List<object[]> resultSet = new List<object[]>();
            Dictionary<string, DBParameter> outputs = new Dictionary<string, DBParameter>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new SqlCommand(procedure, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                SqlParameter sqlParameter = new SqlParameter("@" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));

                                if (parameter.SqlType == ParameterType.OUT)
                                {
                                    sqlParameter.Direction = ParameterDirection.Output;
                                    outputs.Add("@" + parameter.Name, parameter);
                                }
                                else
                                    sqlParameter.Value = parameter.Value;

                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        SqlDataReader dataReader = sqlCommand.ExecuteReader();
                        while (dataReader.Read())
                        {
                            object[] fila = new object[dataReader.FieldCount];
                            dataReader.GetValues(fila);
                            resultSet.Add(fila);
                        }
                        dataReader.Close();

                        if (outputs.Count > 0)
                        {
                            foreach (string key in outputs.Keys)
                            {
                                outputs[key].Value = sqlCommand.Parameters[key].Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Create().Exception(ex);
                    }
                }
            }
            return resultSet;
        }

        private SqlDbType GetSQLDataType(TypeOfValue typeOfValue)
        {
            switch (typeOfValue)
            {
                case TypeOfValue.BOOLEAN:
                    return SqlDbType.Bit;
                case TypeOfValue.DATE:
                    return SqlDbType.Date;
                case TypeOfValue.DATETIME:
                    return SqlDbType.DateTime;
                case TypeOfValue.DECIMAL:
                    return SqlDbType.Decimal;
                case TypeOfValue.INTEGER:
                    return SqlDbType.Int;
                case TypeOfValue.LONG:
                    return SqlDbType.BigInt;
                case TypeOfValue.STRING:
                    return SqlDbType.VarChar;
                case TypeOfValue.TEXT:
                    return SqlDbType.Text;
                default:
                    return SqlDbType.VarChar;
            }
        }
    }
}
