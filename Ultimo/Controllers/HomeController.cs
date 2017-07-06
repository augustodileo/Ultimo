using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using Ultimo.Models;

namespace Ultimo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home   
        [Authorize]
        public ActionResult Index()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT [dbo].[Apuntes].[ApunteId], [dbo].[Apuntes].[Title], [dbo].[Apuntes].[Description], [dbo].[Apuntes].[Precio], [dbo].[Universidades].[UniversidadName], [dbo].[Facultades].[FacultadName], [dbo].[Materias].[MateriaName], [dbo].[Apuntes].[OwnerId], [dbo].[Apuntes].[Foto] FROM [dbo].[Apuntes] LEFT OUTER JOIN [dbo].[Universidades] ON [dbo].[Apuntes].[UniversidadId]=[dbo].[Universidades].[UniversidadId] LEFT OUTER JOIN [dbo].[Materias] ON [dbo].[Apuntes].[MateriaId] = [dbo].[Materias].[MateriaId] LEFT OUTER JOIN [dbo].[Facultades] ON [dbo].[Apuntes].[FacultadId]=[dbo].[Facultades].[FacultadId]";
                var cmd = new SqlCommand(query, connection);
                connection.Open();
                var reader = cmd.ExecuteReader();
                List<Apunte> listApuntes = new List<Apunte>();
                while (reader.Read())
                {
                    var apunteDetails = new Apunte();
                    apunteDetails.Id = reader.GetInt32(reader.GetOrdinal("ApunteId"));
                    apunteDetails.Title = reader["Title"].ToString();
                    apunteDetails.Description = reader["Description"].ToString();
                    apunteDetails.Precio = reader.GetInt32(reader.GetOrdinal("Precio"));
                    apunteDetails.Universidad = reader["UniversidadName"].ToString();
                    apunteDetails.Facultad = "-";
                    if (!reader.IsDBNull(reader.GetOrdinal("FacultadName")))
                    {
                        apunteDetails.Facultad = reader["FacultadName"].ToString();
                    }
                    apunteDetails.Materia = "-";
                    if (!reader.IsDBNull(reader.GetOrdinal("MateriaName")))
                    {
                        apunteDetails.Materia = reader["MateriaName"].ToString();
                    }
                    apunteDetails.OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"));
                    apunteDetails.FotoPath = reader["Foto"].ToString();
                    listApuntes.Add(apunteDetails);
                }
                return View(listApuntes);
            }
        }
    }
}