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
    public partial class FormCreateProducto : Form
    {

        private HomeMain _formPadre;
        int _empresa_id = 0;
        public string parame { get; set; } = "";
        public List<CategoriaModel> _categorias { get; set; }
        private byte[] imagenBytes;

        public FormCreateProducto(HomeMain formPadre, int empresa_id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;
            _categorias = new List<CategoriaModel>();
        }

        private void FormCreateProducto_Load(object sender, EventArgs e)
        {
            CargarCategorias();
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
                            if (imagenBytes == null)
                            {
                                labelError.Text = "Seleccione la Imagen del Producto.";
                            }
                            else
                            {
                                labelError.Text = "";


                                DialogResult dialogResult = MessageBox.Show("¿Desea GUARDAR el nuevo Producto?", "Confirmación", MessageBoxButtons.YesNo);
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
                                            string query = "INSERT INTO producto (nombre, descripcion, costo_metro, empresa_id, categoria_id, path_logo) " +
                                                "VALUES (@nombre, @descripcion, @costo_metro, @empresa_id, @categoria_id, @imagen); " +
                                                "SELECT LAST_INSERT_ID(); ";

                                            using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                                            {
                                                cmd.Parameters.AddWithValue("@nombre", textBoxNombreProducto.Text);
                                                cmd.Parameters.AddWithValue("@descripcion", textBoxDescripcion.Text);
                                                cmd.Parameters.AddWithValue("@costo_metro", textBoxCosto.Text);
                                                cmd.Parameters.AddWithValue("@empresa_id", _empresa_id);
                                                cmd.Parameters.AddWithValue("@categoria_id", Convert.ToInt32(comboBoxCategoria.SelectedValue));
                                                cmd.Parameters.AddWithValue("@imagen", imagenBytes);

                                                conexion.Open();
                                                newId = Convert.ToInt32(cmd.ExecuteScalar());
                                                conexion.Close();
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
                                            MessageBox.Show("Producto creado exitosamente.", "Notificacion");
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

        private void buttonCargarImg_Click(object sender, EventArgs e)
        {
            labelError.Text = "";

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imágenes|*.jpg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pbImagen.Image = Image.FromFile(ofd.FileName);
                    imagenBytes = File.ReadAllBytes(ofd.FileName);
                }
            }
        }
    }
}
