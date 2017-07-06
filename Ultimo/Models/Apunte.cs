using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ultimo.Models
{
    public class Apunte
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Titulo")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Descripcion")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public int Precio { get; set; }

        [Display(Name = "OwnerId")]
        public int OwnerId { get; set; }

        [Display(Name = "Materia")]
        public string Materia { get; set; }

        [Display(Name = "Facultad")]
        public string Facultad { get; set; }

        [Required]
        [Display(Name = "Universidad")]
        public string Universidad { get; set; }

        [Display(Name = "Foto")]
        public String FotoPath { get; set; }

        public Apunte(){}

        public int obtainId(string email)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT [Id] FROM [dbo].[Usuarios] WHERE [Email] = @e";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                    .Value = email;
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        OwnerId = reader.GetInt32(0);
                        reader.Dispose();
                        cmd.Dispose();
                        return OwnerId;
                    }
                    else
                    {
                        OwnerId = -1;
                        reader.Dispose();
                        cmd.Dispose();
                        return OwnerId;
                    }
                }
                else
                {
                    OwnerId = -1;
                    reader.Dispose();
                    cmd.Dispose();
                    return OwnerId;
                }
            }
        }
    }
}