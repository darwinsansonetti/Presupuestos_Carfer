﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Model
{
    public class CategoriaModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int empresa_id { get; set; }
        public int activa { get; set; }
    }
}
