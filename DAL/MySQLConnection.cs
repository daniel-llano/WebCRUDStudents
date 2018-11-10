using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DAL
{
    public class MySQLConnection : IDBConnection
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
            using (var connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new MySqlCommand(command, connection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                MySqlParameter sqlParameter = new MySqlParameter("p" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                if (parameter.TypeOfValue == TypeOfValue.DATETIME)
                                    sqlParameter.Value = DateTime.Parse(parameter.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                else 
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
            using (var connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new MySqlCommand(query, connection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                MySqlParameter sqlParameter = new MySqlParameter("p" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                if (parameter.SqlType == ParameterType.OUT)
                                {
                                    sqlParameter.Direction = ParameterDirection.Output;
                                    outputs.Add("p" + parameter.Name, parameter);
                                }
                                else
                                {
                                    if (parameter.TypeOfValue == TypeOfValue.DATETIME)
                                        sqlParameter.Value = DateTime.Parse(parameter.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                    else
                                        sqlParameter.Value = parameter.Value;
                                }
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        MySqlDataReader dataReader = sqlCommand.ExecuteReader();
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
            using (var connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new MySqlCommand(query, connection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                MySqlParameter sqlParameter = new MySqlParameter("p" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                if (parameter.TypeOfValue == TypeOfValue.DATETIME)
                                    sqlParameter.Value = DateTime.Parse(parameter.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                else
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
            using (var connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new MySqlCommand(procedure, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                MySqlParameter sqlParameter = new MySqlParameter("p" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                if (parameter.TypeOfValue == TypeOfValue.DATETIME)
                                    sqlParameter.Value = DateTime.Parse(parameter.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                else
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
            using (var connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new MySqlCommand(procedure, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                MySqlParameter sqlParameter = new MySqlParameter("p" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));
                                if (parameter.TypeOfValue == TypeOfValue.DATETIME)
                                    sqlParameter.Value = DateTime.Parse(parameter.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                else
                                    sqlParameter.Value = parameter.Value;
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        MySqlParameter returnedId = new MySqlParameter();
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
            using (var connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                using (var sqlCommand = new MySqlCommand(procedure, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                MySqlParameter sqlParameter = new MySqlParameter("p" + parameter.Name, GetSQLDataType(parameter.TypeOfValue));

                                if (parameter.SqlType == ParameterType.OUT)
                                {
                                    sqlParameter.Direction = ParameterDirection.Output;
                                    outputs.Add("p" + parameter.Name, parameter);
                                }
                                else
                                {
                                    if (parameter.TypeOfValue == TypeOfValue.DATETIME)
                                        sqlParameter.Value = DateTime.Parse(parameter.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                    else
                                        sqlParameter.Value = parameter.Value;
                                }

                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }

                        MySqlDataReader dataReader = sqlCommand.ExecuteReader();
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

        private MySqlDbType GetSQLDataType(TypeOfValue typeOfValue)
        {
            switch (typeOfValue)
            {
                case TypeOfValue.BOOLEAN:
                    return MySqlDbType.Bit;
                case TypeOfValue.DATE:
                    return MySqlDbType.Date;
                case TypeOfValue.DATETIME:
                    return MySqlDbType.DateTime;
                case TypeOfValue.DECIMAL:
                    return MySqlDbType.Decimal;
                case TypeOfValue.INTEGER:
                    return MySqlDbType.Int32;
                case TypeOfValue.LONG:
                    return MySqlDbType.Int64;
                case TypeOfValue.STRING:
                    return MySqlDbType.VarChar;
                case TypeOfValue.TEXT:
                    return MySqlDbType.Text;
                case TypeOfValue.CHAR:
                    return MySqlDbType.VarChar;
                default:
                    return MySqlDbType.VarChar;
            }
        }
    }
}
