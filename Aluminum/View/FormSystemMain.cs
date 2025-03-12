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
    public partial class FormSystemMain : Form
    {

        private HomeMain _formPadre;
        int _empresa_id = 0;
        public string parame { get; set; } = "";

        public FormSystemMain(HomeMain formPadre, int empresa_id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;
        }

        private void FormSystemMain_Load(object sender, EventArgs e)
        {
            Filtrar(parame);
        }

        private void Filtrar(string _parame)
        {
            textBoxNewSystem.Text = "";

            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            listViewProductosNew.Items.Clear();

            string sql = "";

            //Se buscan todos los productos
            if (string.IsNullOrEmpty(_parame))
            {
                sql = "select * from sistema where sistema.empresa_id='" + _empresa_id + "' order by sistema.id ASC ";
            }
            else
            {
                sql = "select * from sistema where sistema.empresa_id='" + _empresa_id + "' and sistema.nombre Like '%" + _parame + "%' order by sistema.id ASC ";
            }

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

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

        private void btnAgregarSystem_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            if (textBoxNewSystem.Text != "")
            {
                try
                {
                    string sql = "select * from sistema where sistema.nombre='" + textBoxNewSystem.Text + "' and sistema.empresa_id='" + _empresa_id + "'";

                    HelperQuery _helperQuery = new HelperQuery();
                    MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                    if (rdr.Read())
                    {
                        MessageBox.Show("El tipo de sistema ya existe en la BD.");

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
                            string query = "INSERT INTO sistema (nombre, empresa_id) " +
                                "VALUES (@nombre, @empresa_id)";

                            using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                            {
                                cmd.Parameters.AddWithValue("@nombre", textBoxNewSystem.Text);
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

        private void listViewProductosNew_DoubleClick(object sender, EventArgs e)
        {
            //Se obtiene el ID de la empresa seleccionada
            int system_id = int.Parse(listViewProductosNew.SelectedItems[0].SubItems[0].Text);
            string activo = listViewProductosNew.SelectedItems[0].SubItems[2].Text;
            string _message = "";

            if (activo == "S")
            {
                _message = "¿Esta seguro de DESACTIVAR este Tipo de Sistema?";
            }
            else
            {
                _message = "¿Desea ACTIVAR este Tipo de Sistema?";
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

                    using (MySqlConnection conexion = new MySqlConnection(conexionString))
                    {
                        string query = "UPDATE sistema SET activo = @activo  WHERE id = @id";
                        int activar = 1;

                        if (activo == "S")
                        {
                            activar = 0;
                        }

                        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@activo", activar);
                            cmd.Parameters.AddWithValue("@id", system_id);

                            conexion.Open();
                            cmd.ExecuteNonQuery();
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
