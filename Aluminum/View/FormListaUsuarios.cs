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
    public partial class FormListaUsuarios : Form
    {
        private Index _formPadre;
        public string parame { get; set; } = "";
        public List<UsuarioModel> _usuarios { get; set; }

        public FormListaUsuarios(Index formPadre)
        {
            InitializeComponent();

            _usuarios = new List<UsuarioModel>();
            _formPadre = formPadre; // Referencia al formulario padre
        }

        private void FormListaUsuarios_Load(object sender, EventArgs e)
        {
            panelBanner.BackColor = ColorTranslator.FromHtml("#7746FF");

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
                sql = "SELECT u.*, e.razon_social AS nombre_empresa, e.path_logo AS path_logo_empresa FROM user u " +
                    "JOIN empresa e ON u.empresa_id = e.id " +
                    "ORDER BY u.nombre ASC";
            }
            else
            {
                sql = "SELECT u.*, e.razon_social AS nombre_empresa, e.path_logo AS path_logo_empresa FROM user u " +
                    "INNER JOIN empresa e ON u.empresa_id = e.id " +
                    "WHERE u.nombre LIKE '%" + _parame + "%' " +
                    "ORDER BY u.nombre ASC";
            }

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _usuarios = new List<UsuarioModel>();

                while (rdr.Read())
                {
                    ListViewItem lvi = new ListViewItem(rdr[1].ToString()); //Nombre
                    lvi.SubItems.Add(rdr[3].ToString()); //Documento Identidad
                    lvi.SubItems.Add(rdr[2].ToString()); //Telefonp
                    lvi.SubItems.Add(rdr[6].ToString()); //Email
                    lvi.SubItems.Add(rdr[11].ToString()); //Nombre Empresa
                    lvi.SubItems.Add(rdr[0].ToString()); //ID

                    UsuarioModel _usuario = new UsuarioModel();
                    _usuario.id = int.Parse(rdr[0].ToString());
                    _usuario.nombre = rdr[1].ToString();
                    _usuario.telefono = rdr[2].ToString();
                    _usuario.documento = rdr[3].ToString();
                    _usuario.username = rdr[4].ToString();
                    _usuario.password = rdr[5].ToString();
                    _usuario.email = rdr[6].ToString();
                    _usuario.rol_id = int.Parse(rdr[7].ToString());
                    _usuario.empresa_id = int.Parse(rdr[8].ToString());
                    _usuario.razon_social = rdr[11].ToString();
                    _usuario.path_logo = (byte[])rdr[12];

                    _usuarios.Add(_usuario);

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

        private void button2_Click(object sender, EventArgs e)
        {
            Filtrar(textBoxProducto.Text);
        }

        private void listViewProductosNew_DoubleClick(object sender, EventArgs e)
        {
            //Se obtiene el ID de la empresa seleccionada
            int user_id = int.Parse(listViewProductosNew.SelectedItems[0].SubItems[5].Text);

            UsuarioModel OneUser = _usuarios.FirstOrDefault(m => m.id == user_id);

            // Llamar al método de FormPadre para abrir el FormHijo2
            _formPadre.AbrirFormulario(new FormEditarUsuario(OneUser, _formPadre));
        }
    }
}
