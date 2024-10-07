namespace SistemaVenta.AplicacionWeb.Utilidades.Response
{
    //Esta clase se utiliza como respuesta a todas las solicitudes que se hagan a la página
    public class GenericResponse<TObject>
    {
        public bool Estado { get; set; }
        public string? Mensaje { get; set; } //El "?" es de que va a permitir nulos. En este caso, mensajes nulos
        public TObject? Objeto { get; set; }

        public List<TObject>? ListaObjeto { get; set; }

    }
}
