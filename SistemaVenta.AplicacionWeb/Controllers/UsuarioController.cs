using Microsoft.AspNetCore.Mvc;

//Referencias para el HTTP de Usuario (CRUD)
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
//Fin referencias

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        //Variables privadas y de sólo lectura
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _RolServicio;
        private readonly IMapper _mapper;
        //Fin variables privadas y de sólo lectura

        //Constructor
        public UsuarioController(IUsuarioService usuarioServicio, 
            IRolService rolServicio, IMapper mapper
            )
        {
            _usuarioServicio = usuarioServicio;
            _RolServicio = rolServicio;
            _mapper = mapper;
        }
        //Fin Constructor

        public IActionResult Index()
        {
            return View();
        }

        //Peticiones HTTP
        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _RolServicio.Lista()); //Devuelve una lista de tipo Rol y lo va a almacenar en la variable "lista"
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(await _usuarioServicio.Lista()); //Devuelve una lista de tipo Rol y lo va a almacenar en la variable "lista"
            return StatusCode(StatusCodes.Status200OK, new {data = vmUsuarioLista});
        }

        [HttpPost]
        //[FromForm]: Recibimos los parámetros desde el formulario
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            //El GenericResponse lo uso como un formato estándar para las respuestas
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                //Toda este lógica va a suceder en el caso de que "Foto" tenga algún valor
                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N"); //Solo necesitamos números y letras
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension); //Le estamos dando un nuevo nombre a una foto
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuario_creado = await _usuarioServicio.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);//De este tipo Viewmodel, lo convertimos a usuario

                vmUsuario = _mapper.Map<VMUsuario>(usuario_creado); //Devolvemos a la vista una respuesta

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        //[FromForm]: Recibimos los parámetros desde el formulario
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            //El GenericResponse lo uso como un formato estándar para las respuestas
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                //Toda este lógica va a suceder en el caso de que "Foto" tenga algún valor
                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N"); //Solo necesitamos números y letras
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension); //Le estamos dando un nuevo nombre a una foto
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuario_editado = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);//De este tipo Viewmodel, lo convertimos a usuario
                vmUsuario = _mapper.Map<VMUsuario>(usuario_editado); //Devolvemos a la vista una respuesta

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _usuarioServicio.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}
