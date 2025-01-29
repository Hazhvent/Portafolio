namespace Portafolio.Dto
{
    public class Pagination<T>
    {
        public List<T> List { get; set; } = new List<T>(); //LISTA DEL OBJETO SOLICITADO
        public int Items { get; set; } //CANTIDAD DE ITEMS POR PAGINA
        public int Total { get; set; } //TOTAL DE REGISTROS
        public int Pages { get; set; } //NRO DE PAGINAS 

    }
}
