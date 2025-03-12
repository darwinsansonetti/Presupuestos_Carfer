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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aluminum
{
    public partial class FormEditarEmpresa : Form
    {
        private byte[] imagenBytes;
        private Index _formPadre;
        private bool flag = false;

        EmpresaModel _OneEmpresa = new EmpresaModel();

        public FormEditarEmpresa(EmpresaModel OneEmpresa, Index formPadre)
        {
            InitializeComponent();
            panelBanner.BackColor = ColorTranslator.FromHtml("#7746FF");

            _formPadre = formPadre; // Referencia al formulario padre

            _OneEmpresa = new EmpresaModel();
            _OneEmpresa = OneEmpresa;
        }

        private void FormEditarEmpresa_Load(object sender, EventArgs e)
        {
            textBoxNombreEmpresa.Text = _OneEmpresa.razon_social;
            textBoxDocumento.Text = _OneEmpresa.documento;
            textBoxTelefono.Text = _OneEmpresa.telefono;
            textBoxEmail.Text = _OneEmpresa.email;
            textBoxDireccion.Text = _OneEmpresa.direccion;

            byte[] imagenRecuperada = _OneEmpresa.path_logo as byte[];

            using (MemoryStream ms = new MemoryStream(imagenRecuperada))
            {
                pbImagen.Image = Image.FromStream(ms);
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
                    flag = true;

                    pbImagen.Image = Image.FromFile(ofd.FileName);
                    imagenBytes = File.ReadAllBytes(ofd.FileName);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            if (textBoxNombreEmpresa.Text == "")
            {
                labelError.Text = "Ingrese nombre de la Empresa.";
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
                            if (textBoxDireccion.Text == "")
                            {
                                labelError.Text = "Ingrese la Direccion.";
                            }
                            else
                            {
                                if (imagenBytes == null && flag == true)
                                {
                                    labelError.Text = "Seleccione una Imagen para la Empresa.";
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
                                                string query = "";

                                                if (imagenBytes == null)
                                                {
                                                    query = "UPDATE empresa SET razon_social = @razon_social, documento = @documento, direccion = @direccion," +
                                                    " telefono = @telefono, email = @email WHERE id = @id";
                                                }
                                                else
                                                {
                                                    query = "UPDATE empresa SET razon_social = @razon_social, documento = @documento, direccion = @direccion," +
                                                    " telefono = @telefono, email = @email, path_logo = @imagen WHERE id = @id";
                                                }
                                                
                                                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                                                {
                                                    cmd.Parameters.AddWithValue("@razon_social", textBoxNombreEmpresa.Text);
                                                    cmd.Parameters.AddWithValue("@documento", textBoxDocumento.Text);
                                                    cmd.Parameters.AddWithValue("@direccion", textBoxDireccion.Text);
                                                    cmd.Parameters.AddWithValue("@telefono", textBoxTelefono.Text);
                                                    cmd.Parameters.AddWithValue("@email", textBoxEmail.Text);
                                                    cmd.Parameters.AddWithValue("@imagen", imagenBytes);
                                                    cmd.Parameters.AddWithValue("@id", _OneEmpresa.id);

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
                                            labelError.Text = "No se pudo Crear la Empresa.";
                                        }
                                        finally
                                        {

                                            //Se actualizo con exito el registro y se va a actualizar la lista
                                            if (labelError.Text == "")
                                            {
                                                //IFormLista formInterface = this.Owner as IFormLista;

                                                //if (formInterface != null)
                                                //    formInterface.UpdateEmpresas();

                                                //this.Close();

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

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new HomeAdmin(_formPadre));
        }

        private void buttonCreateUser_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new FormUsuario(_formPadre, _OneEmpresa.id));
        }
    }
}
