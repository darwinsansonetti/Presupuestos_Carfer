using Aluminum.Conexion;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Helpers
{
    public class HelperQuery
    {
        public MySqlDataReader querySelect(MySqlConnection _conn,string sql)
        {
            _conn.Open();

            MySqlCommand cmd = new MySqlCommand(sql, _conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            return rdr;
        }
    }
}
