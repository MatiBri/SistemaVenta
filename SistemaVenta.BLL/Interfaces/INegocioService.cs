using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Referencias
using SistemaVenta.Entity;
//

namespace SistemaVenta.BLL.Interfaces
{
    //Esto es para las peticiones HTTP
    public interface INegocioService
    {
        Task<Negocio> Obtener();

        Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo = "");
    }
}
