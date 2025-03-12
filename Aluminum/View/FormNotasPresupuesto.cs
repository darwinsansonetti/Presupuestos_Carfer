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
    public partial class FormNotasPresupuesto : Form
    {
        private HomeMain _formPadre;
        int presupuesto_id = 0;
        int _empresa_id = 0;

        public FormNotasPresupuesto(HomeMain formPadre, int _presupuesto_id, int empresa_id)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            presupuesto_id = _presupuesto_id;
            _empresa_id = empresa_id;
        }

        private void FormNotasPresupuesto_Load(object sender, EventArgs e)
        {
            label1.Text += presupuesto_id.ToString();
        }

        private void buttonVolver_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("¿Desea guardar las notas para el presupuesto?", "Confirmación", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.Yes)
            {
                try
                {
                    string servidor = "localhost";
                    string bd = "aluminum";
                    string usuario = "root";
                    string password = "";
                    string puerto = "3306";

                    string conexionString = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + password + ";" + "database=" + bd + ";";

                    int filasAfectadas = 0;
                    using (MySqlConnection conexion = new MySqlConnection(conexionString))
                    {
                        string query = "UPDATE presupuesto SET nota1 = @nota1, nota2 = @nota2, nota3 = @nota2  WHERE id = @id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@nota1", textBoxNota1.Text);
                            cmd.Parameters.AddWithValue("@nota2", textBoxNota2.Text);
                            cmd.Parameters.AddWithValue("@nota3", textBoxNota3.Text);
                            cmd.Parameters.AddWithValue("@id", presupuesto_id);

                            conexion.Open();
                            filasAfectadas = cmd.ExecuteNonQuery();
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
                    _formPadre.AbrirFormulario(new FormReportes(_formPadre, _empresa_id));
                }
            }
        }
    }
}
