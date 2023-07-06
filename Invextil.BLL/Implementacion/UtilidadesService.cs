using Invextil.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Invextil.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        //Metodo para encriptar la contraseña en tipo Sha256
        public string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();
            //Encriptar texto
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                //Convertir texto en array de bytes
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));
                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        public string GenerarClave()
        {
            //Genera clave aleatoria con numero y letras
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);
            return clave;
        }
    }
}
