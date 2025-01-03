namespace Portafolio.Dto.Responses
{
    public class GraphicResponse
    {
        public int GraphicId { get; set; }
        public string Marca { get; set; }
        public string Serie { get; set; }
        public string Modelo { get; set; }
        public byte Vram { get; set; }
        public decimal Precio { get; set; }
        public bool Estado { get; set; }

        public List<AdjuntoResponse> Adjuntos { get; set; } = new List<AdjuntoResponse>();
    }
}
