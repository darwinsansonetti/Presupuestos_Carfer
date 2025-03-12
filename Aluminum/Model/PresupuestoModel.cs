using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Model
{
    public class PresupuestoModel
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public double monto_total { get; set; }
        public int cliente_id { get; set; }
        public int empresa_id { get; set; }
        public string nota1 { get; set; }
        public string nota2 { get; set; }
        public string nota3 { get; set; }
        public string nota4 { get; set; }
        public string nota5 { get; set; }
    }
}
