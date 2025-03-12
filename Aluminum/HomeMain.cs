using Aluminum.Model;
using Aluminum.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aluminum
{
    public partial class HomeMain : Form
    {
        UsuarioModel _OneUsuario = new UsuarioModel();

        public HomeMain(UsuarioModel user)
        {
            InitializeComponent();

            _OneUsuario = new UsuarioModel();
            _OneUsuario = user;
        }

        private void HomeMain_Load(object sender, EventArgs e)
        {
            this.Text += "Usuario : " + _OneUsuario.nombre;

            byte[] imagenRecuperada = _OneUsuario.path_logo as byte[];

            using (MemoryStream ms = new MemoryStream(imagenRecuperada))
            {
                pictureBoxLogo.Image = Image.FromStream(ms);
            }

            AbrirFormulario(new FormProductosMain(this, _OneUsuario.empresa_id, new List<int>()));
        }

        public void AbrirFormulario(object formhijo)
        {
            if (this.panelContendero.Controls.Count > 0)
                this.panelContendero.Controls.RemoveAt(0);

            Form fh = formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panelContendero.Controls.Add(fh);
            this.panelContendero.Tag = fh;
            fh.Show();
        }

        private void buttonCerrarSesion_Click(object sender, EventArgs e)
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

        private void buttonProductos_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormProductosMain(this, _OneUsuario.empresa_id, new List<int>()));
        }

        private void buttonUsuarios_Click(object sender, EventArgs e)
        {
            buttonProductos_Click(null, null);
        }

        private void buttonPresupuestos_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormReportes(this, _OneUsuario.empresa_id));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buttonPresupuestos_Click(null, null);
        }

        private void buttonConfiguracion_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormProductoListMain(this, _OneUsuario.empresa_id));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            buttonConfiguracion_Click(null, null);
        }

        private void buttonCategorias_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormCategoriasMain(this, _OneUsuario.empresa_id));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            buttonCategorias_Click(null, null);
        }

        private void buttonClientes_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormClientesMain(this, _OneUsuario.empresa_id));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            buttonClientes_Click(null, null);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            buttonCategorias_Click(null, null);
        }

        private void buttonMateriales_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormMaterialesMain(this, _OneUsuario.empresa_id));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            buttonMateriales_Click(null, null);
        }

        private void buttonSistema_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormSystemMain(this, _OneUsuario.empresa_id));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            buttonSistema_Click(null, null);
        }

        private void HomeMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Mostrar un mensaje de confirmación
            DialogResult result = MessageBox.Show("¿Esta seguro que desea salir del sistema?", "Confirmación",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Si el usuario elige "No", cancelar el cierre de la aplicación
            if (result == DialogResult.No)
            {
                e.Cancel = true; // Cancela el cierre del formulario
            }
        }
    }
}
