using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ultimo.Models;



namespace Ultimo.Models
{
    public class ApunteCreate
    {
        [Required]
        [Display(Name = "Titulo")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Descripcion")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public int Precio { get; set; }

        [Required]
        [Display(Name = "Universidad")]
        public int UniversidadId { get; set; }

        [Display(Name = "Facultad")]
        public int? FacultadId { get; set; }

        [Display(Name = "Materia")]
        public int? MateriaId { get; set; }

        [Display(Name = "Foto")]
        public HttpPostedFileBase Foto { get; set; }

        public ApunteCreate(string newTitle, string newDescription, int newPrecio, int newMateria, int newUniversidadId, HttpPostedFileBase newFoto)
        {
            this.Title = newTitle;
            this.Description = newDescription;
            this.Precio = newPrecio;
            this.MateriaId = newMateria;
            this.UniversidadId = newUniversidadId;
            this.Foto = newFoto;
        }

        public ApunteCreate(){}

        public int Create(Models.ApunteCreate toCreate, int OwnerId)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"Insert into [dbo].[Apuntes] ([Title],[Description],[Precio],[OwnerId],[UniversidadId],[FacultadId],[MateriaId],[Foto]) Values (@t,@d,@p,@oid,@uni,@fac,@mat,@f) RETURN";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                   .Add(new SqlParameter("@t", SqlDbType.NVarChar))
                   .Value = toCreate.Title;
                cmd.Parameters
                   .Add(new SqlParameter("@d", SqlDbType.NVarChar))
                   .Value = toCreate.Description;
                cmd.Parameters
                   .Add(new SqlParameter("@p", SqlDbType.Int))
                   .Value = toCreate.Precio;
                cmd.Parameters
                   .Add(new SqlParameter("@oid", SqlDbType.Int))
                   .Value = OwnerId;
                cmd.Parameters
                   .Add(new SqlParameter("@uni", SqlDbType.Int))
                   .Value = toCreate.UniversidadId;
                if (toCreate.FacultadId != null)
                {
                    cmd.Parameters
                       .Add(new SqlParameter("@fac", SqlDbType.Int))
                       .Value = toCreate.FacultadId;
                }
                else
                {
                    cmd.Parameters
                       .Add(new SqlParameter("@fac", SqlDbType.Int))
                       .Value = DBNull.Value;
                }
                if (toCreate.MateriaId != null)
                {
                    cmd.Parameters
                       .Add(new SqlParameter("@mat", SqlDbType.Int))
                       .Value = toCreate.MateriaId;
                }
                else
                {
                    cmd.Parameters
                       .Add(new SqlParameter("@mat", SqlDbType.Int))
                       .Value = DBNull.Value;
                }

                if (toCreate.Foto != null)
                {
                    string pic = System.IO.Path.GetFileName(toCreate.Foto.FileName);
                    string path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/images/apuntes"), pic);
                    toCreate.Foto.SaveAs(path);
                    cmd.Parameters
                       .Add(new SqlParameter("@f", SqlDbType.NVarChar))
                       .Value = "/images/apuntes/" + pic;
                }
                else
                {
                    cmd.Parameters
                       .Add(new SqlParameter("@f", SqlDbType.NVarChar))
                       .Value = "/images/apuntes/noimage.png";
                }

                connection.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
                string newerQuery = @"SELECT MAX([ApunteId]) as ApunteId FROM [dbo].[Apuntes] WHERE [OwnerId] = @oid";
                var newerCmd = new SqlCommand(newerQuery, connection);
                newerCmd.Parameters
                   .Add(new SqlParameter("@oid", SqlDbType.NVarChar))
                   .Value = OwnerId;
                var newerReader = newerCmd.ExecuteReader();
                if (newerReader.Read())
                {
                    int lastId = newerReader.GetInt32(0);
                    newerReader.Dispose();
                    newerCmd.Dispose();
                    return lastId;
                }
                return -1;
            }
        }
    }
}