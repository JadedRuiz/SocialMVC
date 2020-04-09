using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMVC.Models
{
    public class PostsModel
    {
        public int id_post_usuario { get; set; }
        public string nombre { get; set; }
        public string texto_post { get; set; }
        public string path_post { get; set; }
        public DateTime fecha_post { get; set; }
        public int getLike { get; set; }
        public int getDivierte { get; set; }
        public int getEncanta { get; set; }
        public int getEnoja { get; set; }
        public int getAsombra { get; set; }
        public int getEntristese { get; set; }
    }
}