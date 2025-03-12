using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aluminum.Conexion
{
    public class CConexion
    {
        MySqlConnection conex = new MySqlConnection();

        static string servidor = "localhost";
        static string bd = "aluminum";
        static string usuario = "root";
        static string password = "";
        static string puerto = "3306";

        string cadenaConexion = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + password + ";" + "database=" + bd + ";";

        public MySqlConnection establecerConexion()
        {

            try
            {
                conex.ConnectionString = cadenaConexion;
            }

            catch (MySqlException e)
            {
                MessageBox.Show("No se logró conectar a la base de datos, error: " + e.ToString());
            }

            return conex;

        }
    }
}
