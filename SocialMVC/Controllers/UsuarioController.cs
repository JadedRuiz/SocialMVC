using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialMVC.Models;
using System.IO;
using System.Net.Mail;

namespace SocialMVC.Controllers
{
    public class UsuarioController : Controller
    {
        SocialServiceEntities5 db = new SocialServiceEntities5();
        // GET: Usuario
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            string btnClick = Request["loginBtn"];

            if(btnClick == "Login")
            {
                string email = Request["email"];
                string pass = Request["pass"];
                var result = db.usuario.Where(usuario => usuario.email == email
                && usuario.contraseña == pass).FirstOrDefault();

                if(result != null)
                {
                    Session["nombre"] = result.nombres +" "+ result.apellidos;
                    Session["img_perfil"] = result.path_perfil;
                    Session["id_usuario"] = result.id_usuario;
                    return RedirectToAction("Muro", "Posts");
                }else if(result== null)
                {
                    return View("Index");
                }
            }
            return View("Index");
        }

        public ActionResult logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult Page()
        {
            if (ValidarUsuario.userExist())
            {
                return View();
            }
            else
                return RedirectToAction("Index");
        }
        [HttpGet]
        public string EnviarPass(string correo)
        {
            Boolean band = false;
            var validar = db.usuario.Select(x => x.email).ToList();
            for (var i = 0; i < validar.Count; i++)
            {
                if (validar[i] == correo)
                {
                    band = true;
                }
            }
            if (band == true)
            {
                try
                {
                    var datos = db.usuario.Where(x => x.email == correo).ToList();
                    string mycorreo = "razonable3500@gmail.com";
                    MailMessage envcorreo = new MailMessage();
                    envcorreo.From = new MailAddress(mycorreo);
                    envcorreo.To.Add(datos[0].email);
                    envcorreo.Subject = "Contraseña de SocialService";
                    envcorreo.Body = "Su contraseña es: " + datos[0].contraseña;
                    envcorreo.IsBodyHtml = true;
                    envcorreo.Priority = MailPriority.Normal;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = true;
                    string password = "chxfvqpntbizyimp";
                    smtp.Credentials = new System.Net.NetworkCredential(mycorreo, password);
                    smtp.Send(envcorreo);
                    return "La contraseña ha sido enviada!";
                }
                catch (Exception e)
                {
                    return "Error al enviar correo! " + e.Message;
                }
            }else
            {
                return "El correo no existe!";
            }
        }
        [HttpPost]
        public JsonResult Registro(usuario user, string name_perfil, string name_fondo)
        {
            Boolean band = false;
            RespuestaModel json = new RespuestaModel();
            var validar = db.usuario.Select(x => x.email).ToList();
            for(var i =0; i<validar.Count; i++)
            {
                if(validar[i] == user.email)
                {
                    band = true;
                }
            }
            if (band == true)
            {
                json.ok = false;
                json.message = "El correo ya ha sido registrado! Intente de nuevo";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var nombre = user.nombres + " " + user.apellidos;
                var ruta = AppDomain.CurrentDomain.GetData("APPBASE").ToString() + "Content\\img_users\\" + nombre + "\\img_perfil\\";
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }
                if(user.path_perfil != null)
                {
                    var name = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + name_perfil;
                    Byte[] bytes = Convert.FromBase64String(user.path_perfil);
                    System.IO.File.WriteAllBytes(ruta + name, bytes);
                    user.path_perfil = "../Content/img_users/" + nombre +"/img_perfil/"+ name;
                }
                if (user.path_fondo != null)
                {
                    var name = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + name_fondo;
                    Byte[] bytes = Convert.FromBase64String(user.path_fondo);
                    System.IO.File.WriteAllBytes(ruta + name, bytes);
                    user.path_fondo = "../Content/img_users/" + nombre + "/img_perfil/" + name;
                }
                db.usuario.Add(user);
                db.SaveChanges();
                json.ok = true;
                json.message = "Se han registrado sus datos con éxito!";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}