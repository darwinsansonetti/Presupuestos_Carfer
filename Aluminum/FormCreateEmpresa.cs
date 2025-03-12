using Aluminum.Conexion;
using Aluminum.Helpers;
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
    public partial class FormCreateEmpresa : Form
    {
        private byte[] imagenBytes;

        private Index _formPadre;

        public FormCreateEmpresa(Index formPadre)
        {
            InitializeComponent();
            panelBanner.BackColor = ColorTranslator.FromHtml("#7746FF");

            _formPadre = formPadre; // Referencia al formulario padre
        }

        private void FormCreateEmpresa_Load(object sender, EventArgs e)
        {
            this.textBoxNombreEmpresa.Select();
        }

        private void buttonCargarImg_Click(object sender, EventArgs e)
        {
            labelError.Text = "";

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imágenes|*.jpg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pbImagen.Image = Image.FromFile(ofd.FileName);
                    imagenBytes = File.ReadAllBytes(ofd.FileName);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            int newId = 0;

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
                                if (imagenBytes == null)
                                {
                                    labelError.Text = "Seleccione una Imagen para la Empresa.";
                                }
                                else
                                {
                                    labelError.Text = "";


                                    DialogResult dialogResult = MessageBox.Show("¿Desea GUARDAR la nueva empresa?", "Confirmación", MessageBoxButtons.YesNo);
                                    if (dialogResult == DialogResult.Yes)
                                    {

                                        try
                                        {
                                            string sql = "select * from empresa where empresa.documento='" + textBoxDocumento.Text + "'";

                                            HelperQuery _helperQuery = new HelperQuery();
                                            MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                                            if (rdr.Read())
                                            {
                                                labelError.Text = "El Nro de Documento existe en la BD.";

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
                                                    string query = "INSERT INTO empresa (razon_social, documento, direccion, telefono, email, path_logo) " +
                                                        "VALUES (@razon_social, @documento, @direccion, @telefono, @email, @imagen); " +
                                                        "SELECT LAST_INSERT_ID(); ";

                                                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                                                    {
                                                        cmd.Parameters.AddWithValue("@razon_social", textBoxNombreEmpresa.Text);
                                                        cmd.Parameters.AddWithValue("@documento", textBoxDocumento.Text);
                                                        cmd.Parameters.AddWithValue("@direccion", textBoxDireccion.Text);
                                                        cmd.Parameters.AddWithValue("@telefono", textBoxTelefono.Text);
                                                        cmd.Parameters.AddWithValue("@email", textBoxEmail.Text);
                                                        cmd.Parameters.AddWithValue("@imagen", imagenBytes);

                                                        conexion.Open();
                                                        newId = Convert.ToInt32(cmd.ExecuteScalar());
                                                        conexion.Close();
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            labelError.Text = "No se pudo Crear la Empresa.";
                                        }
                                        finally
                                        {
                                            //Se inserto con exito el registro y se va a actualizar la lista
                                            if (labelError.Text == "")
                                            {
                                                DialogResult dialogResult1 = MessageBox.Show("Empresa creada exitosamente. ¿Desea crear usuarios para esa empresa?", "Confirmación", MessageBoxButtons.YesNo);
                                                if (dialogResult1 == DialogResult.Yes)
                                                {
                                                    _formPadre.AbrirFormulario(new FormUsuario(_formPadre, newId));
                                                }
                                                else
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

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            _formPadre.AbrirFormulario(new HomeAdmin(_formPadre));
        }

        private void textBoxDocumento_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxTelefono_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxDireccion_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pbImagen_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void labelError_Click(object sender, EventArgs e)
        {

        }
    }
}
