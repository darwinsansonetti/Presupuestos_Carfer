using Aluminum.Conexion;
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
    public partial class FormEditarUsuario : Form
    {
        private Index _formPadre;

        UsuarioModel _OneUsuario = new UsuarioModel();

        public FormEditarUsuario(UsuarioModel OneUsuario, Index formPadre)
        {
            InitializeComponent();
            panelBanner.BackColor = ColorTranslator.FromHtml("#7746FF");

            _formPadre = formPadre; // Referencia al formulario padre

            _OneUsuario = new UsuarioModel();
            _OneUsuario = OneUsuario;

            textBoxPass.PasswordChar = '●';
        }

        private void FormEditarUsuario_Load(object sender, EventArgs e)
        {
            textBoxNombreUsuario.Text = _OneUsuario.nombre;
            textBoxDocumento.Text = _OneUsuario.documento;
            textBoxTelefono.Text = _OneUsuario.telefono;
            textBoxEmail.Text = _OneUsuario.email;
            textBoxUsername.Text = _OneUsuario.username;
            textBoxPass.Text = _OneUsuario.password;
            labelNombreEmpresa.Text = _OneUsuario.razon_social;

            byte[] imagenRecuperada = _OneUsuario.path_logo as byte[];

            using (MemoryStream ms = new MemoryStream(imagenRecuperada))
            {
                pbImagen.Image = Image.FromStream(ms);
            }
        }

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new FormListaUsuarios(_formPadre));
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

                                                string query = "UPDATE user SET nombre = @nombre, telefono = @telefono, password = @password  WHERE id = @id";

                                                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                                                {
                                                    cmd.Parameters.AddWithValue("@nombre", textBoxNombreUsuario.Text);
                                                    cmd.Parameters.AddWithValue("@telefono", textBoxTelefono.Text);
                                                    cmd.Parameters.AddWithValue("@password", textBoxPass.Text);
                                                    cmd.Parameters.AddWithValue("@id", _OneUsuario.id);

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
                                            labelError.Text = "No se pudo actualizar la informacion del Usuario.";
                                        }
                                        finally
                                        {
                                            //Se inserto con exito el registro y se va a actualizar la lista
                                            if (labelError.Text == "")
                                            {
                                                _formPadre.AbrirFormulario(new FormListaUsuarios(_formPadre));
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
