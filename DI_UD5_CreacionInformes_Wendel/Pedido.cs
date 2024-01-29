using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_UD5_CreacionInformes_Wendel
{
    internal class Pedido
    {
        private int iDPedido;
        private int iDCliente;
        private String fechaPedido;


        public int IDPedido { get => iDPedido; set => iDPedido = value; }
        public int IDCliente { get => iDCliente; set => iDCliente = value; }
        public string FechaPedido { get => fechaPedido; set => fechaPedido = value; }

        public Pedido(int iDPedido, int iDCliente, string fechaPedido)
        {
            IDPedido = iDPedido;
            IDCliente = iDCliente;
            FechaPedido = fechaPedido;
        }
    }
}
