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

        /// <summary>Método que establece una conexión  con un servidor SQL.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
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

        /// <summary>Método que devuelve el objeto MySqlConnection de la clase.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public MySqlConnection getConexion()
        {
            return this.connection;
        }

        /// <summary>Ejecuta una consulta en la base de datos.</summary>
        /// <param name="consulta">La consulta SQL.</param>
        /// <returns>Devuelve un objeto MySqlDataReader, null en caso de error al ejecutar la consulta.</returns>
        public MySqlDataReader ExecuteQuery(string consulta)
        {
            MySqlDataReader sqlDataReader = null;
            if (this.connection.State ==ConnectionState.Open)
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
