using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialMVC.Models;

namespace SocialMVC.Controllers
{
    public class PostsController : Controller
    {
        // GET: Posts
        public ActionResult Muro()
        {

            if (ValidarUsuario.userExist())
            {
                return View();
            }else
            {
                return RedirectToAction("Index", "Usuario");
            }
        }
    }
}