using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Referencias
using SistemaVenta.BLL.Interfaces;
using System.Security.Cryptography;

namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        public string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0,6); //Retorna una cadena de texto aleatorio
            //"N": Indica que vamos a estar utilizando números y letras
            //Substring: La cadena de texto va a tener un ancho de hassta 6 dígitos

            return clave;
        }

        //Este método sirve para convertir la contraseña del usuario a una clave encriptada, y así guardarla en la BD
        public string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();

            //Creamos el objeto para poder encriptar el texto
            //Todo dentro del using es código por defecto para encriptar texto en SHA-256
            using(SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(texto)); //Convierte nuestro texto en un array de bytes

                //Recorre cada uno de los elementos dentro de result
                foreach(byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }


    }
}
