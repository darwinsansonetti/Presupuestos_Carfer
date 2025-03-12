using Aluminum.Conexion;
using Aluminum.Helpers;
using Aluminum.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;


namespace Aluminum.View
{
    public partial class FormReportes : Form
    {
        private HomeMain _formPadre;
        int _empresa_id = 0;
        public string parame { get; set; } = "";
        private PresupuestoImprimeMode _presupuestoPrint { get; set; }
        private List<ProductosPresupuestoImprimeModel> _productosPrint { get; set; }

        public FormReportes(HomeMain formPadre, int empresa_id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;

            _presupuestoPrint = new PresupuestoImprimeMode();
            _productosPrint = new List<ProductosPresupuestoImprimeModel>();
        }

        private void FormReportes_Load(object sender, EventArgs e)
        {
            Filtrar(parame);
        }

        private void Filtrar(string _parame)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            listViewProductosNew.Items.Clear();

            string sql = "";

            //Se buscan todos los productos
            if (string.IsNullOrEmpty(_parame))
            {
                //sql = "select * from presupuesto where presupuesto.empresa_id='" + _empresa_id + "' order by presupuesto.id DESC ";

                sql = "SELECT presupuesto.*, cliente.* FROM presupuesto JOIN cliente ON presupuesto.cliente_id = cliente.id " +
                    "WHERE presupuesto.empresa_id = '" + _empresa_id + "' ORDER BY presupuesto.id DESC";
            }
            else
            {
                sql = "select presupuesto.*, cliente.* from presupuesto JOIN cliente ON presupuesto.cliente_id = cliente.id " +
                    "WHERE presupuesto.empresa_id='" + _empresa_id + "' and presupuesto.id Like '%" + _parame + "%' order by presupuesto.id DESC ";
            }

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                //_empresas = new List<EmpresaModel>();

