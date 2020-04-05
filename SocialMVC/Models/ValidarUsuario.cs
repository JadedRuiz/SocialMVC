using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMVC.Models
{
    public class ValidarUsuario
    {
        public static bool userExist()
        {
            HttpContext contexto = HttpContext.Current;
            if (contexto.Session["id_usuario"] != null)
            {
                return true;
            }
            else
                return false;
        }

    }
}