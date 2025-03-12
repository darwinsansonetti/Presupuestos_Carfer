using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Model
{
    public class PresupuestoImprimeMode
    {
        //Presupuesto
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public double monto_total { get; set; }
        public int cliente_id { get; set; }
        public int empresa_id { get; set; }
        public string nota1 { get; set; }
        public string nota2 { get; set; }
        public string nota3 { get; set; }

        //Cliente
        public string nombre_cliente { get; set; }
        public string documento_cliente { get; set; }
        public string telefono_cliente { get; set; }

        //Empresa
        public string razon_social { get; set; }
        public string documento_empresa { get; set; }
        public string direccion { get; set; }
        public string telefono_empresa { get; set; }
        public string email { get; set; }
        public byte[] path_logo { get; set; }
    }
}
