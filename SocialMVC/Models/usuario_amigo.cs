//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class usuario_amigo
    {
        public int id_usuario_amigo { get; set; }
        public Nullable<int> usuario_id { get; set; }
        public Nullable<int> id_amigo { get; set; }
        public Nullable<int> tipo { get; set; }
        public Nullable<System.DateTime> fecha_amistad { get; set; }
        public Nullable<int> bloqueado { get; set; }
        public Nullable<int> silenciado { get; set; }
    }
}
