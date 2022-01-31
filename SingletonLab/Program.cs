using System;
using System.Threading;
using System.Data.SqlClient;
using System.Data.Common;

namespace SingletonLab
{

    class SingletonDBCon
    {
        private static SingletonDBCon _instance;
        private static object _lock = new object();
        private SingletonDBCon() { }

        private SqlConnection sqlConnection = new SqlConnection(StringConnection());

        public static SingletonDBCon GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SingletonDBCon();
                }

                return _instance;
            }
        }

        public SqlConnection GetDBConnection()
        {
            return sqlConnection;
        }

        private static String StringConnection()
        {
            string datasource = @"DESKTOP-OBLN0AT";
            string database = "InchirieriAuto";
            string username = "sa";
            string password = "12345";

            return DBSQLServerUtils.GetDBConnection(datasource, database, username, password);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SingletonDBCon newInstance = SingletonDBCon.GetInstance();
            SingletonDBCon newInstance1 = SingletonDBCon.GetInstance();

            if (newInstance == newInstance1)
            {
                Console.WriteLine("Verificare instanta - Aceeasi instanta!");
            }       

            SqlConnection conn = newInstance1.GetDBConnection();

            SqlCommand cmd = conn.CreateCommand();
            conn.Open();
            try
            {
                ExtractData(conn);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private static void ExtractData(SqlConnection conn)
        {
            string sql = "SELECT ClientId, Nume, Prenume, Adresa FROM Clienti";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            using(DbDataReader reader = cmd.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        int clientIdIndex = reader.GetOrdinal("ClientId");
                        long clientId = Convert.ToInt64(reader.GetValue(clientIdIndex));

                        int clientNumeIndex = reader.GetOrdinal("Nume");
                        string clientNume = reader.GetString(clientNumeIndex);

                        int clientPrenumeIndex = reader.GetOrdinal("Prenume");
                        string clientPrenume = reader.GetString(clientPrenumeIndex);

                        int clientAdresaIndex = reader.GetOrdinal("Adresa");
                        string clientAdresa = reader.GetString(clientAdresaIndex);

                        Console.WriteLine("\n");
                        Console.WriteLine("--------------------");
                        //Console.WriteLine("clientIdIndex : " + clientIdIndex);
                        Console.WriteLine("clientId - " + clientId);
                        //Console.WriteLine("clientNumeIndex : " + clientNumeIndex);
                        Console.WriteLine("clientNume - " + clientNume);
                        Console.WriteLine("clientPrenume - " + clientPrenume);
                        Console.WriteLine("clientAdresa - " + clientAdresa);
                    }
                }
            }
        }
    }
}
