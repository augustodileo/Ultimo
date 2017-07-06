using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;


namespace Ultimo.Models
{
    public class Email
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string correo { get; set; }
        [Required]
        [Display(Name = "Token")]
        public string token { get; set; }

        public Email (Models.RegisterModel cons)
        {
            correo = cons.Email;
            token = cons.Token;
        }

        public void SendEmail(Email Data)
        {
            var body = "Prueba";
            var message = new MailMessage();
            message.To.Add(new MailAddress(Data.correo));
            message.From = new MailAddress("tomaskapo97@hotmail.com");
            message.Sender = new MailAddress("tomaskapo97@hotmail.com");
            message.Subject = "Prueba";
            message.ReplyToList.Add("tomaskapo97@hotmail.com");
            message.Body = string.Format(body);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "tomanacho@outlook.com.ar",  // replace with valid value
                    Password = "nachotoma123"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.SendMailAsync(message);
            }
        }
    }

}