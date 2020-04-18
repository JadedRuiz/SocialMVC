using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialMVC.Models;
using System.IO;

namespace SocialMVC.Controllers
{
    public class PostsController : Controller
    {
        SocialServiceEntities3 db = new SocialServiceEntities3();
        // GET: Posts
        public ActionResult Muro()
        {
            if (ValidarUsuario.userExist())
            {
                List<PostsModel> modelo = new List<PostsModel>();
                int id = Convert.ToInt32(Session["id_usuario"]);
                var publicaciones = (from up in db.usuario_post
                                     join p in db.post on up.post_id equals p.id_post
                                     join ua in db.usuario_amigo on up.usuario_id equals ua.id_amigo
                                     join u in db.usuario on ua.id_amigo equals u.id_usuario
                                     where ua.usuario_id == id
                                     select new
                                     {
                                         id_up = up.id_usuario_post,
                                         nombre = u.nombres + " " + u.apellidos,
                                         text_post = p.text_post,
                                         path_img = p.path_img,
                                         fecha = up.fecha_post
                                     }).ToList();
                foreach (var item in publicaciones)
                {
                    int[] reacciones = new int[6];
                    var result = db.reacciones_post.Where(x => x.post_usuario_id == item.id_up).ToList();
                    for(int i =0; i<result.Count; i++)
                    {
                        switch (result[i].reaccion)
                        {
                            case 1: reacciones[0]++; break;
                            case 2: reacciones[1]++; break;
                            case 3: reacciones[2]++; break;
                            case 4: reacciones[3]++; break;
                            case 5: reacciones[4]++; break;
                            case 6: reacciones[5]++; break;
                        }
                    }
                    modelo.Add(new PostsModel
                    {
                        id_post_usuario = item.id_up,
                        nombre = item.nombre,
                        texto_post = item.text_post,
                        path_post = item.path_img,
                        fecha_post = Convert.ToDateTime(item.fecha),
                        getLike = reacciones[0],
                        getEntristese = reacciones[1],
                        getEncanta = reacciones[2],
                        getDivierte = reacciones[3],
                        getAsombra = reacciones[4],
                        getEnoja = reacciones[5]
                    });
                }
                var solicitudes = db.usuario_amigo.Where(x => x.usuario_id == id && x.tipo == 0).Count();
                ViewBag.Posts = modelo;
                ViewData["img_perfil"] = getDatosPerfil();
                ViewData["solicitudes"] = solicitudes;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Usuario");
            }
        }
        [HttpPost]
        public ActionResult NewPost(string text_post, HttpPostedFileBase file)
        {
            if (Request["btnPub"] == "Publicar") {
                int id = Convert.ToInt32(Session["id_usuario"]);
                post pt = new post();
                usuario_post up = new usuario_post();
                pt.text_post = text_post;
                if (file != null)
                {
                    var ruta = AppDomain.CurrentDomain.GetData("APPBASE").ToString() + "Content\\" + Session["nombre"] + "\\img_posts";
                    if (!Directory.Exists(ruta))
                    {
                        Directory.CreateDirectory(ruta);
                    }
                    var name = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + file.FileName;
                    file.SaveAs((ruta + "\\" + name).ToLower());
                    pt.path_img = "../Content/" + Session["nombre"] + "/img_posts/" + name;
                }
                else
                    pt.path_img = "";
                db.post.Add(pt);
                db.SaveChanges();
                up.post_id = pt.id_post;
                up.usuario_id = id;
                var fecha = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                up.fecha_post = Convert.ToDateTime(fecha);
                db.usuario_post.Add(up);
                db.SaveChanges();
                return RedirectToAction("Muro");
            }
            return RedirectToAction("Muro");
        }
        public ActionResult Reaccionar(int tipo, int id_post)
        {
            reacciones_post reaccion = new reacciones_post();
            int id_user = Convert.ToInt32(Session["id_usuario"]);
            var result = db.reacciones_post.Where(x => x.amigo_id == id_user && x.post_usuario_id == id_post).Count();
            if(result == 0)
            {
                reaccion.post_usuario_id = id_post;
                reaccion.amigo_id = id_user;
                reaccion.reaccion = tipo;
                db.reacciones_post.Add(reaccion);
            }else
            {
                var reacciones = db.reacciones_post.Where(x => x.amigo_id == id_user && x.post_usuario_id == id_post).ToList();
                reacciones[0].reaccion = tipo;
            }
            db.SaveChanges();
            return RedirectToAction("Muro");
        }
        public string getDatosPerfil()
        {
            int id = Convert.ToInt32(Session["id_usuario"]);
            var resu = db.usuario.Where(x => x.id_usuario == id).ToList();
            return resu[0].path_perfil;
        }
        public ActionResult Search()
        {
            if (Request["buscar"] == "enviar")
            {
                List<BusquedaModel> datos = new List<BusquedaModel>();
                int id = Convert.ToInt32(Session["id_usuario"]);
                String busqueda = Request["busca"];
                var result = (from u in db.usuario
                              where (u.nombres.Contains(busqueda) ||
                              u.apellidos.Contains(busqueda)) && u.id_usuario != id
                              select new
                              {
                                  id_amigo = u.id_usuario,
                                  nombre = u.nombres + " " + u.apellidos,
                                  path_perfil = u.path_perfil
                              }).ToList();
                var amigos = (from ua in db.usuario_amigo
                              join u in db.usuario on ua.id_amigo equals u.id_usuario
                              where ua.usuario_id == id && (u.nombres.Contains(busqueda) ||
                              u.apellidos.Contains(busqueda))
                              select new
                              {
                                  id_amigo = u.id_usuario,
                                  nombre = u.nombres + " " + u.apellidos,
                                  path_perfil = u.path_perfil,
                                  tipo = ua.tipo 
                              }).ToList();
                foreach(var amigo in amigos)
                {
                    if (amigo.tipo == 0)
                    {
                        datos.Add(new BusquedaModel
                        {
                            id_usuario = amigo.id_amigo,
                            nombre = amigo.nombre,
                            path_perfil = amigo.path_perfil,
                            isAmigo = 0
                        });
                    }
                    if (amigo.tipo == 1)
                    {
                        datos.Add(new BusquedaModel
                        {
                            id_usuario = amigo.id_amigo,
                            nombre = amigo.nombre,
                            path_perfil = amigo.path_perfil,
                            isAmigo = 1
                        });
                    }
                }
                foreach (var item in result)
                {
                    Boolean band = false;
                    foreach(var dato in datos)
                    {
                        if(item.nombre == dato.nombre)
                        {
                            band = true;
                        }
                    }
                    if(band != true)
                    {
                        datos.Add(new BusquedaModel
                        {
                            id_usuario = item.id_amigo,
                            nombre = item.nombre,
                            path_perfil = item.path_perfil,
                            isAmigo = -1
                        });
                    }
                }

                ViewBag.Datos = datos;
                return View();
            }
            return RedirectToAction("Muro");
        }
        public ActionResult addAmigo(int id_amigo)
        {
            int id = Convert.ToInt32(Session["id_usuario"]);
            usuario_amigo ua = new usuario_amigo();
            ua.usuario_id = id_amigo;
            ua.id_amigo = id;
            ua.tipo = 0;
            ua.fecha_amistad = null;
            ua.silenciado = 0;
            ua.bloqueado = 0;
            db.usuario_amigo.Add(ua);
            db.SaveChanges();
            return RedirectToAction("Muro");
        }
    }
}
