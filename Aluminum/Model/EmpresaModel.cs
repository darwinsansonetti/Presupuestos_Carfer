using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Model
{
    public class EmpresaModel
    {
        public int id { get; set; }
        public string razon_social { get; set; }
        public string documento { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public byte[] path_logo { get; set; } // Este campo corresponde al tipo BLOB en la base de datos
    }
}
