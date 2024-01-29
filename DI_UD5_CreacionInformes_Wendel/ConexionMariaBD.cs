using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;

namespace DI_UD5_CreacionInformes_Wendel
{
    internal class ConexionMariaBD
    {
        MySqlConnection connection = null;

        string connectionString = "Server=localhost:3306;Database=fabrica;Uid=root;Pwd=;";

        public void conectar(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    this.connection = connection;
                    connection.Open();
                    Console.WriteLine("Conexión exitosa a MariaDB");
                    //MessageBox.Show("Conexión exitosa a MariaDB");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al conectar a MariaDB: {ex.Message}");
                }
            }
        }

        public MySqlConnection getConexion()
        {
            return this.connection;
        }

        public MySqlDataReader ExecuteQuery(string consulta)
        {
            MySqlDataReader sqlDataReader = null;
            if (true)
            {
                try
                {
                    MySqlConnection connection = getConexion();
                    MySqlCommand command = new MySqlCommand(consulta, connection);
                    sqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return sqlDataReader;
        }

    }
}
