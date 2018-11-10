using System;

namespace DAL
{
    public enum DBType { SQLServer, MySQL, PostgreSQL, Oracle }
    public class DBConnection
    {
        public static IDBConnection TheConnection;
        public static string ConnectionString = string.Empty;
        public static DBType Type = DBType.MySQL;

        private DBConnection() { }

        public static void CreateConnection() {
            if (string.IsNullOrEmpty(ConnectionString)) {
                throw new Exception("You must provide a connection string in order to use a database.");
            }

            switch (Type) {
                case DBType.SQLServer:
                    TheConnection = new SQLServerConnection();
                    break;
                case DBType.MySQL:
                    TheConnection = new MySQLConnection();
                    break;
            }

            TheConnection.ConnectionString = ConnectionString;
        }
    }
}
