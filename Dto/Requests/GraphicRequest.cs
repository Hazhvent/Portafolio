namespace Portafolio.Dto.Requests
{
    public class GraphicRequest
    {
        public int MarcaId { get; set; }
        public int SerieId { get; set; }
        public string Modelo { get; set; }
        public byte Vram { get; set; }
        public decimal Precio { get; set; }

        public List<AdjuntoRequest> Adjuntos { get; set; } = new List<AdjuntoRequest>();
    }
}
