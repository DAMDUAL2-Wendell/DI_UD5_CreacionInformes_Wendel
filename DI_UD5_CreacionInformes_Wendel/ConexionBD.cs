using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace DI_UD5_CreacionInformes_Wendel
{
    // Clase para con los métodos y objetos necesarios para realizar la conexión con la base de datos.
    internal class ConexionBD
    {
        // Objeto SqlConnection con la conexión
        private SqlConnection conexion;

        // Strings con los valores para la conexión.
        private String database = "fabrica";
        private String servidor = "localhost";
        private String usuario = "sa";
        private String password = "abc123.";
        private String CadeaConexion;

        // Constructor que asigna la query de conexión al string CadeaConexion.
        public ConexionBD()
        {
            CadeaConexion = "Persist Security Info=False;" +
                            "User ID=" + usuario +
                            ";Password=" + password +
                            ";Initial Catalog=" + database +
                            ";Server=" + servidor;

        }

        // Metodo para establecer la conexión.
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

        // Metodo para ejecutar una consulta sobre la conexión.
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
                throw new Exception("Error al ejecutar la consulta: " + ex.Message);
            }
        }

        // Método para cerrar la conexión.
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
                throw new Exception("Error al cerrar la conexión: " + ex.Message);
            }
        }

    }
}
