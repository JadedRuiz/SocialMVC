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
        public ActionResult Perfil()
        {

            if (ValidarUsuario.userExist())
            {
                int id = Convert.ToInt32(Session["id_usuario"]);
                var infoPerfil = (from u in db.usuario
                                  where u.id_usuario == id
                                  select new
                                  {
                                      id_usuario = u.id_usuario,
                                      nombres = u.nombres,
                                      apellidos = u.apellidos,
                                      sexo = u.sexo,
                                      fecha = u.fecha_nacimiento,
                                      email = u.email,
                                      contraseña = u.contraseña,
                                      telefono = u.telefono,
                                      path_perfil = u.path_perfil,
                                      descripcion = u.descripcion
                                  }).First();
                var user = new User
                {
                    id_usuario = infoPerfil.id_usuario,
                    nombres = infoPerfil.nombres,
                    apellidos = infoPerfil.apellidos,
                    sexo = Convert.ToInt16(infoPerfil.sexo),
                    fecha_nacimiento = Convert.ToDateTime(infoPerfil.fecha),
                    email = infoPerfil.email,
                    contraseña = infoPerfil.contraseña,
                    telefono = infoPerfil.telefono,
                    path_perfil = infoPerfil.path_perfil,
                    descripcion = infoPerfil.descripcion

                };
                ViewBag.Perfil = user;
                List<PostsModel> modelo = new List<PostsModel>();
                var publicaciones = (from up in db.usuario_post
                                     join p in db.post on up.post_id equals p.id_post
                                     join u in db.usuario on up.usuario_id equals u.id_usuario
                                     where up.usuario_id == id
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
                    for (int i = 0; i < result.Count; i++)
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
                System.Diagnostics.Debug.WriteLine(modelo.Count);
                ViewBag.Posts = modelo;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Usuario");
            }

        }
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
                                     where ua.tipo == 1
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
                var solicitudes = db.usuario_amigo.Where(x => x.id_amigo == id && x.tipo == 0).Count();
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
                //Busqueda solo usuarios que no sean ni amigos ni solicitudes de amistad
                var filtro1= db.usuario_amigo.Select(x => x.id_amigo).ToList();
                var usuarios = db.usuario.Where(p => !filtro1.Contains(p.id_usuario) && (p.nombres.Contains(busqueda) || 
                p.apellidos.Contains(busqueda)) && p.id_usuario!=id).ToList();

                //Busqueda de amigos
                var amigos = (from u in db.usuario
                              join ua in db.usuario_amigo on u.id_usuario equals ua.id_amigo
                              where ua.usuario_id == id && (u.nombres.Contains(busqueda)
                               || u.apellidos.Contains(busqueda))
                              select new
                              {
                                  id_amigo = u.id_usuario,
                                  nombre = u.nombres + " " + u.apellidos,
                                  path_perfil = u.path_perfil,
                                  tipo = ua.tipo
                              }).ToList();
                foreach (var usuario in usuarios)
                {
                    System.Diagnostics.Debug.WriteLine(usuario.nombres);
                    datos.Add(new BusquedaModel
                    {
                        id_usuario = usuario.id_usuario,
                        nombre = usuario.nombres +" "+ usuario.apellidos,
                        path_perfil = usuario.path_perfil,
                        isAmigo = -1
                    });
                }
                foreach (var amigo in amigos)
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
                    if (amigo.tipo == 2)
                    {
                        datos.Add(new BusquedaModel
                        {
                            id_usuario = amigo.id_amigo,
                            nombre = amigo.nombre,
                            path_perfil = amigo.path_perfil,
                            isAmigo = 2
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
            usuario_amigo enviar = new usuario_amigo();
            usuario_amigo notificar = new usuario_amigo();
            //Enviar solicitud
            enviar.usuario_id = id;
            enviar.id_amigo = id_amigo;
            enviar.tipo = 0;
            enviar.fecha_amistad = null;
            enviar.silenciado = null;
            enviar.bloqueado = null;
            //Notificar a usuario
            notificar.usuario_id = id_amigo;
            notificar.id_amigo = id;
            notificar.tipo = 2;
            notificar.fecha_amistad = null;
            notificar.silenciado = null;
            notificar.bloqueado = null;
            db.usuario_amigo.Add(enviar);
            db.usuario_amigo.Add(notificar);
            db.SaveChanges();
            return RedirectToAction("Muro");
        }
        public ActionResult AcceptOrDeneg(int tipo, int id_amigo)
        {
            int id = Convert.ToInt32(Session["id_usuario"]);
            var solicitud = db.usuario_amigo.Where(x => x.id_amigo == id
                && x.usuario_id == id_amigo).ToList();
            var enviar = db.usuario_amigo.Where(x => x.id_amigo == id_amigo
            && x.usuario_id == id).ToList();
            if (tipo == 0)
            {
                solicitud[0].tipo = 1;
                enviar[0].tipo = 1;
            }else
            {
                usuario_amigo amistad = new usuario_amigo();
                amistad.id_amigo = id_amigo;
                amistad.usuario_id = id;
                db.usuario_amigo.Attach(amistad);
                amistad.id_amigo = id;
                amistad.usuario_id = id_amigo;
                db.usuario_amigo.Attach(amistad);
            }
            db.SaveChanges();
            return RedirectToAction("Muro");
        }
    }
}
