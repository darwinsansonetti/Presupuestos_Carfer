using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Model
{
    public class ProductosPresupuestoImprimeModel
    {
        //Registros del Producto
        public int cantidad { get; set; }//1
        public double costo { get; set; }//2
        public int producto_id { get; set; }//3
        public int sistema_id { get; set; }//5
        public int material_id { get; set; }//6
        public double widht { get; set; }//7
        public double height { get; set; }//8

        //Productos en el Presupuesto id = 11
        public string nombre_producto { get; set; }//12
        public string descripcion_producto { get; set; }//13
        public byte[] path_logo { get; set; } //14
        public double costo_metro { get; set; }//15

    }
}
