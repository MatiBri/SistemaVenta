using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IUtilidadesService
    {
        //Servicio de utilidades

        string GenerarClave(); //Esto retorna un código para que el usuario pueda logearse

        string ConvertirSha256(string texto); //Esto es una encriptacion, del tipo Sha256. Este método recibe un texto y lo devolverá encriptado en Sha256
    }
}
