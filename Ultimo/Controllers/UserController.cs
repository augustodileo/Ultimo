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
    public class UserController : Controller
    {
        // GET: User    
        public ActionResult Index(int Id)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT * FROM [dbo].[Usuarios] WHERE [UsuarioId] = @u";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = Id;
                connection.Open();
                var userDetails = new Usuario();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Email"].ToString() == User.Identity.Name)
                    {

                        userDetails.Email = reader["Email"].ToString();
                        userDetails.Name = reader["Name"].ToString();
                        userDetails.Tel = reader["Tel"].ToString();
                        ViewBag.Edit = true;
                    }
                    else
                    {
                        userDetails.Name = reader["Name"].ToString();
                        ViewBag.Edit = false;

                    }
                }
                reader.Close();
                connection.Close();
                return View(userDetails);
            }
        }

        [Authorize]
        public ActionResult NullIndex()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT * FROM [dbo].[Usuarios] WHERE [Email] = @e";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                    .Value = User.Identity.Name;
                connection.Open();
                var userDetails = new Usuario();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userDetails.Email = reader["Email"].ToString();
                    userDetails.Name = reader["Name"].ToString();
                    userDetails.Tel = reader["Tel"].ToString();
                    ViewBag.Edit = true;
                }
                reader.Close();
                connection.Close();
                return View("Index", userDetails);
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            if(Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        // POST
        [HttpPost]
        public ActionResult Login(Models.Usuario user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.Email, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Email, user.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { error = 1490 });
                }
            }
            return RedirectToAction("Index", "Error", new { error = 1489 });
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(Models.RegisterModel toRegister)
        {
            if (ModelState.IsValid)
            {
                if (toRegister.Register(toRegister))
                {
                    FormsAuthentication.SetAuthCookie(toRegister.Email, false);
                    //Models.Email Confirmation = new Models.Email(toRegister);
                    //Confirmation.SendEmail(Confirmation);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Sorry, an error has fucked up");
                }
            }
            return View(toRegister);
        }

        [HttpPost]
        public ActionResult Buscar(string email)
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
                    return Json(new { status = true });
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return Json(new { status = false });
                }
            }
        }
        public ActionResult ConfirmEmail(string token)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"UPDATE [dbo].[Usuarios] SET [Confirmed] = 1 WHERE [Token] = @t";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@t", SqlDbType.NChar))
                    .Value = token;
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                }
            }
            return View();
        }

        public ActionResult Edit()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString))
            {
                string query = @"SELECT * FROM [dbo].[Usuarios] WHERE [Email] = @e";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters
                    .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                    .Value = User.Identity.Name;
                connection.Open();
                var userDetails = new Usuario();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userDetails.Email = reader["Email"].ToString();
                    userDetails.Name = reader["Name"].ToString();
                    userDetails.Tel = reader["Tel"].ToString();
                    ViewBag.Edit = true;
                }
                reader.Close();
                connection.Close();
                return View(userDetails);
            }
        }
    }
}