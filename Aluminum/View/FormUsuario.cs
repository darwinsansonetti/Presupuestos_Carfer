using Aluminum.Conexion;
using Aluminum.Helpers;
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
    public partial class FormUsuario : Form
    {
        private Index _formPadre;
        private int _id = 0;
        public FormUsuario(Index formPadre, int id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _id = id;
        }

        private void FormUsuario_Load(object sender, EventArgs e)
        {
            textBoxPass.PasswordChar = '●';
            textBoxPassRepetir.PasswordChar = '●';
            this.textBoxNombreUsuario.Select();

            //textBoxNombreUsuario.TabIndex = 0;
            //textBoxDocumento.TabIndex = 1;
            //textBoxTelefono.TabIndex = 2;
            //textBoxEmail.TabIndex = 3;
            //textBoxNombreUsuario.TabIndex = 4;
            //textBoxPass.TabIndex = 5;
            //textBoxPassRepetir.TabIndex = 6;
            //button1.TabIndex = 7;
        }

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new HomeAdmin(_formPadre));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            if (textBoxNombreUsuario.Text == "")
            {
                labelError.Text = "Ingrese nombre del Usuario.";
            }
            else
            {
                if (textBoxDocumento.Text == "")
                {
                    labelError.Text = "Ingrese el Nro de Documento.";
                }
                else
                {
                    if (textBoxTelefono.Text == "")
                    {
                        labelError.Text = "Ingrese el Nro de Telefono.";
                    }
                    else
                    {
                        if (textBoxEmail.Text == "")
                        {
                            labelError.Text = "Ingrese el Correo Electronico.";
                        }
                        else
                        {
                            if (textBoxUsername.Text == "")
                            {
                                labelError.Text = "Ingrese el nombre de usuario.";
                            }
                            else
                            {
                                if (textBoxPass.Text == "")
                                {
                                    labelError.Text = "Ingrese la contraseña.";
                                }
                                else
                                {
                                    if (textBoxPassRepetir.Text == "")
                                    {
                                        labelError.Text = "Repita la contraseña.";
                                    }
                                    else
                                    {
                                        if (textBoxPassRepetir.Text != textBoxPass.Text)
                                        {
                                            labelError.Text = "Las contraseña no coinciden.";
                                        }
                                        else
                                        {
                                            labelError.Text = "";


                                            DialogResult dialogResult = MessageBox.Show("¿Desea GUARDAR el nuevo usuario?", "Confirmación", MessageBoxButtons.YesNo);
                                            if (dialogResult == DialogResult.Yes)
                                            {

                                                try
                                                {
                                                    string sql = "select * from user where user.documento='" + textBoxDocumento.Text + "' and user.empresa_id='" + _id + "'"; 

                                                    HelperQuery _helperQuery = new HelperQuery();
                                                    MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                                                    if (rdr.Read())
                                                    {
                                                        labelError.Text = "El documento existe en la empresa."; 

                                                        _conn.Close();
                                                    }
                                                    else
                                                    {
                                                        _conn.Close();

                                                        _conexion = new CConexion();
                                                        _conn = _conexion.establecerConexion();


                                                        string sql1 = "select * from user where user.username='" + textBoxUsername.Text + "'";

                                                        HelperQuery _helperQuery1 = new HelperQuery();
                                                        MySqlDataReader rdr1 = _helperQuery.querySelect(_conn, sql1);

                                                        if (rdr1.Read())
                                                        {
                                                            labelError.Text = "Nombre de usuario en uso.";

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
                                                                string query = "INSERT INTO user (nombre, telefono, documento, username, password, email, rol_id, empresa_id) " +
                                                                    "VALUES (@nombre, @telefono, @documento, @username, @password, @email, @rol_id, @empresa_id); ";

                                                                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                                                                {
                                                                    cmd.Parameters.AddWithValue("@nombre", textBoxNombreUsuario.Text);
                                                                    cmd.Parameters.AddWithValue("@telefono", textBoxTelefono.Text);
                                                                    cmd.Parameters.AddWithValue("@documento", textBoxDocumento.Text);
                                                                    cmd.Parameters.AddWithValue("@username", textBoxUsername.Text);
                                                                    cmd.Parameters.AddWithValue("@password", textBoxPass.Text);
                                                                    cmd.Parameters.AddWithValue("@email", textBoxEmail.Text);
                                                                    cmd.Parameters.AddWithValue("@rol_id", 1);
                                                                    cmd.Parameters.AddWithValue("@empresa_id", _id);

                                                                    conexion.Open();
                                                                    cmd.ExecuteNonQuery();
                                                                    conexion.Close();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    labelError.Text = "No se pudo Crear el Usuario.";
                                                }
                                                finally
                                                {
                                                    //Se inserto con exito el registro y se va a actualizar la lista
                                                    if (labelError.Text == "")
                                                    {
                                                        _formPadre.AbrirFormulario(new HomeAdmin(_formPadre));
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
        }
    }
}
