namespace Portafolio.Helpers
{
    public static class PaginationConstants
    {
        public static readonly int Maximum = 10; //CANTIDAD MAXIMA DE ITEMS POR PAGINA

        public static int Pages (int total, int items) { //NRO DE PAGINAS

            return (int)Math.Ceiling((double)total / items);
        }
    }
}
