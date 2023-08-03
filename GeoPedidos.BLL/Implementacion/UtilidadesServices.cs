using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoPedidos.BLL.Interfaces;
using System.Security.Cryptography;

namespace GeoPedidos.BLL.Implementacion
{
    public class UtilidadesServices : IUtilidadesServices
    {
        public string GenerarClave() // Generar clave al registrarse
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6); // generar clave aleatoria alfanumerico de 6 digitos
            return clave;
        }
    }
}
