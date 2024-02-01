using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_UD5_CreacionInformes_Wendel
{
    // Clase Modelo que representa un articulo en la base de datos.
    internal class Articulo
    {

        private int idArticulo;
        private String descripcion;
        private Decimal precio;

        public int IdArticulo { get { return idArticulo; } set { idArticulo = value; } }
        public String Descripcion { get { return descripcion; } set { descripcion = value; } } 
        public Decimal Precio { get { return precio; } set { precio = value; } }


        public Articulo(int idArticulo, string descripcion, decimal precio)
        {
            IdArticulo = idArticulo;
            Descripcion = descripcion;
            Precio = precio;
        }



        
    }
}
