using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        public IActionResult EnviarClave(string correo, string clave)
        {
            //El ViewData nos va a permitir compartir información la vista
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["URL"] = $"{this.Request.Scheme}//{this.Request.Host}"; //Concateno con el "$". Scheme viene a ser el HTTPS o HTTP de la web

            return View();
        }

        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }
    }
}
