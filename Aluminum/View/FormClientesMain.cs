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
    public partial class FormClientesMain : Form
    {

        private HomeMain _formPadre;
        int _empresa_id = 0;
        public string parame { get; set; } = "";

        public FormClientesMain(HomeMain formPadre, int empresa_id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;
        }

        private void FormClientesMain_Load(object sender, EventArgs e)
        {
            Filtrar(parame);
        }

        private void Filtrar(string _parame)
        {
            textBoxNewNombre.Text = "";
            textBoxNewDocumento.Text = "";
            textBoxNewTelefono.Text = "";

            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            listViewProductosNew.Items.Clear();

            string sql = "";

            //Se buscan todos los productos
            if (string.IsNullOrEmpty(_parame))
            {
                sql = "select * from cliente where cliente.empresa_id='" + _empresa_id + "' AND cliente.id <> 1 order by cliente.id ASC ";
            }
            else
            {
                sql = "select * from cliente where cliente.empresa_id='" + _empresa_id + "' and cliente.nombre Like '%" + _parame + "%' AND cliente.id <> 1 order by cliente.id ASC ";
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
                    lvi.SubItems.Add(rdr[3].ToString());
                    lvi.SubItems.Add(rdr[2].ToString());

                    if (int.Parse(rdr[5].ToString()) == 1)
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

        private void button2_Click(object sender, EventArgs e)
        {
            Filtrar(textBoxProducto.Text);
        }

        private void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            if (textBoxNewNombre.Text == "")
            {
                labelError.Text = "Ingrese nombre del cliente.";
            }
            else
            {
                if (textBoxNewDocumento.Text == "")
                {
                    labelError.Text = "Ingrese el Nro de Documento.";
                }
                else
                {
                    if (textBoxNewTelefono.Text == "")
                    {
                        labelError.Text = "Ingrese el Nro de Telefono.";
                    }
                    else
                    {
                        labelError.Text = "";


                        DialogResult dialogResult = MessageBox.Show("¿Desea GUARDAR el nuevo cliente?", "Confirmación", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {

                            try
                            {
                                string sql = "select * from cliente where cliente.documento='" + textBoxNewDocumento.Text + "' and cliente.empresa_id='" + _empresa_id + "'";

                                HelperQuery _helperQuery = new HelperQuery();
                                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                                if (rdr.Read())
                                {
                                    labelError.Text = "El cliente existe en la empresa.";

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
                                        string query = "INSERT INTO cliente (nombre, telefono, documento, empresa_id) " +
                                            "VALUES (@nombre, @telefono, @documento, @empresa_id); ";

                                        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                                        {
                                            cmd.Parameters.AddWithValue("@nombre", textBoxNewNombre.Text);
                                            cmd.Parameters.AddWithValue("@telefono", textBoxNewTelefono.Text);
                                            cmd.Parameters.AddWithValue("@documento", textBoxNewDocumento.Text);
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
                                labelError.Text = "No se pudo Crear el Cliente.";
                            }
                            finally
                            {
                                //Se inserto con exito el registro y se va a actualizar la lista
                                if (labelError.Text == "")
                                {
                                    Filtrar("");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void listViewProductosNew_DoubleClick(object sender, EventArgs e)
        {
            //Se obtiene el ID de la empresa seleccionada
            int cliente_id = int.Parse(listViewProductosNew.SelectedItems[0].SubItems[0].Text);
            string activo = listViewProductosNew.SelectedItems[0].SubItems[4].Text;
            string _message = "";

            if (activo == "S")
            {
                _message = "¿Esta seguro de DESACTIVAR este Cliente?";
            }
            else
            {
                _message = "¿Desea ACTIVAR nuevamente a este Cliente?";
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
                        string query = "UPDATE cliente SET activo = @activa  WHERE id = @id";
                        int activar = 1;

                        if (activo == "S")
                        {
                            activar = 0;
                        }

                        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@activa", activar);
                            cmd.Parameters.AddWithValue("@id", cliente_id);

                            conexion.Open();
                            filasAfectadas = cmd.ExecuteNonQuery();
                            conexion.Close();
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
