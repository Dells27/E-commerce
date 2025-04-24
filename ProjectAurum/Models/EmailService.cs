using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            // Obtiene los valores de configuración desde appsettings.json
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:Password"];


            // Configura el cliente SMTP
            // SMTP (Simple Mail Transfer Protocol) es un protocolo de comunicación para el envío de correos electrónicos.
            //En este caso usamos el servidor SMTP de Gmail.(se puede ver en el appsettings.json)
            var client = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            // Construye el mensaje de correo
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Permite contenido HTML
            };

            // Envía el mensaje de forma asíncrona
            //Una operación asincrónica en programación es aquella que no bloquea el flujo del programa mientras se ejecuta.
            mailMessage.To.Add(toEmail);
            await client.SendMailAsync(mailMessage);
            return true;
        }
        catch
        {
            // Si ocurre algún error durante el envío, retorna false
            return false;
        }
    }
}
