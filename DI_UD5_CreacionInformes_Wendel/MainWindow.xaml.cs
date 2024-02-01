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
using SpreadsheetLight;

using iText;
using iText.Html2pdf;
using iText.Bouncycastle;
using iText.Kernel.Pdf;
using Path = System.IO.Path;
using iText.Layout;
using Binding = System.Windows.Data.Binding;
using System.ComponentModel;
using System.Collections;

namespace DI_UD5_CreacionInformes_Wendel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Clase con los metodos de conexión con la BBDD
        ConexionBD conexionBD;

        // String para guardar que tipo de consulta se ha realizado para luego agregar ese string
        // en el nombre del fichero a guardar.
        String consultaTipo = "";

        //  Contenido que tendrá la etiqueta H1 en el documento html que se crea antes de convertirlo a PDF.
        String tituloHtml = "";

        public MainWindow()
        {
            InitializeComponent();
            disableButtons();
        }

        // Deshabilitar botones de guardado
        private void disableButtons()
        {
            btnSaveEXCEL.IsEnabled = false;
            btnSavePDF.IsEnabled = false;
        }

        // Habilitar botones de guardado
        private void enableButtons()
        {
            btnSaveEXCEL.IsEnabled = true;
            btnSavePDF.IsEnabled = true;
        }

        private void EstablecerConexion(object sender, RoutedEventArgs e)
        {
            Conectar(sender,e);
        }

        private void Conectar(object sender, RoutedEventArgs e)
        {
            // Instanciar la clase ConexionBD
            this.conexionBD = new ConexionBD();

            // Asignar valores al label en función de la conexión.
            if (this.conexionBD.getConexion() != null)
            {
                labelConexion("Conexion establecida.");
            }
            else
            {
                labelConexion("Error al establecer la conexión.");
            }
        }


        private void CerrarConexion()
        {
            // Si la conexión no es null la intentamos cerrar, también limpiamos el dataGrid y
            // asignamos un string en el  label.
            if (this.conexionBD != null)
            {
                try
                {
                    this.conexionBD.CerrarConexion();
                    dataGrid.Columns.Clear();
                    dataGrid.ItemsSource = null;

                    // Deshabilitar botones de guardado
                    disableButtons();

                    labelConexion("Se ha cerrado la conexión con el servidor.");
                }
                catch (Exception ex)
                {
                    labelConexion("Error al cerrar la conexion: " + ex.Message);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            CerrarConexion();
            base.OnClosing(e);
        }



        private void labelConexion(String msg)
        {
            label.Content = msg;
        }

        /*
         * Consulta a la BBDD de todos los articulos, agregarlos a una List y asignarla al dataGrid.
         */
        private void consultarArticulos(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.conexionBD == null || this.conexionBD.getConexion() == null)
                {
                    return;
                }
                else
                {
                    // Query para enviar a la base de datos.
                    String consulta = "select * from fabrica.articulos;";
                    SqlDataReader sqlDataReader = null;
                    // Ejecutamos la consulta y guardamos el resultado en sqlDataReader.
                    sqlDataReader = this.conexionBD.ExecuteQuery(consulta);
                    List<Articulo> articuloList = new List<Articulo>();
                    if (sqlDataReader != null)
                    {
                        int id;
                        String descripcion;
                        Decimal precio;
                        while (sqlDataReader.Read())
                        {
                            // Guardamos los valores en variables, creamos un objeto Articulo con esos valores
                            // y lo agregamos a la lista articuloList.
                            id = sqlDataReader.GetInt32("Id_articulo");
                            descripcion = sqlDataReader.GetString("Descripcion");
                            precio = sqlDataReader.GetDecimal("Precio");
                            articuloList.Add(new Articulo(id, descripcion, precio));

                        }
                    }
                    // Limpiamos el dataGrid y luego le asignamos el contenido de la lista de articulos.
                    dataGrid.Columns.Clear();
                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = articuloList;
                    this.consultaTipo = "_articulos";
                    this.tituloHtml = "LISTA DE ARTICULOS";

                    // Habilitar botones de guardado
                    enableButtons();

                    if (sqlDataReader != null)
                    {
                        // Cerramoe el objeto SqlDataReader
                        sqlDataReader.Close();
                    }
                }
            }
            // Controlamos las excepciones y mostramos por pantalla el error.
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

                    // Habilitar botones de guardado
                    enableButtons();

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

                    // Habilitar botones de guardado
                    enableButtons();

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
                    labelConexion("Primero debe establecerse una conexión con el servidor.");
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

                    SqlDataReader sqlDataReader = null;

                    sqlDataReader = this.conexionBD.ExecuteQuery(consulta);

                    // Creo una List de array de strings para almacenar los valores de la base de datos, así me evito
                    // crear varias clases que relacionen esos objetos pudiendo crear cualquier consulta sin crear ninguna clase.
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

                            // Creo un Array de string con los valores obtenidos
                            String[] informe = new string[] { cNombre, cDireccion, cCiudad, pIdPedido, pFechaPedido, montoTotal };

                            // Agrego ese array a la lista.
                            strings.Add(informe);

                            //System.Windows.MessageBox.Show(cNombre);
                        }
                        sqlDataReader.Close();
                    }
                    // Limpio el dataGrid
                    dataGrid.Columns.Clear();
                    dataGrid.ItemsSource = null;
                    dataGrid.AutoGenerateColumns = false;
                    // Agrego las cabeceras del dataGrid
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "NombreCliente", Binding = new System.Windows.Data.Binding("[0]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "DireccionCliente", Binding = new System.Windows.Data.Binding("[1]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "CiudadCliente", Binding = new System.Windows.Data.Binding("[2]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID_Pedido", Binding = new System.Windows.Data.Binding("[3]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fecha_Pedido", Binding = new System.Windows.Data.Binding("[4]") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "MontoPagar", Binding = new System.Windows.Data.Binding("[5]") });
                    // Asigno la lista con todos los valores a la propiedad ItemsSource del dataGrid.
                    dataGrid.ItemsSource = strings;
                    // Tipo de consulta para agregar en los ficheros PDF o EXCEL.
                    this.consultaTipo = "_informeFacturas";
                    // Titulo que irá en la cabecera del PDF.
                    this.tituloHtml = "INFORME DE FACTURAS";

                    // Habilitar botones de guardado
                    enableButtons();

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
                    labelConexion("Primero debe establecerse una conexión con el servidor.");
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

                SqlDataReader sqlDataReader = null;
                List<string[]> strings = new List<string[]>();

                    sqlDataReader = this.conexionBD.ExecuteQuery(consulta);


                    if (sqlDataReader != null)
                    {

                        while (sqlDataReader.Read())
                        {
                            String aDescripcion = sqlDataReader.GetString("DescripcionArticulo");
                            String cantidadTotal = sqlDataReader.GetInt32("CantidadTotalVendida").ToString();
                            String[] informe = new string[] { aDescripcion, cantidadTotal};
                            strings.Add(informe);

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

                    // Habilitar botones de guardado
                    enableButtons();

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
        /*
         * Funcion para convertir un DataGrid a HTML
         */
        private string ConvertDataGridToHtml(DataGrid dataGrid)
        {
            // StringBuilder para almacenar el contenido.
            StringBuilder htmlBuilder = new StringBuilder();

            // Encabezado del HTML
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");

            // Aplicar estilos para la tabla
            htmlBuilder.AppendLine("<style>");
            htmlBuilder.AppendLine("table {border-collapse: collapse; width: 100%; margin: auto; border-radius: 5px; overflow: hidden;}");
            htmlBuilder.AppendLine("th, td { border: 1px solid #ddd; padding: 10px; text-align: center; font-family: 'Arial', sans-serif}");
            htmlBuilder.AppendLine("th { background-color: #4CAF50; color: white; font-weight: bold; font-family: 'Georgia', serif; } ");
            htmlBuilder.AppendLine("tr:nth-child(even){background-color: #f2f2f2} ");
            htmlBuilder.AppendLine(".euros {text-align: right;}");
            htmlBuilder.AppendLine("h1{text-align: center; font-family: 'verdana', sans-serif;}");
            htmlBuilder.AppendLine("</style>");

            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");

            // Titulo
            htmlBuilder.AppendLine("<h1>" + this.tituloHtml +"</h1>");

            // Tabla
            htmlBuilder.AppendLine("<table>");
            htmlBuilder.AppendLine("<tr>");

            // Agregar los encabezados de la tabla en negrita (nombre de las columnas).
            foreach(var column in dataGrid.Columns)
            {
                htmlBuilder.AppendLine($"<th>{((DataGridColumn)column).Header}</th>");
            }
            htmlBuilder.AppendLine("</tr>");



            // Contenido de la tabla
            foreach (var item in dataGrid.Items)
            {
                htmlBuilder.AppendLine("<tr>");

                // Iteramos sobre las columnas para obtener el valor de las celdas.
                foreach (var column in dataGrid.Columns)
                {
                    // Obtener el nombre la propiedad de la columna
                    var propertyName = ((DataGridColumn)column).SortMemberPath;

                    // Obtener información en el item sobre esa propiedad.
                    var propertyInfo = item.GetType().GetProperty(propertyName);

                    // Si no es null obtenemos el valor de esa celda y lo agregamos a la tabla
                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(item, null);
                        if (value != null && value.ToString().Contains("€"))
                        {
                            htmlBuilder.AppendLine($"<td class='euros'>{value}</td>");
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

            // Cerrar etiquetas
            htmlBuilder.AppendLine("</table>");
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            // Devolver un String con todo el HTML.
            return htmlBuilder.ToString();

        }

        /**
         * Comprobar si el objeto pasado como parámetro es un número
         * return true en caso de ser un número, false en caso contrario
         */
        public static bool IsNumeric(object value)
        {
            return value is int || value is decimal || value is float || value is double || value is Int16 || value is Int32 || value is Int64;
        }

        // Metodo para guardar el dataGrid en un documento EXCEL
        private void GuardarDatosExcel(object sender, RoutedEventArgs e)
        {
            try {
                // Llamada al método para guardar un fichero excel a partir de un DataTable,
                // le pasamos como parametro la llamada al método DataGridToDataTable que nos devuelve un objeto
                // DataTable a partir del dataGrid
                SaveDataTableExcel(DataGridToDataTable());
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show("Error al guardar el documento excel." + ex.Message);
            }
        }

        // Guarda un DataTable en un fichero de Excel con formato .xlsx.
        public void SaveDataTableExcel(System.Data.DataTable dataTable)
        {
            SLDocument excel = new SLDocument();

            // Random para el color de fondo
            Random random = new Random();

            // Ancho máximo de celda por columna
            double maxAncho = 0;


            // Aplicar estilos al encabezado
            SLStyle estiloEncabezado = new SLStyle();
            estiloEncabezado.SetFontBold(true);
            estiloEncabezado.Fill.SetPatternType(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid);
            estiloEncabezado.Fill.SetPatternForegroundColor(System.Drawing.Color.FromArgb(0, 128, 0));

            for (int col = 1; col<=dataTable.Columns.Count; col++) {
                excel.SetCellValue(1, col,dataTable.Columns[col - 1].ColumnName);
                excel.SetCellStyle(1, col, estiloEncabezado);
                // Valor ancho maximo de la cabecera
                double maxAnchoCabecera = dataTable.Columns[col - 1].ColumnName.Length * 1.2;
                if (maxAnchoCabecera > maxAncho)
                {
                    maxAncho = maxAnchoCabecera;
                }
            }

            // Aplicar estilos al contenido
            SLStyle estiloCelda = new SLStyle();
            //estiloCelda.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);

            for (int row = 0; row < dataTable.Rows.Count; row ++) {
                estiloCelda.Fill.SetPatternType(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid);
                estiloCelda.Fill.SetPatternForegroundColor(System.Drawing.Color.FromArgb(random.Next(150,256), random.Next(150,256), random.Next(150,256)));

                for (int col = 0; col < dataTable.Columns.Count; col ++)
                {
                    object valorCelda = dataTable.Rows[row][col];
                    // Estimacion del ancho de una celda
                    double anchoCelda = (valorCelda != null) ? valorCelda.ToString().Length * 1.2 : 1.0;

                    // Si es mas grande asignarle el valor al valor máximo de ancho.
                    if(anchoCelda > maxAncho)
                    {
                        maxAncho = anchoCelda;
                    }

                    if(valorCelda != null)
                    {
                        if(valorCelda is bool)
                        {
                            excel.SetCellValue(row + 2, col + 1, (bool)valorCelda);
                        }
                        else if (valorCelda is DateTime){
                            excel.SetCellValue(row + 2, col + 1, (DateTime)valorCelda);
                        }
                        else
                        {
                            excel.SetCellValue(row + 2, col + 1, valorCelda.ToString());
                        }
                    }
                    else
                    {
                        excel.SetCellValue(row + 2, col + 1, String.Empty);
                    }

                    excel.SetCellStyle(row + 2, col + 1, estiloCelda);

                    // Establecer el ancho de columna.
                    excel.SetColumnWidth(col, maxAncho);

                }
            }

            excel.ImportDataTable(1,1,dataTable,true);

            // crear archivo con fecha y hora.
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + consultaTipo + ".xlsx";
            if(saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Guardar fichero en la ruta especificada.
                excel.SaveAs(saveFileDialog.FileName);
            }
        }

        // Método para convertir el DataGrid en un objeto DataTable
        private DataTable DataGridToDataTable()
        {
            // Inicializamos un objeto DataTable
            var dt = new DataTable();
            // Iteramos sobre las columnas del DataGrid y agregamos las cabeceras al datatable
            foreach (DataGridColumn dataGridColumn in dataGrid.Columns)
            {
                if (dataGridColumn is DataGridTextColumn)
                {
                    DataGridTextColumn dataGridTextColumn = (DataGridTextColumn)dataGridColumn;
                    string header = GetHeader(dataGridColumn);
                    dt.Columns.Add(header);
                }
            }

            // Iteramos sobre los items del dataGrid, obtenemos los valores de las celdas y lo agregamos
            // en un DataRow (Fila).
            foreach (var item in dataGrid.Items)
            {
                DataRow dataRow = dt.NewRow();
                foreach (var column in dataGrid.Columns)
                {
                    var propertyName = ((DataGridColumn)column).SortMemberPath;
                    var propertyInfo = item.GetType().GetProperty(propertyName);
                    string header = GetHeader(column);
                    if (propertyInfo != null)
                    {
                        // Obtenemos el valor de la celda
                        var value = propertyInfo.GetValue(item, null);
                        // Agregar el valor al DataRow en la posicion de la columna
                        dataRow[header] = value;

                    }
                    else if (item is string[])
                    {
                        var value = ((string[])item)[dataGrid.Columns.IndexOf(column)];
                        dataRow[header] = value;
                    }
                }

                // Agregamos las filas al DataTable
                dt.Rows.Add(dataRow);
            }
            // Devolvemos el objeto DataTable
            return dt;
        }

        // Metodo para obtener el valor de un DataGridcolumn en formato de String.
        private static string GetHeader(DataGridColumn dataGridColumn)
        {
            if(dataGridColumn != null)
            {
                return $"{((DataGridColumn)dataGridColumn).Header}";
            }
            else
            {
                return "Dato Desconocido.";
            }
            return dataGridColumn.Header.ToString();
        }

        // Metodo para Obtener el valor de una celda a partir de un objeto item y un DataGridTextColumn.
        private static object ObtenerValorCelda(object item, DataGridTextColumn textColumn)
        {
            var binding = textColumn.Binding as System.Windows.Data.Binding;
            if(binding != null)
            {
                string propertyPath = binding.Path.Path;
                var property = item.GetType().GetProperty(propertyPath);
                if(property != null)
                {
                    return property.GetValue(item, null);
                }
            }
            return null;
        }

        // Metodo que establece una conexión, realiza una peticion a la base de datos, llena el dataGrid con los valores
        // y guarda los datos en un fichero PDF.
        // No se está utilizando este método, lo tengo a modo de guía.
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
                labelConexion("Se ha producido un error al guardar el documento PDF. " + ex.Message);
            }
        }

        // Método para cerrar la conexión al pulsar el botón de cerrar conexion.
        private void BtnCerrarConexion(object sender, RoutedEventArgs e)
        {
            CerrarConexion();
        }
    }

}


