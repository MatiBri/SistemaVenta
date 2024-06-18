using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Referencias
using SistemaVenta.BLL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using Firebase.Auth;

namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;
        //Constructor
        public FireBaseService(IGenericRepository<Configuracion> repoitorio) 
        { 
            _repositorio = repoitorio;
        }
        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage")); //Consultamos la tabla de

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                //Autorización
                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]); //Vamos a iniciar unas credenciales con correo y contraseña de forma asíncrona

                //Token de cancelación
                var cancellation = new CancellationTokenSource();

                //Esta tarea ejecuta el servicio de Firebase Storage
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        //Autenticación de fábrica. Función anónima que ejecuta esta tarea
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true //En caso de un error que lo cancele
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .PutAsync(StreamArchivo, cancellation.Token); //Configuro el envío del archivo al servidor Storage de Firebase

                UrlImagen = await task; //Este task es el que esta arriba, que crea un nuevo FirebaseStorage
            }
            catch
            {
                UrlImagen = "";
                throw;
            }
            return UrlImagen;
        }
        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage")); //Consultamos la tabla de

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                //Autorización
                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]); //Vamos a iniciar unas credenciales con correo y contraseña de forma asíncrona

                //Token de cancelación
                var cancellation = new CancellationTokenSource();

                //Esta tarea ejecuta el servicio de Firebase Storage
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        //Autenticación de fábrica. Función anónima que ejecuta esta tarea
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true //En caso de un error que lo cancele
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .DeleteAsync(); //Elimina el archivo de nuestro servicio

                await task;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
