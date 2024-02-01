using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_UD5_CreacionInformes_Wendel
{
    // Clase Modelo que representa un cliente en la base de datos.
    internal class Cliente
    {
        private int idCliente;
        private string nombre;
        private string direccion;
        private string ciudad;

        public int IdCliente { get => idCliente; set => idCliente = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Ciudad { get => ciudad; set => ciudad = value; }

        public Cliente(int idCliente, string nombre, string direccion, string ciudad)
        {
            IdCliente = idCliente;
            Nombre = nombre;
            Direccion = direccion;
            Ciudad = ciudad;
        }


    }
}
