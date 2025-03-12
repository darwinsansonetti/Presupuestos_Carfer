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

namespace Aluminum.View
{
    public partial class Index : Form
    {
        public Index()
        {
            InitializeComponent();
        }

        private void Index_Load(object sender, EventArgs e)
        {
            AbrirFormulario(new HomeAdmin(this));
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

        private void button1_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new HomeAdmin(this));
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

        private void buttonUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormListaUsuarios(this));
        }
    }
}
