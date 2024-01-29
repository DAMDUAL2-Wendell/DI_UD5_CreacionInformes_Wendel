using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using iText;
using iText.Html2pdf;
using iText.Bouncycastle;
using iText.Kernel.Pdf;
using Path = System.IO.Path;
using iText.Layout;
using Binding = System.Windows.Data.Binding;
using System.ComponentModel;

namespace DI_UD5_CreacionInformes_Wendel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConexionBD conexionBD;
        String consultaTipo = "";
        String tituloHtml = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Conectar(sender,e);
        }

        private void Conectar(object sender, RoutedEventArgs e)
        {
            this.conexionBD = new ConexionBD();

            if (this.conexionBD.getConexion() != null)
            {
                labelConexion("");
            }
            else
            {
                labelConexion("Error al establecer la conexión.");
            }
        }


        private void CerrarConexion()
        {
            if (this.conexionBD != null)
            {
                this.conexionBD.CerrarConexion();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            CerrarConexion();
            base.OnClosing(e);
        }



        private void labelConexion(String msg)
        {
            try
            {
                String mensaje = "";
                if (msg != null)
                {
                    mensaje += msg;
                }
                if (this.conexionBD != null && this.conexionBD.getConexion() != null)
                {
                    label.Content = "Conexion establecida correctamente." + mensaje;
                }
                else
                {
                    label.Content = "No hay conexion. " + mensaje;
                }

            }
            catch (Exception ex) { }
        }

        private void consultarArticulos(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.conexionBD == null || this.conexionBD.getConexion() == null)
                {
                    labelConexion("");
                    return;
                }
                else
                {
                    String consulta = "select * from fabrica.articulos;";
                    SqlDataReader sqlDataReader = null;
                    sqlDataReader = this.conexionBD.ExecuteQuery(consulta);
                    List<Articulo> articuloList = new List<Articulo>();
                    if (sqlDataReader != null)
                    {
                        int id;
                        String descripcion;
                        Decimal precio;
                        while (sqlDataReader.Read())
                        {
                            id = sqlDataReader.GetInt32("Id_articulo");
                            descripcion = sqlDataReader.GetString("Descripcion");
                            precio = sqlDataReader.GetDecimal("Precio");
                            articuloList.Add(new Articulo(id, descripcion, precio));

                        }
                    }

                    dataGrid.Columns.Clear();
                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = articuloList;
                    this.consultaTipo = "_articulos";
                    this.tituloHtml = "LISTA DE ARTICULOS";
                    labelConexion("");

                    if (sqlDataReader != null)
                    {
                        sqlDataReader.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                labelConexion("Error al ejecutar la consulta: " + ex.Message);
            }
            catch (Exception ex)
            {
                labelConexion("Error general: " + ex.Message);
            }
        }


        private void consultarClientes(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.conexionBD == null || this.conexionBD.getConexion() == null)
                {
                    labelConexion("");
                    return;
                }
                else
                {
                    String consulta = "select * from fabrica.clientes;";
                    SqlDataReader sqlDataReader = null;
                    sqlDataReader = this.conexionBD.ExecuteQuery(consulta);
                    List<Cliente> clienteList = new List<Cliente>();
                    if (sqlDataReader != null)
                    {
                        int id;
                        String nombre;
                        String direccion;
                        String ciudad;
                        while (sqlDataReader.Read())
                        {
                            id = sqlDataReader.GetInt32("ID_Cliente");
                            nombre = sqlDataReader.GetString("Nombre");
                            direccion = sqlDataReader.GetString("Direccion");
                            ciudad = sqlDataReader.GetString("Ciudad");
                            clienteList.Add(new Cliente(id, nombre, direccion, ciudad));

                        }
                    }
                    dataGrid.Columns.Clear();
                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = clienteList;
                    this.consultaTipo = "_clientes";
                    this.tituloHtml = "LISTA DE CLIENTES";
                    labelConexion("");

                    if (sqlDataReader != null)
                    {
                        sqlDataReader.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                labelConexion("Error al ejecutar la consulta: " + ex.Message);
            }
            catch (Exception ex)
            {
                labelConexion("Error general: " + ex.Message);
            }
        }

        private void consultarPedidos(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.conexionBD == null || this.conexionBD.getConexion() == null)
                {
                    labelConexion("");
                    return;
                }

                String consulta = "select * from fabrica.pedidos;";
                SqlDataReader sqlDataReader = null;
                sqlDataReader = this.conexionBD.ExecuteQuery(consulta);
                List<Pedido> pedidosList = new List<Pedido>();

                if (sqlDataReader != null)
                {
                    while (sqlDataReader.Read())
                    {
                        int idPedido = sqlDataReader.GetInt32("ID_Pedido");
                        int idCliente = sqlDataReader.GetInt32("ID_Cliente");
                        DateTime fecha = sqlDataReader.GetDateTime("Fecha_Pedido");
                        pedidosList.Add(new Pedido(idPedido, idCliente, fecha.ToString()));
                    }

                    dataGrid.Columns.Clear();
                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = pedidosList;
                    this.consultaTipo = "_pedidos";
                    this.tituloHtml = "LISTA DE PEDIDOS";
                    sqlDataReader.Close();
                }
                else
                {
                    labelConexion("Error al ejecutar la consulta: ");
                }

                
            }
            catch (SqlException ex)
            {
                labelConexion("Error al ejecutar la consulta: " + ex.Message);
            }
            catch (Exception ex)
            {
                labelConexion("Error general: " + ex.Message);
            }
        }


        public void informeFacturas(object sender, RoutedEventArgs e)
        {
            try {
                if (this.conexionBD == null || this.conexionBD.getConexion() == null)
                {
                    labelConexion("");
                }
                else
                {
                    String consulta =
                        "SELECT " +
                            "c.Nombre AS NombreCliente, " +
                            "c.Direccion AS DireccionCliente, " +
                            "c.Ciudad AS CiudadCliente, " +
                            "p.ID_Pedido, " +
                            "p.Fecha_Pedido, " +
                            "SUM(d.Precio * d.Unidades) AS MontoPagar " +
                        "FROM " +
                            "fabrica.clientes c " +
                            "INNER JOIN fabrica.pedidos p ON c.ID_Cliente = p.ID_Cliente " +
                            "INNER JOIN fabrica.detalle_pedidos d ON p.ID_Pedido = d.ID_Pedido " +
                        "GROUP BY " +
                            "c.Nombre, c.Direccion, c.Ciudad, p.ID_Pedido, p.Fecha_Pedido " +
                        "ORDER BY " +
                            "c.Nombre, p.Fecha_Pedido;";

                    // SqlCommand sqlCommand = new SqlCommand(consulta);
                    //SqlConnection sqlConnection = this.conexionBD.getConexion();
                    SqlDataReader sqlDataReader = null;

                    //sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader = this.conexionBD.ExecuteQuery(consulta);

                    List<string[]> strings = new List<string[]>();

                    if (sqlDataReader != null)
                    {

                        while (sqlDataReader.Read())
                        {
                            String cNombre = sqlDataReader.GetString("NombreCliente");
                            String cDireccion = sqlDataReader.GetString("DireccionCliente");
                            String cCiudad = sqlDataReader.GetString("CiudadCliente");
                            String pIdPedido = sqlDataReader.GetInt32("ID_Pedido").ToString();
                            String pFechaPedido = sqlDataReader.GetDateTime("Fecha_Pedido").ToString();
                            String montoTotal = sqlDataReader.GetDecimal("MontoPagar").ToString() + " €";

                            String[] informe = new string[] { cNombre, cDireccion, cCiudad, pIdPedido, pFechaPedido, montoTotal };

                            strings.Add(informe);

                            //System.Windows.MessageBox.Show(cNombre);
                        }
                        sqlDataReader.Close();
                    }
                    dataGrid.Columns.Clear();
                    dataGrid.ItemsSource = null;
                    dataGrid.AutoGenerateColumns = false;
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "NombreCliente", Binding = new System.Windows.Data.Binding("[0]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "DireccionCliente", Binding = new System.Windows.Data.Binding("[1]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "CiudadCliente", Binding = new System.Windows.Data.Binding("[2]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID_Pedido", Binding = new System.Windows.Data.Binding("[3]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fecha_Pedido", Binding = new System.Windows.Data.Binding("[4]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "MontoPagar", Binding = new System.Windows.Data.Binding("[5]") });

                    dataGrid.ItemsSource = strings;
                    this.consultaTipo = "_informeFacturas";
                    this.tituloHtml = "INFORME DE FACTURAS";

                }
            }
            catch (SqlException ex)
            {
                labelConexion("Error al ejecutar la consulta: " + ex.Message);
            }
            catch (Exception ex)
            {
                labelConexion("Error general: " + ex.Message);
            }


        }

        public void informeVentas(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.conexionBD == null || this.conexionBD.getConexion() == null)
                {
                    labelConexion("");
                }
                else
                {
                    String consulta = 
                    "SELECT a.Descripcion AS DescripcionArticulo, " +
                        "SUM(d.Unidades) AS CantidadTotalVendida " +
                    "FROM " +
                        "fabrica.articulos a " +
                        "INNER JOIN fabrica.detalle_pedidos d ON a.Id_articulo = d.ID_Articulo " +
                    "GROUP BY " +
                        "a.Descripcion " +
                    "ORDER BY " +
                        "CantidadTotalVendida DESC;";

                //SqlCommand sqlCommand = new SqlCommand(consulta);
                //SqlConnection conexion = this.conexionBD.getConexion();
                SqlDataReader sqlDataReader = null;
                List<string[]> strings = new List<string[]>();


                    // sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader = this.conexionBD.ExecuteQuery(consulta);


                    if (sqlDataReader != null)
                    {

                        while (sqlDataReader.Read())
                        {
                            String aDescripcion = sqlDataReader.GetString("DescripcionArticulo");
                            String cantidadTotal = sqlDataReader.GetInt32("CantidadTotalVendida").ToString();

                            String[] informe = new string[] { aDescripcion, cantidadTotal};

                            strings.Add(informe);

                            //System.Windows.MessageBox.Show(cNombre);
                        }
                        sqlDataReader.Close();
                    }
                    dataGrid.Columns.Clear();
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = strings;
                    this.consultaTipo = "_informeVentas";
                    this.tituloHtml = "INFORME DE VENTAS";

                    dataGrid.AutoGenerateColumns = false;
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "DescripcionArticulo", Binding = new System.Windows.Data.Binding("[0]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "CantidadTotalVendida", Binding = new System.Windows.Data.Binding("[1]") });


            }
            }
            catch (SqlException ex)
            {
                labelConexion("Error al ejecutar la consulta: " + ex.Message);
            }
            catch (Exception ex)
            {
                labelConexion("Error general: " + ex.Message);
            }

        }


        private void SaveToPdfButton_Click(object sender, RoutedEventArgs e) {


            string htmlConStilos = ConvertDataGridToHtml(dataGrid);

            
            
            string tempHtmlFile = Path.Combine(Path.GetTempPath(), "temp.html");
            File.WriteAllText(tempHtmlFile, htmlConStilos);

                SaveFileDialog guardarPDF = new SaveFileDialog();


            guardarPDF.FileName = DateTime.Now.ToString("yyyyMMdd-HHmm") + this.consultaTipo + ".pdf";

                if (guardarPDF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PdfWriter writer = new PdfWriter(guardarPDF.FileName);

                    var pdfWriter = new PdfWriter(writer);

                    var pdfDocument = new PdfDocument(pdfWriter);

                    HtmlConverter.ConvertToPdf(htmlConStilos, pdfWriter);

                string rutaGuardado = guardarPDF.FileName;

                System.Windows.MessageBox.Show($"El PDF se ha guardado en: " + rutaGuardado);


            }




        }

        private string ConvertDataGridToHtml(DataGrid dataGrid)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");

            // Aplicar estilos
            htmlBuilder.AppendLine("<style>");
            htmlBuilder.AppendLine("table {border-collapse: collapse; width: 100%; margin: auto; border-radius: 5px; overflow: hidden;}");
            htmlBuilder.AppendLine("th, td { border: 1px solid #ddd; padding: 10px; text-align: center; font-family: 'Arial', sans-serif}");
            htmlBuilder.AppendLine("th { background-color: #4CAF50; color: white; font-weight: bold; font-family: 'Georgia', serif; } ");
            htmlBuilder.AppendLine("tr:nth-child(even){background-color: #f2f2f2} ");
            htmlBuilder.AppendLine(".number {text-align: right;}");
            htmlBuilder.AppendLine("h1{text-align: center; font-family: 'verdana', sans-serif;}");
            htmlBuilder.AppendLine("</style>");



            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine("<h1>" + this.tituloHtml +"</h1>");
            htmlBuilder.AppendLine("<table>");

            htmlBuilder.AppendLine("<tr>");

            foreach(var column in dataGrid.Columns)
            {
                htmlBuilder.AppendLine($"<th>{((DataGridColumn)column).Header}</th>");
            }
            htmlBuilder.AppendLine("</tr>");



            // Contenido de la tabla
            foreach (var item in dataGrid.Items)
            {
                htmlBuilder.AppendLine("<tr>");

                foreach (var column in dataGrid.Columns)
                {
                    var propertyName = ((DataGridColumn)column).SortMemberPath;
                    var propertyInfo = item.GetType().GetProperty(propertyName);

                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(item, null);
                        //htmlBuilder.AppendLine($"<td>{value}</td>");
                        if (value != null && IsNumeric(value))
                        {
                            htmlBuilder.AppendLine($"<td class='number'>{value}</td>");
                        }
                        else
                        {
                            htmlBuilder.AppendLine($"<td>{value}</td>");
                        }
                    }
                    else if (item is string[])
                    {
                        var value = ((string[])item)[dataGrid.Columns.IndexOf(column)];
                        htmlBuilder.AppendLine($"<td>{value}</td>");
                    }
                    else
                    {
                        htmlBuilder.AppendLine("<td></td>");
                    }
                }

                htmlBuilder.AppendLine("</tr>");
            }

            htmlBuilder.AppendLine("</table>");
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            return htmlBuilder.ToString();

        }

        public static bool IsNumeric(object value)
        {
            return value is int || value is decimal || value is float || value is double || value is Int16 || value is Int32 || value is Int64;
        }


        public void guardarDatosClientePDF(object sender, RoutedEventArgs e)
        {
            
            try
            {

                SqlConnection conexion = conexionBD.getConexion();


                String tr = "<tr id=\"titulos\">" +
                                "<td id=\"ID\">@IDCliente</td>" +
                                "<td id=\"Nombre\">@Nombre</td>" +
                                "<td id=\"Direccion\">@Direccion</td>" +
                                "<td id=\"Ciudad\">@Ciudad</td>" +
                             "</tr>";

                SqlDataReader sqlDataReader = null;

                String consulta = "select * from Fabrica.fabrica.clientes;";

                String datosClientes = "";

                int id = -1;
                String nombre = "";
                String direccion = "";
                String ciudad = "";


                if(conexion != null)
                {
                    SqlCommand sqlCommand = new SqlCommand(consulta);
                    sqlCommand.Connection = conexion;
                    sqlDataReader = sqlCommand.ExecuteReader();

                    List<Cliente> clienteList = new List<Cliente>();


                    while (sqlDataReader.Read())
                    {
                        String datoCliente = "";
                        datoCliente = tr;


                        id = sqlDataReader.GetInt32("ID_Cliente");

                       

                        nombre = sqlDataReader.GetString("Nombre");
                        direccion = sqlDataReader.GetString("Direccion");
                        ciudad = sqlDataReader.GetString("Ciudad");
                        clienteList.Add(new Cliente(id, nombre, direccion, ciudad));


                        // Reemplazar datos cliente
                        datoCliente = datoCliente.Replace("@IDCliente",id.ToString());
                        datoCliente = datoCliente.Replace("@Nombre", nombre);
                        datoCliente = datoCliente.Replace("@Direccion", direccion);
                        datoCliente = datoCliente.Replace("@Ciudad", ciudad);

                        Cliente cliente = new Cliente(id,nombre,direccion,ciudad);

                        clienteList.Add(cliente);


                        datosClientes += datoCliente;
                         

                    } // while

                    dataGrid.ItemsSource = clienteList;


                    SaveFileDialog guardarPDF = new SaveFileDialog();

                    
                    guardarPDF.FileName = DateTime.Now.ToString("yyyyMMdd-HHss") + ".pdf";
                    
                    // String con el documento HTML
                    String plantillaHtml = new StreamReader("../../../Recursos/Plantilla-clientes.html").ReadToEnd();

                    //System.Windows.MessageBox.Show(datosClientes);


                    plantillaHtml = plantillaHtml.Replace("@CLIENTE_DATOS", datosClientes);

                    if (guardarPDF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        PdfWriter writer = new PdfWriter(guardarPDF.FileName);
                        HtmlConverter.ConvertToPdf(plantillaHtml, writer);

                    }
                    sqlDataReader.Close();

                }

            }
            catch (Exception ex)
            {

            }
        }


    }

}