                while (rdr.Read())
                {
                    ListViewItem lvi = new ListViewItem(rdr[0].ToString());
                    lvi.SubItems.Add(rdr[13].ToString());
                    lvi.SubItems.Add(rdr[2].ToString());

                    listViewProductosNew.Items.Add(lvi);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró realizar la búsqueda, error: " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }
        }

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new FormProductosMain(_formPadre, _empresa_id, new List<int>()));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Filtrar(textBoxProducto.Text);
        }

        private void listViewProductosNew_DoubleClick(object sender, EventArgs e)
        {
            //Se obtiene el ID de la empresa seleccionada
            int presupuesto_id = int.Parse(listViewProductosNew.SelectedItems[0].SubItems[0].Text);
            
            DialogResult dialogResult = MessageBox.Show("¿Desea Generar el presupuesto en PDF?", "Confirmación", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                CConexion _conexion = new CConexion();
                MySqlConnection _conn = _conexion.establecerConexion();

                try
                {
                    string sql = "SELECT presupuesto.*, cliente.*, empresa.* " +
                                 "FROM presupuesto " +
                                 "JOIN cliente ON presupuesto.cliente_id = cliente.id " +
                                 "JOIN empresa ON presupuesto.empresa_id = empresa.id " + // Unir con empresa
                                 "WHERE presupuesto.id = '" + presupuesto_id + "'";

                    HelperQuery _helperQuery = new HelperQuery();
                    MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                    if (rdr.Read())
                    {
                        _presupuestoPrint = new PresupuestoImprimeMode();

                        //Informacion del Presupuesto
                        _presupuestoPrint.id = int.Parse(rdr[0].ToString());
                        _presupuestoPrint.fecha = Convert.ToDateTime(rdr[1]);
                        _presupuestoPrint.monto_total = Convert.ToDouble(rdr[2]);
                        _presupuestoPrint.cliente_id = int.Parse(rdr[3].ToString());
                        _presupuestoPrint.empresa_id = int.Parse(rdr[4].ToString());
                        _presupuestoPrint.nota1 = rdr[5].ToString();
                        _presupuestoPrint.nota2 = rdr[6].ToString();
                        _presupuestoPrint.nota3 = rdr[7].ToString();

                        //Informacion del Cliente
                        _presupuestoPrint.nombre_cliente = rdr[13].ToString();
                        _presupuestoPrint.documento_cliente = rdr[15].ToString();
                        _presupuestoPrint.telefono_cliente = rdr[14].ToString();

                        //Informacion de la Empresa
                        _presupuestoPrint.razon_social = rdr[21].ToString();
                        _presupuestoPrint.documento_empresa = rdr[22].ToString();
                        _presupuestoPrint.direccion = rdr[23].ToString();
                        _presupuestoPrint.telefono_empresa = rdr[24].ToString();
                        _presupuestoPrint.email = rdr[25].ToString();
                        _presupuestoPrint.path_logo = (byte[])rdr[26];

                        _conn.Close();


                        CConexion _conexion1 = new CConexion();
                        MySqlConnection _conn1 = _conexion1.establecerConexion();

                        sql = "SELECT presupuestoproducto.*, producto.* " +
                                 "FROM presupuestoproducto " +
                                 "JOIN producto ON presupuestoproducto.producto_id = producto.id " +
                                 "WHERE presupuestoproducto.presupuesto_id = '" + _presupuestoPrint.id + "'";


                        _productosPrint = new List<ProductosPresupuestoImprimeModel>();

                        HelperQuery _helperQuery1 = new HelperQuery();
                        MySqlDataReader rdr1 = _helperQuery1.querySelect(_conn1, sql);

                        while (rdr1.Read())
                        {
                            ProductosPresupuestoImprimeModel _oneProducto = new ProductosPresupuestoImprimeModel();

                            //Registros del Producto
                            _oneProducto.cantidad = int.Parse(rdr1[1].ToString());
                            _oneProducto.costo = Convert.ToDouble(rdr1[2]);
                            _oneProducto.producto_id = int.Parse(rdr1[3].ToString());
                            _oneProducto.sistema_id = int.Parse(rdr1[5].ToString());
                            _oneProducto.material_id = int.Parse(rdr1[6].ToString());
                            _oneProducto.widht = Convert.ToDouble(rdr1[7]);
                            _oneProducto.height = Convert.ToDouble(rdr1[8]);

                            //Productos en el Presupuesto
                            _oneProducto.nombre_producto = rdr1[12].ToString();
                            _oneProducto.descripcion_producto = rdr1[13].ToString();
                            _oneProducto.path_logo = (byte[])rdr1[14];
                            _oneProducto.costo_metro = Convert.ToDouble(rdr1[15]);

                            _productosPrint.Add(_oneProducto);
                        }

                        _conn1.Close();

                        try
                        {
                            // 1. Cargar la plantilla HTML desde los Recursos
                            string htmlTemplate = Properties.Resources.plantilla; // plantilla es el nombre del recurso

                            // 🔹 2. Crear la imagen en iTextSharp
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(_presupuestoPrint.path_logo);
                            img.ScaleAbsolute(100, 100); // Ajustar tamaño
                            img.Alignment = Element.ALIGN_CENTER;

                            // 2. Reemplazar marcadores dinámicos
                            string htmlContent = htmlTemplate.Replace("{{fecha}}", DateTime.Now.ToString("dd/MM/yyyy"))
                                .Replace("{{titulo}}", "Presupuesto_" + _presupuestoPrint.id + ".pdf")
                                .Replace("{{nombre_empresa}}", _presupuestoPrint.razon_social)
                                .Replace("{{direccion_empresa}}", _presupuestoPrint.direccion)
                                .Replace("{{telefono_empresa}}", _presupuestoPrint.telefono_empresa)
                                .Replace("{{email_empresa}}", _presupuestoPrint.email)
                                .Replace("{{nombre_cliente}}", _presupuestoPrint.nombre_cliente)
                                .Replace("{{documento_cliente}}", _presupuestoPrint.documento_cliente)
                                .Replace("{{logo}}", "");

                            // 3. Definir la ruta donde se guardará el PDF
                            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Presupuesto_" + _presupuestoPrint.id + ".pdf");

                            // 4. Crear un nuevo documento PDF
                            using (FileStream stream = new FileStream(outputPath, FileMode.Create))
                            {
                                Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();

                                // 5. Convertir el HTML a PDF usando XMLWorker
                                using (StringReader sr = new StringReader(htmlContent))
                                {
                                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                                }

                                // 🔹 6. Insertar la imagen en el lugar correcto (donde estaba {{logo}})
                                PdfPTable table = new PdfPTable(3); // 3 columnas (como en la plantilla)
                                table.WidthPercentage = 100;
                                float[] columnWidths = { 20f, 60f, 20f }; // Proporciones de ancho
                                table.SetWidths(columnWidths);

                                // 🔹 2. Definir el tamaño de la celda (ajústalo según lo necesites)
                                float cellWidth = 100f;  // Ancho de la celda
                                float cellHeight = 100f; // Alto de la celda

                                // 🔹 3. Ajustar la imagen para que encaje dentro de la celda
                                img.ScaleToFit(cellWidth - 10, cellHeight - 10); // Descontamos 10px para margen interno
                                img.Alignment = Element.ALIGN_CENTER;

                                // 🖼️ Agregar imagen en la primera celda
                                PdfPCell cellWithImage = new PdfPCell(img)
                                {
                                    FixedHeight = cellHeight,  // Fijamos el alto para que respete el tamaño de la celda
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    VerticalAlignment = Element.ALIGN_MIDDLE, // Centra la imagen en la celda
                                    Border = iTextSharp.text.Rectangle.BOX,  // Borde visible
                                    Padding = 5  // Espacio interno para que no toque los bordes
                                };
                                table.AddCell(cellWithImage);

                                // 📄 Agregar datos de la empresa (columna 2)
                                PdfPCell cellWithCompanyData = new PdfPCell
                                {
                                    //Border = iTextSharp.text.Rectangle.NO_BORDER
                                };
                                cellWithCompanyData.AddElement(new Paragraph(" " + _presupuestoPrint.razon_social, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
                                cellWithCompanyData.AddElement(new Paragraph("  NIF: " + _presupuestoPrint.documento_empresa, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                cellWithCompanyData.AddElement(new Paragraph("  " + _presupuestoPrint.direccion, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                cellWithCompanyData.AddElement(new Paragraph("  Tel: " + _presupuestoPrint.telefono_empresa, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                cellWithCompanyData.AddElement(new Paragraph("  " + _presupuestoPrint.email, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                table.AddCell(cellWithCompanyData);

                                // 📑 Información del comprobante (columna 3)
                                PdfPCell cellWithInvoiceData = new PdfPCell
                                {
                                    //Border = iTextSharp.text.Rectangle.NO_BORDER
                                };
                                cellWithInvoiceData.AddElement(new Paragraph("  PRESUPUESTO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
                                cellWithInvoiceData.AddElement(new Paragraph("  Nro: " + _presupuestoPrint.id, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                table.AddCell(cellWithInvoiceData);

                                pdfDoc.Add(table); // Agregar la tabla con la imagen al PDF

                                //Espacio
                                PdfPTable tableEspacio = new PdfPTable(1);
                                tableEspacio.WidthPercentage = 100;
                                tableEspacio.AddCell(new PdfPCell(new Phrase(" ")) { Border = iTextSharp.text.Rectangle.NO_BORDER, FixedHeight = 10f });
                                pdfDoc.Add(tableEspacio);

                                // Cabecera Modulo Cliente
                                PdfPTable tableCabeceraCliente = new PdfPTable(1); // 1 columnas
                                tableCabeceraCliente.WidthPercentage = 100;

                                // Definir Espacio
                                tableCabeceraCliente.AddCell(new PdfPCell(new Phrase("Información del Cliente", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                                {
                                    //Border = iTextSharp.text.Rectangle.NO_BORDER,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    BackgroundColor = new BaseColor(230, 230, 230) // Color gris claro
                                });

                                pdfDoc.Add(tableCabeceraCliente); // Agregar tabla del cliente


                                // 🔹 3. Agregar la información del cliente (Cliente, Documento, Fecha)
                                PdfPTable tableCliente = new PdfPTable(3); // 2 columnas
                                tableCliente.WidthPercentage = 100;
                                float[] columnWidths1 = { 60f, 20f, 20f }; // Proporciones de ancho
                                table.SetWidths(columnWidths1);

                                // 📄 Agregar datos del clientes (columna 1)
                                PdfPCell cellWithClienteData = new PdfPCell
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE, // Centra la imagen en la celda
                                    //Border = iTextSharp.text.Rectangle.NO_BORDER
                                };

                                cellWithClienteData.AddElement(new Paragraph("Nombre: " + _presupuestoPrint.nombre_cliente, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                tableCliente.AddCell(cellWithClienteData);

                                // 📄 Agregar datos del clientes (columna 1)
                                PdfPCell cellWithClienteDcumento = new PdfPCell
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE, // Centra la imagen en la celda
                                    //Border = iTextSharp.text.Rectangle.NO_BORDER
                                };

                                cellWithClienteDcumento.AddElement(new Paragraph("Documento: " + _presupuestoPrint.documento_cliente, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                tableCliente.AddCell(cellWithClienteDcumento);

                                // 📄 Agregar datos del clientes (columna 1)
                                PdfPCell cellWithClienteFecha = new PdfPCell
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE, // Centra la imagen en la celda
                                    //Border = iTextSharp.text.Rectangle.NO_BORDER
                                };

                                cellWithClienteFecha.AddElement(new Phrase("Fecha Emisión: " + _presupuestoPrint.fecha.ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                tableCliente.AddCell(cellWithClienteFecha);

                                pdfDoc.Add(tableCliente); // Agregar tabla del cliente

                                //Espacio
                                tableEspacio = new PdfPTable(1);
                                tableEspacio.WidthPercentage = 200;
                                tableEspacio.AddCell(new PdfPCell(new Phrase(" ")) { Border = iTextSharp.text.Rectangle.NO_BORDER, FixedHeight = 10f });
                                pdfDoc.Add(tableEspacio);

                                // 🔹 Definir tabla con 3 columnas
                                PdfPTable tableItems = new PdfPTable(3);
                                tableItems.WidthPercentage = 100;
                                float[] columnWidthsItems = { 20f, 60f, 20f }; // Proporciones de ancho
                                tableItems.SetWidths(columnWidthsItems);

                                // 🔹 Recorrer lista de productos (ejemplo)
                                foreach (var item in _productosPrint)  // listaProductos es List<Model>
                                {
                                    // 📸 Imagen en la primera columna
                                    iTextSharp.text.Image imgItem = iTextSharp.text.Image.GetInstance(item.path_logo);
                                    imgItem.ScaleToFit(90, 90); // Ajusta el tamaño de la imagen
                                    imgItem.Alignment = Element.ALIGN_CENTER;

                                    PdfPCell cellImage = new PdfPCell(imgItem)
                                    {
                                        FixedHeight = 100f,  // Tamaño de la celda
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        Border = iTextSharp.text.Rectangle.BOX,
                                        Padding = 5
                                    };
                                    tableItems.AddCell(cellImage);

                                    // 📄 Datos del producto (columna 2)
                                    PdfPCell cellDescripcion = new PdfPCell();
                                    cellDescripcion.AddElement(new Paragraph(" " + item.nombre_producto, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                                    cellDescripcion.AddElement(new Paragraph(" System:   " + obtenerSistema(item.sistema_id), FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                    cellDescripcion.AddElement(new Paragraph(" Material: " + obtenerMaterial(item.material_id), FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                    cellDescripcion.AddElement(new Paragraph(" Widht:    " + item.widht, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                    cellDescripcion.AddElement(new Paragraph(" Height:   " + item.height, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                    tableItems.AddCell(cellDescripcion);

                                    // 💲 Precio u otra información (columna 3)
                                    PdfPCell cellPrecio = new PdfPCell();
                                    cellPrecio.AddElement(new Paragraph(" Units:   " + item.cantidad, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                    cellPrecio.AddElement(new Paragraph("\n\n\n"));
                                    cellPrecio.AddElement(new Paragraph(" Amount:  " + item.costo, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
                                    
                                    tableItems.AddCell(cellPrecio);
                                }

                                // Definir el porcentaje de ancho
                                int totalColumns = tableItems.NumberOfColumns;
                                int anchoContenido = (int)(totalColumns * 0.35); // 35% del ancho
                                int anchoEspacio = totalColumns - anchoContenido; // 65% de espacio vacío

                                // Agregar primera fila
                                tableItems.AddCell(new PdfPCell(new Phrase(" ")) { Border = PdfPCell.NO_BORDER, Colspan = anchoEspacio }); // Celda vacía
                                tableItems.AddCell(new PdfPCell(new Phrase("Subtotal: $" + _presupuestoPrint.monto_total.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                                {
                                    Colspan = anchoContenido,
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 5f
                                });

                                // Agregar segunda fila
                                tableItems.AddCell(new PdfPCell(new Phrase(" ")) { Border = PdfPCell.NO_BORDER, Colspan = anchoEspacio });
                                tableItems.AddCell(new PdfPCell(new Phrase("IVA: $" + "0.00", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                                {
                                    Colspan = anchoContenido,
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 5f
                                });

                                // Agregar tercera fila (Total)
                                tableItems.AddCell(new PdfPCell(new Phrase(" ")) { Border = PdfPCell.NO_BORDER, Colspan = anchoEspacio });
                                tableItems.AddCell(new PdfPCell(new Phrase("Total: $" + _presupuestoPrint.monto_total.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                                {
                                    Colspan = anchoContenido,
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 10f,
                                    BackgroundColor = new BaseColor(230, 230, 230) // Color gris claro para destacar el total
                                });

                                // Agregar una fila vacía para crear espacio antes de "NOTAS"
                                PdfPCell emptyRow = new PdfPCell(new Phrase(" "))
                                {
                                    Colspan = tableItems.NumberOfColumns,
                                    FixedHeight = 15f, // Ajustar la altura del espacio
                                    Border = PdfPCell.NO_BORDER // Sin bordes para que no se vea una línea
                                };

                                // Agregar la fila vacía a la tabla
                                tableItems.AddCell(emptyRow);

                                // Crear la celda para el encabezado "Notas"
                                PdfPCell cellNotas = new PdfPCell(new Phrase("NOTAS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                                {
                                    Colspan = tableItems.NumberOfColumns,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    Padding = 8f,
                                    BackgroundColor = new BaseColor(230, 230, 230) // Color gris claro para destacar el total
                                };

                                // Agregar la celda de "NOTAS"
                                tableItems.AddCell(cellNotas);

                                // Lista de notas para solo agregar las que tienen contenido
                                List<string> notas = new List<string>();

                                if (!string.IsNullOrWhiteSpace(_presupuestoPrint.nota1)) notas.Add(_presupuestoPrint.nota1);
                                if (!string.IsNullOrWhiteSpace(_presupuestoPrint.nota2)) notas.Add(_presupuestoPrint.nota2);
                                if (!string.IsNullOrWhiteSpace(_presupuestoPrint.nota3)) notas.Add(_presupuestoPrint.nota3);

                                // Si hay notas, se agregan debajo del encabezado "NOTAS"
                                if (notas.Count > 0)
                                {
                                    foreach (string nota in notas)
                                    {
                                        PdfPCell cellNota = new PdfPCell(new Phrase(nota, FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                                        {
                                            Colspan = tableItems.NumberOfColumns, // Ocupar todo el ancho
                                            Border = PdfPCell.BOX, // Mantener bordes
                                            Padding = 5f,
                                            MinimumHeight = 20f, // Altura mínima para evitar celdas muy pequeñas
                                            UseAscender = true // Asegura mejor alineación del texto
                                        };

                                        tableItems.AddCell(cellNota);
                                    }
                                }


                                // 🔹 Agregar la tabla de productos al documento PDF
                                pdfDoc.Add(tableItems);

                                // 🔹 5. Cerrar el documento
                                pdfDoc.Close();
                            }

                            MessageBox.Show($"PDF generado con éxito en:\n{outputPath}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al generar el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //labelError.Text = "No se pudo Crear el Usuario.";
                }
                finally
                {
                    
                }
            }
        }

        private string obtenerMaterial(int material_id)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();
            string nombre = "";

            string sql = "select material.nombre from material where material.id='" + material_id + "'";

            try
            {
                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                if (rdr.Read())
                {
                    nombre = rdr[0].ToString();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("No se logró realizar la búsqueda, error: " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }

            return nombre;
        }

        private string obtenerSistema(int sistema_id)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();
            string nombre = "";

            string sql = "select sistema.nombre from sistema where sistema.id='" + sistema_id + "'";

            try
            {
                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                if (rdr.Read())
                {
                    nombre = rdr[0].ToString();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("No se logró realizar la búsqueda, error: " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }

            return nombre;
        }
    }
}
