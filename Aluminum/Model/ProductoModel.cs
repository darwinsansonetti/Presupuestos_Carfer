using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Model
{
    public class ProductoModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public byte[] path_logo { get; set; } // Este campo corresponde al tipo BLOB en la base de datos
        public double costo_metro { get; set; }
        public int empresa_id { get; set; }
        public int categoria_id { get; set; }
        public int activo { get; set; }
    }
}
