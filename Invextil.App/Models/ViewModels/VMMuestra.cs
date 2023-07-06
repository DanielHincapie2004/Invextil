using Invextil.Entity;

namespace Invextil.App.Models.ViewModels
{
    public class VMMuestra
    {
        public int IdMuestra { get; set; }
        public int? idTela { get; set; }
        public string? Tela { get; set; }
        public string? DocumentoCliente { get; set; }

        public string? NombreCliente { get; set; }

        public string? Direccion { get; set; }

    }
}
