using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IUsuarioService
    {
        //Todos estos métodos van a ser asíncronos. Razón por la que uso Task
        Task<List<Usuario>> Lista(); //Devuelve una lista de usuarios

        //Stream: Provee una vista genérica de un objeto, en éste caso una foto
        //string UrlPlantillaCorreo: Con esto nosotros podemos enviarle el correo al usuario
        Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = ""); //Crea un usuario
        
    }
}
