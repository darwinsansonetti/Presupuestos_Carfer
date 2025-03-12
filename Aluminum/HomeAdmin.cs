using Aluminum.Conexion;
using Aluminum.Helpers;
using Aluminum.Model;
using Aluminum.View;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aluminum
{
    public partial class HomeAdmin : Form, IFormLista
    {
        public string parame { get; set; } = "";
        public List<EmpresaModel> _empresas { get; set; }

        private Index _formPadre;

        public HomeAdmin(Index formPadre)
        {
            InitializeComponent();

            _empresas = new List<EmpresaModel>();
            _formPadre = formPadre; // Referencia al formulario padre
        }

        private void HomeAdmin_Load(object sender, EventArgs e)
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
                sql = "select * from empresa order by empresa.razon_social ASC ";
            }
            else
            {
                sql = "select * from empresa where empresa.razon_social Like '%" + _parame + "%' order by empresa.razon_social ASC ";
            }

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _empresas = new List<EmpresaModel>();

                while (rdr.Read())
                {
                    ListViewItem lvi = new ListViewItem(rdr[1].ToString());
                    lvi.SubItems.Add(rdr[2].ToString());
                    lvi.SubItems.Add(rdr[4].ToString());
                    lvi.SubItems.Add(rdr[5].ToString());
                    lvi.SubItems.Add(rdr[0].ToString());

                    EmpresaModel _empresa = new EmpresaModel();
                    _empresa.id = int.Parse(rdr[0].ToString());
                    _empresa.razon_social = rdr[1].ToString();
                    _empresa.documento = rdr[2].ToString();
                    _empresa.direccion = rdr[3].ToString();
                    _empresa.telefono = rdr[4].ToString();
                    _empresa.email = rdr[5].ToString();
                    _empresa.path_logo = (byte[])rdr[6];

                    _empresas.Add(_empresa);

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

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Desea cerrar la sesion?", "Confirmación", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Crear una instancia del formulario de Login
                FormLogin loginForm = new FormLogin();

                // Iniciar el formulario de login en un nuevo hilo
                Thread hilo = new Thread(new ThreadStart(() =>
                {
                    Application.Run(loginForm);
                }));

                hilo.SetApartmentState(ApartmentState.STA);
                hilo.Start();

                // Cerrar el formulario actual de manera segura
                this.Invoke(new Action(() =>
                {
                    this.Close();
                }));

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Filtrar(textBoxProducto.Text);
        }

        private void buttonCrearEmpresa_Click(object sender, EventArgs e)
        {
            // Llamar al método de FormPadre para abrir el FormHijo2
            _formPadre.AbrirFormulario(new FormCreateEmpresa(_formPadre));
        }

        public void UpdateEmpresas()
        {
            Filtrar("");
        }

        private void listViewProductosNew_DoubleClick(object sender, EventArgs e)
        {
            //Se obtiene el ID de la empresa seleccionada
            int empresa_id = int.Parse(listViewProductosNew.SelectedItems[0].SubItems[4].Text);

            EmpresaModel OneEmpresa = _empresas.FirstOrDefault(m => m.id == empresa_id);

            //FormEditarEmpresa a = new FormEditarEmpresa(OneEmpresa);
            //a.ShowDialog(this);



            // Llamar al método de FormPadre para abrir el FormHijo2
            _formPadre.AbrirFormulario(new FormEditarEmpresa(OneEmpresa, _formPadre));
        }
    }
}
