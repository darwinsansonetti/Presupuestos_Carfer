using Aluminum.Model;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aluminum.Helpers
{
    public class PdfGenerate
    {
        public static void CrearEmpresaPDF(List<EmpresaModel> empresas, string filePath)
        {
            // Crear un FileStream para el archivo de salida
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                // Usar PdfWriter con el FileStream
                PdfWriter writer = new PdfWriter(fs);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Agregar título
                document.Add(new Paragraph("Lista de Empresas").SetFontSize(18));

                // Iterar sobre la lista de empresas
                foreach (var _empresa in empresas)
                {
                    document.Add(new Paragraph($"Razon_Social: {_empresa.razon_social}"));
                    document.Add(new Paragraph($"Documento: ${_empresa.documento}"));

                    //// Verificar si la imagen existe antes de agregarla
                    //if (File.Exists(producto.ImagenPath))
                    //{
                    //    Image img = new Image(ImageDataFactory.Create(producto.ImagenPath))
                    //        .ScaleToFit(100, 100); // Ajustar tamaño de imagen
                    //    document.Add(img);
                    //}
                    document.Add(new Paragraph("\n"));
                }

                // Cerrar el documento (esto guarda el archivo)
                document.Close();
            }
        }
    }
}
