namespace Portafolio.Dto.Requests
{
    public class AdjuntoRequest
    {
        public int GraphicId { get; set; }  
        public byte Tipo { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }        
        public long Peso { get; set; }

    }
}
