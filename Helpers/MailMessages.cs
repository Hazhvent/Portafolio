
namespace Portafolio.Helpers
{
    public static class MailMessages
    {

        public static string GetCode(string code) {
            string title = MailTools.Title("Confirme su correo") + MailTools.BreakLine();
            string body = MailTools.Paragraph("Tu codigo de verificacion es:  " + MailTools.Bold(code));
            string total = title + body;
            return total;
        }

        public static string GetReservation(string code)
        {
            string title = MailTools.Title("Gracias por reservar en CoffeTime!") + MailTools.BreakLine();
            string body = MailTools.Paragraph("Tu codigo de reservacion es:  " + MailTools.Bold(code)) +
                          MailTools.Paragraph("Puedes consultar los detalles de tu reservacion ingresando tu codigo en nuestra pagina web!") +
                          MailTools.Paragraph(MailTools.Bold("Te esperamos!"));
            string total = title + body;
            return total;
        }

        public static string GetDeleteConfirmation()
        {
            string title = MailTools.Title("Lamentamos tu decision") + MailTools.BreakLine();
            string body = MailTools.Paragraph("Tu reservacion ha sido eliminada." ) +
                          MailTools.Paragraph("Puedes realizar otra reservacion sin ningun problema!") +
                          MailTools.Paragraph(MailTools.Bold("No nos defraudes!"));
            string total = title + body;
            return total;
        }




    }
}
