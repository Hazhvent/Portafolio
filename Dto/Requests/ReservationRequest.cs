namespace Portafolio.Dto.Requests
{
    public class ReservationRequest
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Celular { get; set; }
        public string Correo { get; set; }
        public byte Hora { get; set; }
        public string Codigo { get; set; }
    }
}
