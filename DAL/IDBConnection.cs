using System.Collections.Generic;

namespace DAL
{
    public interface IDBConnection
    {
        /// <summary>
        /// Property to hold the connection string for the class which implement this interface.
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary>
        /// Execute a SQL insert, update, delete command to a database. 
        /// </summary>
        /// <param name="command">The SQL Command to execute</param>
        /// <param name="parameters">The list of parameters to pass to the command if any are necessary</param>
        /// <returns>Returns a boolean value indicating if the execution was successful or not</returns>
        bool ExecCommand(string command, params DBParameter[] parameters);

        /// <summary>
        /// Execute a SQL select query to a database.
        /// </summary>
        /// <param name="query">The SQL select to execute</param>
        /// <param name="parameters">The list of parameters to pass to the query if any are necessary</param>
        /// <returns>It return a 2-dimensional array in a form of a list of vector of objects which can be converted to a list of objects later</returns>
        List<object[]> ExecQuery(string query, params DBParameter[] parameters);

        /// <summary>
        /// Execute a SQL stored procedure which runs a select query to a database and returns a cursor to iterate.
        /// </summary>
        /// <param name="procedure">The name of the procedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the procedure if any are necessary</param>
        /// <returns>It return a 2-dimensional array in a form of a list of vector of objects which can be converted to a list of objects later</returns>
        List<object[]> ExecStoredProcQuery(string procedure, params DBParameter[] parameters);

        /// <summary>
        /// Execute a SQL select query to a database, which returns a single value or scalar from it, could be the result of a SQL function like a count, average, etc.
        /// </summary>
        /// <param name="query">The SQL select to execute</param>
        /// <param name="parameters">The list of parameters to pass to the query if any are necessary</param>
        /// <returns>It returns a single object with the scalar obtained from database, which can be parsed to right type after</returns>
        object ExecScalar(string query, params DBParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure which run an insert, update, delete command to a database. 
        /// </summary>
        /// <param name="procedure">The name of the procedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the command if any are necessary</param>
        /// <returns>Returns a boolean value indicating if the execution was successful or not</returns>
        bool ExecStoredProc(string procedure, params DBParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure which run an insert command to a database. 
        /// </summary>
        /// <param name="procedure">The name of the procedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the command</param>
        /// <returns>Returns a long value with the ID of the new row created, if the value is -1 it means that an error occurred in the execution of the stored procedure</returns>
        long ExecStoredProcAdd(string procedure, params DBParameter[] parameters);
    }
}
