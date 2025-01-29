using Portafolio.Dto;
using Portafolio.Helpers;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace Portafolio.Services
{
    public class EmailService
    {
        private readonly EmailProvider _outlookProvider;
        private readonly EmailProvider _gmailProvider;
        private readonly EmailProvider _zohoProvider;
        private readonly NetworkCredential _outlookCredentials;
        private readonly NetworkCredential _gmailCredentials;
        private readonly NetworkCredential _zohoCredentials;
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _outlookProvider = GetProvider(0);//0: Proveedor OUTLOOK
            _gmailProvider = GetProvider(1);//1: Proveedor GMAIL
            _zohoProvider = GetProvider(2);//2: Proveedor ZOHO
            _outlookCredentials = GetCredential(0);//0: Credenciales de OUTLOOK
            _gmailCredentials = GetCredential(1);//0: Credenciales de GMAIL
            _zohoCredentials = GetCredential(2);//0: Credenciales de ZOHO
        }
        //OBTIENE LA LISTA RUTAS DE LOS ARCHIVOS ADJUNTOS
        public List<Attachment> GetAttachedFiles()
        {
            List<Attachment> attachments = new();

            try
            {
                //LEE EL ARCHIVO DE RUTAS
                IConfigurationSection attachedFilesSection = _configuration.GetSection("attachedFiles");
                //OBTIENE LAS RUTAS
                string[] filePaths = attachedFilesSection.Get<string[]>();

                if (filePaths != null)
                {
                    //AGREGA CADA RUTA A LA LISTA DE ARCHIVOS ADJUNTOS
                    foreach (string filePath in filePaths)
                    {
                        attachments.Add(new Attachment(filePath));
                        Debug.WriteLine("Archivo cargado desde: "+filePath);
                    }
                }
                else
                {
                    Debug.WriteLine("El archivo de rutas no contiene ninguna ruta.");
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al leer el archivo de rutas: {ex.Message}");
            }

            return attachments;
        }
        //ASIGNA CREDENCIALES
        private NetworkCredential GetCredential(int pos)
        {
            NetworkCredential temp = new();
            //LEE EL ARCHIVO DE CREDENCIALES
            IConfigurationSection credentialsSection = _configuration.GetSection("emailCredentials");
            //OBTIENE LAS CREDENCIALES
            var credentials = credentialsSection.Get<List<EmailCredential>>();
            //ASIGNA EL VALOR DE LOS CAMPOS DE LA CREDENCIAL SEGUN LA POSICION ELEGIDA
            if (credentials != null)
            {
                temp.UserName = credentials[pos].User;
                temp.Password = credentials[pos].Pass;
            }
            else
            {
                Debug.WriteLine("El archivo de credenciales no contiene ninguna información.");
            }
            return temp;
        }
        //ASIGNA PROVEEDORES
        private EmailProvider GetProvider(int pos)
        {
            EmailProvider temp = new();
            //LEE EL ARCHIVO DE PROVEEDORES
            IConfigurationSection ProvidersSection = _configuration.GetSection("emailProviders");
            //OBTIENE LOS PROVEEDORES
            var providers = ProvidersSection.Get<List<EmailProvider>>();
            //ASIGNA EL VALOR DE LOS CAMPOS DEl PROVEEDOR SEGUN LA POSICION ELEGIDA
            if (providers != null)
            {
                temp.Host = providers[pos].Host;
                temp.Port = providers[pos].Port;
            }
            else
            {
                Debug.WriteLine("El archivo de proveedores no contiene ninguna información.");
            }
            return temp;
        }
        
        //MANDA LOS CORREOS
        public void SendEmail(NetworkCredential sender, string receiver, string subject, EmailProvider provider, string message, bool files)
        {
            MailMessage email = new()
            {
                From = new MailAddress(sender.UserName),
                Subject = subject,
                Body = message
            };

            email.To.Add(receiver);

            if (files) {
                foreach (Attachment file in GetAttachedFiles())
                {
                    email.Attachments.Add(file);
                }
            }
           
            email.IsBodyHtml = true;

            SmtpClient client = new(provider.Host, provider.Port)
            {
                EnableSsl = true,
                Credentials = sender
            };

            client.Send(email);
        }

        //ENVIA EL CODIGO DE CONFIRMACION
        public string SendCode(string receiver, string subject) {
            string code = RandomGenerator.SetCode(4);
            SendEmail(_zohoCredentials, receiver, subject, _zohoProvider, MailMessages.GetCode(code), false);//ENVIO DESDE UN CLIENTE ZOHO
            return code;
        }

        //ENVIA EL CODIGO DE RESERVACION
        public void SendReservation(string receiver, string subject, string code)
        {
            SendEmail(_zohoCredentials, receiver, subject, _zohoProvider, MailMessages.GetReservation(code), true);//ENVIO DESDE UN CLIENTE ZOHO
        }

        //ENVIA MENSAJE DE ELIMINACION
        public void SendDeleteReservation(string receiver, string subject)
        {
            SendEmail(_zohoCredentials, receiver, subject, _zohoProvider, MailMessages.GetDeleteConfirmation(), false);//ENVIO DESDE UN CLIENTE ZOHO
        }
    }
}
