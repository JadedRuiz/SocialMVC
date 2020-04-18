using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMVC.Models
{
    public class BusquedaModel
    {
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string path_perfil { get; set; }
        public int isAmigo { get; set; }
    }
}