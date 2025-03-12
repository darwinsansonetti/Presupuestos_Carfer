using Aluminum.Conexion;
using Aluminum.Helpers;
using Aluminum.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aluminum.View
{
    public partial class FormProductosMain : Form
    {

        private HomeMain _formPadre;
        public List<CategoriaModel> _categorias { get; set; }
        int _empresa_id = 0;        
        public List<ProductoModel> _productos { get; set; }
        public List<int> _productosSeleccioandos { get; set; }
        int widthPanel = 0;

        private List<PictureBox> pictureBoxesSeleccionados = new List<PictureBox>();
        private List<SistemaModel> _system = new List<SistemaModel>();
        private List<MaterialModel> _material = new List<MaterialModel>();

        public FormProductosMain(HomeMain formPadre, int empresa_id, List<int> productosSeleccionados)
        {
            InitializeComponent();

            _formPadre = formPadre; // Referencia al formulario padre
            _empresa_id = empresa_id;
            _categorias = new List<CategoriaModel>();
            _productosSeleccioandos = new List<int>();

            panelContendorImg.SizeChanged += panelContendorImg_SizeChanged;

            _productosSeleccioandos = new List<int>();
            _productosSeleccioandos = productosSeleccionados;

            _system = new List<SistemaModel>();
            _material = new List<MaterialModel>();
        }

        private void panelContendorImg_SizeChanged(object sender, EventArgs e)
        {
            int anchoPictureBox = 165;
            int margen = 20; // Margen opcional para separar los PictureBox

            // Se utiliza ClientSize.Width para obtener el ancho interno real del panel.
            int columnas = panelContendorImg.ClientSize.Width / (anchoPictureBox + margen);

            if (columnas > 0)
            {
                // Llama a un método que reorganiza los PictureBox según el número de columnas calculado.
                OrganizarPictureBoxes(columnas);
                CentrarContenido();
            }
        }

        private void OrganizarPictureBoxes(int columnas)
        {
            int margen = 20;
            int anchoPictureBox = 165;
            int altoPictureBox = 167; // Puedes ajustarlo según tus necesidades

            // Por ejemplo, suponiendo que tienes una lista de PictureBoxes en panelContendorImg.Controls
            for (int i = 0; i < panelContendorImg.Controls.Count; i++)
            {
                // Calcula la posición en el grid
                int columna = i % columnas;
                int fila = i / columnas;

                // Asigna la posición (X, Y) tomando en cuenta el margen
                panelContendorImg.Controls[i].Location = new Point(
                    columna * (anchoPictureBox + margen),
                    fila * (altoPictureBox + margen)
                );
            }
        }

        private void CentrarContenido()
        {
            int margen = 10;
            int anchoPictureBox = 165;

            int columnas = panelContendorImg.ClientSize.Width / (anchoPictureBox + margen);
            if (columnas == 0) columnas = 1; // Evita división por 0

            int filas = (int)Math.Ceiling((double)panelContendorImg.Controls.Count / columnas);

            int anchoTotal = columnas * (anchoPictureBox + margen) - margen;

            // Calcular el punto de inicio X para centrar horizontalmente
            int xInicio = (panelContendorImg.ClientSize.Width - anchoTotal) / 2;
            xInicio = Math.Max(0, xInicio); // Asegura que no sea negativo

            for (int i = 0; i < panelContendorImg.Controls.Count; i++)
            {
                int columna = i % columnas;
                int fila = i / columnas;

                // Solo modificamos X para centrar horizontalmente, Y permanece igual
                panelContendorImg.Controls[i].Location = new Point(
                    xInicio + columna * (anchoPictureBox + margen),
                    panelContendorImg.Controls[i].Location.Y // Mantiene la posición vertical
                );
            }
        }

        private void FormProductosMain_Load(object sender, EventArgs e)
        {
            widthPanel = panelContendorImg.ClientSize.Width;

            cargarProductos();
            CargarCategorias();
            CargarSistemas();
            CargarMaterial();
        }

        private void CargarMaterial()
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            string sql = "SELECT * FROM material where material.empresa_id='" + _empresa_id + "' and material.activo = 1";

            try
            {
                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _material = new List<MaterialModel>();

                while (rdr.Read())
                {
                    MaterialModel _OneMaterial = new MaterialModel();
                    _OneMaterial.id = int.Parse(rdr[0].ToString());
                    _OneMaterial.nombre = rdr[1].ToString();
                    _OneMaterial.empresa_id = int.Parse(rdr[2].ToString());
                    _OneMaterial.activo = int.Parse(rdr[3].ToString());

                    _material.Add(_OneMaterial);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró realizar la búsqueda, error: " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }
        }

        private void CargarSistemas()
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            string sql = "SELECT * FROM sistema where sistema.empresa_id='" + _empresa_id + "' and sistema.activo = 1";

            try
            {
                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _system = new List<SistemaModel>();

                while (rdr.Read())
                {
                    SistemaModel _Onesystem = new SistemaModel();
                    _Onesystem.id = int.Parse(rdr[0].ToString());
                    _Onesystem.nombre = rdr[1].ToString();
                    _Onesystem.empresa_id = int.Parse(rdr[2].ToString());
                    _Onesystem.activo = int.Parse(rdr[3].ToString());

                    _system.Add(_Onesystem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró realizar la búsqueda, error: " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }
        }

        private void cargarProductos()
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            string sql = "SELECT * FROM producto where producto.empresa_id='" + _empresa_id + "' and producto.activo = 1";

            try
            {
                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _productos = new List<ProductoModel>();

                while (rdr.Read())
                {
                    ProductoModel _producto = new ProductoModel();
                    _producto.id = int.Parse(rdr[0].ToString());
                    _producto.nombre = rdr[1].ToString();
                    _producto.descripcion = rdr[2].ToString();
                    _producto.path_logo = (byte[])rdr[3];
                    _producto.costo_metro = Convert.ToDouble(rdr[4].ToString());
                    _producto.empresa_id = int.Parse(rdr[5].ToString());
                    _producto.categoria_id = int.Parse(rdr[6].ToString());
                    _producto.activo = int.Parse(rdr[7].ToString());

                    _productos.Add(_producto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró realizar la búsqueda, error: " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }
        }

        private void CargarCategorias()
        {
            CConexion _conexion = new CConexion();
            MySqlConnection _conn = _conexion.establecerConexion();

            string sql = "SELECT * from categoria where categoria.empresa_id='" + _empresa_id + "' and categoria.activa = 1";

            try
            {

                HelperQuery _helperQuery = new HelperQuery();
                MySqlDataReader rdr = _helperQuery.querySelect(_conn, sql);

                _categorias = new List<CategoriaModel>();
                _categorias.Add(
                    new CategoriaModel()
                    {
                        id = 9999999,
                        activa = 1,
                        empresa_id = _empresa_id,
                        nombre = "Todas"
                    }    
                );

                while (rdr.Read())
                {
                    CategoriaModel _categoria = new CategoriaModel();
                    _categoria.id = int.Parse(rdr[0].ToString());
                    _categoria.nombre = rdr[1].ToString();
                    _categoria.empresa_id = int.Parse(rdr[2].ToString());
                    _categoria.activa = int.Parse(rdr[3].ToString());

                    _categorias.Add(_categoria);
                }

                comboBoxCategoria.DataSource = _categorias;
                comboBoxCategoria.DisplayMember = "nombre"; // Lo que se mostrará
                comboBoxCategoria.ValueMember = "id";         // El valor asociado
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró realizar la búsqueda, error: " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }
        }

        private void buttonVolver_Click(object sender, EventArgs e)
        {

            //_formPadre.AbrirFormulario(new FormProductosMain(_formPadre));
        }

        private void comboBoxCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCategoria.SelectedItem is CategoriaModel categoria)
            {
                int _categoriaIdSelect = categoria.id;
                ConsultarDatos(_categoriaIdSelect);
            }
        }

        // Método para realizar la consulta SQL con base en el id seleccionado
        private void ConsultarDatos(int id)
        {
            panelContendorImg.Controls.Clear();
            List<ProductoModel> subProductos = new List<ProductoModel>();

            //Se traen todos los productos
            if (id == 9999999)
            {
                subProductos = _productos;
            }
            else
            {
               subProductos = _productos.Where(p => p.categoria_id == id).ToList();
            }

            //int columnas = 4;              // Número de columnas fijas
            int margen = 20;                 // Espacio entre controles
            int anchoPictureBox = 165;       // Ancho de cada PictureBox
            int altoPictureBox = 167;        // Alto de cada PictureBox

            // Calcula el número de columnas que caben en panel1
            int columnas = panelContendorImg.ClientSize.Width / (anchoPictureBox + margen);

            // Recorrer cada imagen en la lista
            for (int i = 0; i < subProductos.Count; i++)
            {
                byte[] imagenRecuperada = subProductos[i].path_logo as byte[];

                // Crear CheckBox
                CheckBox chk = new CheckBox
                {
                    Size = new Size(20, 20),
                    BackColor = Color.Transparent, // Para no tapar la imagen
                    Tag = subProductos[i].id // Guardar el ID en el Tag del CheckBox también
                };

                PictureBox pb = new PictureBox();
                using (MemoryStream ms = new MemoryStream(imagenRecuperada))
                {
                    // Crear y configurar el PictureBox
                    pb.Image = Image.FromStream(ms);
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Size = new Size(anchoPictureBox, altoPictureBox);
                    pb.BackColor = Color.White;
                    pb.BorderStyle = BorderStyle.None;
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;
                    pb.BorderStyle = BorderStyle.FixedSingle;
                    pb.Tag = subProductos[i].id; // Asignar el ID del modelo al Tag


                    //Para obtener las imagenes que estan seleccionadas y marcarlas
                    if (_productosSeleccioandos.Contains(subProductos[i].id))
                    {
                        // Si no está seleccionado, lo agregamos a la lista
                        pb.BorderStyle = BorderStyle.Fixed3D; // Resalta el borde
                        pb.BackColor = Color.LightBlue; // Color de fondo de selección
                        //pictureBoxesSeleccionados.Add(pb);

                        chk.Checked = true;
                    }
                    else
                    {
                        // Si ya está seleccionado, lo deseleccionamos
                        pb.BorderStyle = BorderStyle.FixedSingle;
                        pb.BackColor = Color.Transparent; // Restaurar fondo
                        //pictureBoxesSeleccionados.Remove(pb);

                        chk.Checked = false;
                    }
                }


                // Calcular la fila y columna en función del índice
                int columna = i % columnas;       // Resto de la división: posición horizontal
                int fila = i / columnas;            // División entera: posición vertical

                // Ubicar el PictureBox dentro del panel
                pb.Location = new Point(columna * (anchoPictureBox + margen),
                                        fila * (altoPictureBox + margen));

                // Agregar evento Click para acceder al ID cuando se haga clic en la imagen
                //pb.Click += PictureBox_Click;


                // Posicionar el CheckBox en la esquina inferior derecha
                chk.Location = new Point(pb.Width - chk.Width - 5, pb.Height - chk.Height - 5);

                // Evento de clic en el CheckBox (dispara PictureBox_Click)
                chk.Click += (sender, e) => PictureBox_Click(pb, e);

                // Agregar CheckBox al PictureBox
                pb.Controls.Add(chk);


                // Agregar el PictureBox al panel
                panelContendorImg.Controls.Add(pb);
            }

            CentrarContenido();
        }

        //private void CheckBox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (sender is CheckBox chk && chk.Parent is PictureBox pb)
        //    {
        //        if (chk.Checked)
        //        {
        //            pb.BorderStyle = BorderStyle.Fixed3D; // Destacar imagen seleccionada
        //            pb.BackColor = Color.LightBlue; // Cambiar color de fondo
        //        }
        //        else
        //        {
        //            pb.BorderStyle = BorderStyle.FixedSingle;
        //            pb.BackColor = Color.Transparent;
        //        }
        //    }
        //}

        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pb && pb.Tag is int id)
            {
                //MessageBox.Show($"ID de la imagen seleccionada: {id}");

                // Buscar el CheckBox dentro del PictureBox
                CheckBox chk = pb.Controls.OfType<CheckBox>().FirstOrDefault();
                if (chk != null)
                {
                    // Alternar el estado del CheckBox
                    //chk.Checked = !chk.Checked;

                    if (chk.Checked)
                    {
                        // Si se marca el CheckBox, resaltar el PictureBox
                        pb.BorderStyle = BorderStyle.Fixed3D;
                        pb.BackColor = Color.LightBlue;
                        pictureBoxesSeleccionados.Add(pb);
                        _productosSeleccioandos.Add(id);
                    }
                    else
                    {
                        // Si se desmarca el CheckBox, restaurar el PictureBox
                        pb.BorderStyle = BorderStyle.FixedSingle;
                        pb.BackColor = Color.Transparent;
                        pictureBoxesSeleccionados.Remove(pb);
                        _productosSeleccioandos.Remove(id);
                    }
                }
            }
        }

        private void buttonVolver_Click_1(object sender, EventArgs e)
        {
            if (_productosSeleccioandos.Count() > 0)
            {
                if (_system.Count() > 0)
                {
                    if (_material.Count() > 0)
                    {
                        _formPadre.AbrirFormulario(new FormStep2(_formPadre, _empresa_id, _productosSeleccioandos, _productos, _categorias, _system, _material));
                    }
                    else
                    {
                        MessageBox.Show("Es necesario Cargar los Tipos de Materiales.");
                    }
                }
                else
                {
                    MessageBox.Show("Es necesario Cargar los Tipos de Sistemas.");
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar por lo menos un producto.");
            }
        }
    }
}
