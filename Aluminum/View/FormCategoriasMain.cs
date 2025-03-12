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
    public partial class FormCategoriasMain : Form
    {

        private HomeMain _formPadre;
        int _empresa_id = 0;
        public string parame { get; set; } = "";

        public FormCategoriasMain(HomeMain formPadre, int empresa_id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;
        }

        private void FormCategoriasMain_Load(object sender, EventArgs e)
        {
            Filtrar(parame);
        }

        private void Filtrar(string _parame)
        {
            textBoxNewCategoria.Text = "";

            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            listViewProductosNew.Items.Clear();

            string sql = "";

            //Se buscan todos los productos
            if (string.IsNullOrEmpty(_parame))
            {
                sql = "select * from categoria where categoria.empresa_id='" + _empresa_id + "' order by categoria.id ASC ";
            }
            else 
            {
                sql = "select * from categoria where categoria.empresa_id='" + _empresa_id + "' and categoria.nombre Like '%" + _parame + "%' order by categoria.id ASC ";
            }

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                //_empresas = new List<EmpresaModel>();

                while (rdr.Read())
                {
                    ListViewItem lvi = new ListViewItem(rdr[0].ToString());
                    lvi.SubItems.Add(rdr[1].ToString());

                    if (int.Parse(rdr[3].ToString()) == 1)
                    {
                        lvi.SubItems.Add("S");
                    }
                    else
                    {
                        lvi.SubItems.Add("N");
                    }

                    //EmpresaModel _empresa = new EmpresaModel();
                    //_empresa.id = int.Parse(rdr[0].ToString());
                    //_empresa.razon_social = rdr[1].ToString();
                    //_empresa.documento = rdr[2].ToString();
                    //_empresa.direccion = rdr[3].ToString();
                    //_empresa.telefono = rdr[4].ToString();
                    //_empresa.email = rdr[5].ToString();
                    //_empresa.path_logo = (byte[])rdr[6];

                    //_empresas.Add(_empresa);

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

        private void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            if (textBoxNewCategoria.Text != "")
            {
                try
                {
                    string sql = "select * from categoria where categoria.nombre='" + textBoxNewCategoria.Text + "' and categoria.empresa_id='" + _empresa_id + "'";

                    HelperQuery _helperQuery = new HelperQuery();
                    MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                    if (rdr.Read())
                    {
                        MessageBox.Show("Existe una Categoria con ese nombre.");

                        _conn.Close();
                    }
                    else
                    {
                        _conn.Close();

                        string servidor = "localhost";
                        string bd = "aluminum";
                        string usuario = "root";
                        string password = "";
                        string puerto = "3306";

                        string conexionString = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + password + ";" + "database=" + bd + ";";

                        using (MySqlConnection conexion = new MySqlConnection(conexionString))
                        {
                            string query = "INSERT INTO categoria (nombre, empresa_id) " +
                                "VALUES (@nombre, @empresa_id)";

                            using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                            {
                                cmd.Parameters.AddWithValue("@nombre", textBoxNewCategoria.Text);
                                cmd.Parameters.AddWithValue("@empresa_id", _empresa_id);

                                conexion.Open();
                                cmd.ExecuteNonQuery();
                                conexion.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //labelError.Text = "No se pudo Crear el Usuario.";
                }
                finally
                {
                    Filtrar("");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Filtrar(textBoxProducto.Text);
        }

        private void listViewProductosNew_DoubleClick(object sender, EventArgs e)
        {
            //Se obtiene el ID de la empresa seleccionada
            int categoria_id = int.Parse(listViewProductosNew.SelectedItems[0].SubItems[0].Text);
            string activo = listViewProductosNew.SelectedItems[0].SubItems[2].Text;
            string _message = "";

            if (activo == "S")
            {
                _message = "¿Esta seguro de DESACTIVAR esta categoria? Los PRODUCTOS de la categoria tambien seran desactivados.";
            }
            else
            {
                _message = "¿Desea ACTIVAR esta categoria?";
            }

            DialogResult dialogResult = MessageBox.Show(_message, "Confirmación", MessageBoxButtons.YesNo);
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

                    int filasAfectadas = 0;
                    using (MySqlConnection conexion = new MySqlConnection(conexionString))
                    {
                        string query = "UPDATE categoria SET activa = @activa  WHERE id = @id";
                        int activar = 1;

                        if (activo == "S")
                        {
                            activar = 0;
                        }

                        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@activa", activar);
                            cmd.Parameters.AddWithValue("@id", categoria_id);

                            conexion.Open();
                            filasAfectadas = cmd.ExecuteNonQuery();
                            conexion.Close();
                        }
                    }

                    if (filasAfectadas <= 0)
                    {
                        MessageBox.Show("No se encontró un registro con ese ID.");
                    }
                    else
                    {
                        using (MySqlConnection conexion1 = new MySqlConnection(conexionString))
                        {
                            string query1 = "UPDATE producto SET activo = @activo WHERE categoria_id = @id";
                            int activar = 1;

                            if (activo == "S")
                            {
                                activar = 0;
                            }

                            using (MySqlCommand cmd1 = new MySqlCommand(query1, conexion1))
                            {
                                cmd1.Parameters.AddWithValue("@activo", activar);
                                cmd1.Parameters.AddWithValue("@id", categoria_id);

                                conexion1.Open();
                                cmd1.ExecuteNonQuery();
                                conexion1.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //labelError.Text = "No se pudo actualizar la informacion del Usuario.";
                }
                finally
                {
                    Filtrar("");
                }
            }
        }
    }
}
