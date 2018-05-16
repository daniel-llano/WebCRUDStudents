using System;

namespace DAL
{
    public enum DBType { SQLServer, MySQL, PostgreSQL, Oracle }
    public class DBConnection
    {
        public static IDBConnection TheConnection;
        public static string ConnectionString = string.Empty;

        private DBConnection() { }

        public static void CreateConnection(DBType type) {
            if (string.IsNullOrEmpty(ConnectionString)) {
                throw new Exception("You must provide a connection string in order to use a database.");
            }

            switch (type) {
                case DBType.SQLServer:
                    TheConnection = new SQLServerConnection();
                    break;
            }

            TheConnection.ConnectionString = ConnectionString;
        }
    }
}
