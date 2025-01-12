namespace Portafolio.Dto.Requests
{
    public class GraphicRequest
    {
        public int MarcaId { get; set; }
        public int SerieId { get; set; }
        public int ModeloId { get; set; }

        public List<AdjuntoRequest> Adjuntos { get; set; } = new List<AdjuntoRequest>();
    }
}
