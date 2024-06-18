using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Referencias
using System.Net;
using System.Net.Mail;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        //Contexto de Generic Repository
        private readonly IGenericRepository<Configuracion> _repositorio;

        public CorreoService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            //Lógica del correo
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo")); //Consultamos la tabla de Configuracion

                Dictionary<string, string> Config = query.ToDictionary(keySelector : c => c.Propiedad, elementSelector: c => c.Valor); //Llamamos a las columnas Propiedad y Valor de la tabla Configuracion en la BD
                //Ambas se almacenan dentro del diccionario, en el primer string = Propiedad, segundo string = Valor

                //Creamos unas credenciales con el correo y la clave, definidos en nuestra tabla BD
                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage()
                {
                    //From es el origen del correo. Esto va a ser quién está enviando el correo
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true //Decimos que trabajamos con una estructura HTML en el body, porque el correo es y se muestra en una página
                };

                correo.To.Add(new MailAddress(CorreoDestino)); //Esta es una nueva dirección de correo

                //Configuro el cliente-servidor
                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                };

                clienteServidor.Send(correo); //Envia el correo que configuramos
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
