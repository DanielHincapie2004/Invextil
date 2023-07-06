namespace Invextil.App.Utilidades.Response
{
    public class GenericResponse<TObject>
    {
        //Respuesta a cada solicitud en el sitio
        public bool Estado { get; set; }
        public string? Mensaje { get; set; }
        public TObject? Objeto { get; set; }
        public List<TObject>? ListaObjeto { get; set; }
    }
}
