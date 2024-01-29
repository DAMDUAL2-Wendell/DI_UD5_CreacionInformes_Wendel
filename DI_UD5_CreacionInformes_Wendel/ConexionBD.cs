using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace DI_UD5_CreacionInformes_Wendel
{
    internal class ConexionBD
    {
        private SqlConnection conexion;
        private String database = "fabrica";
        private String servidor = "localhost";
        private String usuario = "sa";
        private String password = "abc123.";
        private String CadeaConexion;

        public ConexionBD()
        {
            CadeaConexion = "Persist Security Info=False;" +
                            "User ID=" + usuario +
                            ";Password=" + password +
                            ";Initial Catalog=" + database +
                            ";Server=" + servidor;

        }

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

        /*
        public SqlDataReader ExecuteQuery(string consulta)
        {
            SqlDataReader sqlDataReader = null;
            if (true)
            {
                try
                {
                    SqlConnection connection = getConexion();
                    SqlCommand command = new SqlCommand(consulta, connection);
                    sqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                 }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return sqlDataReader;
        }
        */
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
