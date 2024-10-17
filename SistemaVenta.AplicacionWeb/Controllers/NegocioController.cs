using Microsoft.AspNetCore.Mvc;

//Referencias
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
//

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class NegocioController : Controller
    {
        //Servicios a utilizar
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        //

        //Constructor
        public NegocioController(IMapper mapper, INegocioService negocioService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
        }
        //
        public IActionResult Index()
        {
            return View();
        }


        //Métodos HTTP utilizados
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                //Devuelvo toda la info de negocio
                VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());
                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        //Metodo guardar cambios
        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm] string modelo)
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                //Devuelvo toda la info de negocio
                VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);

                string nombreLogo = "";
                Stream logoStream = null;

                if (logo != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N"); //La N es para obtener solamente números y letras
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombre_en_codigo, extension);
                    logoStream = logo.OpenReadStream(); //Lo almacenamos en logoStream
                }

                Negocio negocio_editado = await _negocioService.GuardarCambios(_mapper.Map<Negocio>(vmNegocio), logoStream, nombreLogo);

                vmNegocio = _mapper.Map<VMNegocio>(negocio_editado);

                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
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
