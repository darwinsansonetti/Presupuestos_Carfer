using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Model
{
    public class ProductosPresupuestoModel
    {
        public int id { get; set; }
        public int cantidad { get; set; }
        public double costo { get; set; }
        public int producto_id { get; set; }
        public int sistema_id { get; set; }
        public int material_id { get; set; }
        public double widht { get; set; }
        public double height { get; set; }
    }
}
