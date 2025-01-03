namespace Portafolio.Helpers
{
    public static class MailTools
    {
        //título
        public static string Title(string text)
        {
            return "<h1>" + text + "</h1>";
        }

        //cursiva
        public static string Cursive(string text)
        {
            return "<em>" + text + "</em>";
        }

        //negrita
        public static string Bold(string text)
        {
            return "<b>" + text + "</b>";
        }

        //subrayado
        public static string Underline(string text)
        {
            return "<u>" + text + "</u>";
        }

        //párrafo
        public static string Paragraph(string text)
        {
            return "<p>" + text + "</p>";
        }

        //salto de linea
        public static string BreakLine()
        {
            return "<br>";
        }
    }
}