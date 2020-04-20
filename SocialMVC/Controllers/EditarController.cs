using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialMVC.Models;
using System.IO;
namespace SocialMVC.Controllers
{
    public class EditarController : Controller
    {
        // GET: Editar
        public ActionResult Editar()
        {
            User model = new User();
            if (ValidarUsuario.userExist())
            {
                using (SocialServiceEntities4 db = new SocialServiceEntities4())
                {
                    int idUsuario = Convert.ToInt32(Session["id_usuario"]);
                    var MyUser = db.usuario.Find(idUsuario);
                    model.nombres = MyUser.nombres;
                    model.apellidos = MyUser.apellidos;
                    model.telefono = MyUser.telefono;
                    model.descripcion = MyUser.descripcion;
                    model.fecha_nacimiento = Convert.ToDateTime(MyUser.fecha_nacimiento);
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Editar(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SocialServiceEntities4 db = new SocialServiceEntities4())
                    {
                        int idUsuario = Convert.ToInt32(Session["id_usuario"]);
                        var User = db.usuario.Find(idUsuario);
                        User.nombres = model.nombres;
                        User.apellidos = model.apellidos;
                        User.telefono = model.telefono;
                        User.descripcion = model.descripcion;
                        User.fecha_nacimiento = model.fecha_nacimiento;
                        //Quizas igual la foto pero por ahora nop
                        db.Entry(User).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Redirect("/Posts/Perfil");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}