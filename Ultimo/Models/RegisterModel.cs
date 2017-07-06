using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ultimo.Models
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                   @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                   @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                   ErrorMessage = "Email is not valid")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Telefono")]
        public string Tel { get; set; }

        [Display(Name = "Token")]
        public string Token { get; set; }
        //METHODS
        public bool Register(Models.RegisterModel toRegister)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                
                toRegister.Token = Helpers.SHA1.Encode(toRegister.Email + toRegister.Password + toRegister.Name + toRegister.Tel);
                string query = @"Insert into [dbo].[Usuarios] ([Email],[Password],[Name],[Tel],[Token]) Values (@e,@p,@n,@t,@tk)";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                   .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                   .Value = toRegister.Email;
                cmd.Parameters
                    .Add(new SqlParameter("@p", SqlDbType.NChar))
                    //.Value = toRegister.Password;
                    .Value = Helpers.SHA1.Encode(toRegister.Password);
                cmd.Parameters
                    .Add(new SqlParameter("@n", SqlDbType.NVarChar))
                    .Value = toRegister.Name;
                cmd.Parameters
                    .Add(new SqlParameter("@t", SqlDbType.NChar))
                    .Value = toRegister.Tel;
                cmd.Parameters
                    .Add(new SqlParameter("@tk", SqlDbType.NChar))
                    .Value = toRegister.Token;
                connection.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
                return true;
            }
        }
    }
}