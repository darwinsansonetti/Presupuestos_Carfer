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

namespace Aluminum.View
{
    public partial class FormProductoListMain : Form
    {

        private HomeMain _formPadre;
        int _empresa_id = 0;
        public string parame { get; set; } = "";
        public List<ProductoModel> _productos { get; set; }

        public FormProductoListMain(HomeMain formPadre, int empresa_id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;
            _productos = new List<ProductoModel>();
        }

        private void FormProductoListMain_Load(object sender, EventArgs e)
        {
            Filtrar(parame);
        }

        private void Filtrar(string _parame)
        {
            textBoxProducto.Text = "";

            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            listViewProductosNew.Items.Clear();

            string sql = "";

            //Se buscan todos los productos
            if (string.IsNullOrEmpty(_parame))
            {
                sql = "SELECT u.*, e.nombre AS nombre_categoria FROM producto u " +
                                    "INNER JOIN categoria e ON u.categoria_id = e.id " +
                                    "where u.empresa_id='" + _empresa_id + "' order by u.id ASC ";
            }
            else
            {
                sql = "SELECT u.*, e.nombre AS nombre_categoria FROM producto u " +
                                    "INNER JOIN categoria e ON u.categoria_id = e.id " +
                                    "where u.empresa_id='" + _empresa_id + "' and u.nombre Like '%" + _parame + "%' order by u.id ASC ";
            }

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _productos = new List<ProductoModel>();

                while (rdr.Read())
                {
                    ListViewItem lvi = new ListViewItem(rdr[0].ToString());
                    lvi.SubItems.Add(rdr[1].ToString());
                    //lvi.SubItems.Add(rdr[7].ToString());

                    if (int.Parse(rdr[7].ToString()) == 1)
                    {
                        lvi.SubItems.Add("S");
                    }
                    else
                    {
                        lvi.SubItems.Add("N");
                    }

                    ProductoModel _producto = new ProductoModel();
                    _producto.id = int.Parse(rdr[0].ToString());
                    _producto.nombre = rdr[1].ToString();
                    _producto.descripcion = rdr[2].ToString();
                    _producto.path_logo = (byte[])rdr[3];
                    _producto.costo_metro = Convert.ToDouble(rdr[4].ToString());
                    _producto.empresa_id = int.Parse(rdr[5].ToString());
                    _producto.categoria_id = int.Parse(rdr[6].ToString());
                    _producto.activo = int.Parse(rdr[7].ToString());

                    _productos.Add(_producto);

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

        private void buttonCrearProducto_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new FormCreateProducto(_formPadre, _empresa_id));
        }

        private void listViewProductosNew_DoubleClick(object sender, EventArgs e)
        {
            //Se obtiene el ID del producto seleccionado
            int producto_id = int.Parse(listViewProductosNew.SelectedItems[0].SubItems[0].Text);

            ProductoModel OneProducto = _productos.FirstOrDefault(m => m.id == producto_id);

            // Llamar al método de FormPadre para abrir el FormHijo2
            _formPadre.AbrirFormulario(new FormEditarProducto(OneProducto, _formPadre));
        }
    }
}
