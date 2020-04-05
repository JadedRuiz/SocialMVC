using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialMVC.Models;

namespace SocialMVC.Controllers
{
    public class UsuarioController : Controller
    {
        SocialServiceEntities db = new SocialServiceEntities();
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
                    Session["nombre"] = result.nombres + result.apellidos;
                    Session["id_usuario"] = result.id_usuario;
                    return RedirectToAction("Page", "Usuario");
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
    }
}