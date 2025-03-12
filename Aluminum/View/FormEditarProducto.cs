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
    public partial class FormEditarProducto : Form
    {
        private byte[] imagenBytes;
        private HomeMain _formPadre;
        private bool flag = false;
        public List<CategoriaModel> _categorias { get; set; }

        int _empresa_id = 0;

        ProductoModel _OneProducto = new ProductoModel();

        public FormEditarProducto(ProductoModel OneProducto, HomeMain formPadre)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre

            _OneProducto = new ProductoModel();
            _OneProducto = OneProducto;

            _categorias = new List<CategoriaModel>();
        }

        private void FormEditarProducto_Load(object sender, EventArgs e)
        {
            textBoxNombreProducto.Text = _OneProducto.nombre;
            textBoxDescripcion.Text = _OneProducto.descripcion;
            textBoxCosto.Text = _OneProducto.costo_metro.ToString();
            _empresa_id = _OneProducto.empresa_id;

            CargarCategorias();
            comboBoxCategoria.SelectedValue = _OneProducto.categoria_id;

            byte[] imagenRecuperada = _OneProducto.path_logo as byte[];

            using (MemoryStream ms = new MemoryStream(imagenRecuperada))
            {
                pbImagen.Image = Image.FromStream(ms);
            }
        }

        private void CargarCategorias()
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            string sql = "SELECT * from categoria where categoria.empresa_id='" + _empresa_id + "' and categoria.activa = 1";

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _categorias = new List<CategoriaModel>();

                while (rdr.Read())
                {
                    CategoriaModel _categoria = new CategoriaModel();
                    _categoria.id = int.Parse(rdr[0].ToString());
                    _categoria.nombre = rdr[1].ToString();
                    _categoria.empresa_id = int.Parse(rdr[2].ToString());
                    _categoria.activa = int.Parse(rdr[3].ToString());

                    _categorias.Add(_categoria);
                }

                comboBoxCategoria.DataSource = _categorias;
                comboBoxCategoria.DisplayMember = "Nombre"; // Lo que se mostrará
                comboBoxCategoria.ValueMember = "id";         // El valor asociado
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

        private void buttonCargarImg_Click(object sender, EventArgs e)
        {
            labelError.Text = "";

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imágenes|*.jpg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    flag = true;

                    pbImagen.Image = Image.FromFile(ofd.FileName);
                    imagenBytes = File.ReadAllBytes(ofd.FileName);
                }
            }
        }

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new FormProductoListMain(_formPadre, _empresa_id));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            int newId = 0;

            if (textBoxNombreProducto.Text == "")
            {
                labelError.Text = "Ingrese nombre del Producto.";
            }
            else
            {
                if (textBoxDescripcion.Text == "")
                {
                    labelError.Text = "Ingrese la Descripcion del Producto.";
                }
                else
                {
                    if (textBoxCosto.Text == "")
                    {
                        labelError.Text = "Ingrese el costo por metro lineal del producto.";
                    }
                    else
                    {
                        if (comboBoxCategoria.SelectedValue == null)
                        {
                            labelError.Text = "Seleccione una Categoria.";
                        }
                        else
                        {
                            if (imagenBytes == null && flag == true)
                            {
                                labelError.Text = "Seleccione una Imagen para el producto.";
                            }
                            else
                            {
                                labelError.Text = "";


                                DialogResult dialogResult = MessageBox.Show("¿Desea GUARDAR los cambios?", "Confirmación", MessageBoxButtons.YesNo);
                                if (dialogResult == DialogResult.Yes)
                                {

                                    try
                                    {
                                        string servidor = "localhost";
                                        string bd = "aluminum";
                                        string usuario = "root";
                                        string password = "";
                                        string puerto = "3306";

                                        string conexionString = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + password + ";" + "database=" + bd + ";";

                                        using (MySqlConnection conexion = new MySqlConnection(conexionString))
                                        {
                                            string query = "";

                                            if (imagenBytes == null)
                                            {
                                                query = "UPDATE producto SET nombre = @nombre, descripcion = @descripcion, costo_metro = @costo_metro WHERE id = @id";
                                            }
                                            else
                                            {
                                                query = "UPDATE producto SET nombre = @nombre, descripcion = @descripcion, costo_metro = @costo_metro, path_logo = @imagen WHERE id = @id";
                                            }

                                            using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                                            {
                                                cmd.Parameters.AddWithValue("@nombre", textBoxNombreProducto.Text);
                                                cmd.Parameters.AddWithValue("@descripcion", textBoxDescripcion.Text);
                                                cmd.Parameters.AddWithValue("@costo_metro", textBoxCosto.Text);
                                                cmd.Parameters.AddWithValue("@imagen", imagenBytes);
                                                cmd.Parameters.AddWithValue("@id", _OneProducto.id);

                                                conexion.Open();
                                                int filasAfectadas = cmd.ExecuteNonQuery();
                                                conexion.Close();

                                                if (filasAfectadas <= 0)
                                                    labelError.Text = "No se encontró un registro con ese ID.";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        labelError.Text = "No se pudo Crear el Producto.";
                                    }
                                    finally
                                    {
                                        //Se inserto con exito el registro y se va a actualizar la lista
                                        if (labelError.Text == "")
                                        {
                                            _formPadre.AbrirFormulario(new FormProductoListMain(_formPadre, _empresa_id));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
