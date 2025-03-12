using Aluminum.Conexion;
using Aluminum.Helpers;
using Aluminum.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aluminum.View
{
    public partial class FormStep2 : Form
    {

        private HomeMain _formPadre;
        public List<CategoriaModel> _categorias { get; set; }
        int _empresa_id = 0;
        public List<ProductoModel> _productos { get; set; }
        public List<int> _productosSeleccioandos { get; set; }
        private List<SistemaModel> _system { get; set; }
        private List<MaterialModel> _materiales { get; set; }
        private int ClienteID = 0;
        private PresupuestoModel _presupuesto { get; set; }
        private List<ProductosPresupuestoModel> _productospresupuesto { get; set; }


        public FormStep2(HomeMain formPadre, int empresa_id, List<int> productosSeleccionados, List<ProductoModel> productos, 
        List<CategoriaModel> categoria, List<SistemaModel> system, List<MaterialModel> materiales)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;
            _categorias = new List<CategoriaModel>();
            _categorias = _categorias;
            _productos = new List<ProductoModel>();
            _productos = productos;
            _productosSeleccioandos = new List<int>();
            _productosSeleccioandos = productosSeleccionados;

            _system = new List<SistemaModel>();
            _system = system;

            _materiales = new List<MaterialModel>();
            _materiales = materiales;

            _presupuesto = new PresupuestoModel();
            _productospresupuesto = new List<ProductosPresupuestoModel>();
        }

        private void FormStep2_Load(object sender, EventArgs e)
        {
            CargarPanelproductos();
        }

        private void CargarPanelproductos()
        {
            int yOffset = 10; // Margen superior inicial
            int index = 1; // Contador de iteración

            foreach (int valor in _productosSeleccioandos)
            {
                ProductoModel _producto = _productos.Where(p => p.id == valor).FirstOrDefault();

                Panel panelHijo = new Panel
                {
                    Height = 103,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle, // Agrega borde para mejor visualización
                    Location = new Point(0, yOffset),
                    Width = panelContendorPaneles.ClientSize.Width, // Ajusta el ancho dinámicamente
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right // Se ajusta al tamaño del padre
                };

                byte[] imagenRecuperada = _producto.path_logo as byte[];
                PictureBox pictureBox = new PictureBox();

                using (MemoryStream ms = new MemoryStream(imagenRecuperada))
                {
                    pictureBox.Image = Image.FromStream(ms);
                    pictureBox.Size = new Size(80, 80); // Tamaño del PictureBox
                    pictureBox.Location = new Point(10, 10); // Posición dentro del panel hijo
                    pictureBox.BackColor = Color.LightGray; // Color de fondo
                    pictureBox.BorderStyle = BorderStyle.FixedSingle; // Borde para visualización
                    pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left; // Anclaje
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage; // Ajustar imagen si se agrega
                }

                // Crear Label al lado superior derecho del PictureBox
                Label labelPos = new Label
                {
                    Text = $"Pos {index} : {_producto.nombre}", // Muestra "Pos X"
                    Location = new Point(pictureBox.Right + 5, pictureBox.Top), // A la derecha y arriba del PictureBox
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = Color.Black
                };

                // Crear Label debajo de labelPos con el texto "System : "
                Label labelSystem = new Label
                {
                    Text = "System :  ",
                    Location = new Point(pictureBox.Right + 5, labelPos.Bottom + 5), // A la derecha del PictureBox, debajo de labelPos
                    AutoSize = true,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    ForeColor = Color.Black
                };

                // Crear ComboBox debajo de labelSystem
                ComboBox comboBoxSystem = new ComboBox
                {
                    Name = $"comboBoxSystem_{index}", // Nombre único para cada ComboBox
                    Location = new Point(labelSystem.Right, labelSystem.Top), // Ubicado a la derecha de labelSystem, con un pequeño espacio
                    Width = 220,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Cargar el ComboBox con la lista de categorías sin compartir el DataSource entre los ComboBoxes
                comboBoxSystem.DataSource = new List<SistemaModel>(_system); // Crear una nueva lista para evitar referencias compartidas
                comboBoxSystem.DisplayMember = "Nombre";
                comboBoxSystem.ValueMember = "Id";

                // NUEVO Label "Width :"
                Label labelWidth = new Label
                {
                    Text = "Width :   ",
                    Location = new Point(comboBoxSystem.Right + 25, labelSystem.Top),
                    AutoSize = true,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    ForeColor = Color.Black
                };

                // NUEVO TextBox para ingresar el ancho (único por panel)
                TextBox textBoxWidth = new TextBox
                {
                    Name = $"textBoxWidth_{index}", // Nombre único para cada TextBox
                    Location = new Point(labelWidth.Right, labelSystem.Top),
                    Width = 80
                };

                // Evento KeyPress para permitir solo números y un solo punto decimal
                textBoxWidth.KeyPress += (sender, e) =>
                {
                    // Permitir números (0-9), un solo punto decimal (.) y la tecla Backspace
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                    {
                        e.Handled = true; // Bloquear caracteres inválidos
                    }

                    // Evitar múltiples puntos decimales
                    if (e.KeyChar == '.' && textBoxWidth.Text.Contains("."))
                    {
                        e.Handled = true; // Bloquear si ya hay un punto decimal
                    }
                };

                // Evento TextChanged para prevenir pegado de texto no válido
                textBoxWidth.TextChanged += (sender, e) =>
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(textBoxWidth.Text, "[^0-9.]"))
                    {
                        textBoxWidth.Text = System.Text.RegularExpressions.Regex.Replace(textBoxWidth.Text, "[^0-9.]", "");
                        textBoxWidth.SelectionStart = textBoxWidth.Text.Length; // Mantener el cursor al final
                    }
                };


                // Crear el nuevo Label "Material :"
                Label labelMaterial = new Label
                {
                    Text = "Material : ",
                    Location = new Point(pictureBox.Right + 5, labelSystem.Bottom + 10), // Posicionamos debajo de labelSystem
                    AutoSize = true,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    ForeColor = Color.Black
                };

                // Crear ComboBox **al lado** de labelMaterial
                ComboBox comboBoxMaterial = new ComboBox
                {
                    Name = $"comboBoxMaterial_{index}", // Nombre único para cada ComboBox
                    Location = new Point(labelMaterial.Right + 2, labelMaterial.Top), // Ubicado a la derecha de labelMaterial
                    Width = 220,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Cargar el ComboBox con la lista de materiales
                comboBoxMaterial.DataSource = new List<MaterialModel>(_materiales); // Crear una nueva lista para evitar referencias compartidas
                comboBoxMaterial.DisplayMember = "Nombre";
                comboBoxMaterial.ValueMember = "Id";

                // NUEVO Label "Height :"
                Label labelHeight = new Label
                {
                    Text = "Height : ",
                    Location = new Point(comboBoxMaterial.Right + 20, labelMaterial.Top),
                    AutoSize = true,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    ForeColor = Color.Black
                };

                // NUEVO TextBoxHeight alineado con textBoxWidth
                TextBox textBoxHeight = new TextBox
                {
                    Name = $"textBoxHeight_{index}", // Nombre único para cada TextBox
                    Location = new Point(labelHeight.Right + 5, labelMaterial.Top),
                    Width = 80
                };

                // Evento KeyPress para permitir solo números y un solo punto decimal
                textBoxHeight.KeyPress += (sender, e) =>
                {
                    // Permitir números (0-9), un solo punto decimal (.) y la tecla Backspace
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                    {
                        e.Handled = true; // Bloquear caracteres inválidos
                    }

                    // Evitar múltiples puntos decimales
                    if (e.KeyChar == '.' && textBoxHeight.Text.Contains("."))
                    {
                        e.Handled = true; // Bloquear si ya hay un punto decimal
                    }
                };

                // Evento TextChanged para prevenir pegado de texto no válido
                textBoxHeight.TextChanged += (sender, e) =>
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(textBoxHeight.Text, "[^0-9.]"))
                    {
                        textBoxHeight.Text = System.Text.RegularExpressions.Regex.Replace(textBoxHeight.Text, "[^0-9.]", "");
                        textBoxHeight.SelectionStart = textBoxHeight.Text.Length; // Mantener el cursor al final
                    }
                };


                // NUEVO Label "Cantidad :"
                Label labelCantidad = new Label
                {
                    Text = "Units : ",
                    Location = new Point(textBoxWidth.Right + 20, labelSystem.Top), // Separación de 20px
                    AutoSize = true,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    ForeColor = Color.Black
                };

                // NUEVO TextBoxCantidad alineado con textBoxWidth y textBoxHeight
                TextBox textBoxCantidad = new TextBox
                {
                    Name = $"textBoxCantidad_{index}", // Nombre único para cada TextBox
                    Location = new Point(labelCantidad.Right + 5, labelSystem.Top),
                    Width = 80
                };

                // Evento KeyPress para permitir solo números
                textBoxCantidad.KeyPress += (sender, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    {
                        e.Handled = true; // Bloquea la entrada si no es número
                    }
                };

                // Evento TextChanged para prevenir pegado de texto no numérico
                textBoxCantidad.TextChanged += (sender, e) =>
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(textBoxCantidad.Text, "[^0-9]"))
                    {
                        textBoxCantidad.Text = System.Text.RegularExpressions.Regex.Replace(textBoxCantidad.Text, "[^0-9]", "");
                        textBoxCantidad.SelectionStart = textBoxCantidad.Text.Length; // Mantener el cursor al final
                    }
                };


                // NUEVO Label "Total :"
                Label labelTotal = new Label
                {
                    Text = "Amount : ",
                    Location = new Point(textBoxWidth.Right + 20, labelMaterial.Top),
                    AutoSize = true,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    ForeColor = Color.Black
                };

                // NUEVO TextBoxTotal alineado con textBoxWidth y textBoxHeight
                TextBox textBoxTotal = new TextBox
                {
                    Name = $"textBoxTotal_{index}", // Nombre único para cada TextBox
                    Location = new Point(labelCantidad.Right + 5, labelMaterial.Top),
                    Width = 80
                };

                // Evento KeyPress para permitir solo números y un solo punto decimal
                textBoxTotal.KeyPress += (sender, e) =>
                {
                    // Permitir números (0-9), un solo punto decimal (.) y la tecla Backspace
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                    {
                        e.Handled = true; // Bloquear caracteres inválidos
                    }

                    // Evitar múltiples puntos decimales
                    if (e.KeyChar == '.' && textBoxTotal.Text.Contains("."))
                    {
                        e.Handled = true; // Bloquear si ya hay un punto decimal
                    }
                };

                // Evento TextChanged para prevenir pegado de texto no válido
                textBoxTotal.TextChanged += (sender, e) =>
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(textBoxTotal.Text, "[^0-9.]"))
                    {
                        textBoxTotal.Text = System.Text.RegularExpressions.Regex.Replace(textBoxTotal.Text, "[^0-9.]", "");
                        textBoxTotal.SelectionStart = textBoxTotal.Text.Length; // Mantener el cursor al final
                    }
                };

                // NUEVO TextBox oculto en cada PanelHijo con un valor asignado
                TextBox textBoxHidden = new TextBox
                {
                    Name = $"textBoxHidden_{index}", // Nombre único basado en la iteración
                    Location = new Point(10, 10),
                    Width = 100,
                    Visible = false,
                    Text = valor.ToString() // Asigna un valor único basado en la iteración
                };

                // Agregar controles al panel hijo
                panelHijo.Controls.Add(pictureBox);
                panelHijo.Controls.Add(labelPos);
                panelHijo.Controls.Add(labelSystem);
                panelHijo.Controls.Add(comboBoxSystem);
                panelHijo.Controls.Add(labelWidth);
                panelHijo.Controls.Add(textBoxWidth);
                panelHijo.Controls.Add(labelCantidad);
                panelHijo.Controls.Add(textBoxCantidad);
                panelHijo.Controls.Add(labelMaterial);
                panelHijo.Controls.Add(comboBoxMaterial);
                panelHijo.Controls.Add(labelHeight);
                panelHijo.Controls.Add(textBoxHeight);
                panelHijo.Controls.Add(labelTotal);
                panelHijo.Controls.Add(textBoxTotal);
                panelHijo.Controls.Add(textBoxHidden);

                panelContendorPaneles.Controls.Add(panelHijo);

                yOffset += 103 + 10; // Altura del panel + separación
                index++; // Incrementar el contador de posición
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new FormProductosMain(_formPadre, _empresa_id, _productosSeleccioandos));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxProducto.Text != "")
            {
                textBoxNewNombre.Text = "";
                textBoxNewDocumento.Text = "";
                textBoxNewTelefono.Text = "";
                textBoxIdCliente.Text = "";

                CConexion _conexion = new CConexion();
                MySqlConnection _conn = _conexion.establecerConexion();

                string sql = "select * from cliente where cliente.empresa_id='" + _empresa_id + "' and cliente.documento= '" + textBoxProducto.Text + "'";

                try
                {
                    HelperQuery _helperQuery = new HelperQuery();
                    MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                    if (rdr.Read())
                    {
                        //Si el cliente esta activo
                        if (int.Parse(rdr[5].ToString()) == 1)
                        {
                            ClienteID = int.Parse(rdr[0].ToString());

                            textBoxIdCliente.Text = rdr[0].ToString();
                            textBoxNewNombre.Text = rdr[1].ToString();
                            textBoxNewDocumento.Text = rdr[3].ToString();
                            textBoxNewTelefono.Text = rdr[2].ToString();

                            textBoxProducto.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("El cliente se encuentra DESACTIVADO");
                        }
                    }
                    else
                    {
                        MessageBox.Show("El cliente no se encuentra Registrado.");
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
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxIdCliente.Text = "";
            textBoxNewNombre.Text = "";
            textBoxNewDocumento.Text = "";
            textBoxNewTelefono.Text = "";

            textBoxProducto.Text = "";

            if (checkBox1.Checked)
            {
                ClienteID = 0;

                textBoxIdCliente.Text = "1";
                textBoxNewNombre.Text = "";
                textBoxNewDocumento.Text = "";
                textBoxNewTelefono.Text = "";
            }
        }

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            //Si el presupuesto es personalizado
            if (checkBox1.Checked == false && textBoxNewNombre.Text == "")
            {
                MessageBox.Show("Si desea crear un presupuesto no personalizado, seleccione la opcion 'A quien pueda interesar'");
            }
            else
            {
                if (checkBox1.Checked == true)
                {
                    CConexion _conexion = new CConexion();
                    MySqlConnection _conn = _conexion.establecerConexion();

                    string sql = "select * from cliente where cliente.id = 1";

                    try
                    {
                        HelperQuery _helperQuery = new HelperQuery();
                        MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                        if (rdr.Read())
                        {
                            ClienteID = int.Parse(rdr[0].ToString());

                            ObtenerDataPresupuesto();
                        }
                        else
                        {
                            MessageBox.Show("El cliente no se encuentra Registrado.");
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
                else
                {
                    ObtenerDataPresupuesto();
                }
            }
        }

        private void ObtenerDataPresupuesto()
        {
            int index = 1; // Inicializamos el índice
            bool flag = false;
            double total = 0;
            int newId = 0;

            _presupuesto = new PresupuestoModel();
            _productospresupuesto = new List<ProductosPresupuestoModel>();

            foreach (Panel panelHijo in panelContendorPaneles.Controls.OfType<Panel>())
            {
                // Accedemos a los controles usando el índice único de cada panel
                ComboBox comboBoxSystem = panelHijo.Controls.Find($"comboBoxSystem_{index}", true).FirstOrDefault() as ComboBox;
                TextBox textBoxWidth = panelHijo.Controls.Find($"textBoxWidth_{index}", true).FirstOrDefault() as TextBox;
                ComboBox comboBoxMaterial = panelHijo.Controls.Find($"comboBoxMaterial_{index}", true).FirstOrDefault() as ComboBox;
                TextBox textBoxHeight = panelHijo.Controls.Find($"textBoxHeight_{index}", true).FirstOrDefault() as TextBox;
                TextBox textBoxCantidad = panelHijo.Controls.Find($"textBoxCantidad_{index}", true).FirstOrDefault() as TextBox;
                TextBox textBoxTotal = panelHijo.Controls.Find($"textBoxTotal_{index}", true).FirstOrDefault() as TextBox;
                TextBox textBoxHidden = panelHijo.Controls.Find($"textBoxHidden_{index}", true).FirstOrDefault() as TextBox;
                 
                if (textBoxWidth?.Text == "" || textBoxHeight?.Text == "" || textBoxCantidad?.Text == "" || textBoxTotal?.Text == "")
                {
                    flag = true;
                    break;
                }

                index++; // Incrementar el índice para el siguiente panel

                //Calcula el total general para el presupuesto
                total += double.Parse(textBoxTotal?.Text);

                //Agregamos los productos en una lista para luego ser insertados en la tabla PresupuestoProducto
                ProductosPresupuestoModel OneProducto = new ProductosPresupuestoModel();
                OneProducto.cantidad = int.Parse(textBoxCantidad?.Text);
                OneProducto.costo = double.Parse(textBoxTotal?.Text);
                OneProducto.producto_id = int.Parse(textBoxHidden?.Text);
                OneProducto.sistema_id = (int)comboBoxSystem.SelectedValue;
                OneProducto.material_id = (int)comboBoxMaterial.SelectedValue; OneProducto.costo = double.Parse(textBoxTotal?.Text);
                OneProducto.widht = double.Parse(textBoxWidth?.Text);
                OneProducto.height = double.Parse(textBoxHeight?.Text);

                _productospresupuesto.Add(OneProducto);
            }

            if (flag == true)
            {
                MessageBox.Show("Falta informacion en algun producto a presupuestar.");
            }
            else
            {
                _presupuesto.fecha = DateTime.Now;
                _presupuesto.monto_total = total;
                _presupuesto.cliente_id = ClienteID;
                _presupuesto.empresa_id = _empresa_id;

                DialogResult dialogResult = MessageBox.Show("¿Desea guardar el presupuesto?", "Confirmación", MessageBoxButtons.YesNo);
                
                //Si responde que NO entonces de una vez se guarda el presupuesto y se crea el PDF
                if (dialogResult == DialogResult.Yes)
                {
                    string servidor = "localhost";
                    string bd = "aluminum";
                    string usuario = "root";
                    string password = "";
                    string puerto = "3306";

                    string conexionString = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + password + ";" + "database=" + bd + ";";

                    using (MySqlConnection conexion = new MySqlConnection(conexionString))
                    {
                        string query = "INSERT INTO presupuesto (fecha, monto_total, cliente_id, empresa_id) " +
                            "VALUES (@fecha, @monto_total, @cliente_id, @empresa_id); " +
                            "SELECT LAST_INSERT_ID(); ";

                        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@fecha", _presupuesto.fecha);
                            cmd.Parameters.AddWithValue("@monto_total", _presupuesto.monto_total);
                            cmd.Parameters.AddWithValue("@cliente_id", _presupuesto.cliente_id);
                            cmd.Parameters.AddWithValue("@empresa_id", _presupuesto.empresa_id);

                            conexion.Open();
                            newId = Convert.ToInt32(cmd.ExecuteScalar());
                            conexion.Close();
                        }

                        if (newId > 0)
                        {
                            using (MySqlConnection conexion1 = new MySqlConnection(conexionString))
                            {
                                conexion1.Open();
                                foreach (ProductosPresupuestoModel OneProductoPresupuesto in _productospresupuesto)
                                {
                                    query = "INSERT INTO presupuestoproducto (cantidad, costo, producto_id, presupuesto_id, sistema_id, material_id, widht, height) VALUES " +
                                        "                         (@cantidad, @costo, @producto_id, @presupuesto_id, @sistema_id, @material_id, @widht, @height)";

                                    using (MySqlCommand cmd1 = new MySqlCommand(query, conexion1))
                                    {
                                        cmd1.Parameters.AddWithValue("@cantidad", OneProductoPresupuesto.cantidad);
                                        cmd1.Parameters.AddWithValue("@costo", OneProductoPresupuesto.costo);
                                        cmd1.Parameters.AddWithValue("@producto_id", OneProductoPresupuesto.producto_id);
                                        cmd1.Parameters.AddWithValue("@presupuesto_id", newId);
                                        cmd1.Parameters.AddWithValue("@sistema_id", OneProductoPresupuesto.sistema_id);
                                        cmd1.Parameters.AddWithValue("@material_id", OneProductoPresupuesto.material_id);
                                        cmd1.Parameters.AddWithValue("@widht", OneProductoPresupuesto.widht);
                                        cmd1.Parameters.AddWithValue("@height", OneProductoPresupuesto.height);
                                        cmd1.ExecuteNonQuery();
                                    }
                                }
                                conexion1.Close();
                            }

                            DialogResult dialogResult1 = MessageBox.Show($"Presupuesto creado con el Nro {newId}. ¿Desea agregarle notas al presupuesto?", "Confirmación", MessageBoxButtons.YesNo);

                            //Si responde que NO entonces de una vez se guarda el presupuesto y se crea el PDF
                            if (dialogResult1 == DialogResult.Yes)
                            {
                                _formPadre.AbrirFormulario(new FormNotasPresupuesto(_formPadre, newId, _empresa_id));
                            }
                            else
                            {
                                _formPadre.AbrirFormulario(new FormReportes(_formPadre, _empresa_id));
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error creando el presupuesto.");
                        }
                    }
                }
            }
        }
    }
}

