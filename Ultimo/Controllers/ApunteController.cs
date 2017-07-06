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
    public class ApunteController : Controller
    {
        List<Materia> listMaterias = new List<Materia>();

        // GET: Apunte
        public ActionResult Index(int Id)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT [dbo].[Apuntes].[ApunteId], [dbo].[Apuntes].[Title], [dbo].[Apuntes].[Description], [dbo].[Apuntes].[Precio], [dbo].[Universidades].[UniversidadName], [dbo].[Facultades].[FacultadName], [dbo].[Materias].[MateriaName], [dbo].[Apuntes].[OwnerId], [dbo].[Apuntes].[Foto] FROM [dbo].[Apuntes] LEFT OUTER JOIN [dbo].[Universidades] ON [dbo].[Apuntes].[UniversidadId]=[dbo].[Universidades].[UniversidadId] LEFT OUTER JOIN [dbo].[Materias] ON [dbo].[Apuntes].[MateriaId] = [dbo].[Materias].[MateriaId] LEFT OUTER JOIN [dbo].[Facultades] ON [dbo].[Apuntes].[FacultadId]=[dbo].[Facultades].[FacultadId] WHERE [dbo].[Apuntes].[ApunteId] = @id";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@id", SqlDbType.NVarChar))
                    .Value = Id;
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
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
                        return View(apunteDetails);
                    }
                }
                return RedirectToAction("Index", "Error", new { Id = 404 });
            }
        }
        public ActionResult NullIndex()
        {
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Crear()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT * FROM [dbo].[Universidades] ORDER BY [UniversidadName]";
                var cmd = new SqlCommand(query, connection);
                connection.Open();
                var reader = cmd.ExecuteReader();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem { Value = "0", Text = "Seleccione universidad" });
                while (reader.Read())
                {
                    list.Add(new SelectListItem { Value = reader.GetInt32(0).ToString(), Text = reader["UniversidadName"].ToString() });
                }
                ViewBag.FacultadID = new List<SelectListItem>();
                ViewBag.MateriaID = new List<SelectListItem>();
                ViewBag.UniversidadId = list;
            }
            return View();
        }
        // POST
        [HttpPost]
        public ActionResult Crear(Models.ApunteCreate toCreate)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
                {
                    string query = @"SELECT [UsuarioId] FROM [dbo].[Usuarios] WHERE [Email] = @e";
                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters
                        .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                        .Value = User.Identity.Name;
                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    int OwnerId;
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            OwnerId = reader.GetInt32(0);
                        }
                        else
                        {
                            OwnerId = -1;
                        }
                    }
                    else
                    {
                        OwnerId = -1;
                    }
                    reader.Dispose();
                    cmd.Dispose();
                    if (OwnerId > 0)
                    {
                        int lastId = toCreate.Create(toCreate, OwnerId);
                        if (lastId > 0)
                        {
                            return RedirectToAction("Index", "Apunte", new { id = lastId });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Error", new { id = 1488 });
                        }
                    }
                    return RedirectToAction("Index", "Error", new { id = 1487 });
                }
            }
            return RedirectToAction("Index", "Error", new { id = 1486 });
        }
        [HttpPost]
        public ActionResult RellenarFac(int UniversidadId)
        {
            if (UniversidadId != 0)
            {
                using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
                {
                    string query = @"SELECT * FROM [dbo].[Facultades] WHERE [UniversidadId] = @UniId";
                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters
                            .Add(new SqlParameter("@UniId", SqlDbType.Int))
                            .Value = UniversidadId;
                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        List<Facultad> ListFacultades = new List<Facultad>();
                        while (reader.Read())
                        {
                            Facultad toList = new Facultad();
                            toList.FacultadId = reader.GetInt32(reader.GetOrdinal("FacultadId"));
                            toList.FacultadName = reader["FacultadName"].ToString();
                            toList.UniversidadId = reader.GetInt32(reader.GetOrdinal("UniversidadId"));
                            ListFacultades.Add(toList);
                        }
                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return Json(ListFacultades, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        query = @"SELECT * FROM [dbo].[Materias] WHERE [UniversidadId] = @UniId";
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters
                                .Add(new SqlParameter("@UniId", SqlDbType.Int))
                                .Value = UniversidadId;
                        connection.Open();
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            List<Materia> ListMaterias = new List<Materia>();
                            while (reader.Read())
                            {
                                Materia toList = new Materia();
                                toList.MateriaId = reader.GetInt32(reader.GetOrdinal("MateriaId"));
                                toList.MateriaName = reader["MateriaName"].ToString();
                                toList.UniversidadId = reader.GetInt32(reader.GetOrdinal("UniversidadId"));
                                ListMaterias.Add(toList);
                            }
                            return Json(ListMaterias, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { GotIt = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            } else {
                return Json(new { GotIt = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RellenarMat(int FacultadId)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT * FROM [dbo].[Materias] WHERE [FacultadId] = @FacId";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                        .Add(new SqlParameter("@FacId", SqlDbType.Int))
                        .Value = FacultadId;
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    List<Materia> ListMaterias = new List<Materia>();
                    while (reader.Read())
                    {
                        Materia toList = new Materia();
                        toList.MateriaId = reader.GetInt32(reader.GetOrdinal("MateriaId"));
                        toList.MateriaName = reader["MateriaName"].ToString();
                        toList.FacultadId = reader.GetInt32(reader.GetOrdinal("FacultadId"));
                        toList.UniversidadId = reader.GetInt32(reader.GetOrdinal("UniversidadId"));
                        ListMaterias.Add(toList);
                    }
                    return Json(ListMaterias, JsonRequestBehavior.AllowGet);
                }
                return Json(new { GotIt = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}