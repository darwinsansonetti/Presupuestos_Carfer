using Aluminum.Conexion;
using Aluminum.Helpers;
using Aluminum.Model;
using Aluminum.View;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aluminum
{
    public partial class FormLogin : Form
    {
        private Thread hilo;
        public UsuarioModel user = new UsuarioModel();

        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            textBoxPass.PasswordChar = '●';
            this.textBoxUser.Select();

            if (Properties.Settings.Default.UsuarioRecordado != "")
            {
                textBoxUser.Text = Properties.Settings.Default.UsuarioRecordado;
                this.textBoxPass.Select();
            }

            user = new UsuarioModel();
        }

        // Cerrar la aplicación
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            if (textBoxUser.Text == "")
            {
                labelError.Text = "Ingrese nombre de Usuario.";
            }
            else
            {
                if (textBoxPass.Text == "")
                {
                    labelError.Text = "Ingrese su contraseña.";
                }
                else
                {
                    labelError.Text = "";

                    try
                    {

                        string sql = "select * from master where master.username='" + textBoxUser.Text + "' and master.password='" + textBoxPass.Text + "'";

                        HelperQuery _helperQuery = new HelperQuery();
                        MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                        //Se busca el usuario en la tabla Master, si existe se le muestra el formulario administrador
                        if (rdr.Read())
                        {
                            // Cerrar hilos abiertos antes de iniciar uno nuevo
                            CerrarHilos();

                            hilo = new Thread(new ThreadStart(IniciarAplicacionAdmin));
                            hilo.SetApartmentState(ApartmentState.STA);
                            hilo.Start();
                            base.Close();
                        }
                        else
                        {
                            _conn.Close();

                            sql = "SELECT u.*, e.razon_social AS nombre_empresa, e.path_logo AS path_logo_empresa FROM user u " +
                                    "INNER JOIN empresa e ON u.empresa_id = e.id " +
                                    "where u.username='" + textBoxUser.Text + "' and u.password='" + textBoxPass.Text + "'";

                            _helperQuery = new HelperQuery();
                            rdr = _helperQuery.querySelect(_conn, sql);

                            if (rdr.Read())
                            {
                                user = new UsuarioModel();
                                user.id = int.Parse(rdr[0].ToString());
                                user.nombre = rdr[1].ToString();
                                user.telefono = rdr[2].ToString();
                                user.documento = rdr[3].ToString();
                                user.username = rdr[4].ToString();
                                user.password = rdr[5].ToString();
                                user.email = rdr[6].ToString();
                                user.rol_id = int.Parse(rdr[7].ToString());
                                user.empresa_id = int.Parse(rdr[8].ToString());
                                user.razon_social = rdr[11].ToString();
                                user.path_logo = (byte[])rdr[12];

                                if (checkBoxRecordar.Checked == true)
                                {
                                    Properties.Settings.Default.UsuarioRecordado = textBoxUser.Text;
                                    Properties.Settings.Default.Save();
                                }

                                int id = int.Parse(rdr[0].ToString());
                                var _username = rdr[1].ToString();
                                var _pass = rdr[2].ToString();

                                // Cerrar hilos abiertos antes de iniciar uno nuevo
                                CerrarHilos();

                                hilo = new Thread(new ThreadStart(IniciarAplicacionMain));
                                hilo.SetApartmentState(ApartmentState.STA);
                                hilo.Start();
                                base.Close();
                            }
                            else
                            {
                                labelError.Text = "No existe usuario con esas credenciales.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No existe el usuario.");
                    }
                    finally
                    {
                        _conn.Close();
                    }
                }
            }
        }

        private void CerrarHilos()
        {
            foreach (ProcessThread t in Process.GetCurrentProcess().Threads)
            {
                try
                {
                    if (t.Id != Thread.CurrentThread.ManagedThreadId) // No cerramos el hilo actual
                    {
                        t.Dispose(); // Intenta liberar el hilo
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se pudo cerrar el hilo: " + ex.Message);
                }
            }
        }

        private void IniciarAplicacionMain()
        {
            try
            {
                Globals.home = new HomeMain(user);
                Globals.home.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error iniciando Aplicacion");
            }
        }

        private void IniciarAplicacionAdmin()
        {
            try
            {
                Globals.HomeInicio = new Index();
                Globals.HomeInicio.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error iniciando Aplicacion");
            }
        }
    }
}
