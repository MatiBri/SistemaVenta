using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    //La clase que implementa ésta interfaz: UsuarioService
    public interface IUsuarioService
    {
        //Todos estos métodos van a ser asíncronos. Razón por la que uso Task
        Task<List<Usuario>> Lista(); //Devuelve una lista de usuarios

        //Stream: Provee una vista genérica de un objeto, en éste caso una foto
        //string UrlPlantillaCorreo: Con esto nosotros podemos enviarle el correo al usuario
        Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = ""); //Crea un usuario
        Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "");
        Task<bool> Eliminar(int IdUsuario); //Le pasamos el ID del usuario que queremos eliminar
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave); //Con esto obtenemos un usuario
        Task<Usuario> ObtenerPorId(int IdUsuario); //Acá devolvemos un usuario por el ID del usuario que se necesita


        //Estos dos son para el formulario del perfil de usuario para que pueda cambiar sus datos y contraseña
        Task<bool> GuardarPerfil(Usuario entidad);
        Task<bool> CambiarClave(int IdUsuario, string ClaveActual, string ClaveNueva);
        Task<bool> ReestablecerClave(string CorreoDestino, string UrlPlantillaCorreo);
        //
    }
}
