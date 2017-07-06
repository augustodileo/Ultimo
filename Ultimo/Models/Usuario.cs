using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Ultimo.Models
{
    public class Usuario
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Telefono")]
        public string Tel { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }
        public bool Buscar (string email)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT [Email] FROM [dbo].[Usuarios] WHERE [Email] = @e ";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                    .Value = email;
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
            }
        }
        public bool IsValid(string email, string password)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT [Email] FROM [dbo].[Usuarios] WHERE [Email] = @e AND [Password] = @p";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                    .Value = email;
                cmd.Parameters
                    .Add(new SqlParameter("@p", SqlDbType.NChar))
                    //.Value = password;
                    .Value = Helpers.SHA1.Encode(password);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }
    }
}