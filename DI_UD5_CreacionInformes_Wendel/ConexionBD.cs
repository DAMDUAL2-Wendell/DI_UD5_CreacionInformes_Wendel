using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace DI_UD5_CreacionInformes_Wendel
{
    /// <summary>Clase con los métodos necesarios para establecer una conexión con la base de datos.</summary>
    internal class ConexionBD
    {
        // Objeto SqlConnection con la conexión
        public SqlConnection conexion;

        // Strings con los valores para la conexión.
        private String database = "fabrica";
        private String servidor = "localhost";
        private String usuario = "sa";
        private String password = "abc123.";
        private String CadeaConexion;

        /// <summary>Constructor de la clase <see cref="ConexionBD" />, asigna la query de conexión al string CadeaConexion.</summary>
        public ConexionBD()
        {
            CadeaConexion = "Persist Security Info=False;" +
                            "User ID=" + usuario +
                            ";Password=" + password +
                            ";Initial Catalog=" + database +
                            ";Server=" + servidor;

        }
        /// <summary>Establece una conexión con la base de datos.</summary>
        /// <returns>Devuelve el objeto SqlConnection, null en caso de fallo en la conexión.</returns>
        public SqlConnection getConexion()
        {
            if(conexion == null)
            {
                try {
                    this.conexion = new SqlConnection(CadeaConexion);
                    this.conexion.Open();
                } catch {
                
                }
            }
                return conexion;

        }

        public bool IsConnected()
        {
            return this.conexion.State == ConnectionState.Open;
        }

        /// <summary>Ejecuta una consulta en la base de datos.</summary>
        /// <param name="consulta">La consulta SQL.</param>
        /// <returns>Devuelve un objeto SqlDataReader, null en caso de error al ejecutar la consulta.</returns>
        public SqlDataReader ExecuteQuery(string query)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(query, this.conexion);
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                this.CerrarConexion(); // Asegurarse de cerrar la conexión en caso de error
                //throw new Exception("Error al ejecutar la consulta: " + ex.Message);
            }
            return null;
        }

        /// <summary>Método para cerrar la conexión.</summary>
        public void CerrarConexion()
        {
            try
            {
                if (this.conexion.State == ConnectionState.Open)
                {
                    this.conexion.Close();
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Error al cerrar la conexión: " + ex.Message);
            }
        }

    }
}
